﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Adam;
using Adam.Interactables;

namespace Adam
{
    public enum EnemyType
    {
        Snake, Potato, Shade, Drone, Bloodless
    }

    class Enemy : Entity
    {
        public Texture2D singleTexture;
        protected Rectangle radiusRect;
        public Rectangle topMidBound;
        public Rectangle damageBox;
        public Vector2 velocity;
        protected Vector2 maxVelocity;
        public bool isFacingRight;
        public bool isDead;
        public bool wasMean;
        public bool isPlayerToTheRight;
        public bool isPlayerAbove;
        public bool needsToJump;
        public bool isFlying;
        public bool canPassThroughWalls;
        protected bool isInRange;
        public int health;
        protected int switchFrame, currentFrame;
        protected Vector2 frameCount;
        protected double frameTimer;
        protected double beMeanTimer;
        protected double projCooldownTimer;
        protected SoundEffect meanSound, attackSound, deathSound;
        protected SoundEffectInstance meanSoundInstance, attackSoundInstance, deathSoundInstance;
        protected Map map;
        protected Player player;
        public EnemyType CurrentEnemyType;
        protected GameTime gameTime;
        protected List<ParabolicProjectile> projList = new List<ParabolicProjectile>();
        protected List<Particle> effectList = new List<Particle>();
        protected List<Entity> entities;

        public Enemy()
        {

        }

        protected void Initialize()
        {
            radiusRect = new Rectangle(drawRectangle.X, drawRectangle.Y, 2000, 2000);
            position = new Vector2(drawRectangle.X, drawRectangle.Y);
        }

        public virtual void Update(Player player, GameTime gameTime, List<Entity> entities)
        {
            //Each class implements their own update logic.
            //Call base.Update for the basic update logic.

            this.entities = entities;
            this.player = player;
            this.gameTime = gameTime;

            //See if player in range
            radiusRect.X = drawRectangle.X - radiusRect.Width / 2;
            radiusRect.Y = drawRectangle.Y - radiusRect.Height / 2;

            if (radiusRect.Intersects(player.collRectangle) == false)
            {
                isInRange = false;
                return;
            }
            else isInRange = true;

            if (!isInRange)
                return;

            //if in range, do below

            if (player.collRectangle.X > drawRectangle.X)
                isPlayerToTheRight = true;
            else isPlayerToTheRight = false;

            if (player.collRectangle.Y < drawRectangle.Y)
                isPlayerAbove = true;
            else isPlayerAbove = false;

            //Velocity
            position.X += (int)(velocity.X * (float)(60 * gameTime.ElapsedGameTime.TotalSeconds));
            position.Y += (int)(velocity.Y * (float)(60 * gameTime.ElapsedGameTime.TotalSeconds));

            drawRectangle.X = (int)position.X;
            drawRectangle.Y = (int)position.Y;           


            //Random chance of being mean.
            int shouldIShowDominace = Map.randGen.Next(0, 1000);
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

            //Update death effect
            foreach (var eff in effectList)
            {
                eff.Update(gameTime);
                if (eff.ToDelete())
                {
                    effectList.Remove(eff);
                    break;
                }
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var proj in projList)
                proj.Draw(spriteBatch);
            foreach (var eff in effectList)
                eff.Draw(spriteBatch);

            //spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/temp"), collRectangle, Color.Red);
            //spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/temp"), damageBox, Color.Green);
            //spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/temp"), drawRectangle, Color.Blue);
        }

        public void GetDisintegratedRectangles(out Rectangle[] rectangles)
        {
            Vector2 size = new Vector2(singleTexture.Width / Game1.Tilesize, singleTexture.Height / Game1.Tilesize);
            int xSize = 4 * (int)size.X;
            int ySize = 4 * (int)size.Y;
            int width = singleTexture.Width / xSize;
            int height = singleTexture.Height / ySize;
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

        public void BeMean()
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
        }

        public void Kill()
        {
            PlayDeathSound();
            isDead = true;
            health = 0;
            entities.Add(new Food(this));
        }

        public void CreateDeathEffect()
        {
            Rectangle[] rectangles;
            GetDisintegratedRectangles(out rectangles);

            foreach (var rec in rectangles)
            {
                Particle eff = new Particle();
                eff.CreateEnemyDeathEffect(this, rec);
                effectList.Add(eff);
            }
        }

    }
}
