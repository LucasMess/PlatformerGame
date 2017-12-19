using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThereMustBeAnotherWay.Projectiles
{

    public interface IProjectile
    {
        void Update();
        void SpawnInitialBurst();
        void SpawnExplosion();
        void SpawnParticles();
        void SpawnRibbon();

        int DamageRadius { get; }
        int ParticleSpawnTime { get; }
        int RibbonSpawnTime { get; }
    }
}

