using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Adam.Misc;

namespace Adam.Obstacles
{
    class MachineGun : Obstacle
    {
        bool isFiring;
        Timer firingTimer = new Timer();
        Timer bulletSpacingTimer = new Timer();
        const int BulletInterval = 50;
        const int FiringInterval = 1000;
        bool firingRight;

        protected override Rectangle DrawRectangle
        {
            get
            {
                return collRectangle;
            }
        }

        public MachineGun(Tile sourceTile)
        {
            attackBox = new Rectangle(sourceTile.drawRectangle.X, sourceTile.drawRectangle.Y - Main.Tilesize * 5, Main.Tilesize, Main.Tilesize * 5);
            collRectangle = sourceTile.drawRectangle;
        }

        public override void Update()
        {
            firingTimer.Increment();
            if (firingTimer.TimeElapsedInMilliSeconds > FiringInterval)
            {
                firingTimer.Reset();
                isFiring = !isFiring;
            }

            if (isFiring)
            {
                bulletSpacingTimer.Increment();
                if (bulletSpacingTimer.TimeElapsedInMilliSeconds > BulletInterval)
                {
                    if (firingRight)
                    {
                        MachineGunParticle par = new MachineGunParticle(this, 8);
                        ExplosionParticle exp = new ExplosionParticle(collRectangle.X + 8, collRectangle.Y, Color.White, .7f);
                        GameWorld.Instance.particles.Add(exp);
                        GameWorld.Instance.particles.Add(par);
                        firingRight = !firingRight;
                    }
                    else
                    {                      
                        MachineGunParticle par = new MachineGunParticle(this, 24);
                        ExplosionParticle exp = new ExplosionParticle(collRectangle.X + 24, collRectangle.Y, Color.White,.7f);
                        GameWorld.Instance.particles.Add(exp);
                        GameWorld.Instance.particles.Add(par);
                        firingRight = !firingRight;
                    }
                    bulletSpacingTimer.Reset();
                }
            }

            base.Update();
        }
    }
}
