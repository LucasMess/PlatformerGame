using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Adam;
using Adam.Interactables;
using Adam.Misc.Interfaces;
using Adam.Obstacles;
using Adam.Lights;
using Adam.Characters.Enemies;
using Adam.Levels;
using Adam.Misc;
using Adam.PlayerCharacter;

namespace Adam
{
    /// <summary>
    /// Particles cannot check collision and should disappear after a while.
    /// </summary>
    public class Particle : Entity
    {
        Texture2D _nextTexture;
        Vector2 _originalPosition, _originalVelocity;
        Vector2 _differenceInPosition, _endPosition;
        Random _randGen;
        double _frameTimer;
        double _opacityTimer;
        double _travelTimer;
        double _respawnTimer;
        Vector2 _frameCount;
        protected Vector2 Position;
        int _currentFrame;
        public bool IsComplete;
        bool _hasChangedDirection;
        bool _dead;
        float _rotation, _rotationSpeed, _rotationDelta;
        Color color = Color.White;
        public Light light;
        Player _player;

        public enum ParticleType
        {
            ChestSparkles,
            Impact,
            GameZzz,
            MenuZzz,
            SnakeVenom,
            WeaponBurst,
            EnemyDesintegration,
            PlayerChronoshift,
            TileParticle,
            PlayerDesintegration,
            PlayerRespawn,
            MusicNotes,
            JetpackSmoke,
            Blood,
            Lava,
            TookDamage,
            DeathSmoke,
            Sparkles,
        }
        public ParticleType CurrentParticle;

        protected override Rectangle DrawRectangle
        {
            get
            {
                return CollRectangle;
            }
        }

        public Particle()
        {

        }

        public Particle(Projectile projectile, Tile[] tileArray)
        {
            SourceRectangle = new Rectangle(0, 0, 32, 32);
            CurrentParticle = ParticleType.Impact;
            Texture = ContentHelper.LoadTexture("Explosion");
            if (projectile.GetVelocity().X > 0)
                CollRectangle = new Rectangle(tileArray[projectile.TileHit].DrawRectangle.X - 32, projectile.GetCollRectangle().Center.Y - 32, 64, 64);
            else CollRectangle = new Rectangle(tileArray[projectile.TileHit].DrawRectangle.X + 32, projectile.GetCollRectangle().Center.Y - 32, 64, 64);
            _frameCount = new Vector2(Texture.Width / 32, Texture.Height / 32);
        }

        public Particle(Enemy enemy, Projectile projectile)
        {
            SourceRectangle = new Rectangle(0, 0, 32, 32);
            CurrentParticle = ParticleType.Impact;
            Texture = ContentHelper.LoadTexture("Explosion");

            if (projectile.GetVelocity().X > 0)
                CollRectangle = new Rectangle(enemy.GetCollRectangle().X - enemy.GetCollRectangle().Width / 2, projectile.GetCollRectangle().Center.Y - SourceRectangle.Height / 2, 32, 32);
            else CollRectangle = new Rectangle(enemy.GetCollRectangle().X + enemy.GetCollRectangle().Width / 2, projectile.GetCollRectangle().Center.Y - SourceRectangle.Height / 2, 32, 32);
            _frameCount = new Vector2(Texture.Width / 32, Texture.Height / 32);
        }

        public Particle(Projectile proj)
        {
            CurrentParticle = ParticleType.SnakeVenom;
            Texture = ContentHelper.LoadTexture("Effects/venom_blob");
            _randGen = new Random();
            CollRectangle = new Rectangle(proj.GetCollRectangle().X + proj.GetCollRectangle().Width / 2, proj.GetCollRectangle().Y, 8, 8);
            if (proj.GetVelocity().X > 0)
                Velocity.X = -2;
            else Velocity.X = 2;
            Velocity.Y = GameWorld.RandGen.Next(1, 4);
            if (GameWorld.RandGen.Next(0, 2) == 0)
                Velocity.Y = -Velocity.Y;
            SourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Opacity = 1f;
        }

        public Particle(Player player)
        {
            _randGen = new Random();
            SourceRectangle = new Rectangle(0, 0, 14, 14);
            CurrentParticle = ParticleType.GameZzz;
            Texture = ContentHelper.LoadTexture("Effects/new_Z");
            color = new Color(255, 255, 255, 255);
            if (player.IsFacingRight)
                Position = new Vector2(player.GetCollRectangle().X + _randGen.Next(-5, 5) + player.GetCollRectangle().Width - 8, player.GetCollRectangle().Y + _randGen.Next(-5, 5));
            else Position = new Vector2(player.GetCollRectangle().Center.X + _randGen.Next(0, 20), player.GetCollRectangle().Y + _randGen.Next(-5, 5));
            CollRectangle = new Rectangle((int)Position.X, (int)Position.Y, 8, 8);

        }

        public Particle(Rectangle rectangle)
        {
            _randGen = new Random();
            SourceRectangle = new Rectangle(0, 0, 14, 14);
            CurrentParticle = ParticleType.MenuZzz;
            Texture = ContentHelper.LoadTexture("Effects/new_Z");
            color = new Color(255, 255, 255, 255);
            Position = new Vector2(rectangle.X + _randGen.Next(-5, 5) + rectangle.Width - 8, rectangle.Y + _randGen.Next(-5, 5));
            CollRectangle = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
        }

        public void CreateWeaponBurstEffect(Player player, Projectile proj, ContentManager content)
        {
            //position = player.weapon.tipPos;
            Velocity = new Vector2(0, 0);
            int maxTanSpeed = 10;
            Velocity.X = (float)(GameWorld.RandGen.Next((int)(proj.GetVelocity().X - maxTanSpeed), (int)(proj.GetVelocity().X + maxTanSpeed + 1)));
            Velocity.Y = (float)(GameWorld.RandGen.Next((int)(proj.GetVelocity().Y - maxTanSpeed), (int)(proj.GetVelocity().Y + maxTanSpeed + 1)));
            CurrentParticle = ParticleType.WeaponBurst;
            Texture = ContentHelper.LoadTexture("Effects/laser_burst");
            int randSize = GameWorld.RandGen.Next(2, 5);
            CollRectangle = new Rectangle((int)Position.X, (int)Position.Y, randSize, randSize);
            SourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Opacity = 1;
        }

        public void CreateEnemyDisintegrationEffect(Enemy enemy, Rectangle sourceRectangle, Projectile proj)
        {
            CurrentParticle = ParticleType.EnemyDesintegration;
            Texture = enemy.Texture;
            CollRectangle = new Rectangle(enemy.GetCollRectangle().X + sourceRectangle.X, enemy.GetCollRectangle().Y + sourceRectangle.Y,
                sourceRectangle.Width, sourceRectangle.Height);
            this.SourceRectangle = sourceRectangle;
            int maxTanSpeed = 10;
            Velocity.X = (float)(GameWorld.RandGen.Next((int)(proj.GetVelocity().X - maxTanSpeed), (int)(proj.GetVelocity().X + maxTanSpeed + 1)));
            Velocity.Y = (float)(GameWorld.RandGen.Next((int)(proj.GetVelocity().Y - maxTanSpeed), (int)(proj.GetVelocity().Y + maxTanSpeed + 1)));
        }

        public void CreateEnemyDeathEffect(Enemy enemy, Rectangle sourceRectangle)
        {
            CurrentParticle = ParticleType.EnemyDesintegration;
            Texture = enemy.Texture;
            CollRectangle = new Rectangle(enemy.GetCollRectangle().X + sourceRectangle.X, enemy.GetCollRectangle().Y + sourceRectangle.Y,
                sourceRectangle.Width, sourceRectangle.Height);
            this.SourceRectangle = sourceRectangle;
            int maxTanSpeed = 5;
            Velocity = new Vector2(GameWorld.RandGen.Next(-maxTanSpeed, maxTanSpeed), GameWorld.RandGen.Next(-maxTanSpeed, maxTanSpeed));
        }

        public void CreateBloodEffect(Player player, GameWorld map)
        {
            CurrentParticle = ParticleType.Blood;
            Texture = ContentHelper.LoadTexture("Effects/blood");
            CollRectangle = new Rectangle(player.GetCollRectangle().Center.X, player.GetCollRectangle().Center.Y, 8, 8);
            SourceRectangle = new Rectangle(GameWorld.RandGen.Next(0, 4) * 8, 0, 8, 8);
            CollRectangle = CollRectangle;
            Velocity = new Vector2(GameWorld.RandGen.Next(-10, 10), GameWorld.RandGen.Next(-10, 10));
            Position = new Vector2(CollRectangle.X, CollRectangle.Y);
        }

        public void CreatePlayerChronoshiftEffect(Player player, Rectangle sourceRectangle)
        {
            CurrentParticle = ParticleType.PlayerChronoshift;

        }

        public void CreateTileParticleEffect(Tile tile, Player player)
        {
            CurrentParticle = ParticleType.TileParticle;
            Texture = tile.Texture;
            CollRectangle = new Rectangle(player.GetCollRectangle().Center.X - 2, player.GetCollRectangle().Y + player.GetCollRectangle().Height, 8, 8);
            SourceRectangle = new Rectangle(tile.SourceRectangle.X, tile.SourceRectangle.Y, 4, 4);
            SourceRectangle.X += (GameWorld.RandGen.Next(0, 4) * Main.Tilesize / 4);
            Velocity.X = (-player.GetVelocity().X / 2) * (float)GameWorld.RandGen.NextDouble();
            Velocity.Y = GameWorld.RandGen.Next(-1, 1);
            Opacity = 1;
        }

        public void CreateMusicNotesEffect(CdPlayer cd)
        {
            CurrentParticle = ParticleType.MusicNotes;
            Texture = ContentHelper.LoadTexture("Effects/music_notes");
            CollRectangle = new Rectangle(cd.GetCollRectangle().X, cd.GetCollRectangle().Y, 16, 16);
            SourceRectangle = new Rectangle(32 * GameWorld.RandGen.Next(0, 2), 0, 32, 32);
            int randX = GameWorld.RandGen.Next(0, 2);
            float velocityX = .5f;
            if (randX == 0)
            {
                CollRectangle.X -= CollRectangle.Width;
                Velocity.X = -velocityX;
            }
            else
            {
                CollRectangle.X += CollRectangle.Width;
                Velocity.X = velocityX;
            }

            Velocity.Y = -(float)GameWorld.RandGen.NextDouble();
            Position = new Vector2(CollRectangle.X, CollRectangle.Y);
        }

        public void CreateJetPackSmokeParticle(JetpackPowerUp jet)
        {
            CurrentParticle = ParticleType.JetpackSmoke;
            Texture = ContentHelper.LoadTexture("Effects/smoke");
            CollRectangle = new Rectangle(jet.GetCollRectangle().Center.X - 8, jet.GetCollRectangle().Y + jet.GetCollRectangle().Height, 16, 16);
            SourceRectangle = new Rectangle(GameWorld.RandGen.Next(0, 4) * 16, 0, 16, 16);
            Velocity.Y = 3f;
            Velocity.X = GameWorld.RandGen.Next(-1, 2);
            Position = new Vector2(CollRectangle.X, CollRectangle.Y);
            Opacity = 1f;
        }

        public void CreateJetPackSmokeParticle(Player player)
        {
            CurrentParticle = ParticleType.JetpackSmoke;
            Texture = ContentHelper.LoadTexture("Effects/smoke");
            CollRectangle = new Rectangle(player.GetCollRectangle().Center.X - 8, player.GetCollRectangle().Y + player.GetCollRectangle().Height, 16, 16);
            SourceRectangle = new Rectangle(GameWorld.RandGen.Next(0, 4) * 16, 0, 16, 16);
            Velocity.Y = 3f;
            Velocity.X = GameWorld.RandGen.Next(-1, 2);
            Position = new Vector2(CollRectangle.X, CollRectangle.Y);
            Opacity = 1f;
        }

        public void CreateLavaParticle(Liquid lava, GameWorld map)
        {
            CurrentParticle = ParticleType.Lava;
            //texture = ContentHelper.LoadTexture("Effects/lava");
            Texture = GameWorld.ParticleSpriteSheet;
            CollRectangle = new Rectangle(lava.GetCollRectangle().Center.X - 8, lava.GetCollRectangle().Center.Y - 8, 8, 8);
            SourceRectangle = new Rectangle(0, 0, 8, 8);
            Velocity.Y = -10f;
            Position = new Vector2(CollRectangle.X, CollRectangle.Y);
            Opacity = 1f;
            light = new Lights.DynamicPointLight(this, 1, true, Color.Orange, .3f);
            GameWorld.Instance.LightEngine.AddDynamicLight(light);
        }

        public void CreateDeathSmoke(Entity entity)
        {
            CurrentParticle = ParticleType.DeathSmoke;
            Texture = ContentHelper.LoadTexture("Effects/smoke");
            CollRectangle = new Rectangle(GameWorld.RandGen.Next(entity.GetCollRectangle().X, entity.GetCollRectangle().Right - 16), GameWorld.RandGen.Next(entity.GetCollRectangle().Y, entity.GetCollRectangle().Bottom - 16), 16, 16);
            SourceRectangle = new Rectangle(GameWorld.RandGen.Next(0, 4) * 16, 0, 16, 16);
            Velocity.X = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-2, 3));
            Velocity.Y = -.5f;
            Position = new Vector2(CollRectangle.X, CollRectangle.Y);
            Opacity = 2;
        }

        public void CreateTookDamage(Entity entity)
        {
            CurrentParticle = ParticleType.TookDamage;
            Texture = ContentHelper.LoadTexture("Sparkles");
            CollRectangle = new Rectangle(GameWorld.RandGen.Next(entity.GetCollRectangle().X, entity.GetCollRectangle().Right - 8), GameWorld.RandGen.Next(entity.GetCollRectangle().Y, entity.GetCollRectangle().Bottom - 8), 8, 8);
            SourceRectangle = new Rectangle(0, 0, 8, 8);
            Velocity.X = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-4, 5));
            Velocity.Y = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-1, 2));
            Position = new Vector2(CollRectangle.X, CollRectangle.Y);
            Opacity = 1;
            color = Color.Red;
        }

        public void CreateSparkles(Entity entity)
        {
            CurrentParticle = ParticleType.Sparkles;
            Texture = ContentHelper.LoadTexture("Sparkles");
            CollRectangle = new Rectangle(GameWorld.RandGen.Next(entity.GetCollRectangle().X, entity.GetCollRectangle().Right - 8), GameWorld.RandGen.Next(entity.GetCollRectangle().Y, entity.GetCollRectangle().Bottom - 8), 8, 8);
            SourceRectangle = new Rectangle(0, 0, 8, 8);
            Velocity.X = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-4, 5));
            Velocity.Y = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-7, 0));
            Position = new Vector2(CollRectangle.X, CollRectangle.Y);
            Opacity = 2;
            color = Color.White;
        }

        public virtual void Update(GameTime gameTime)
        {
            GameWorld gameWorld = GameWorld.Instance;
            switch (CurrentParticle)
            {
                case ParticleType.ChestSparkles:
                    CollRectangle.X += (int)Velocity.X;
                    CollRectangle.Y += (int)Velocity.Y;
                    Velocity.Y += .3f;
                    Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Velocity.X = Velocity.X * 0.99f;
                    break;
                case ParticleType.Impact:
                    Animate(gameTime);
                    break;
                case ParticleType.GameZzz:
                    CollRectangle.X = (int)Position.X;
                    CollRectangle.Y = (int)Position.Y;
                    Position += Velocity;
                    Velocity = new Vector2(_randGen.Next(-1, 2), -.3f);
                    _opacityTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (_opacityTimer > 1)
                    {
                        Opacity -= .1f;
                    }
                    if (Opacity < 0)
                        ToDelete = true;
                    break;
                case ParticleType.MenuZzz:
                    CollRectangle.X = (int)Position.X;
                    CollRectangle.Y = (int)Position.Y;
                    Position += Velocity;
                    Velocity = new Vector2(_randGen.Next(-1, 2), -1f);
                    _opacityTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (_opacityTimer > 1)
                    {
                        Opacity -= .1f;
                    }
                    if (Opacity < 0)
                        ToDelete = true;
                    break;
                case ParticleType.SnakeVenom:
                    CollRectangle.X += (int)Velocity.X;
                    CollRectangle.Y += (int)Velocity.Y;
                    Opacity -= .03f;
                    Velocity.Y += .09f;

                    if (Opacity < 0)
                        ToDelete = true;
                    break;
                case ParticleType.WeaponBurst:
                    CollRectangle.X += (int)Velocity.X;
                    CollRectangle.Y += (int)Velocity.Y;
                    Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Velocity.X = Velocity.X * 0.99f;
                    Velocity.Y = Velocity.Y * 0.99f;
                    if (Opacity < 0)
                        ToDelete = true;
        
                    break;
                case ParticleType.EnemyDesintegration:
                    CollRectangle.X += (int)Velocity.X;
                    CollRectangle.Y += (int)Velocity.Y;
                    Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Velocity.X = Velocity.X * 0.99f;
                    Velocity.Y = Velocity.Y * 0.99f;
                    if (Opacity < 0)
                        ToDelete = true;
                    break;
                case ParticleType.PlayerChronoshift:

                    break;
                case ParticleType.TileParticle:
                    CollRectangle.X += (int)Velocity.X;
                    CollRectangle.Y += (int)Velocity.Y;
                    Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //velocity.X = velocity.X * 0.99f;
                    //velocity.Y = velocity.Y * 0.99f;
                    if (Opacity < 0)
                        ToDelete = true;
                    break;
                case ParticleType.PlayerDesintegration:
                    Position += Velocity;

                    CollRectangle.X = (int)Position.X;
                    CollRectangle.Y = (int)Position.Y;

                    if (_respawnTimer > 500)
                    {
                        //if (!player.isWaitingForRespawn)
                        //{
                        //    Opacity = 0f;
                        //    ToDelete = true;
                        //    respawnTimer = 0;
                        //}
                        //else
                        //{
                        //    // velocity = new Vector2((endPosition.X - position.X) / 50, (endPosition.Y - position.Y) / 50);
                        //}
                    }
                    else
                    {
                        Velocity.X = Velocity.X * 0.99f;
                        Velocity.Y = Velocity.Y * 0.99f;
                    }
                    break;
                case ParticleType.MusicNotes:
                    Position += Velocity;

                    CollRectangle.X = (int)Position.X;
                    CollRectangle.Y = (int)Position.Y;

                    Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Opacity <= 0)
                        ToDelete = true;
                    break;
                case ParticleType.JetpackSmoke:
                    Position += Velocity;

                    CollRectangle.X = (int)Position.X;
                    CollRectangle.Y = (int)Position.Y;

                    Velocity.X = Velocity.X * 0.99f;
                    Velocity.Y = Velocity.Y * 0.99f;

                    Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Opacity <= 0)
                        ToDelete = true;
                    break;
                case ParticleType.Blood:
                    if (_dead)
                        return;

                    Position += Velocity;

                    CollRectangle.X = (int)Position.X;
                    CollRectangle.Y = (int)Position.Y;

                    CollRectangle = CollRectangle;

                    Velocity.Y += .3f;

                    if (this.IsTouchingTerrain(gameWorld))
                    {
                        Velocity.X = 0;
                        Velocity.Y = 0;
                        _dead = true;
                    }
                    break;
                case ParticleType.Lava:
                    Position += Velocity;

                    CollRectangle.X = (int)Position.X;
                    CollRectangle.Y = (int)Position.Y;

                    CollRectangle = CollRectangle;

                    Velocity.Y += .3f;

                    Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Opacity <= 0)
                        ToDelete = true;
                    break;
                case ParticleType.DeathSmoke:
                    Position += Velocity;

                    CollRectangle.X = (int)Position.X;
                    CollRectangle.Y = (int)Position.Y;

                    Velocity.X = Velocity.X * 0.95f;
                    Velocity.Y = Velocity.Y * 0.99f;

                    Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Opacity <= 0)
                        ToDelete = true;
                    break;
                case ParticleType.TookDamage:
                    Position += Velocity;

                    CollRectangle.X = (int)Position.X;
                    CollRectangle.Y = (int)Position.Y;

                    Velocity.X = Velocity.X * 0.95f;
                    Velocity.Y = Velocity.Y * 0.99f;

                    Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Opacity <= 0)
                        ToDelete = true;
                    break;
                case ParticleType.Sparkles:
                    Position += Velocity;

                    CollRectangle.X = (int)Position.X;
                    CollRectangle.Y = (int)Position.Y;

                    Velocity.X = Velocity.X * 0.99f;
                    Velocity.Y = Velocity.Y * 0.99f;

                    Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Opacity <= 0)
                        ToDelete = true;
                    break;
            }
            CollRectangle = CollRectangle;
        }

        public virtual void Animate(GameTime gameTime)
        {
            int switchFrame = 10;
            _frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_frameTimer >= switchFrame)
            {
                if (_frameCount.X != 0)
                {
                    _frameTimer = 0;
                    SourceRectangle.X += Texture.Width / (int)_frameCount.X;
                    _currentFrame++;
                }
            }
            if (_currentFrame >= _frameCount.X)
            {
                Opacity = 0;
            }

        }

        protected void DefaultBehavior()
        {
            GameTime gameTime = GameWorld.Instance.GameTime;
            Position += Velocity;

            CollRectangle.X = (int)Position.X;
            CollRectangle.Y = (int)Position.Y;

            Velocity.X = Velocity.X * 0.99f;
            Velocity.Y = Velocity.Y * 0.99f;

            Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Opacity <= 0)
                ToDelete = true;
        }

        protected void NoOpacityNoFrictionBehavior()
        {
            GameTime gameTime = GameWorld.Instance.GameTime;
            Position += Velocity;

            CollRectangle.X = (int)Position.X;
            CollRectangle.Y = (int)Position.Y;

            if (Opacity <= 0)
                ToDelete = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentParticle == ParticleType.SnakeVenom)
                spriteBatch.Draw(Texture, CollRectangle, SourceRectangle, color * Opacity);
            else
                spriteBatch.Draw(Texture, CollRectangle, SourceRectangle, color * Opacity);

        }

        public void DrawLights(SpriteBatch spriteBatch)
        {
            if (light != null)
                light.Draw(spriteBatch);
        }

    }

    public class JumpSmokeParticle : Particle
    {
        public JumpSmokeParticle(Entity entity)
        {
            Texture = ContentHelper.LoadTexture("Effects/smoke");
            CollRectangle = new Rectangle(entity.GetCollRectangle().Center.X - 4, entity.GetCollRectangle().Bottom - 4, 8, 8);
            SourceRectangle = new Rectangle(8 * GameWorld.RandGen.Next(0, 4), 0, 8, 8);
            Position = new Vector2(CollRectangle.X, CollRectangle.Y);

            Velocity.X = (float)GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-3 - (int)entity.GetVelocity().X / 2, 3 - (int)entity.GetVelocity().X / 2);
            Velocity.Y = (float)GameWorld.RandGen.NextDouble() * -1f;
            Opacity = 1f;
        }

        public override void Update(GameTime gameTime)
        {
            DefaultBehavior();
        }

    }

    public class TestSmokeParticle : Particle
    {
        public TestSmokeParticle(int x, int y)
        {
            Texture = ContentHelper.LoadTexture("Sparkles");
            CollRectangle = new Rectangle(x, y, 8, 8);
            SourceRectangle = new Rectangle(0, 0, 8, 8);
            Position = new Vector2(CollRectangle.X, CollRectangle.Y);

            Velocity.X = (float)GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-3, 4);
            Velocity.Y = (float)GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-3, 4);
            Opacity = 1f;
        }

        public override void Update(GameTime gameTime)
        {
            DefaultBehavior();
        }

        public static void Generate(int count, Entity entity)
        {
            for (int i = 0; i < count; i++)
            {
                int x = GameWorld.RandGen.Next(entity.GetCollRectangle().X, entity.GetCollRectangle().X+  entity.GetCollRectangle().Width - 4);
                int y = GameWorld.RandGen.Next(entity.GetCollRectangle().Y, entity.GetCollRectangle().Y + entity.GetCollRectangle().Height - 4);
                TestSmokeParticle par = new TestSmokeParticle(x, y);
                GameWorld.Instance.Particles.Add(par);
            }
        }
    }

    public class StompSmokeParticle : Particle
    {
        public StompSmokeParticle(Entity entity)
        {
            Texture = ContentHelper.LoadTexture("Effects/smoke");
            CollRectangle = new Rectangle(entity.GetCollRectangle().Center.X - 4, entity.GetCollRectangle().Bottom - 4, 8, 8);
            SourceRectangle = new Rectangle(8 * GameWorld.RandGen.Next(0, 4), 0, 8, 8);
            Position = new Vector2(CollRectangle.X, CollRectangle.Y);

            Velocity.X = (float)GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-3, 3);
            Velocity.Y = (float)GameWorld.RandGen.NextDouble() * -1f;

            Opacity = 1f;
        }

        public override void Update(GameTime gameTime)
        {
            DefaultBehavior();
        }

        /// <summary>
        /// Generate many of this particle at once.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="entity"></param>
        public static void Generate(int number, Entity entity)
        {
            for (int i = 0; i < number; i++)
            {
                StompSmokeParticle par = new StompSmokeParticle(entity);
                GameWorld.Instance.Particles.Add(par);
            }
        }
    }

    public class ConstructionSmokeParticle : Particle
    {
        public ConstructionSmokeParticle(Rectangle rect)
        {
            Texture = ContentHelper.LoadTexture("Effects/smoke");
            CollRectangle = new Rectangle(GameWorld.RandGen.Next(rect.X, rect.Right - 16), GameWorld.RandGen.Next(rect.Y, rect.Bottom - 16), 16, 16);
            SourceRectangle = new Rectangle(GameWorld.RandGen.Next(0, 4) * 16, 0, 16, 16);
            Velocity.X = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-5, 6));
            Velocity.Y = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-5, 6));
            Position = new Vector2(CollRectangle.X, CollRectangle.Y);
            Opacity = 2;
        }

        public override void Update(GameTime gameTime)
        {
            DefaultBehavior();

            Velocity.Y += .3f;
        }
    }

    public class DestructionTileParticle : Particle
    {
        public DestructionTileParticle(Tile tile, Rectangle source)
        {
            Texture = tile.Texture;
            CollRectangle = new Rectangle(tile.DrawRectangle.Center.X, tile.DrawRectangle.Center.Y, 8, 8);
            SourceRectangle = source;
            Velocity.X = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-5, 6));
            Velocity.Y = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-5, 6));
            Position = new Vector2(CollRectangle.X, CollRectangle.Y);
            Opacity = 2;

        }
        public override void Update(GameTime gameTime)
        {
            DefaultBehavior();

            Velocity.Y += .3f;
        }
    }

    public class TrailParticle : Particle
    {
        public TrailParticle(Entity source, Color color)
        {
            Texture = GameWorld.ParticleSpriteSheet;
            CollRectangle = new Rectangle(source.GetCollRectangle().Center.X, source.GetCollRectangle().Center.Y, 8, 8);
            SourceRectangle = new Rectangle(8, 0, 8, 8);
            int buffer = 1;
            Velocity.X = GameWorld.RandGen.Next((int)-source.GetVelocity().X - buffer, (int)-source.GetVelocity().X + buffer + 1) * (float)GameWorld.RandGen.NextDouble();
            Velocity.Y = GameWorld.RandGen.Next((int)-source.GetVelocity().Y - buffer, (int)-source.GetVelocity().Y + buffer + 1);
            Position = new Vector2(CollRectangle.X, CollRectangle.Y);
            Opacity = .5f;

            light = new Lights.DynamicPointLight(this, .5f, false, color, 1);
            GameWorld.Instance.LightEngine.AddDynamicLight(light);
        }

        public override void Update(GameTime gameTime)
        {
            DefaultBehavior();
        }
    }

    public class FlameParticle : Particle
    {
        /// <summary>
        /// Creates a new flame particle that hurts the player.
        /// </summary>
        /// <param name="source">The origin of the flame particle.</param>
        /// <param name="sourceRectangle">The position of the texture in the spritesheet.</param>
        public FlameParticle(Entity source, Color color)
        {
            SourceRectangle = new Rectangle(32 * 8, 12 * 8, 8, 8);
            Texture = GameWorld.SpriteSheet;
            CollRectangle = new Rectangle(source.GetCollRectangle().Center.X - 4, source.GetCollRectangle().Y - 8, 8, 8);
            Position = new Vector2(CollRectangle.X, CollRectangle.Y);
            Opacity = 1;

            Velocity.X = (float)(GameWorld.RandGen.Next(-1, 2) * GameWorld.RandGen.NextDouble());
            Velocity.Y = -3f;

            light = new DynamicPointLight(this, .5f, false, color, .5f);
            GameWorld.Instance.LightEngine.AddDynamicLight(light);
        }

        public override void Update(GameTime gameTime)
        {
            DefaultBehavior();
        }
    }

    public class EntityFlameParticle : Particle
    {
        public EntityFlameParticle(Entity source, Color color)
        {
            SourceRectangle = new Rectangle(32 * 8, 12 * 8, 8, 8);
            Texture = GameWorld.SpriteSheet;
            int randX = (int)(GameWorld.RandGen.Next(0, source.GetCollRectangle().Width) * GameWorld.RandGen.NextDouble());
            int randY = (int)(GameWorld.RandGen.Next(0, source.GetCollRectangle().Height) * GameWorld.RandGen.NextDouble());
            CollRectangle = new Rectangle(source.GetCollRectangle().X + randX - 4, source.GetCollRectangle().Y + randY - 4, 8, 8);
            Position = new Vector2(CollRectangle.X, CollRectangle.Y);
            Opacity = .5f;

            Velocity.X = (float)(GameWorld.RandGen.Next(-1, 2) * GameWorld.RandGen.NextDouble());
            Velocity.Y = -3f;

            light = new DynamicPointLight(this, .5f, false, color, .5f);
            GameWorld.Instance.LightEngine.AddDynamicLight(light);
        }

        public override void Update(GameTime gameTime)
        {
            DefaultBehavior();
        }
    }

    public class MachineGunParticle : Particle
    {
        SoundFx _hitSound;

        public MachineGunParticle(Entity source, int xCoor)
        {
            SourceRectangle = new Rectangle(264, 96, 8, 8);
            Texture = GameWorld.SpriteSheet;
            CollRectangle = new Rectangle(source.GetCollRectangle().X + xCoor - 4, source.GetCollRectangle().Y + 4, 8, 8);
            Position = new Vector2(CollRectangle.X, CollRectangle.Y);
            Opacity = 1f;
            Velocity.Y = -8f;
            _hitSound = new SoundFx("Sounds/Machine Gun/bulletHit",this);

            light = new DynamicPointLight(this, .05f, false, Color.White, .5f);
            GameWorld.Instance.LightEngine.AddDynamicLight(light);
        }

        public override void Update(GameTime gameTime)
        {
            NoOpacityNoFrictionBehavior();

            if (CollRectangle.Intersects(GameWorld.Instance.Player.GetCollRectangle()))
            {
                
            }

            base.Update();
        }
    }

    public class ExplosionParticle : Particle
    {
        public ExplosionParticle(int x, int y, Color color, float intensity)
        {
            CollRectangle = new Rectangle(x - 4, y - 4, 8, 8);
            Position = new Vector2(CollRectangle.X, CollRectangle.Y);
            Opacity = 1;
            SourceRectangle = new Rectangle(32 * 8, 12 * 8, 8, 8);
            Texture = GameWorld.SpriteSheet;

            light = new DynamicPointLight(this, intensity, false, color, intensity);
            GameWorld.Instance.LightEngine.AddDynamicLight(light);
        }

        public override void Update(GameTime gameTime)
        {
            Opacity -= .5f;
            if (Opacity <= 0)
                ToDelete = true;
        }

    }

    public class SparkParticle : Particle
    {
        public SparkParticle(Entity source, Color color)
        {
            SourceRectangle = new Rectangle(272, 96, 8, 8);
            Texture = GameWorld.SpriteSheet;
            CollRectangle = new Rectangle(source.GetCollRectangle().X  - 4, source.GetCollRectangle().Y + 4, 8, 8);
            Velocity.X = GameWorld.RandGen.Next(-6, 7);
            Position = new Vector2(CollRectangle.X, CollRectangle.Y);
            Opacity = .7f;

            light = new DynamicPointLight(this, .05f, false, color, .5f);
            GameWorld.Instance.LightEngine.AddDynamicLight(light);
        }

        public override void Update(GameTime gameTime)
        {
            DefaultBehavior();
            Velocity.Y += .3f;
            base.Update();
        }
    }
    
}
