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
            if (IsCollidableWithEnemies) CheckCollisionWithOtherEnemies();

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
                TakeDamage(GameWorld.GetPlayer(), damage);
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
            GameWorld.Entities.Add(new Food(this));
        }

        /// <summary>
        /// Check to see if player is doing anything to the enemy.
        /// </summary>
        private void CheckInteractionsWithPlayer()
        {
            Player player = GameWorld.Player;

            //Deals damage to player if he is touching.
            if (IsIntersectingPlayer() && !IsAboutToDie)
            {
                player.TakeDamage(this, GetTouchDamage());
            }

            foreach (Projectile proj in GameWorld.PlayerProjectiles)
            {
                if (proj.GetCollRectangle().Intersects(CollRectangle))
                {
                    TakeDamage(player,proj.DamageOnHit);
                    proj.ToDelete = true;
                }
            }

        }

        private void CheckCollisionWithOtherEnemies()
        {
            foreach (Entity en in GameWorld.Entities)
            {
                if (en is Enemy && en != this)
                {
                    Enemy enemy = (Enemy) en;
                    if (!enemy.IsCollidableWithEnemies) continue;
                    if (en.GetCollRectangle().Intersects(CollRectangle))
                    {
                        int collResolve = 1;
                        // If en is to the right.
                        if (en.GetCollRectangle().X > CollRectangle.X)
                        {
                            en.SetVelX(collResolve);
                            SetVelX(-collResolve);
                        }
                        else
                        {
                            en.SetVelX(-collResolve);
                            SetVelX(collResolve);
                        };
                    }
                }
            }
        }
    }
}
