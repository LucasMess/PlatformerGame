using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Misc.Interfaces
{
    /// <summary>
    /// Laws of gravity apply.
    /// </summary>
    interface INewtonian
    {
        /// <summary>
        /// The value of gravity. Set it to 0 for default.
        /// </summary>
        float GravityStrength { get; set; }
        bool IsFlying { get; set; }
        bool IsJumping { get; set; }
        bool IsAboveTile { get; set; }
    }
}
