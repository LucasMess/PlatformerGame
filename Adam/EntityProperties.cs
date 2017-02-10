using Adam.Misc;
using Adam.Misc.Sound;
using Microsoft.Xna.Framework;

namespace Adam
{
    public partial class Entity
    {
        protected ComplexAnimation ComplexAnim = new ComplexAnimation();

        readonly Timer _hitRecentlyTimer = new Timer(true);
        readonly Timer _deathAnimationTimer = new Timer(true);
        protected readonly Timer _respawnTimer = new Timer(true);

        public SoundFxManager Sounds { get; set; }
        /// <summary>
        /// Determines whether this entity should perform collision checks with other collidable objects.
        /// </summary>
        public bool IsCollidable { get; set; } = true;

        /// <summary>
        /// Where the entity was originally placed at. Used to avoid respawning entities when defining textures.
        /// </summary>
        public int TileIndexSpawn { get; private set; }

        /// <summary>
        /// Determines whether this entity should be affected by gravity.
        /// </summary>
        public bool ObeysGravity { get; set; } = true;

        /// <summary>
        /// Determines whether the entity is in the air.
        /// </summary>
        public bool IsJumping { get; set; }

        /// <summary>
        /// Determines if the entity has recently taken damage.
        /// </summary>
        public bool IsTakingDamage
        {
            get; private set;
        }
        /// <summary>
        /// Used in calculations such as how far back the entity is knocked back and friction against ground.
        /// </summary>
        public int Weight { get; set; } = 10;

        public Vector2 RespawnPos { get; set; }

    }
}
