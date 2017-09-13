﻿using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.PlayerCharacter;
using Microsoft.Xna.Framework;

namespace ThereMustBeAnotherWay.Characters.Enemies
{
    /// <summary>
    /// Class inherited by all enemies that contains basic functionality
    /// </summary>
    public abstract partial class Enemy : Character
    {
        const short RangeRadius = 2000;
        const int MeanResetTime = 500000;

        Timer _wasMeanTimer = new Timer(true);

        protected Enemy()
        {
            GameWorld.GetPlayer().PlayerAttacked += OnPlayerAttack;
            HasFinishedDying += Enemy_HasFinishedDying;
            RespawnPos = new Vector2(CollRectangle.X, CollRectangle.Y);
            AddAnimationToQueue("idle");
        }

        public bool IsCollidableWithEnemies { get; set; } = true;

        /// <summary>
        /// Makes the enemy drop gems.
        /// </summary>
        /// <param name="entity"></param>
        private void Enemy_HasFinishedDying(Entity entity)
        {
            _respawnTimer.ResetAndWaitFor(120 * 1000);
            _respawnTimer.SetTimeReached += Revive;
            PlayDeathSound();
        }

        /// <summar
        /// The box on the enemy that defines where it can take damage from the player jumping on it.
        /// </summary>
        protected virtual Rectangle DamageBox
        {
            get { return new Rectangle(CollRectangle.X - 5, CollRectangle.Y - 20, CollRectangle.Width + 10, CollRectangle.Height / 2); }
        }

        /// <summary>
        /// The rectangle that the player must intersect for the enemy to be in range for drawing and updating.
        /// </summary>
        Rectangle _rangeRect;
        protected Rectangle RangeRect
        {
            get
            {
                if (_rangeRect == null)
                {
                    _rangeRect = new Rectangle(0, 0, RangeRadius, RangeRadius);
                }

                _rangeRect.X = CollRectangle.X - _rangeRect.Width / 2;
                _rangeRect.Y = CollRectangle.Y - _rangeRect.Height / 2;

                return _rangeRect;
            }
        }

        /// <summary>
        /// Sets the maximum health the entity can have. Enemy types must declare their max health.
        /// </summary>
        public override abstract int MaxHealth
        {
            get;
        }

        /// <summary>
        /// The sound played when the enemy is standing still.
        /// </summary>
        protected abstract SoundFx MeanSound
        {
            get;
        }

        /// <summary>
        /// The sound played when the enemy attacks the player.
        /// </summary>
        protected abstract SoundFx AttackSound
        {
            get;
        }

        /// <summary>
        /// The sound played when the enemy dies.
        /// </summary>
        protected abstract SoundFx DeathSound
        {
            get;
        }

        SoundFx _disappearSound;
        /// <summary>
        /// The sound played when the enemy poofs.
        /// </summary>
        protected SoundFx DisappearSound
        {
            get
            {
                if (_disappearSound == null)
                {
                    _disappearSound = new SoundFx("Sounds/Player/enemy_kill");
                }
                return _disappearSound;
            }
        }

        SoundFx _jumpedOnSound;
        /// <summary>
        /// The sound played when the player jumps on the enemy.
        /// </summary>
        protected SoundFx HitSound
        {
            get
            {
                if (_jumpedOnSound == null)
                {
                    _jumpedOnSound = new SoundFx("Player/enemy_jumpedOn");
                }
                return _jumpedOnSound;
            }
        }

        /// <summary>
        /// Return whether the enemy is currently in range of the player.
        /// </summary>
        /// <returns></returns>
        protected bool IsInRange()
        {
            Player player = GameWorld.Player;
            return (RangeRect.Intersects(player.GetCollRectangle()));
        }



        int _startTimeOfBeingMean;
        /// <summary>
        /// Checks to see if enemy should make mean sound.
        /// </summary>
        /// <returns></returns>
        private bool IsTimeToBeMean()
        {
            //Starts the timer at a random position.
            if (_startTimeOfBeingMean == 0)
            {
                _startTimeOfBeingMean = TMBAW_Game.Random.Next(0, MeanResetTime);
                _wasMeanTimer.SetToInMilliseconds(_startTimeOfBeingMean);
            }

            return (_wasMeanTimer.TimeElapsedInMilliSeconds > MeanResetTime);
        }


        private Rectangle _vulnerableArea;
        /// <summary>
        /// The part of the collision rectangle that the player can hit.
        /// </summary>
        public Rectangle VulnerableArea
        {
            get
            {
                _vulnerableArea.X = CollRectangle.X;
                _vulnerableArea.Y = CollRectangle.Y;
                _vulnerableArea.Width = CollRectangle.Width;
                _vulnerableArea.Height = (int)(CollRectangle.Height * .2);
                return _vulnerableArea;
            }
        }


        private Rectangle _damagingArea;
        /// <summary>
        /// The part of the collision rectangle that will damage the player.
        /// </summary>
        public Rectangle DamagingArea
        {
            get
            {
                _damagingArea.Width = CollRectangle.Width;
                _damagingArea.Height = (int)(CollRectangle.Height - CollRectangle.Height * .2);
                _damagingArea.X = CollRectangle.X;
                _damagingArea.Y = CollRectangle.Y + (int)(CollRectangle.Height * .2);
                return _vulnerableArea;
            }
        }

        /// <summary>
        /// Returns true is the player is to the right of the enemy.
        /// </summary>
        /// <returns></returns>
        protected bool IsPlayerToTheRight()
        {
            Player player = GameWorld.Player;
            if (player.GetCollRectangle().X > CollRectangle.X)
                return true;
            else return false;
        }

        /// <summary>
        /// Returns true if the player is above the enemy.
        /// </summary>
        /// <returns></returns>
        protected bool IsPlayerAbove()
        {
            Player player = GameWorld.Player;
            if (player.GetCollRectangle().Y < CollRectangle.Y)
                return true;
            else return false;
        }

        /// <summary>
        /// Returns true if the player is intersecting the enemy's collision rectangle.
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsIntersectingPlayer()
        {
            Player player = GameWorld.Player;
            return (player.GetCollRectangle().Intersects(CollRectangle));
        }

        /// <summary>
        /// Returns true if the player is intersecting the enemy's damage box.
        /// </summary>
        /// <returns></returns>
        protected bool IsBeingAttacked()
        {
            Player player = GameWorld.Player;
            return (player.GetCollRectangle().Intersects(DamageBox) && player.GetCollRectangle().Y < DamageBox.Y && player.GetVelocity().Y > 1);
        }

        /// <summary>
        /// Returns the amount of damage this enemy deals when being touched.
        /// </summary>
        /// <returns></returns>
        protected int GetTouchDamage()
        {
            return 0;

        }

        /// <summary>
        /// Returns the amount of damage this enemy deals when it hits something with its projectile.
        /// </summary>
        /// <returns></returns>
        public int GetProjectileDamage()
        {
            return 0;
        }



    }
}
