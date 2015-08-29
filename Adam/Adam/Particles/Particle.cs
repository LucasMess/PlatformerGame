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
            gameWorld = GameWorld.Instance;
        }

        public Particle(Projectile projectile, Tile[] tileArray)
        {
            sourceRectangle = new Rectangle(0, 0, 32, 32);
            CurrentParticle = ParticleType.Impact;
            Texture = ContentHelper.LoadTexture("Explosion");
            if (projectile.velocity.X > 0)
                collRectangle = new Rectangle(tileArray[projectile.tileHit].drawRectangle.X - 32, projectile.collRectangle.Center.Y - 32, 64, 64);
            else collRectangle = new Rectangle(tileArray[projectile.tileHit].drawRectangle.X + 32, projectile.collRectangle.Center.Y - 32, 64, 64);
            frameCount = new Vector2(Texture.Width / 32, Texture.Height / 32);
        }

        public Particle(Enemy enemy, Projectile projectile)
        {
            sourceRectangle = new Rectangle(0, 0, 32, 32);
            CurrentParticle = ParticleType.Impact;
            Texture = ContentHelper.LoadTexture("Explosion");

            if (projectile.velocity.X > 0)
                collRectangle = new Rectangle(enemy.collRectangle.X - enemy.collRectangle.Width / 2, projectile.collRectangle.Center.Y - sourceRectangle.Height / 2, 32, 32);
            else collRectangle = new Rectangle(enemy.collRectangle.X + enemy.collRectangle.Width / 2, projectile.collRectangle.Center.Y - sourceRectangle.Height / 2, 32, 32);
            frameCount = new Vector2(Texture.Width / 32, Texture.Height / 32);
        }

        public Particle(Projectile proj)
        {
            CurrentParticle = ParticleType.SnakeVenom;
            Texture = ContentHelper.LoadTexture("Effects/venom_blob");
            randGen = new Random();
            collRectangle = new Rectangle(proj.collRectangle.X + proj.collRectangle.Width / 2, proj.collRectangle.Y, 8, 8);
            if (proj.velocity.X > 0)
                velocity.X = -2;
            else velocity.X = 2;
            velocity.Y = GameWorld.RandGen.Next(1, 4);
            if (GameWorld.RandGen.Next(0, 2) == 0)
                velocity.Y = -velocity.Y;
            sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            opacity = 1f;
        }

        public Particle(Player player)
        {
            randGen = new Random();
            sourceRectangle = new Rectangle(0, 0, 14, 14);
            CurrentParticle = ParticleType.GameZZZ;
            Texture = ContentHelper.LoadTexture("Effects/new_Z");
            color = new Color(255, 255, 255, 255);
            if (player.isFacingRight)
                position = new Vector2(player.collRectangle.X + randGen.Next(-5, 5) + player.collRectangle.Width - 8, player.collRectangle.Y + randGen.Next(-5, 5));
            else position = new Vector2(player.collRectangle.Center.X + randGen.Next(0, 20), player.collRectangle.Y + randGen.Next(-5, 5));
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
            this.collRectangle = new Rectangle((int)position.X, (int)position.Y, 32, 32);
        }

        public void CreateWeaponBurstEffect(Player player, Projectile proj, ContentManager Content)
        {
            position = player.weapon.tipPos;
            velocity = new Vector2(0, 0);
            int maxTanSpeed = 10;
            velocity.X = (float)(GameWorld.RandGen.Next((int)(proj.velocity.X - maxTanSpeed), (int)(proj.velocity.X + maxTanSpeed + 1)));
            velocity.Y = (float)(GameWorld.RandGen.Next((int)(proj.velocity.Y - maxTanSpeed), (int)(proj.velocity.Y + maxTanSpeed + 1)));
            CurrentParticle = ParticleType.WeaponBurst;
            Texture = ContentHelper.LoadTexture("Effects/laser_burst");
            int randSize = GameWorld.RandGen.Next(2, 5);
            collRectangle = new Rectangle((int)position.X, (int)position.Y, randSize, randSize);
            sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            opacity = 1;
        }

        public void CreateEnemyDisintegrationEffect(Enemy enemy, Rectangle sourceRectangle, Projectile proj)
        {
            CurrentParticle = ParticleType.EnemyDesintegration;
            Texture = enemy.Texture;
            collRectangle = new Rectangle(enemy.collRectangle.X + sourceRectangle.X, enemy.collRectangle.Y + sourceRectangle.Y,
                sourceRectangle.Width, sourceRectangle.Height);
            this.sourceRectangle = sourceRectangle;
            int maxTanSpeed = 10;
            velocity.X = (float)(GameWorld.RandGen.Next((int)(proj.velocity.X - maxTanSpeed), (int)(proj.velocity.X + maxTanSpeed + 1)));
            velocity.Y = (float)(GameWorld.RandGen.Next((int)(proj.velocity.Y - maxTanSpeed), (int)(proj.velocity.Y + maxTanSpeed + 1)));
        }

        public void CreateEnemyDeathEffect(Enemy enemy, Rectangle sourceRectangle)
        {
            CurrentParticle = ParticleType.EnemyDesintegration;
            Texture = enemy.Texture;
            collRectangle = new Rectangle(enemy.collRectangle.X + sourceRectangle.X, enemy.collRectangle.Y + sourceRectangle.Y,
                sourceRectangle.Width, sourceRectangle.Height);
            this.sourceRectangle = sourceRectangle;
            int maxTanSpeed = 5;
            velocity = new Vector2(GameWorld.RandGen.Next(-maxTanSpeed, maxTanSpeed), GameWorld.RandGen.Next(-maxTanSpeed, maxTanSpeed));
        }

        public void CreateBloodEffect(Player player, GameWorld map)
        {
            CurrentParticle = ParticleType.Blood;
            Texture = ContentHelper.LoadTexture("Effects/blood");
            collRectangle = new Rectangle(player.collRectangle.Center.X, player.collRectangle.Center.Y, 8, 8);
            sourceRectangle = new Rectangle(GameWorld.RandGen.Next(0, 4) * 8, 0, 8, 8);
            collRectangle = collRectangle;
            velocity = new Vector2(GameWorld.RandGen.Next(-10, 10), GameWorld.RandGen.Next(-10, 10));
            position = new Vector2(collRectangle.X, collRectangle.Y);
            this.gameWorld = map;
        }

        public void CreatePlayerChronoshiftEffect(Player player, Rectangle sourceRectangle)
        {
            CurrentParticle = ParticleType.PlayerChronoshift;
           
        }

        public void CreateTileParticleEffect(Tile tile, Player player)
        {
            CurrentParticle = ParticleType.TileParticle;
            Texture = tile.texture;
            collRectangle = new Rectangle(player.collRectangle.Center.X - 2, player.collRectangle.Y + player.collRectangle.Height, 8, 8);
            sourceRectangle = new Rectangle(tile.sourceRectangle.X, tile.sourceRectangle.Y, 4, 4);
            if (tile is SpecialTile)
            {
                SpecialTile t = (SpecialTile)tile;
                sourceRectangle = new Rectangle(t.sourceRectangle.X, t.sourceRectangle.Y, 4, 4);
            }
            sourceRectangle.X += (GameWorld.RandGen.Next(0, 4) * Main.Tilesize / 4);
            velocity.X = (-player.velocity.X / 2) * (float)GameWorld.RandGen.NextDouble();
            velocity.Y = GameWorld.RandGen.Next(-1, 1);
            opacity = 1;
        }

        public void CreatePlayerDesintegrationEffect(Player player, Rectangle sourceRectangle)
        {
            this.player = player;
            CurrentParticle = ParticleType.PlayerDesintegration;
            Texture = player.GetSingleTexture();
            collRectangle = new Rectangle(player.collRectangle.X + sourceRectangle.X, player.collRectangle.Y + sourceRectangle.Y,
                sourceRectangle.Width, sourceRectangle.Height);
            this.sourceRectangle = sourceRectangle;
            int maxTanSpeed = 5;
            velocity = new Vector2(GameWorld.RandGen.Next(-maxTanSpeed, maxTanSpeed), GameWorld.RandGen.Next(-maxTanSpeed, maxTanSpeed));
            originalPosition = new Vector2(collRectangle.X, collRectangle.Y);
            differenceInPosition = new Vector2(player.respawnPos.X - player.collRectangle.X, player.respawnPos.Y - player.collRectangle.Y);
            endPosition = originalPosition + differenceInPosition;
            position = new Vector2(collRectangle.X, collRectangle.Y);
        }

        public void CreateMusicNotesEffect(CDPlayer cd)
        {
            CurrentParticle = ParticleType.MusicNotes;
            Texture = ContentHelper.LoadTexture("Effects/music_notes");
            collRectangle = new Rectangle(cd.collRectangle.X, cd.collRectangle.Y, 16, 16);
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
            collRectangle = new Rectangle(jet.collRectangle.Center.X - 8, jet.collRectangle.Y + jet.collRectangle.Height, 16, 16);
            sourceRectangle = new Rectangle(GameWorld.RandGen.Next(0, 4) * 16, 0, 16, 16);
            velocity.Y = 3f;
            velocity.X = GameWorld.RandGen.Next(-1, 2);
            position = new Vector2(collRectangle.X, collRectangle.Y);
            opacity = 1f;
        }

        public void CreateJetPackSmokeParticle(Player player)
        {
            CurrentParticle = ParticleType.JetpackSmoke;
            Texture = ContentHelper.LoadTexture("Effects/smoke");
            collRectangle = new Rectangle(player.collRectangle.Center.X - 8, player.collRectangle.Y + player.collRectangle.Height, 16, 16);
            sourceRectangle = new Rectangle(GameWorld.RandGen.Next(0, 4) * 16, 0, 16, 16);
            velocity.Y = 3f;
            velocity.X = GameWorld.RandGen.Next(-1, 2);
            position = new Vector2(collRectangle.X, collRectangle.Y);
            opacity = 1f;
        }

        public void CreateLavaParticle(Liquid lava, GameWorld map)
        {
            CurrentParticle = ParticleType.Lava;
            //texture = ContentHelper.LoadTexture("Effects/lava");
            Texture = GameWorld.Particle_SpriteSheet;
            collRectangle = new Rectangle(lava.collRectangle.Center.X - 8, lava.collRectangle.Center.Y - 8, 8, 8);
            sourceRectangle = new Rectangle(0, 0, 8, 8);
            velocity.Y = -10f;
            position = new Vector2(collRectangle.X, collRectangle.Y);
            opacity = 1f;
            light = new Lights.DynamicPointLight(this,1,true, Color.Orange, .3f);
            GameWorld.Instance.lightEngine.AddDynamicLight(light);
            this.gameWorld = map;
        }

        public void CreateDeathSmoke(Entity entity)
        {
            CurrentParticle = ParticleType.DeathSmoke;
            Texture = ContentHelper.LoadTexture("Effects/smoke");
            collRectangle = new Rectangle(GameWorld.RandGen.Next(entity.collRectangle.X, entity.collRectangle.Right - 16), GameWorld.RandGen.Next(entity.collRectangle.Y, entity.collRectangle.Bottom - 16), 16, 16);
            sourceRectangle = new Rectangle(GameWorld.RandGen.Next(0, 4) * 16, 0, 16, 16);
            velocity.X = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-2, 3));
            velocity.Y = -.5f;
            position = new Vector2(collRectangle.X, collRectangle.Y);
            opacity = 2;
        }

        public void CreateTookDamage(Entity entity)
        {
            CurrentParticle = ParticleType.TookDamage;
            Texture = ContentHelper.LoadTexture("Sparkles");
            collRectangle = new Rectangle(GameWorld.RandGen.Next(entity.collRectangle.X, entity.collRectangle.Right - 8), GameWorld.RandGen.Next(entity.collRectangle.Y, entity.collRectangle.Bottom - 8), 8, 8);
            sourceRectangle = new Rectangle(0, 0, 8, 8);
            velocity.X = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-4, 5));
            velocity.Y = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-1, 2));
            position = new Vector2(collRectangle.X, collRectangle.Y);
            opacity = 1;
            color = Color.Red;
        }

        public void CreateSparkles(Entity entity)
        {
            CurrentParticle = ParticleType.Sparkles;
            Texture = ContentHelper.LoadTexture("Sparkles");
            collRectangle = new Rectangle(GameWorld.RandGen.Next(entity.collRectangle.X, entity.collRectangle.Right - 8), GameWorld.RandGen.Next(entity.collRectangle.Y, entity.collRectangle.Bottom - 8), 8, 8);
            sourceRectangle = new Rectangle(0, 0, 8, 8);
            velocity.X = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-4, 5));
            velocity.Y = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-7, 0));
            position = new Vector2(collRectangle.X, collRectangle.Y);
            opacity = 2;
            color = Color.White;
        }

        public virtual void Update(GameTime gameTime)
        {
            gameWorld = GameWorld.Instance;
            switch (CurrentParticle)
            {
                case ParticleType.ChestSparkles:
                    collRectangle.X += (int)velocity.X;
                    collRectangle.Y += (int)velocity.Y;
                    velocity.Y += .3f;
                    opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
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
                        opacity -= .1f;
                    }
                    if (opacity < 0)
                        toDelete = true;
                    break;
                case ParticleType.MenuZZZ:
                    collRectangle.X = (int)position.X;
                    collRectangle.Y = (int)position.Y;
                    position += velocity;
                    velocity = new Vector2(randGen.Next(-1, 2), -1f);
                    opacityTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (opacityTimer > 1)
                    {
                        opacity -= .1f;
                    }
                    if (opacity < 0)
                        toDelete = true;
                    break;
                case ParticleType.SnakeVenom:
                    collRectangle.X += (int)velocity.X;
                    collRectangle.Y += (int)velocity.Y;
                    opacity -= .03f;
                    velocity.Y += .09f;

                    if (opacity < 0)
                        toDelete = true;
                    break;
                case ParticleType.WeaponBurst:
                    collRectangle.X += (int)velocity.X;
                    collRectangle.Y += (int)velocity.Y;
                    opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    velocity.X = velocity.X * 0.99f;
                    velocity.Y = velocity.Y * 0.99f;
                    if (opacity < 0)
                        toDelete = true;
                    light.Update(this);
                    break;
                case ParticleType.EnemyDesintegration:
                    collRectangle.X += (int)velocity.X;
                    collRectangle.Y += (int)velocity.Y;
                    opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    velocity.X = velocity.X * 0.99f;
                    velocity.Y = velocity.Y * 0.99f;
                    if (opacity < 0)
                        toDelete = true;
                    break;
                case ParticleType.PlayerChronoshift:
                    
                    break;
                case ParticleType.TileParticle:
                    collRectangle.X += (int)velocity.X;
                    collRectangle.Y += (int)velocity.Y;
                    opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //velocity.X = velocity.X * 0.99f;
                    //velocity.Y = velocity.Y * 0.99f;
                    if (opacity < 0)
                        toDelete = true;
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
                            opacity = 0f;
                            toDelete = true;
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

                    opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (opacity <= 0)
                        toDelete = true;
                    break;
                case ParticleType.JetpackSmoke:
                    position += velocity;

                    collRectangle.X = (int)position.X;
                    collRectangle.Y = (int)position.Y;

                    velocity.X = velocity.X * 0.99f;
                    velocity.Y = velocity.Y * 0.99f;

                    opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (opacity <= 0)
                        toDelete = true;
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

                    opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (opacity <= 0)
                        toDelete = true;
                    break;
                case ParticleType.DeathSmoke:
                    position += velocity;

                    collRectangle.X = (int)position.X;
                    collRectangle.Y = (int)position.Y;

                    velocity.X = velocity.X * 0.95f;
                    velocity.Y = velocity.Y * 0.99f;

                    opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (opacity <= 0)
                        toDelete = true;
                    break;
                case ParticleType.TookDamage:
                    position += velocity;

                    collRectangle.X = (int)position.X;
                    collRectangle.Y = (int)position.Y;

                    velocity.X = velocity.X * 0.95f;
                    velocity.Y = velocity.Y * 0.99f;

                    opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (opacity <= 0)
                        toDelete = true;
                    break;
                case ParticleType.Sparkles:
                    position += velocity;

                    collRectangle.X = (int)position.X;
                    collRectangle.Y = (int)position.Y;

                    velocity.X = velocity.X * 0.99f;
                    velocity.Y = velocity.Y * 0.99f;

                    opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (opacity <= 0)
                        toDelete = true;
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
                opacity = 0;
            }

        }

        protected void DefaultBehavior()
        {
            GameTime gameTime = GameWorld.Instance.gameTime;
            position += velocity;

            collRectangle.X = (int)position.X;
            collRectangle.Y = (int)position.Y;

            collRectangle = collRectangle;

            velocity.X = velocity.X * 0.99f;
            velocity.Y = velocity.Y * 0.99f;

            opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (opacity <= 0)
                toDelete = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentParticle == ParticleType.SnakeVenom)
                spriteBatch.Draw(Texture, collRectangle, sourceRectangle, color * opacity);                
            else
                spriteBatch.Draw(Texture, collRectangle, sourceRectangle, color * opacity);

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
            collRectangle = new Rectangle(entity.collRectangle.Center.X - 4, entity.collRectangle.Bottom - 4, 8, 8);
            sourceRectangle = new Rectangle(8 * GameWorld.RandGen.Next(0, 4), 0, 8, 8);
            collRectangle = collRectangle;
            position = new Vector2(collRectangle.X, collRectangle.Y);

            velocity.X = (float)GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-3 - (int)entity.velocity.X/2, 3 - (int)entity.velocity.X/2);
            velocity.Y = (float)GameWorld.RandGen.NextDouble() * -1f;
            opacity = 1f;
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
            collRectangle = new Rectangle(entity.collRectangle.Center.X - 4, entity.collRectangle.Bottom - 4, 8, 8);
            sourceRectangle = new Rectangle(8 * GameWorld.RandGen.Next(0, 4), 0, 8, 8);
            collRectangle = collRectangle;
            position = new Vector2(collRectangle.X, collRectangle.Y);

            velocity.X = (float)GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-3, 3);
            velocity.Y = (float)GameWorld.RandGen.NextDouble() * -1f;

            opacity = 1f;
        }

        public override void Update(GameTime gameTime)
        {
            DefaultBehavior();
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
            opacity = 2;
        }

        public override void Update(GameTime gameTime)
        {
            DefaultBehavior();

            velocity.Y += .3f;
        }
    }

    public class DestructionTileParticle :Particle
    {
        public DestructionTileParticle(Tile tile, Rectangle source)
        {
            Texture = tile.texture;
            collRectangle = new Rectangle(tile.drawRectangle.Center.X, tile.drawRectangle.Center.Y, 8, 8);
            sourceRectangle = source;
            velocity.X = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-5, 6));
            velocity.Y = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-5, 6));
            position = new Vector2(collRectangle.X, collRectangle.Y);
            opacity = 2;

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
            collRectangle = new Rectangle(source.collRectangle.Center.X, source.collRectangle.Center.Y, 8, 8);
            collRectangle = collRectangle;
            sourceRectangle = new Rectangle(8, 0, 8, 8);
            int buffer = 1;
            velocity.X = GameWorld.RandGen.Next((int)-source.velocity.X - buffer,(int)-source.velocity.X + buffer + 1) * (float)GameWorld.RandGen.NextDouble();
            velocity.Y = GameWorld.RandGen.Next((int)-source.velocity.Y - buffer, (int)-source.velocity.Y + buffer + 1);
            position = new Vector2(collRectangle.X, collRectangle.Y);
            opacity = .5f;

            light = new Lights.DynamicPointLight(this, .5f, false, color, 1);
            GameWorld.Instance.lightEngine.AddDynamicLight(light);
        }

        public override void Update(GameTime gameTime)
        {
            DefaultBehavior();
        }
    }

    public class ChronoshiftParticle : Particle
    {
        double changeDirectionTimer;
        bool hasChangedDirection;

        public ChronoshiftParticle(Entity entity)
        {
            int maxVel = 10;
            velocity = new Vector2(GameWorld.RandGen.Next(-maxVel, maxVel+1), GameWorld.RandGen.Next(-maxVel, maxVel+1));
            collRectangle = new Rectangle(entity.collRectangle.Center.X, entity.collRectangle.Center.Y, 8, 8);
            collRectangle = collRectangle;
            position = new Vector2(collRectangle.X, collRectangle.Y);

            Color colorful = new Color(GameWorld.RandGen.Next(0,256), GameWorld.RandGen.Next(0, 256), GameWorld.RandGen.Next(0, 256), 255);
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
                velocity.X = velocity.X * 1/.95f;
                velocity.Y = velocity.Y * 1/.95f;
            }

            if (changeDirectionTimer > 1200)
            {
                GameWorld.Instance.player.isChronoshifting = false;
               
                toDelete = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
    //DO NOTHING
        }
    }

}
