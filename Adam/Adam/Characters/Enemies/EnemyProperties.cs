using Adam.Misc;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;
using Adam.PlayerCharacter;

namespace Adam.Characters.Enemies
{
    /// <summary>
    /// Class inherited by all enemies that contains basic functionality
    /// </summary>
    public abstract partial class Enemy : Character
    {
        const short RangeRadius = 2000;
        const int MeanResetTime = 500000;
        bool _isTakingDamage = false;

        Timer _wasMeanTimer = new Timer(true);

        protected Enemy()
        {
            GameWorld.Instance.GetPlayer().PlayerAttacked += OnPlayerAttack;
            HasFinishedDying += Enemy_HasFinishedDying;
            RespawnPos = new Vector2(CollRectangle.X, CollRectangle.Y);
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
            Gem.Generate(MaxHealth/10,this);
            PlayDeathSound();
        }

        /// <summary>
        /// The ID that identifies the enemy type.
        /// </summary>
        public abstract byte Id
        {
            get;
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
            Player player = GameWorld.Instance.Player;
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
                _startTimeOfBeingMean = GameWorld.RandGen.Next(0, MeanResetTime);
                _wasMeanTimer.SetToInMilliseconds(_startTimeOfBeingMean);
            }

            return (_wasMeanTimer.TimeElapsedInMilliSeconds > MeanResetTime);
        }

        /// <summary>
        /// Returns true is the player is to the right of the enemy.
        /// </summary>
        /// <returns></returns>
        protected bool IsPlayerToTheRight()
        {
            Player player = GameWorld.Instance.Player;
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
            Player player = GameWorld.Instance.Player;
            if (player.GetCollRectangle().Y < CollRectangle.Y)
                return true;
            else return false;
        }

        /// <summary>
        /// Returns true if the player is intersecting the enemy's collision rectangle.
        /// </summary>
        /// <returns></returns>
        protected bool IsIntersectingPlayer()
        {
            Player player = GameWorld.Instance.Player;
            return (player.GetCollRectangle().Intersects(CollRectangle));
        }

        /// <summary>
        /// Returns true if the player is intersecting the enemy's damage box.
        /// </summary>
        /// <returns></returns>
        protected bool IsBeingAttacked()
        {
            Player player = GameWorld.Instance.Player;
            return (player.GetCollRectangle().Intersects(DamageBox) && player.GetCollRectangle().Y < DamageBox.Y && player.GetVelocity().Y > 1);
        }

        /// <summary>
        /// Returns the amount of damage this enemy deals when being touched.
        /// </summary>
        /// <returns></returns>
        protected int GetTouchDamage()
        {
            switch (Id)
            {
                case 201:
                    return EnemyDb.SnakeTouchDamage;
                case 202:
                    return EnemyDb.FrogTouchDamage;
                case 204:
                    return EnemyDb.LostTouchDamage;
                case 205:
                    return EnemyDb.HellboarTouchDamage;
                case 206:
                    return EnemyDb.FallingBoulderTouchDamage;
                case 207:
                    return EnemyDb.BatTouchDamage;
                case 208:
                    return EnemyDb.DuckTouchDamage;
                case 209:
                    return 0;
                default:
                    return 0;
            }

        }

        /// <summary>
        /// Returns the amount of damage this enemy deals when it hits something with its projectile.
        /// </summary>
        /// <returns></returns>
        public int GetProjectileDamage()
        {
            switch (Id)
            {
                case 0:
                    return EnemyDb.SnakeProjectileDamage;
                default:
                    return 0;
            }
        }



    }
}
