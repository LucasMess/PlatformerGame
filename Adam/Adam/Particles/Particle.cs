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
using Adam.Misc;

namespace Adam
{
    public class Particle : Entity
    {
        Texture2D nextTexture;
        Vector2 originalPosition, originalVelocity;
        Vector2 differenceInPosition, endPosition;
        Random randGen;
        double frameTimer;
        double opacityTimer;
        double travelTimer;
        double respawnTimer;
        Vector2 frameCount;
        protected Vector2 position;
        int currentFrame;
        public bool isComplete;
        bool hasChangedDirection;
        bool dead;
        float rotation, rotationSpeed, rotationDelta;
        Color color = Color.White;
        public Light light;
        Player player;

        public enum ParticleType
        {
            ChestSparkles,
            Impact,
            GameZZZ,
            MenuZZZ,
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
                return collRectangle;
            }
        }

        public Particle()
        {

        }

        public Particle(Projectile projectile, Tile[] tileArray)
        {
            sourceRectangle = new Rectangle(0, 0, 32, 32);
            CurrentParticle = ParticleType.Impact;
            Texture = ContentHelper.LoadTexture("Explosion");
            if (projectile.GetVelocity().X > 0)
                collRectangle = new Rectangle(tileArray[projectile.tileHit].drawRectangle.X - 32, projectile.GetCollRectangle().Center.Y - 32, 64, 64);
            else collRectangle = new Rectangle(tileArray[projectile.tileHit].drawRectangle.X + 32, projectile.GetCollRectangle().Center.Y - 32, 64, 64);
            frameCount = new Vector2(Texture.Width / 32, Texture.Height / 32);
        }

        public Particle(Enemy enemy, Projectile projectile)
        {
            sourceRectangle = new Rectangle(0, 0, 32, 32);
            CurrentParticle = ParticleType.Impact;
            Texture = ContentHelper.LoadTexture("Explosion");

            if (projectile.GetVelocity().X > 0)
                collRectangle = new Rectangle(enemy.GetCollRectangle().X - enemy.GetCollRectangle().Width / 2, projectile.GetCollRectangle().Center.Y - sourceRectangle.Height / 2, 32, 32);
            else collRectangle = new Rectangle(enemy.GetCollRectangle().X + enemy.GetCollRectangle().Width / 2, projectile.GetCollRectangle().Center.Y - sourceRectangle.Height / 2, 32, 32);
            frameCount = new Vector2(Texture.Width / 32, Texture.Height / 32);
        }

        public Particle(Projectile proj)
        {
            CurrentParticle = ParticleType.SnakeVenom;
            Texture = ContentHelper.LoadTexture("Effects/venom_blob");
            randGen = new Random();
            collRectangle = new Rectangle(proj.GetCollRectangle().X + proj.GetCollRectangle().Width / 2, proj.GetCollRectangle().Y, 8, 8);
            if (proj.GetVelocity().X > 0)
                velocity.X = -2;
            else velocity.X = 2;
            velocity.Y = GameWorld.RandGen.Next(1, 4);
            if (GameWorld.RandGen.Next(0, 2) == 0)
                velocity.Y = -velocity.Y;
            sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Opacity = 1f;
        }

        public Particle(Player player)
        {
            randGen = new Random();
            sourceRectangle = new Rectangle(0, 0, 14, 14);
            CurrentParticle = ParticleType.GameZZZ;
            Texture = ContentHelper.LoadTexture("Effects/new_Z");
            color = new Color(255, 255, 255, 255);
            if (player.IsFacingRight)
                position = new Vector2(player.GetCollRectangle().X + randGen.Next(-5, 5) + player.GetCollRectangle().Width - 8, player.GetCollRectangle().Y + randGen.Next(-5, 5));
            else position = new Vector2(player.GetCollRectangle().Center.X + randGen.Next(0, 20), player.GetCollRectangle().Y + randGen.Next(-5, 5));
            collRectangle = new Rectangle((int)position.X, (int)position.Y, 8, 8);

        }

        public Particle(Rectangle rectangle)
        {
            randGen = new Random();
            sourceRectangle = new Rectangle(0, 0, 14, 14);
            CurrentParticle = ParticleType.MenuZZZ;
            Texture = ContentHelper.LoadTexture("Effects/new_Z");
            color = new Color(255, 255, 255, 255);
            position = new Vector2(rectangle.X + randGen.Next(-5, 5) + rectangle.Width - 8, rectangle.Y + randGen.Next(-5, 5));
            collRectangle = new Rectangle((int)position.X, (int)position.Y, 32, 32);
        }

        public void CreateWeaponBurstEffect(Player player, Projectile proj, ContentManager Content)
        {
            position = player.weapon.tipPos;
            velocity = new Vector2(0, 0);
            int maxTanSpeed = 10;
            velocity.X = (float)(GameWorld.RandGen.Next((int)(proj.GetVelocity().X - maxTanSpeed), (int)(proj.GetVelocity().X + maxTanSpeed + 1)));
            velocity.Y = (float)(GameWorld.RandGen.Next((int)(proj.GetVelocity().Y - maxTanSpeed), (int)(proj.GetVelocity().Y + maxTanSpeed + 1)));
            CurrentParticle = ParticleType.WeaponBurst;
            Texture = ContentHelper.LoadTexture("Effects/laser_burst");
            int randSize = GameWorld.RandGen.Next(2, 5);
            collRectangle = new Rectangle((int)position.X, (int)position.Y, randSize, randSize);
            sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Opacity = 1;
        }

        public void CreateEnemyDisintegrationEffect(Enemy enemy, Rectangle sourceRectangle, Projectile proj)
        {
            CurrentParticle = ParticleType.EnemyDesintegration;
            Texture = enemy.Texture;
            collRectangle = new Rectangle(enemy.GetCollRectangle().X + sourceRectangle.X, enemy.GetCollRectangle().Y + sourceRectangle.Y,
                sourceRectangle.Width, sourceRectangle.Height);
            this.sourceRectangle = sourceRectangle;
            int maxTanSpeed = 10;
            velocity.X = (float)(GameWorld.RandGen.Next((int)(proj.GetVelocity().X - maxTanSpeed), (int)(proj.GetVelocity().X + maxTanSpeed + 1)));
            velocity.Y = (float)(GameWorld.RandGen.Next((int)(proj.GetVelocity().Y - maxTanSpeed), (int)(proj.GetVelocity().Y + maxTanSpeed + 1)));
        }

        public void CreateEnemyDeathEffect(Enemy enemy, Rectangle sourceRectangle)
        {
            CurrentParticle = ParticleType.EnemyDesintegration;
            Texture = enemy.Texture;
            collRectangle = new Rectangle(enemy.GetCollRectangle().X + sourceRectangle.X, enemy.GetCollRectangle().Y + sourceRectangle.Y,
                sourceRectangle.Width, sourceRectangle.Height);
            this.sourceRectangle = sourceRectangle;
            int maxTanSpeed = 5;
            velocity = new Vector2(GameWorld.RandGen.Next(-maxTanSpeed, maxTanSpeed), GameWorld.RandGen.Next(-maxTanSpeed, maxTanSpeed));
        }

        public void CreateBloodEffect(Player player, GameWorld map)
        {
            CurrentParticle = ParticleType.Blood;
            Texture = ContentHelper.LoadTexture("Effects/blood");
            collRectangle = new Rectangle(player.GetCollRectangle().Center.X, player.GetCollRectangle().Center.Y, 8, 8);
            sourceRectangle = new Rectangle(GameWorld.RandGen.Next(0, 4) * 8, 0, 8, 8);
            collRectangle = collRectangle;
            velocity = new Vector2(GameWorld.RandGen.Next(-10, 10), GameWorld.RandGen.Next(-10, 10));
            position = new Vector2(collRectangle.X, collRectangle.Y);
        }

        public void CreatePlayerChronoshiftEffect(Player player, Rectangle sourceRectangle)
        {
            CurrentParticle = ParticleType.PlayerChronoshift;

        }

        public void CreateTileParticleEffect(Tile tile, Player player)
        {
            CurrentParticle = ParticleType.TileParticle;
            Texture = tile.texture;
            collRectangle = new Rectangle(player.GetCollRectangle().Center.X - 2, player.GetCollRectangle().Y + player.GetCollRectangle().Height, 8, 8);
            sourceRectangle = new Rectangle(tile.sourceRectangle.X, tile.sourceRectangle.Y, 4, 4);
            if (tile is SpecialTile)
            {
                SpecialTile t = (SpecialTile)tile;
                sourceRectangle = new Rectangle(t.sourceRectangle.X, t.sourceRectangle.Y, 4, 4);
            }
            sourceRectangle.X += (GameWorld.RandGen.Next(0, 4) * Main.Tilesize / 4);
            velocity.X = (-player.GetVelocity().X / 2) * (float)GameWorld.RandGen.NextDouble();
            velocity.Y = GameWorld.RandGen.Next(-1, 1);
            Opacity = 1;
        }

        public void CreatePlayerDesintegrationEffect(Player player, Rectangle sourceRectangle)
        {
            this.player = player;
            CurrentParticle = ParticleType.PlayerDesintegration;
            Texture = player.GetSingleTexture();
            collRectangle = new Rectangle(player.GetCollRectangle().X + sourceRectangle.X, player.GetCollRectangle().Y + sourceRectangle.Y,
                sourceRectangle.Width, sourceRectangle.Height);
            this.sourceRectangle = sourceRectangle;
            int maxTanSpeed = 5;
            velocity = new Vector2(GameWorld.RandGen.Next(-maxTanSpeed, maxTanSpeed), GameWorld.RandGen.Next(-maxTanSpeed, maxTanSpeed));
            originalPosition = new Vector2(collRectangle.X, collRectangle.Y);
            differenceInPosition = new Vector2(player.respawnPos.X - player.GetCollRectangle().X, player.respawnPos.Y - player.GetCollRectangle().Y);
            endPosition = originalPosition + differenceInPosition;
            position = new Vector2(collRectangle.X, collRectangle.Y);
        }

        public void CreateMusicNotesEffect(CDPlayer cd)
        {
            CurrentParticle = ParticleType.MusicNotes;
            Texture = ContentHelper.LoadTexture("Effects/music_notes");
            collRectangle = new Rectangle(cd.GetCollRectangle().X, cd.GetCollRectangle().Y, 16, 16);
            sourceRectangle = new Rectangle(32 * GameWorld.RandGen.Next(0, 2), 0, 32, 32);
            int randX = GameWorld.RandGen.Next(0, 2);
            float velocityX = .5f;
            if (randX == 0)
            {
                collRectangle.X -= collRectangle.Width;
                velocity.X = -velocityX;
            }
            else
            {
                collRectangle.X += collRectangle.Width;
                velocity.X = velocityX;
            }

            velocity.Y = -(float)GameWorld.RandGen.NextDouble();
            position = new Vector2(collRectangle.X, collRectangle.Y);
        }

        public void CreateJetPackSmokeParticle(JetpackPowerUp jet)
        {
            CurrentParticle = ParticleType.JetpackSmoke;
            Texture = ContentHelper.LoadTexture("Effects/smoke");
            collRectangle = new Rectangle(jet.GetCollRectangle().Center.X - 8, jet.GetCollRectangle().Y + jet.GetCollRectangle().Height, 16, 16);
            sourceRectangle = new Rectangle(GameWorld.RandGen.Next(0, 4) * 16, 0, 16, 16);
            velocity.Y = 3f;
            velocity.X = GameWorld.RandGen.Next(-1, 2);
            position = new Vector2(collRectangle.X, collRectangle.Y);
            Opacity = 1f;
        }

        public void CreateJetPackSmokeParticle(Player player)
        {
            CurrentParticle = ParticleType.JetpackSmoke;
            Texture = ContentHelper.LoadTexture("Effects/smoke");
            collRectangle = new Rectangle(player.GetCollRectangle().Center.X - 8, player.GetCollRectangle().Y + player.GetCollRectangle().Height, 16, 16);
            sourceRectangle = new Rectangle(GameWorld.RandGen.Next(0, 4) * 16, 0, 16, 16);
            velocity.Y = 3f;
            velocity.X = GameWorld.RandGen.Next(-1, 2);
            position = new Vector2(collRectangle.X, collRectangle.Y);
            Opacity = 1f;
        }

        public void CreateLavaParticle(Liquid lava, GameWorld map)
        {
            CurrentParticle = ParticleType.Lava;
            //texture = ContentHelper.LoadTexture("Effects/lava");
            Texture = GameWorld.Particle_SpriteSheet;
            collRectangle = new Rectangle(lava.GetCollRectangle().Center.X - 8, lava.GetCollRectangle().Center.Y - 8, 8, 8);
            sourceRectangle = new Rectangle(0, 0, 8, 8);
            velocity.Y = -10f;
            position = new Vector2(collRectangle.X, collRectangle.Y);
            Opacity = 1f;
            light = new Lights.DynamicPointLight(this, 1, true, Color.Orange, .3f);
            GameWorld.Instance.lightEngine.AddDynamicLight(light);
        }

        public void CreateDeathSmoke(Entity entity)
        {
            CurrentParticle = ParticleType.DeathSmoke;
            Texture = ContentHelper.LoadTexture("Effects/smoke");
            collRectangle = new Rectangle(GameWorld.RandGen.Next(entity.GetCollRectangle().X, entity.GetCollRectangle().Right - 16), GameWorld.RandGen.Next(entity.GetCollRectangle().Y, entity.GetCollRectangle().Bottom - 16), 16, 16);
            sourceRectangle = new Rectangle(GameWorld.RandGen.Next(0, 4) * 16, 0, 16, 16);
            velocity.X = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-2, 3));
            velocity.Y = -.5f;
            position = new Vector2(collRectangle.X, collRectangle.Y);
            Opacity = 2;
        }

        public void CreateTookDamage(Entity entity)
        {
            CurrentParticle = ParticleType.TookDamage;
            Texture = ContentHelper.LoadTexture("Sparkles");
            collRectangle = new Rectangle(GameWorld.RandGen.Next(entity.GetCollRectangle().X, entity.GetCollRectangle().Right - 8), GameWorld.RandGen.Next(entity.GetCollRectangle().Y, entity.GetCollRectangle().Bottom - 8), 8, 8);
            sourceRectangle = new Rectangle(0, 0, 8, 8);
            velocity.X = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-4, 5));
            velocity.Y = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-1, 2));
            position = new Vector2(collRectangle.X, collRectangle.Y);
            Opacity = 1;
            color = Color.Red;
        }

        public void CreateSparkles(Entity entity)
        {
            CurrentParticle = ParticleType.Sparkles;
            Texture = ContentHelper.LoadTexture("Sparkles");
            collRectangle = new Rectangle(GameWorld.RandGen.Next(entity.GetCollRectangle().X, entity.GetCollRectangle().Right - 8), GameWorld.RandGen.Next(entity.GetCollRectangle().Y, entity.GetCollRectangle().Bottom - 8), 8, 8);
            sourceRectangle = new Rectangle(0, 0, 8, 8);
            velocity.X = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-4, 5));
            velocity.Y = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-7, 0));
            position = new Vector2(collRectangle.X, collRectangle.Y);
            Opacity = 2;
            color = Color.White;
        }

        public virtual void Update(GameTime gameTime)
        {
            GameWorld gameWorld = GameWorld.Instance;
            switch (CurrentParticle)
            {
                case ParticleType.ChestSparkles:
                    collRectangle.X += (int)velocity.X;
                    collRectangle.Y += (int)velocity.Y;
                    velocity.Y += .3f;
                    Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    velocity.X = velocity.X * 0.99f;
                    light.Update(this);
                    break;
                case ParticleType.Impact:
                    Animate(gameTime);
                    break;
                case ParticleType.GameZZZ:
                    collRectangle.X = (int)position.X;
                    collRectangle.Y = (int)position.Y;
                    position += velocity;
                    velocity = new Vector2(randGen.Next(-1, 2), -.3f);
                    opacityTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (opacityTimer > 1)
                    {
                        Opacity -= .1f;
                    }
                    if (Opacity < 0)
                        ToDelete = true;
                    break;
                case ParticleType.MenuZZZ:
                    collRectangle.X = (int)position.X;
                    collRectangle.Y = (int)position.Y;
                    position += velocity;
                    velocity = new Vector2(randGen.Next(-1, 2), -1f);
                    opacityTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (opacityTimer > 1)
                    {
                        Opacity -= .1f;
                    }
                    if (Opacity < 0)
                        ToDelete = true;
                    break;
                case ParticleType.SnakeVenom:
                    collRectangle.X += (int)velocity.X;
                    collRectangle.Y += (int)velocity.Y;
                    Opacity -= .03f;
                    velocity.Y += .09f;

                    if (Opacity < 0)
                        ToDelete = true;
                    break;
                case ParticleType.WeaponBurst:
                    collRectangle.X += (int)velocity.X;
                    collRectangle.Y += (int)velocity.Y;
                    Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    velocity.X = velocity.X * 0.99f;
                    velocity.Y = velocity.Y * 0.99f;
                    if (Opacity < 0)
                        ToDelete = true;
                    light.Update(this);
                    break;
                case ParticleType.EnemyDesintegration:
                    collRectangle.X += (int)velocity.X;
                    collRectangle.Y += (int)velocity.Y;
                    Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    velocity.X = velocity.X * 0.99f;
                    velocity.Y = velocity.Y * 0.99f;
                    if (Opacity < 0)
                        ToDelete = true;
                    break;
                case ParticleType.PlayerChronoshift:

                    break;
                case ParticleType.TileParticle:
                    collRectangle.X += (int)velocity.X;
                    collRectangle.Y += (int)velocity.Y;
                    Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //velocity.X = velocity.X * 0.99f;
                    //velocity.Y = velocity.Y * 0.99f;
                    if (Opacity < 0)
                        ToDelete = true;
                    break;
                case ParticleType.PlayerDesintegration:
                    position += velocity;

                    collRectangle.X = (int)position.X;
                    collRectangle.Y = (int)position.Y;
                    if (player.isWaitingForRespawn)
                        respawnTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (respawnTimer > 500)
                    {
                        if (!player.isWaitingForRespawn)
                        {
                            Opacity = 0f;
                            ToDelete = true;
                            respawnTimer = 0;
                        }
                        else
                        {
                            // velocity = new Vector2((endPosition.X - position.X) / 50, (endPosition.Y - position.Y) / 50);
                        }
                    }
                    else
                    {
                        velocity.X = velocity.X * 0.99f;
                        velocity.Y = velocity.Y * 0.99f;
                    }
                    break;
                case ParticleType.MusicNotes:
                    position += velocity;

                    collRectangle.X = (int)position.X;
                    collRectangle.Y = (int)position.Y;

                    Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Opacity <= 0)
                        ToDelete = true;
                    break;
                case ParticleType.JetpackSmoke:
                    position += velocity;

                    collRectangle.X = (int)position.X;
                    collRectangle.Y = (int)position.Y;

                    velocity.X = velocity.X * 0.99f;
                    velocity.Y = velocity.Y * 0.99f;

                    Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Opacity <= 0)
                        ToDelete = true;
                    break;
                case ParticleType.Blood:
                    if (dead)
                        return;

                    position += velocity;

                    collRectangle.X = (int)position.X;
                    collRectangle.Y = (int)position.Y;

                    collRectangle = collRectangle;

                    velocity.Y += .3f;

                    if (this.IsTouchingTerrain(gameWorld))
                    {
                        velocity.X = 0;
                        velocity.Y = 0;
                        dead = true;
                    }
                    break;
                case ParticleType.Lava:
                    position += velocity;

                    collRectangle.X = (int)position.X;
                    collRectangle.Y = (int)position.Y;

                    collRectangle = collRectangle;

                    velocity.Y += .3f;

                    Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Opacity <= 0)
                        ToDelete = true;
                    break;
                case ParticleType.DeathSmoke:
                    position += velocity;

                    collRectangle.X = (int)position.X;
                    collRectangle.Y = (int)position.Y;

                    velocity.X = velocity.X * 0.95f;
                    velocity.Y = velocity.Y * 0.99f;

                    Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Opacity <= 0)
                        ToDelete = true;
                    break;
                case ParticleType.TookDamage:
                    position += velocity;

                    collRectangle.X = (int)position.X;
                    collRectangle.Y = (int)position.Y;

                    velocity.X = velocity.X * 0.95f;
                    velocity.Y = velocity.Y * 0.99f;

                    Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Opacity <= 0)
                        ToDelete = true;
                    break;
                case ParticleType.Sparkles:
                    position += velocity;

                    collRectangle.X = (int)position.X;
                    collRectangle.Y = (int)position.Y;

                    velocity.X = velocity.X * 0.99f;
                    velocity.Y = velocity.Y * 0.99f;

                    Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Opacity <= 0)
                        ToDelete = true;
                    break;
            }
            collRectangle = collRectangle;
        }

        public virtual void Animate(GameTime gameTime)
        {
            int switchFrame = 10;
            frameTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (frameTimer >= switchFrame)
            {
                if (frameCount.X != 0)
                {
                    frameTimer = 0;
                    sourceRectangle.X += Texture.Width / (int)frameCount.X;
                    currentFrame++;
                }
            }
            if (currentFrame >= frameCount.X)
            {
                Opacity = 0;
            }

        }

        protected void DefaultBehavior()
        {
            GameTime gameTime = GameWorld.Instance.gameTime;
            position += velocity;

            collRectangle.X = (int)position.X;
            collRectangle.Y = (int)position.Y;

            velocity.X = velocity.X * 0.99f;
            velocity.Y = velocity.Y * 0.99f;

            Opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Opacity <= 0)
                ToDelete = true;
        }

        protected void NoOpacityNoFrictionBehavior()
        {
            GameTime gameTime = GameWorld.Instance.gameTime;
            position += velocity;

            collRectangle.X = (int)position.X;
            collRectangle.Y = (int)position.Y;

            if (Opacity <= 0)
                ToDelete = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentParticle == ParticleType.SnakeVenom)
                spriteBatch.Draw(Texture, collRectangle, sourceRectangle, color * Opacity);
            else
                spriteBatch.Draw(Texture, collRectangle, sourceRectangle, color * Opacity);

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
            collRectangle = new Rectangle(entity.GetCollRectangle().Center.X - 4, entity.GetCollRectangle().Bottom - 4, 8, 8);
            sourceRectangle = new Rectangle(8 * GameWorld.RandGen.Next(0, 4), 0, 8, 8);
            position = new Vector2(collRectangle.X, collRectangle.Y);

            velocity.X = (float)GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-3 - (int)entity.GetVelocity().X / 2, 3 - (int)entity.GetVelocity().X / 2);
            velocity.Y = (float)GameWorld.RandGen.NextDouble() * -1f;
            Opacity = 1f;
        }

        public override void Update(GameTime gameTime)
        {
            DefaultBehavior();
        }

    }

    public class StompSmokeParticle : Particle
    {
        public StompSmokeParticle(Entity entity)
        {
            Texture = ContentHelper.LoadTexture("Effects/smoke");
            collRectangle = new Rectangle(entity.GetCollRectangle().Center.X - 4, entity.GetCollRectangle().Bottom - 4, 8, 8);
            sourceRectangle = new Rectangle(8 * GameWorld.RandGen.Next(0, 4), 0, 8, 8);
            position = new Vector2(collRectangle.X, collRectangle.Y);

            velocity.X = (float)GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-3, 3);
            velocity.Y = (float)GameWorld.RandGen.NextDouble() * -1f;

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
                GameWorld.Instance.particles.Add(par);
            }
        }
    }

    public class ConstructionSmokeParticle : Particle
    {
        public ConstructionSmokeParticle(Rectangle rect)
        {
            Texture = ContentHelper.LoadTexture("Effects/smoke");
            collRectangle = new Rectangle(GameWorld.RandGen.Next(rect.X, rect.Right - 16), GameWorld.RandGen.Next(rect.Y, rect.Bottom - 16), 16, 16);
            sourceRectangle = new Rectangle(GameWorld.RandGen.Next(0, 4) * 16, 0, 16, 16);
            velocity.X = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-5, 6));
            velocity.Y = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-5, 6));
            position = new Vector2(collRectangle.X, collRectangle.Y);
            Opacity = 2;
        }

        public override void Update(GameTime gameTime)
        {
            DefaultBehavior();

            velocity.Y += .3f;
        }
    }

    public class DestructionTileParticle : Particle
    {
        public DestructionTileParticle(Tile tile, Rectangle source)
        {
            Texture = tile.texture;
            collRectangle = new Rectangle(tile.drawRectangle.Center.X, tile.drawRectangle.Center.Y, 8, 8);
            sourceRectangle = source;
            velocity.X = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-5, 6));
            velocity.Y = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-5, 6));
            position = new Vector2(collRectangle.X, collRectangle.Y);
            Opacity = 2;

        }
        public override void Update(GameTime gameTime)
        {
            DefaultBehavior();

            velocity.Y += .3f;
        }
    }

    public class TrailParticle : Particle
    {
        public TrailParticle(Entity source, Color color)
        {
            Texture = GameWorld.Particle_SpriteSheet;
            collRectangle = new Rectangle(source.GetCollRectangle().Center.X, source.GetCollRectangle().Center.Y, 8, 8);
            sourceRectangle = new Rectangle(8, 0, 8, 8);
            int buffer = 1;
            velocity.X = GameWorld.RandGen.Next((int)-source.GetVelocity().X - buffer, (int)-source.GetVelocity().X + buffer + 1) * (float)GameWorld.RandGen.NextDouble();
            velocity.Y = GameWorld.RandGen.Next((int)-source.GetVelocity().Y - buffer, (int)-source.GetVelocity().Y + buffer + 1);
            position = new Vector2(collRectangle.X, collRectangle.Y);
            Opacity = .5f;

            light = new Lights.DynamicPointLight(this, .5f, false, color, 1);
            GameWorld.Instance.lightEngine.AddDynamicLight(light);
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
            sourceRectangle = new Rectangle(32 * 8, 12 * 8, 8, 8);
            Texture = GameWorld.SpriteSheet;
            collRectangle = new Rectangle(source.GetCollRectangle().Center.X - 4, source.GetCollRectangle().Y - 8, 8, 8);
            position = new Vector2(collRectangle.X, collRectangle.Y);
            Opacity = 1;

            velocity.X = (float)(GameWorld.RandGen.Next(-1, 2) * GameWorld.RandGen.NextDouble());
            velocity.Y = -3f;

            light = new DynamicPointLight(this, .5f, false, color, .5f);
            GameWorld.Instance.lightEngine.AddDynamicLight(light);
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
            sourceRectangle = new Rectangle(32 * 8, 12 * 8, 8, 8);
            Texture = GameWorld.SpriteSheet;
            int randX = (int)(GameWorld.RandGen.Next(0, source.GetCollRectangle().Width) * GameWorld.RandGen.NextDouble());
            int randY = (int)(GameWorld.RandGen.Next(0, source.GetCollRectangle().Height) * GameWorld.RandGen.NextDouble());
            collRectangle = new Rectangle(source.GetCollRectangle().X + randX - 4, source.GetCollRectangle().Y + randY - 4, 8, 8);
            position = new Vector2(collRectangle.X, collRectangle.Y);
            Opacity = .5f;

            velocity.X = (float)(GameWorld.RandGen.Next(-1, 2) * GameWorld.RandGen.NextDouble());
            velocity.Y = -3f;

            light = new DynamicPointLight(this, .5f, false, color, .5f);
            GameWorld.Instance.lightEngine.AddDynamicLight(light);
        }

        public override void Update(GameTime gameTime)
        {
            DefaultBehavior();
        }
    }

    public class MachineGunParticle : Particle, ICollidable
    {
        SoundFx hitSound;

        public MachineGunParticle(Entity source, int xCoor)
        {
            sourceRectangle = new Rectangle(264, 96, 8, 8);
            Texture = GameWorld.SpriteSheet;
            collRectangle = new Rectangle(source.GetCollRectangle().X + xCoor - 4, source.GetCollRectangle().Y + 4, 8, 8);
            position = new Vector2(collRectangle.X, collRectangle.Y);
            Opacity = 1f;
            velocity.Y = -8f;
            hitSound = new SoundFx("Sounds/Machine Gun/bulletHit",this);

            light = new DynamicPointLight(this, .05f, false, Color.White, .5f);
            GameWorld.Instance.lightEngine.AddDynamicLight(light);
        }

        public void OnCollisionWithTerrainAbove(TerrainCollisionEventArgs e)
        {
        }

        public void OnCollisionWithTerrainAnywhere(TerrainCollisionEventArgs e)
        {
            Opacity = 0;
            ExplosionParticle par = new ExplosionParticle(collRectangle.X + 4, collRectangle.Y, Color.White, .5f);
            SparkParticle spar = new SparkParticle(this, Color.Orange);
            GameWorld.Instance.particles.Add(spar);
            GameWorld.Instance.particles.Add(par);
            hitSound.Play();
        }

        public void OnCollisionWithTerrainBelow(TerrainCollisionEventArgs e)
        {
        }

        public void OnCollisionWithTerrainLeft(TerrainCollisionEventArgs e)
        {
        }

        public void OnCollisionWithTerrainRight(TerrainCollisionEventArgs e)
        {
        }

        public override void Update(GameTime gameTime)
        {
            NoOpacityNoFrictionBehavior();

            if (collRectangle.Intersects(GameWorld.Instance.player.GetCollRectangle()))
            {
                GameWorld.Instance.player.TakeDamageAndKnockBack(EnemyDB.MachineGun_ProjDamage);
            }

            base.Update();
        }
    }

    public class ExplosionParticle : Particle
    {
        public ExplosionParticle(int x, int y, Color color, float intensity)
        {
            collRectangle = new Rectangle(x - 4, y - 4, 8, 8);
            position = new Vector2(collRectangle.X, collRectangle.Y);
            Opacity = 1;
            sourceRectangle = new Rectangle(32 * 8, 12 * 8, 8, 8);
            Texture = GameWorld.SpriteSheet;

            light = new DynamicPointLight(this, intensity, false, color, intensity);
            GameWorld.Instance.lightEngine.AddDynamicLight(light);
        }

        public override void Update(GameTime gameTime)
        {
            Opacity -= .5f;
            if (Opacity <= 0)
                ToDelete = true;
        }

    }

    public class SparkParticle : Particle , ICollidable
    {
        public SparkParticle(Entity source, Color color)
        {
            sourceRectangle = new Rectangle(272, 96, 8, 8);
            Texture = GameWorld.SpriteSheet;
            collRectangle = new Rectangle(source.GetCollRectangle().X  - 4, source.GetCollRectangle().Y + 4, 8, 8);
            velocity.X = GameWorld.RandGen.Next(-6, 7);
            position = new Vector2(collRectangle.X, collRectangle.Y);
            Opacity = .7f;

            light = new DynamicPointLight(this, .05f, false, color, .5f);
            GameWorld.Instance.lightEngine.AddDynamicLight(light);
        }

        public override void Update(GameTime gameTime)
        {
            DefaultBehavior();
            velocity.Y += .3f;
            base.Update();
        }

        public void OnCollisionWithTerrainAbove(TerrainCollisionEventArgs e)
        {
            velocity.Y = -velocity.Y * .1f;
        }

        public void OnCollisionWithTerrainAnywhere(TerrainCollisionEventArgs e)
        {

        }

        public void OnCollisionWithTerrainBelow(TerrainCollisionEventArgs e)
        {
            velocity.Y = -velocity.Y * .1f;
        }

        public void OnCollisionWithTerrainLeft(TerrainCollisionEventArgs e)
        {
            velocity.X = -velocity.X;
        }

        public void OnCollisionWithTerrainRight(TerrainCollisionEventArgs e)
        {
            velocity.X = -velocity.X;
        }
    }

    public class ChronoshiftParticle : Particle
    {
        double changeDirectionTimer;
        bool hasChangedDirection;

        public ChronoshiftParticle(Entity entity)
        {
            int maxVel = 10;
            velocity = new Vector2(GameWorld.RandGen.Next(-maxVel, maxVel + 1), GameWorld.RandGen.Next(-maxVel, maxVel + 1));
            collRectangle = new Rectangle(entity.GetCollRectangle().Center.X, entity.GetCollRectangle().Center.Y, 8, 8);
            collRectangle = collRectangle;
            position = new Vector2(collRectangle.X, collRectangle.Y);

            Color colorful = new Color(GameWorld.RandGen.Next(0, 256), GameWorld.RandGen.Next(0, 256), GameWorld.RandGen.Next(0, 256), 255);
            light = new Lights.DynamicPointLight(this, .5f, false, colorful, 1f);
            GameWorld.Instance.lightEngine.AddDynamicLight(light);
        }

        public override void Update(GameTime gameTime)
        {
            position += velocity;

            collRectangle.X = (int)position.X;
            collRectangle.Y = (int)position.Y;

            collRectangle = collRectangle;

            velocity.X = velocity.X * 0.95f;
            velocity.Y = velocity.Y * 0.95f;

            changeDirectionTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (changeDirectionTimer > 1000)
            {
                if (!hasChangedDirection)
                {
                    hasChangedDirection = true;
                    velocity = -velocity;

                    if (velocity == new Vector2(0, 0))
                    {
                        //velocity = new Vector2(1, 1);
                    }
                }
                GameWorld.Instance.player.chronoDeactivateSound.PlayIfStopped();
                velocity.X = velocity.X * 1 / .95f;
                velocity.Y = velocity.Y * 1 / .95f;
            }

            if (changeDirectionTimer > 1200)
            {
                GameWorld.Instance.player.isChronoshifting = false;

                ToDelete = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //DO NOTHING
        }
    }

}
