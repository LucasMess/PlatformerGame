using Adam.Misc;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Characters.Enemies
{
    /// <summary>
    /// Class inherited by all enemies that contains basic functionality
    /// </summary>
    public abstract partial class Enemy : Entity
    {
        const short RangeRadius = 2000;
        const int MeanResetTime = 500000;

        Timer hitByPlayerTimer = new Timer();
        Timer wasMeanTimer = new Timer();

        /// <summary>
        /// The ID that identifies the enemy type.
        /// </summary>
        public abstract byte ID
        {
            get;
        }

        /// <summary>
        /// The original rectangle when the enemy spawned.
        /// </summary>
        public abstract Rectangle RespawnLocation
        {
            get;
        }

        /// <summar
        /// The box on the enemy that defines where it can take damage from the player jumping on it.
        /// </summary>
        protected virtual Rectangle DamageBox
        {
            get { return new Rectangle(collRectangle.X - 5, collRectangle.Y - 20, collRectangle.Width + 10, collRectangle.Height / 2); }
        }

        /// <summary>
        /// The rectangle that the player must intersect for the enemy to be in range for drawing and updating.
        /// </summary>
        Rectangle rangeRect;
        protected Rectangle RangeRect
        {
            get
            {
                if (rangeRect == null)
                {
                    rangeRect = new Rectangle(0, 0, RangeRadius, RangeRadius);
                }

                rangeRect.X = collRectangle.X - rangeRect.Width / 2;
                rangeRect.Y = collRectangle.Y - rangeRect.Height / 2;

                return rangeRect;
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

        SoundFx disappearSound;
        /// <summary>
        /// The sound played when the enemy poofs.
        /// </summary>
        protected SoundFx DisappearSound
        {
            get
            {
                if (disappearSound == null)
                {
                    disappearSound = new SoundFx("Sounds/Player/enemy_kill");
                }
                return disappearSound;
            }
        }

        SoundFx jumpedOnSound;
        /// <summary>
        /// The sound played when the player jumps on the enemy.
        /// </summary>
        protected SoundFx JumpedOnSound
        {
            get
            {
                if (jumpedOnSound == null)
                {
                    jumpedOnSound = new SoundFx("Sounds/Player/enemy_jumpedOn");
                }
                return jumpedOnSound;
            }
        }

        /// <summary>
        /// Return whether the enemy is currently in range of the player.
        /// </summary>
        /// <returns></returns>
        protected bool IsInRange()
        {
            Player player = GameWorld.Instance.player;
            return (RangeRect.Intersects(player.GetCollRectangle()));
        }

        /// <summary>
        /// Checks to see if the enemy has recently taken damage.
        /// </summary>
        /// <returns></returns>
        protected bool HasTakenDamageRecently()
        {
            return (hitByPlayerTimer.TimeElapsedInSeconds < .2);
        }


        int startTimeOfBeingMean;
        /// <summary>
        /// Checks to see if enemy should make mean sound.
        /// </summary>
        /// <returns></returns>
        private bool IsTimeToBeMean()
        {
            //Starts the timer at a random position.
            if (startTimeOfBeingMean == 0)
            {
                startTimeOfBeingMean = GameWorld.RandGen.Next(0, MeanResetTime);
                wasMeanTimer.SetToInMilliseconds(startTimeOfBeingMean);
            }

            return (wasMeanTimer.TimeElapsedInMilliSeconds > MeanResetTime);
        }

        /// <summary>
        /// Returns true is the player is to the right of the enemy.
        /// </summary>
        /// <returns></returns>
        protected bool IsPlayerToTheRight()
        {
            Player player = GameWorld.Instance.player;
            if (player.GetCollRectangle().X > collRectangle.X)
                return true;
            else return false;
        }

        /// <summary>
        /// Returns true if the player is above the enemy.
        /// </summary>
        /// <returns></returns>
        protected bool IsPlayerAbove()
        {
            Player player = GameWorld.Instance.player;
            if (player.GetCollRectangle().Y < collRectangle.Y)
                return true;
            else return false;
        }

        /// <summary>
        /// Returns true if the player is intersecting the enemy's collision rectangle.
        /// </summary>
        /// <returns></returns>
        protected bool IsIntersectingPlayer()
        {
            Player player = GameWorld.Instance.player;
            return (player.GetCollRectangle().Intersects(collRectangle));
        }

        /// <summary>
        /// Returns true if the player is intersecting the enemy's damage box.
        /// </summary>
        /// <returns></returns>
        protected bool IsBeingAttacked()
        {
            Player player = GameWorld.Instance.player;
            return (player.GetCollRectangle().Intersects(DamageBox) && player.GetCollRectangle().Y < DamageBox.Y && player.GetVelocity().Y > 1);
        }

        /// <summary>
        /// Returns the amount of damage this enemy deals when being touched.
        /// </summary>
        /// <returns></returns>
        protected int GetTouchDamage()
        {
            switch (ID)
            {
                case 201:
                    return EnemyDB.Snake_TouchDamage;
                case 202:
                    return EnemyDB.Frog_TouchDamage;
                case 204:
                    return EnemyDB.Lost_TouchDamage;
                case 205:
                    return EnemyDB.Hellboar_TouchDamage;
                case 206:
                    return EnemyDB.FallingBoulder_TouchDamage;
                case 207:
                    return EnemyDB.Bat_TouchDamage;
                case 208:
                    return EnemyDB.Duck_TouchDamage;
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
            switch (ID)
            {
                case 0:
                    return EnemyDB.Snake_ProjectileDamage;
                default:
                    return 0;
            }
        }


    }
}
