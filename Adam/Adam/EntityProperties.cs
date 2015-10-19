using Adam.Misc;
using Adam.Misc.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    public partial class Entity
    {
        protected ComplexAnimation complexAnim = new ComplexAnimation();


        public SoundFxManager Sounds { get; set; }

        /// <summary>
        /// Determines whether this entity should perform collision checks with other collidable objects.
        /// </summary>
        public bool IsCollidable { get; set; } = true;

        /// <summary>
        /// Determines whether this entity should be affected by gravity.
        /// </summary>
        public bool ObeysGravity { get; set; } = true;

        /// <summary>
        /// Determines whether the entity is in the air.
        /// </summary>
        public bool IsJumping { get; set; }


    }
}
