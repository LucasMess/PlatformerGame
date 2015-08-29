using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Adam.Misc;
using Adam.Interactables;

namespace Adam.Characters.Enemies
{
    public abstract partial class Enemy : Entity
    {
        /// <summary>
        /// Updates the enemy.
        /// </summary>
        public override void Update()
        {
            hitByPlayerTimer.Increment();
            wasMeanTimer.Increment();
            PlayMeanSound();
            CheckInteractionsWithPlayer();

            base.Update();
        }

        /// <summary>
        /// Deals a certain amount of damage to the player.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            Health -= damage;
            hitByPlayerTimer.Reset();

            //Creates damage particles.
            for (int i = 0; i < damage; i++)
            {
                Particle par = new Particle();
                par.CreateTookDamage(this);
                GameWorld.Instance.particles.Add(par);
            }
        }

        /// <summary>
        /// The color that the enemy will be drawn in.
        /// </summary>
        public override Color Color
        {
            get
            {
                //If the enemy was recently hit, its color is changed to red.
                if (HasTakenDamageRecently())
                    Color = Color.Red;
                else Color = Color.White;

                return base.Color;
            }

            set
            {
                base.Color = value;
            }
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
        public void Kill()
        {
            PlayDeathSound();
            CreateFoodItem();
            CreateDeathEffect();
            health = 0;            

            //Create smoke particles.
            for (int i = 0; i < 10; i++)
            {
                Particle particle = new Particle();
                particle.CreateDeathSmoke(this);
                GameWorld.Instance.particles.Add(particle);
            }
        }

        /// <summary>
        /// Creates a food item that is part of the enemy for the player to consume.
        /// </summary>
        protected void CreateFoodItem()
        {
            GameWorld.Instance.entities.Add(new Food(this));
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
                GameWorld.Instance.particles.Add(eff);
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

        /// <summary>
        /// Check to see if player is doing anything to the enemy.
        /// </summary>
        private void CheckInteractionsWithPlayer()
        {
            Player player = GameWorld.Instance.player;

            //Deals damage to enemy is player is attacking.
            if (IsBeingAttacked())
            {
                player.DealDamage(this);
            }

            //Deals damage to player if he is touching.
            else if (IsIntersectingPlayer())
            {
                player.TakeDamageAndKnockBack(GetTouchDamage());
            }

        }



    }
}
