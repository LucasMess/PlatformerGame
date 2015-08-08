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

namespace Adam
{
    class Particle : Entity
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
        int currentFrame;
        bool toDelete;
        public bool isComplete;
        bool hasChangedDirection;
        bool dead;
        float rotation, rotationSpeed, rotationDelta;
        Color color = Color.White;
        Light light;
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

        public Particle()
        {
            gameWorld = GameWorld.Instance;
        }

        public Particle(Chest chest, int seed)
        {
            CurrentParticle = ParticleType.ChestSparkles;
            randGen = new Random(seed);
            velocity = new Vector2(randGen.Next(-7, 7), randGen.Next(-10, -1));
            texture = ContentHelper.LoadTexture("Sparkles");
            int size = randGen.Next(texture.Width, texture.Width * 2);
            drawRectangle = new Rectangle(chest.rectangle.X + texture.Width / 2, chest.rectangle.Y + texture.Height / 2, size, size);
            sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            light = new Light();
            light.EffectLight(1, this, Content);

            //rectangle.Width = texture.Width;
            //rectangle.Height = texture.Height;

        }

        public Particle(Projectile projectile, Tile[] tileArray)
        {
            sourceRectangle = new Rectangle(0, 0, 32, 32);
            CurrentParticle = ParticleType.Impact;
            texture = ContentHelper.LoadTexture("Explosion");
            if (projectile.velocity.X > 0)
                drawRectangle = new Rectangle(tileArray[projectile.tileHit].drawRectangle.X - 32, projectile.collRectangle.Center.Y - 32, 64, 64);
            else drawRectangle = new Rectangle(tileArray[projectile.tileHit].drawRectangle.X + 32, projectile.collRectangle.Center.Y - 32, 64, 64);
            frameCount = new Vector2(texture.Width / 32, texture.Height / 32);
        }

        public Particle(Enemy enemy, Projectile projectile)
        {
            sourceRectangle = new Rectangle(0, 0, 32, 32);
            CurrentParticle = ParticleType.Impact;
            texture = ContentHelper.LoadTexture("Explosion");

            if (projectile.velocity.X > 0)
                drawRectangle = new Rectangle(enemy.drawRectangle.X - enemy.drawRectangle.Width / 2, projectile.collRectangle.Center.Y - sourceRectangle.Height / 2, 32, 32);
            else drawRectangle = new Rectangle(enemy.drawRectangle.X + enemy.drawRectangle.Width / 2, projectile.collRectangle.Center.Y - sourceRectangle.Height / 2, 32, 32);
            frameCount = new Vector2(texture.Width / 32, texture.Height / 32);
        }

        public Particle(Projectile proj)
        {
            CurrentParticle = ParticleType.SnakeVenom;
            texture = ContentHelper.LoadTexture("Effects/venom_blob");
            randGen = new Random();
            drawRectangle = new Rectangle(proj.collRectangle.X + proj.collRectangle.Width / 2, proj.collRectangle.Y, 8, 8);
            if (proj.velocity.X > 0)
                velocity.X = -2;
            else velocity.X = 2;
            velocity.Y = GameWorld.RandGen.Next(1, 4);
            if (GameWorld.RandGen.Next(0, 2) == 0)
                velocity.Y = -velocity.Y;
            sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            opacity = 1f;
        }

        public Particle(Player player)
        {
            randGen = new Random();
            sourceRectangle = new Rectangle(0, 0, 14, 14);
            CurrentParticle = ParticleType.GameZZZ;
            texture = ContentHelper.LoadTexture("Effects/new_Z");
            color = new Color(255, 255, 255, 255);
            if (player.isFacingRight)
                position = new Vector2(player.collRectangle.X + randGen.Next(-5, 5) + player.collRectangle.Width - 8, player.collRectangle.Y + randGen.Next(-5, 5));
            else position = new Vector2(player.collRectangle.Center.X + randGen.Next(0, 20), player.collRectangle.Y + randGen.Next(-5, 5));
            drawRectangle = new Rectangle((int)position.X, (int)position.Y, 8, 8);

        }

        public Particle(Rectangle rectangle)
        {
            randGen = new Random();
            sourceRectangle = new Rectangle(0, 0, 14, 14);
            CurrentParticle = ParticleType.MenuZZZ;
            texture = ContentHelper.LoadTexture("Effects/new_Z");
            color = new Color(255, 255, 255, 255);
            position = new Vector2(rectangle.X + randGen.Next(-5, 5) + rectangle.Width - 8, rectangle.Y + randGen.Next(-5, 5));
            this.drawRectangle = new Rectangle((int)position.X, (int)position.Y, 32, 32);
        }

        public void CreateWeaponBurstEffect(Player player, Projectile proj, ContentManager Content)
        {
            position = player.weapon.tipPos;
            velocity = new Vector2(0, 0);
            int maxTanSpeed = 10;
            velocity.X = (float)(GameWorld.RandGen.Next((int)(proj.velocity.X - maxTanSpeed), (int)(proj.velocity.X + maxTanSpeed + 1)));
            velocity.Y = (float)(GameWorld.RandGen.Next((int)(proj.velocity.Y - maxTanSpeed), (int)(proj.velocity.Y + maxTanSpeed + 1)));
            CurrentParticle = ParticleType.WeaponBurst;
            texture = ContentHelper.LoadTexture("Effects/laser_burst");
            int randSize = GameWorld.RandGen.Next(2, 5);
            drawRectangle = new Rectangle((int)position.X, (int)position.Y, randSize, randSize);
            sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            opacity = 1;
            light = new Light();
            light.EffectLight(.5f, this, Content);
        }

        public void CreateEnemyDisintegrationEffect(Enemy enemy, Rectangle sourceRectangle, Projectile proj)
        {
            CurrentParticle = ParticleType.EnemyDesintegration;
            texture = enemy.texture;
            drawRectangle = new Rectangle(enemy.drawRectangle.X + sourceRectangle.X, enemy.drawRectangle.Y + sourceRectangle.Y,
                sourceRectangle.Width, sourceRectangle.Height);
            this.sourceRectangle = sourceRectangle;
            int maxTanSpeed = 10;
            velocity.X = (float)(GameWorld.RandGen.Next((int)(proj.velocity.X - maxTanSpeed), (int)(proj.velocity.X + maxTanSpeed + 1)));
            velocity.Y = (float)(GameWorld.RandGen.Next((int)(proj.velocity.Y - maxTanSpeed), (int)(proj.velocity.Y + maxTanSpeed + 1)));
        }

        public void CreateEnemyDeathEffect(Enemy enemy, Rectangle sourceRectangle)
        {
            CurrentParticle = ParticleType.EnemyDesintegration;
            texture = enemy.texture;
            drawRectangle = new Rectangle(enemy.drawRectangle.X + sourceRectangle.X, enemy.drawRectangle.Y + sourceRectangle.Y,
                sourceRectangle.Width, sourceRectangle.Height);
            this.sourceRectangle = sourceRectangle;
            int maxTanSpeed = 5;
            velocity = new Vector2(GameWorld.RandGen.Next(-maxTanSpeed, maxTanSpeed), GameWorld.RandGen.Next(-maxTanSpeed, maxTanSpeed));
        }

        public void CreateBloodEffect(Player player, GameWorld map)
        {
            CurrentParticle = ParticleType.Blood;
            texture = ContentHelper.LoadTexture("Effects/blood");
            drawRectangle = new Rectangle(player.drawRectangle.Center.X, player.drawRectangle.Center.Y, 8, 8);
            sourceRectangle = new Rectangle(GameWorld.RandGen.Next(0, 4) * 8, 0, 8, 8);
            collRectangle = drawRectangle;
            velocity = new Vector2(GameWorld.RandGen.Next(-10, 10), GameWorld.RandGen.Next(-10, 10));
            position = new Vector2(drawRectangle.X, drawRectangle.Y);
            this.gameWorld = map;
        }

        public void CreatePlayerChronoshiftEffect(Player player, Rectangle sourceRectangle)
        {
            CurrentParticle = ParticleType.PlayerChronoshift;
            texture = player.previousSingleTexture;
            nextTexture = player.newSingleTexture;
            drawRectangle = new Rectangle(player.collRectangle.X + sourceRectangle.X, player.collRectangle.Y + sourceRectangle.Y,
                sourceRectangle.Width, sourceRectangle.Height);
            this.sourceRectangle = sourceRectangle;
            int maxSpeed = 8;
            velocity.X = (float)(GameWorld.RandGen.Next(-maxSpeed, maxSpeed + 1));
            velocity.Y = (float)(GameWorld.RandGen.Next(-maxSpeed, maxSpeed + 1));
            originalVelocity = velocity;
            originalPosition = new Vector2(drawRectangle.X, drawRectangle.Y);
            rotationDelta = .02f;
        }

        public void CreateTileParticleEffect(Tile tile, Player player)
        {
            CurrentParticle = ParticleType.TileParticle;
            texture = tile.texture;
            drawRectangle = new Rectangle(player.collRectangle.Center.X - 2, player.collRectangle.Y + player.collRectangle.Height, 8, 8);
            sourceRectangle = new Rectangle(tile.sourceRectangle.X, tile.sourceRectangle.Y, 4, 4);
            if (tile is AnimatedTile)
            {
                AnimatedTile t = (AnimatedTile)tile;
                sourceRectangle = new Rectangle(t.sourceRectangle.X, t.sourceRectangle.Y, 4, 4);
            }
            sourceRectangle.X += (GameWorld.RandGen.Next(0, 4) * Game1.Tilesize / 4);
            velocity.X = (-player.velocity.X / 2) * (float)GameWorld.RandGen.NextDouble();
            velocity.Y = GameWorld.RandGen.Next(-1, 1);
            opacity = 1;
        }

        public void CreatePlayerDesintegrationEffect(Player player, Rectangle sourceRectangle)
        {
            this.player = player;
            CurrentParticle = ParticleType.PlayerDesintegration;
            texture = player.GetSingleTexture();
            drawRectangle = new Rectangle(player.drawRectangle.X + sourceRectangle.X, player.drawRectangle.Y + sourceRectangle.Y,
                sourceRectangle.Width, sourceRectangle.Height);
            this.sourceRectangle = sourceRectangle;
            int maxTanSpeed = 5;
            velocity = new Vector2(GameWorld.RandGen.Next(-maxTanSpeed, maxTanSpeed), GameWorld.RandGen.Next(-maxTanSpeed, maxTanSpeed));
            originalPosition = new Vector2(drawRectangle.X, drawRectangle.Y);
            differenceInPosition = new Vector2(player.respawnPos.X - player.position.X, player.respawnPos.Y - player.position.Y);
            endPosition = originalPosition + differenceInPosition;
            position = new Vector2(drawRectangle.X, drawRectangle.Y);
        }

        public void CreateMusicNotesEffect(CDPlayer cd)
        {
            CurrentParticle = ParticleType.MusicNotes;
            texture = ContentHelper.LoadTexture("Effects/music_notes");
            drawRectangle = new Rectangle(cd.drawRectangle.X, cd.drawRectangle.Y, 16, 16);
            sourceRectangle = new Rectangle(32 * GameWorld.RandGen.Next(0, 2), 0, 32, 32);
            int randX = GameWorld.RandGen.Next(0, 2);
            float velocityX = .5f;
            if (randX == 0)
            {
                drawRectangle.X -= drawRectangle.Width;
                velocity.X = -velocityX;
            }
            else
            {
                drawRectangle.X += drawRectangle.Width;
                velocity.X = velocityX;
            }

            velocity.Y = -(float)GameWorld.RandGen.NextDouble();
            position = new Vector2(drawRectangle.X, drawRectangle.Y);
        }

        public void CreateJetPackSmokeParticle(JetpackPowerUp jet)
        {
            CurrentParticle = ParticleType.JetpackSmoke;
            texture = ContentHelper.LoadTexture("Effects/smoke");
            drawRectangle = new Rectangle(jet.drawRectangle.Center.X - 8, jet.drawRectangle.Y + jet.drawRectangle.Height, 16, 16);
            sourceRectangle = new Rectangle(GameWorld.RandGen.Next(0, 4) * 16, 0, 16, 16);
            velocity.Y = 3f;
            velocity.X = GameWorld.RandGen.Next(-1, 2);
            position = new Vector2(drawRectangle.X, drawRectangle.Y);
            opacity = 1f;
        }

        public void CreateJetPackSmokeParticle(Player player)
        {
            CurrentParticle = ParticleType.JetpackSmoke;
            texture = ContentHelper.LoadTexture("Effects/smoke");
            drawRectangle = new Rectangle(player.drawRectangle.Center.X - 8, player.drawRectangle.Y + player.drawRectangle.Height, 16, 16);
            sourceRectangle = new Rectangle(GameWorld.RandGen.Next(0, 4) * 16, 0, 16, 16);
            velocity.Y = 3f;
            velocity.X = GameWorld.RandGen.Next(-1, 2);
            position = new Vector2(drawRectangle.X, drawRectangle.Y);
            opacity = 1f;
        }

        public void CreateLavaParticle(Liquid lava, GameWorld map)
        {
            CurrentParticle = ParticleType.Lava;
            //texture = ContentHelper.LoadTexture("Effects/lava");
            texture = Game1.DefaultTexture;
            drawRectangle = new Rectangle(lava.collRectangle.Center.X, lava.collRectangle.Y + 16, 16, 16);
            velocity.Y = -10f;
            velocity.X = GameWorld.RandGen.Next(-2, 3);
            position = new Vector2(drawRectangle.X, drawRectangle.Y);
            opacity = 10f;
            this.gameWorld = map;
        }

        public void CreateDeathSmoke(Entity entity)
        {
            CurrentParticle = ParticleType.DeathSmoke;
            texture = ContentHelper.LoadTexture("Effects/smoke");
            drawRectangle = new Rectangle(GameWorld.RandGen.Next(entity.collRectangle.X, entity.collRectangle.Right - 16), GameWorld.RandGen.Next(entity.collRectangle.Y, entity.collRectangle.Bottom - 16), 16, 16);
            sourceRectangle = new Rectangle(GameWorld.RandGen.Next(0, 4) * 16, 0, 16, 16);
            velocity.X = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-2, 3));
            velocity.Y = -.5f;
            position = new Vector2(drawRectangle.X, drawRectangle.Y);
            opacity = 2;
        }

        public void CreateTookDamage(Entity entity)
        {
            CurrentParticle = ParticleType.TookDamage;
            texture = ContentHelper.LoadTexture("Sparkles");
            drawRectangle = new Rectangle(GameWorld.RandGen.Next(entity.collRectangle.X, entity.collRectangle.Right - 8), GameWorld.RandGen.Next(entity.collRectangle.Y, entity.collRectangle.Bottom - 8), 8, 8);
            sourceRectangle = new Rectangle(0, 0, 8, 8);
            velocity.X = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-4, 5));
            velocity.Y = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-1, 2));
            position = new Vector2(drawRectangle.X, drawRectangle.Y);
            opacity = 1;
            color = Color.Red;
        }

        public void CreateSparkles(Entity entity)
        {
            CurrentParticle = ParticleType.Sparkles;
            texture = ContentHelper.LoadTexture("Sparkles");
            drawRectangle = new Rectangle(GameWorld.RandGen.Next(entity.collRectangle.X, entity.collRectangle.Right - 8), GameWorld.RandGen.Next(entity.collRectangle.Y, entity.collRectangle.Bottom - 8), 8, 8);
            sourceRectangle = new Rectangle(0, 0, 8, 8);
            velocity.X = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-4, 5));
            velocity.Y = (float)(GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-7, 0));
            position = new Vector2(drawRectangle.X, drawRectangle.Y);
            opacity = 2;
            color = Color.White;
        }

        public virtual void Update(GameTime gameTime)
        {
            gameWorld = GameWorld.Instance;
            switch (CurrentParticle)
            {
                case ParticleType.ChestSparkles:
                    drawRectangle.X += (int)velocity.X;
                    drawRectangle.Y += (int)velocity.Y;
                    velocity.Y += .3f;
                    opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    velocity.X = velocity.X * 0.99f;
                    light.Update(this);
                    break;
                case ParticleType.Impact:
                    Animate(gameTime);
                    break;
                case ParticleType.GameZZZ:
                    drawRectangle.X = (int)position.X;
                    drawRectangle.Y = (int)position.Y;
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
                    drawRectangle.X = (int)position.X;
                    drawRectangle.Y = (int)position.Y;
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
                    drawRectangle.X += (int)velocity.X;
                    drawRectangle.Y += (int)velocity.Y;
                    opacity -= .03f;
                    velocity.Y += .09f;

                    if (opacity < 0)
                        toDelete = true;
                    break;
                case ParticleType.WeaponBurst:
                    drawRectangle.X += (int)velocity.X;
                    drawRectangle.Y += (int)velocity.Y;
                    opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    velocity.X = velocity.X * 0.99f;
                    velocity.Y = velocity.Y * 0.99f;
                    if (opacity < 0)
                        toDelete = true;
                    light.Update(this);
                    break;
                case ParticleType.EnemyDesintegration:
                    drawRectangle.X += (int)velocity.X;
                    drawRectangle.Y += (int)velocity.Y;
                    opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    velocity.X = velocity.X * 0.99f;
                    velocity.Y = velocity.Y * 0.99f;
                    if (opacity < 0)
                        toDelete = true;
                    break;
                case ParticleType.PlayerChronoshift:
                    travelTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                    drawRectangle.X += (int)velocity.X;
                    drawRectangle.Y += (int)velocity.Y;

                    float velocityFriction = .96f;
                    if (!hasChangedDirection)
                    {
                        velocity.X = velocity.X * velocityFriction;
                        velocity.Y = velocity.Y * velocityFriction;
                        rotation += rotationSpeed;
                        rotationSpeed += rotationDelta;
                        if (color.A > 0)
                            color.A -= 5;
                    }
                    if (travelTimer > 1000 && !hasChangedDirection)
                    {
                        hasChangedDirection = true;
                        texture = nextTexture;
                        travelTimer = 0;
                        velocity = -velocity;
                        color.A = 0;
                    }
                    if (hasChangedDirection)
                    {
                        velocity.X = velocity.X * (2 - velocityFriction);
                        velocity.Y = velocity.Y * (2 - velocityFriction);
                        rotation += rotationSpeed;
                        rotationSpeed -= rotationDelta;
                        if (travelTimer > 1000)
                        {
                            velocity = new Vector2(0, 0);
                            drawRectangle.X = (int)originalPosition.X;
                            drawRectangle.Y = (int)originalPosition.Y;
                            isComplete = true;
                        }
                        if (color.A < 255)
                            color.A += 5;
                    }
                    break;
                case ParticleType.TileParticle:
                    drawRectangle.X += (int)velocity.X;
                    drawRectangle.Y += (int)velocity.Y;
                    opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //velocity.X = velocity.X * 0.99f;
                    //velocity.Y = velocity.Y * 0.99f;
                    if (opacity < 0)
                        toDelete = true;
                    break;
                case ParticleType.PlayerDesintegration:
                    position += velocity;

                    drawRectangle.X = (int)position.X;
                    drawRectangle.Y = (int)position.Y;
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

                    drawRectangle.X = (int)position.X;
                    drawRectangle.Y = (int)position.Y;

                    opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (opacity <= 0)
                        toDelete = true;
                    break;
                case ParticleType.JetpackSmoke:
                    position += velocity;

                    drawRectangle.X = (int)position.X;
                    drawRectangle.Y = (int)position.Y;

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

                    drawRectangle.X = (int)position.X;
                    drawRectangle.Y = (int)position.Y;

                    collRectangle = drawRectangle;

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

                    drawRectangle.X = (int)position.X;
                    drawRectangle.Y = (int)position.Y;

                    collRectangle = drawRectangle;

                    velocity.Y += .3f;

                    opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (opacity <= 0)
                        toDelete = true;
                    break;
                case ParticleType.DeathSmoke:
                    position += velocity;

                    drawRectangle.X = (int)position.X;
                    drawRectangle.Y = (int)position.Y;

                    velocity.X = velocity.X * 0.95f;
                    velocity.Y = velocity.Y * 0.99f;

                    opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (opacity <= 0)
                        toDelete = true;
                    break;
                case ParticleType.TookDamage:
                    position += velocity;

                    drawRectangle.X = (int)position.X;
                    drawRectangle.Y = (int)position.Y;

                    velocity.X = velocity.X * 0.95f;
                    velocity.Y = velocity.Y * 0.99f;

                    opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (opacity <= 0)
                        toDelete = true;
                    break;
                case ParticleType.Sparkles:
                    position += velocity;

                    drawRectangle.X = (int)position.X;
                    drawRectangle.Y = (int)position.Y;

                    velocity.X = velocity.X * 0.99f;
                    velocity.Y = velocity.Y * 0.99f;

                    opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (opacity <= 0)
                        toDelete = true;
                    break;
            }
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
                    sourceRectangle.X += texture.Width / (int)frameCount.X;
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

            drawRectangle.X = (int)position.X;
            drawRectangle.Y = (int)position.Y;

            velocity.X = velocity.X * 0.99f;
            velocity.Y = velocity.Y * 0.99f;

            opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (opacity <= 0)
                toDelete = true;
        }



        public bool ToDelete()
        {
            if (toDelete)
                return true;
            else return false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentParticle == ParticleType.SnakeVenom)
                spriteBatch.Draw(texture, drawRectangle, sourceRectangle, color * opacity);
            else if (CurrentParticle == ParticleType.PlayerChronoshift)
                spriteBatch.Draw(texture, drawRectangle, sourceRectangle, color * opacity, rotation, new Vector2(sourceRectangle.Width / 2, sourceRectangle.Height / 2), SpriteEffects.None, 0);
            else
                spriteBatch.Draw(texture, drawRectangle, sourceRectangle, color * opacity);

        }

        public void DrawLights(SpriteBatch spriteBatch)
        {
            if (light != null)
                light.Draw(spriteBatch);
        }

    }

    class JumpSmokeParticle : Particle
    {
        public JumpSmokeParticle(Entity entity)
        {
            texture = ContentHelper.LoadTexture("Effects/smoke");
            drawRectangle = new Rectangle(entity.collRectangle.Center.X - 4, entity.collRectangle.Bottom - 4, 8, 8);
            sourceRectangle = new Rectangle(8 * GameWorld.RandGen.Next(0, 4), 0, 8, 8);
            collRectangle = drawRectangle;
            position = new Vector2(drawRectangle.X, drawRectangle.Y);

            velocity.X = (float)GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-3 - (int)entity.velocity.X/2, 3 - (int)entity.velocity.X/2);
            velocity.Y = (float)GameWorld.RandGen.NextDouble() * -1f;
            opacity = 1f;
        }

        public override void Update(GameTime gameTime)
        {
            DefaultBehavior();
        }

    }

    class StompSmokeParticle : Particle
    {
        public StompSmokeParticle(Entity entity)
        {
            texture = ContentHelper.LoadTexture("Effects/smoke");
            drawRectangle = new Rectangle(entity.collRectangle.Center.X - 4, entity.collRectangle.Bottom - 4, 8, 8);
            sourceRectangle = new Rectangle(8 * GameWorld.RandGen.Next(0, 4), 0, 8, 8);
            collRectangle = drawRectangle;
            position = new Vector2(drawRectangle.X, drawRectangle.Y);

            velocity.X = (float)GameWorld.RandGen.NextDouble() * GameWorld.RandGen.Next(-3, 3);
            velocity.Y = (float)GameWorld.RandGen.NextDouble() * -1f;

            opacity = 1f;
        }

        public override void Update(GameTime gameTime)
        {
            DefaultBehavior();
        }

    }

}
