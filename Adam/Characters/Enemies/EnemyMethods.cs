using ThereMustBeAnotherWay.Interactables;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.PlayerCharacter;
using Microsoft.Xna.Framework;

namespace ThereMustBeAnotherWay.Characters.Enemies
{
    public abstract partial class Enemy : Character
    {
        /// <summary>
        /// Updates the enemy.
        /// </summary>
        public override void Update()
        {
            if (TMBAW_Game.CurrentGameMode == GameMode.Play)
            {
                PlayMeanSound();
                CheckInteractionsWithPlayer();
                if (IsCollidableWithEnemies)
                    CheckCollisionWithOtherEnemies();
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
            {
                TakeDamage(GameWorld.GetPlayers()[0], damage);
                HitSound?.Play();
            }
        }

        protected override void Kill()
        {
            if (!IsDead)
            {
                Gem.Generate(MaxHealth / 10, this);
            }
            base.Kill();
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
            foreach (Player player in GameWorld.GetPlayers())
            {
                // Deal damage to the entity that is damaging the other.
                if (IsIntersectingPlayer() && !IsPlayingDeathAnimation)
                {
                    if (player.AttackArea.Intersects(VulnerableArea) && player.GetVelocity().Y > 0)
                    {
                        this.TakeDamage(player, player.GetDamage());
                        SetVelY(player.GetVelocity().Y);
                        player.OnJumpOnAnotherEntity(this);
                    }
                    else
                    {
                        player.TakeDamage(this, GetTouchDamage());
                    }
                }
            }

        }

        private void CheckCollisionWithOtherEnemies()
        {
            foreach (Entity en in GameWorld.Entities)
            {
                if (en is Enemy && en != this)
                {
                    Enemy enemy = (Enemy)en;
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
