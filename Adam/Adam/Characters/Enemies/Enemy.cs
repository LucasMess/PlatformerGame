using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Adam;
using Adam.Interactables;
using Adam.Misc;
using System.Threading;

namespace Adam
{
    public enum EnemyType
    {
        Snake, Potato, Shade, Drone, Bloodless, Hellboar, Frog
    }

    public class Enemy : Entity
    {
        protected Rectangle radiusRect;
        public Rectangle topMidBound;
        public Rectangle damageBox;
        protected Vector2 maxVelocity;
        public bool isFacingRight;
        public bool isDead;
        public bool wasMean;
        public bool isPlayerToTheRight;
        public bool isPlayerAbove;
        public bool needsToJump;
        public bool isFlying;
        public bool canPassThroughWalls;
        protected bool tookDamage;
        protected bool isInRange;
        public int health;
        public int maxHealth;
        protected int switchFrame, currentFrame;
        protected Vector2 frameCount;
        protected double frameTimer;
        protected double beMeanTimer;
        protected double projCooldownTimer;
        double damagedIncapableTimer;
        protected SoundEffect meanSound, attackSound, deathSound;
        private SoundFx killedByPlayerSound;
        private SoundFx jumpedOnSound;
        protected SoundEffectInstance meanSoundInstance, attackSoundInstance, deathSoundInstance;
        protected Player player;
        public EnemyType CurrentEnemyType;
        protected GameTime gameTime;

        public Enemy()
        {
        }

        protected void Initialize()
        {
            radiusRect = new Rectangle(collRectangle.X, collRectangle.Y, 2000, 2000);
            maxHealth = health;

            killedByPlayerSound = new SoundFx("Sounds/Player/enemy_kill");
            jumpedOnSound = new SoundFx("Sounds/Player/enemy_jumpedOn");

            if (meanSound != null)
                meanSoundInstance = meanSound.CreateInstance();
        }

        public virtual void Update(Player player, GameTime gameTime)
        {
            this.player = player;
            this.gameTime = gameTime;
            this.gameWorld = GameWorld.Instance;

            if (tookDamage) goto BeingHit;

            //Each public class implements their own update logic.
            //Call base.Update for the basic update logic.

            //See if player in range
            radiusRect.X = collRectangle.X - radiusRect.Width / 2;
            radiusRect.Y = collRectangle.Y - radiusRect.Height / 2;

            if (radiusRect.Intersects(player.collRectangle) == false)
            {
                isInRange = false;
                return;
            }
            else isInRange = true;

            if (!isInRange)
                return;

            //if in range, do below

            if (player.collRectangle.X > collRectangle.X)
                isPlayerToTheRight = true;
            else isPlayerToTheRight = false;

            if (player.collRectangle.Y < collRectangle.Y)
                isPlayerAbove = true;
            else isPlayerAbove = false;

            //Velocity

            //Random chance of being mean.
            int shouldIShowDominace = GameWorld.RandGen.Next(0, 100);
            if (shouldIShowDominace == 1)
            {
                BeMean();
            }

            //If 
            if (wasMean)
            {
                beMeanTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (beMeanTimer > 1)
                {
                    wasMean = true;
                    beMeanTimer = 0;
                }
            }

            if (health <= 0 && !isDead)
            {
                Kill();
            }
        
            base.Update();


            BeingHit:
            if (tookDamage)
            {
                damagedIncapableTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (damagedIncapableTimer > 300)
                {
                    damagedIncapableTimer = 0;
                    tookDamage = false;
                }
            }           
           

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Color color;
            if (tookDamage) color = Color.Red;
            else color = Color.White;
            if (isDead) return;
            if (isFacingRight)
                spriteBatch.Draw(texture, drawRectangle, sourceRectangle, color, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
            else spriteBatch.Draw(texture, drawRectangle, sourceRectangle, color, 0, new Vector2(0, 0), SpriteEffects.None, 0);

            // spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/temp"), collRectangle, Color.Red * .5f);
            //spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/temp"), damageBox, Color.Green * .5f);
            // spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/temp"), drawRectangle, Color.Blue *.5f);
        }

        public void GetDisintegratedRectangles(out Rectangle[] rectangles)
        {
            Vector2 size = new Vector2(drawRectangle.Width / Main.Tilesize, drawRectangle.Height / Main.Tilesize);
            int xSize = 4 * (int)size.X;
            int ySize = 4 * (int)size.Y;
            int width = sourceRectangle.Width / xSize;
            int height = sourceRectangle.Height / ySize;
            rectangles = new Rectangle[xSize * ySize];

            int i = 0;
            for (int h = 0; h < ySize; h++)
            {
                for (int w = 0; w < xSize; w++)
                {
                    rectangles[i] = new Rectangle(w * width, h * height, width, height);
                    i++;
                }
            }
        }

        public int GetTouchDamage()
        {
            switch (CurrentEnemyType)
            {
                case EnemyType.Snake:
                    return EnemyDB.Snake_TouchDamage;
                case EnemyType.Potato:
                    return EnemyDB.Potato_TouchDamage;
                case EnemyType.Hellboar:
                    return EnemyDB.Hellboar_TouchDamage;
                case EnemyType.Frog:
                    return EnemyDB.Frog_TouchDamage;
            }
            return 0;
        }

        public int GetProjectileDamage()
        {
            switch (CurrentEnemyType)
            {
                case EnemyType.Snake:
                    return EnemyDB.Snake_ProjectileDamage;
            }
            return 0;
        }

        public int AddScore(Player player)
        {
            switch (CurrentEnemyType)
            {
                case EnemyType.Snake:
                    return EnemyDB.Snake_Score;
                case EnemyType.Potato:
                    return EnemyDB.Potato_Score;
            }
            return 0;
        }

        public virtual void BeMean()
        {
            if (!wasMean && meanSound != null)
            {
                meanSoundInstance.Play();
                meanSoundInstance.Volume = GetSoundVolume(player);
                wasMean = true;
            }
        }

        public void PlayDeathSound()
        {
            if (deathSound != null)
            {
                deathSoundInstance.Play();
                deathSoundInstance.Volume = GetSoundVolume(player);
            }
        }

        public void PlayAttackSound()
        {
            if (attackSound != null)
            {
                attackSoundInstance.Play();
                attackSoundInstance.Volume = GetSoundVolume(player);
            }
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            jumpedOnSound.Play();
            tookDamage = true;

            for (int i = 0; i < damage; i++)
            {
                Particle par = new Particle();
                par.CreateTookDamage(this);
                GameWorld.Instance.particles.Add(par);
            }
        }

        public void Kill()
        {
            for (int i = 0; i < 10; i++)
            {
                Particle particle = new Particle();
                particle.CreateDeathSmoke(this);
                GameWorld.Instance.particles.Add(particle);
            }

            CreateDeathEffect();
            killedByPlayerSound.Play();
            PlayDeathSound();
            isDead = true;
            health = 0;
            GameWorld.Instance.entities.Add(new Food(this));
        }

        public void CreateDeathEffect()
        {
            Rectangle[] rectangles;
            GetDisintegratedRectangles(out rectangles);

            foreach (var rec in rectangles)
            {
                Particle eff = new Particle();
                eff.CreateEnemyDeathEffect(this, rec);
                GameWorld.Instance.particles.Add(eff);
            }
        }

    }
}
