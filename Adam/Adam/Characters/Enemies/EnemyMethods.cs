using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Adam.Misc;
using Adam.Interactables;
using Adam.Levels;

namespace Adam.Characters.Enemies
{
    public abstract partial class Enemy : Character
    {
        /// <summary>
        /// Updates the enemy.
        /// </summary>
        public override void Update()
        {
            Rectangle check = RespawnLocation;
            _wasMeanTimer.Increment();
            PlayMeanSound();
            CheckInteractionsWithPlayer();

            if (IsDead())
            {
                Kill();
            }

            base.Update();
        }

        /// <summary>
        /// Event fired when player attacks.
        /// </summary>
        /// <param name="damageArea"></param>
        /// <param name="damage"></param>
        private void OnPlayerAttack(Rectangle damageArea, int damage)
        {
            if (damageArea.Intersects(CollRectangle))
                TakeDamage(GameWorld.Instance.GetPlayer(), damage);
        }

        /// <summary>
        /// Checks to see if it is time to be mean again.
        /// </summary>
        private void PlayMeanSound()
        {
            if (IsTimeToBeMean())
                MeanSound?.PlayIfStopped();
        }

        /// <summary>
        /// Plays attack sound.
        /// </summary>
        protected void PlayAttackSound()
        {
            AttackSound?.PlayIfStopped();
        }

        /// <summary>
        /// Plays death sound.
        /// </summary>
        protected void PlayDeathSound()
        {
            DeathSound?.PlayIfStopped();
        }

        /// <summary>
        /// Sets the enemy's health to zero and creates all death particle effects.
        /// </summary>
        public override void Kill()
        {
            PlayDeathSound();
            CreateFoodItem();
            CreateDeathEffect();
            Health = 0;            

            //Create smoke particles.
            for (int i = 0; i < 10; i++)
            {
                Particle particle = new Particle();
                particle.CreateDeathSmoke(this);
                GameWorld.Instance.Particles.Add(particle);
            }

            Gem.Generate(MaxHealth / 10, this);
        }

        /// <summary>
        /// Creates a food item that is part of the enemy for the player to consume.
        /// </summary>
        protected void CreateFoodItem()
        {
            GameWorld.Instance.Entities.Add(new Food(this));
        }

        /// <summary>
        /// Creates the broken enemy particle effect.
        /// </summary>
        private void CreateDeathEffect()
        {
            Rectangle[] rectangles;
            GetDisintegratedRectangles(out rectangles);

            foreach (var rec in rectangles)
            {
                Particle eff = new Particle();
                eff.CreateEnemyDeathEffect(this, rec);
                GameWorld.Instance.Particles.Add(eff);
            }
        }

        /// <summary>
        /// Returns the texture of the enemy as small rectangles for particle effects.
        /// </summary>
        /// <param name="rectangles"></param>
        public void GetDisintegratedRectangles(out Rectangle[] rectangles)
        {
            Vector2 size = new Vector2(DrawRectangle.Width / Main.Tilesize, DrawRectangle.Height / Main.Tilesize);
            int xSize = 4 * (int)size.X;
            int ySize = 4 * (int)size.Y;
            int width = SourceRectangle.Width / xSize;
            int height = SourceRectangle.Height / ySize;
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

        /// <summary>
        /// Check to see if player is doing anything to the enemy.
        /// </summary>
        private void CheckInteractionsWithPlayer()
        {
            Player player = GameWorld.Instance.Player;

            //Deals damage to player if he is touching.
            if (IsIntersectingPlayer())
            {
                player.TakeDamage(this, GetTouchDamage());
            }

        }

        public override void Revive()
        {
            CollRectangle = RespawnLocation;

            base.Revive();
        }



    }
}
