using Adam.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    public partial class Player : Entity
    {
        Timer onFireTimer = new Timer();
        Timer fireTickTimer = new Timer();
        Timer fireSpawnTimer = new Timer();

        /// <summary>
        /// Returns true if player is on fire and is taking damage per second.
        /// </summary>
        public bool IsOnFire { get; set; }
    }
}
