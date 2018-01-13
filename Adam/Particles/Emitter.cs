using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThereMustBeAnotherWay.Levels;

namespace ThereMustBeAnotherWay.Particles
{
    /// <summary>
    /// Emits a given particle on a timer.
    /// </summary>
    public partial class ParticleSystem
    {
        internal List<Emitter> _emitters = new List<Emitter>();

        /// <summary>
        /// Generates a new particle on an interval.
        /// </summary>
        public class Emitter
        {
            private Misc.GameTimer _timer = new Misc.GameTimer();
            public Emitter(ParticleData data, int intervalInMilliseconds)
            {
                GameWorld.ParticleSystem._emitters.Add(this);       
                _timer.ResetAndWaitFor(intervalInMilliseconds);
                _timer.SetTimeReached += delegate ()
                {
                    GameWorld.ParticleSystem.Add(data);
                };
            }

            internal void Update()
            {
                _timer.Increment();
            }
        }
    }
}
