using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Misc.Sound;
using Microsoft.Xna.Framework;
using ThereMustBeAnotherWay.Levels;

namespace ThereMustBeAnotherWay
{
    public partial class Entity
    {
        protected ComplexAnimation _complexAnimation = new ComplexAnimation();

        readonly GameTimer _hitRecentlyTimer = new GameTimer(false, 10000);
        readonly GameTimer _deathAnimationTimer = new GameTimer(true);
        protected readonly GameTimer _respawnTimer = new GameTimer(true);

        public SoundFxManager Sounds { get; set; }
        /// <summary>
        /// Determines whether this entity should perform collision checks with other collidable objects.
        /// </summary>

        private bool _isCollidable = true;
        public bool IsCollidable
        {
            get
            {
                if (IsPlayingDeathAnimation)
                {
                    return false;
                }
                else return _isCollidable;
            }

            set
            {
                _isCollidable = value;
            }
        }

        /// <summary>
        /// Where the entity was originally placed at. Used to avoid respawning entities when defining textures.
        /// </summary>
        public int TileIndexSpawn { get; set; } = -1;

        /// <summary>
        /// Determines whether this entity should be affected by gravity.
        /// </summary>
        public bool ObeysGravity { get; set; } = true;


        private bool _isJumping;
        /// <summary>
        /// Determines whether the entity is in the air.
        /// </summary>
        public bool IsJumping
        {
            get
            {
                // Prevent entities from jumping while in water.
                 if (IsInWater()) return true;
                return _isJumping;
            }
            set
            {
                _isJumping = value;
            }
        }

        /// <summary>
        /// Determines if the entity has recently taken damage.
        /// </summary>
        public bool IsTakingDamage
        {
            get; private set;
        }
        /// <summary>
        /// Used in calculations such as how far back the entity is knocked back and friction against ground. If friction is zero,
        /// then no friction is applied.
        /// </summary>
        public int Weight { get; set; } = 10;

        public Vector2 RespawnPos { get; set; }

    }
}
