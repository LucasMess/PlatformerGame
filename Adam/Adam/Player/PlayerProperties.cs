using Adam.Characters;
using Adam.Misc;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    public partial class Player : Character
    {
        Timer onFireTimer = new Timer();
        Timer fireTickTimer = new Timer();
        Timer fireSpawnTimer = new Timer();
        Timer movementParticlesTimer = new Timer();

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
                return (GameWorld.Instance.tileArray[GetTileIndex()].isClimbable);
            }
        }

        /// <summary>
        /// The amount of damage the player deals.
        /// </summary>
        private int damageAmount { get; set; }

        /// <summary>
        /// The area in which the player deals damage.
        /// </summary>
        private Rectangle damageArea { get; set; } 

    }
}
