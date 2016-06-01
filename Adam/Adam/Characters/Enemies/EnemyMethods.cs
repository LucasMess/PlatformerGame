using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Adam.Misc;
using Adam.Interactables;
using Adam.Levels;
using Adam.PlayerCharacter;

namespace Adam.Characters.Enemies
{
    public abstract partial class Enemy : Character
    {
        /// <summary>
        /// Updates the enemy.
        /// </summary>
        public override void Update()
        {
            PlayMeanSound();
            CheckInteractionsWithPlayer();

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
            {
                TakeDamage(GameWorld.Instance.GetPlayer(), damage);
                HitSound?.Play();
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
        /// Creates a food item that is part of the enemy for the player to consume.
        /// </summary>
        protected void CreateFoodItem()
        {
            GameWorld.Instance.Entities.Add(new Food(this));
        }

        /// <summary>
        /// Check to see if player is doing anything to the enemy.
        /// </summary>
        private void CheckInteractionsWithPlayer()
        {
            Player player = GameWorld.Instance.Player;

            //Deals damage to player if he is touching.
            if (IsIntersectingPlayer() && !IsAboutToDie)
            {
                player.TakeDamage(this, GetTouchDamage());
            }

            foreach (Projectile proj in GameWorld.Instance.PlayerProjectiles)
            {
                if (proj.GetCollRectangle().Intersects(CollRectangle))
                {
                    TakeDamage(player,proj.DamageOnHit);
                    proj.ToDelete = true;
                }
            }

        }
    }
}
