using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThereMustBeAnotherWay.Particles
{
    /// <summary>
    /// Passed to other particle components. Contains information about how the particle should be created.
    /// </summary>
    public struct ParticleData
    {
        public ParticleType Type { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Color Color { get; set; }
        public string Text { get; set; }
    }
}
