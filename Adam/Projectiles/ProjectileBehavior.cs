using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThereMustBeAnotherWay.Projectiles
{
    public static class ProjectileBehavior
    {
        public const int DEFAULT_RADIUS = 10;
        public const int DEFAULT_RIBBON_WIDTH = 16;
        public const int DEFAULT_EXPLOSION_WIDTH = 48;
        public const int DEFAULT_PARTICLE_SPAWN_INTERVAL = 400;
        public const bool DEFAULT_CAN_COLLIDE_TERRAIN = true;
        public const bool DEFAULT_CAN_COLLIDE_OTHER_PROJECTILES = false;

        public static void ApplyFade(Projectile projectile, float rate = .005f)
        {

        }

        public static void ApplyGravity(Projectile projectile, float strength = 1f)
        {

        }
    }
}
