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
    public abstract partial class NewEnemy : Entity
    {
        const short RangeRadius = 2000;
        const short MeanResetTime = 5000;

        Timer hitByPlayerTimer;
        Timer wasMeanTimer;

        /// <summary>
        /// The box on the enemy that defines where it can take damage from the player jumping on it.
        /// </summary>
        protected virtual Rectangle DamageBox
        {
            get { return DamageBox; }
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

        int health;
        bool healthGiven;
        /// <summary>
        /// The current health of the enemy.
        /// </summary>
        protected int Health
        {
            get
            {
                if (!healthGiven)
                {
                    health = MaxHealth;
                }

                return health;
            }
            set
            {
                health = value;
            }
        }

        int maxHealth;
        /// <summary>
        /// The maximum amount of health the enemy can have. This is the value that is given to the enemy when it respawns.
        /// </summary>
        protected abstract int MaxHealth
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
            return (RangeRect.Intersects(player.collRectangle));
        }

        /// <summary>
        /// Checks to see if the enemy has recently taken damage.
        /// </summary>
        /// <returns></returns>
        protected bool HasTakenDamageRecently()
        {
            return (hitByPlayerTimer.TimeElapsedInSeconds < 2);
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
    }
}
