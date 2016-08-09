using Adam.Characters;
using Adam.Levels;
using Adam.Misc;
using Microsoft.Xna.Framework;

namespace Adam.PlayerCharacter
{
    public partial class Player : Character
    {
        Timer _onFireTimer = new Timer(true);
        Timer _fireTickTimer = new Timer(true);
        Timer _fireSpawnTimer = new Timer(true);
        Timer _movementParticlesTimer = new Timer(true);

        /// <summary>
        /// Returns true if player is on fire and is taking damage per second.
        /// </summary>
        public bool IsOnFire { get; set; }

        /// <summary>
        /// Returns true if the player is holding down the run fast button.
        /// </summary>
        public bool IsRunningFast { get; set; }

        /// <summary>
        /// Returns true if the player is currently on top of vines.
        /// </summary>
        public bool IsOnVines
        {
            get
            {
                int index = GetTileIndex();
                Tile tile1 = GameWorld.GetTile(index);
                Tile tile2 = GameWorld.GetTileBelow(index);
                if ((tile1 != null && tile1.IsClimbable) || (tile2 != null && tile2.IsClimbable))
                {
                    return true;
                }
                ObeysGravity = true;
                return false;
            }
        }

        /// <summary>
        /// The amount of damage the player deals.
        /// </summary>
        private int DamageAmount { get; set; }

        /// <summary>
        /// The area in which the player deals damage.
        /// </summary>
        private Rectangle DamageArea { get; set; }

    }
}
