using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;
using Microsoft.Xna.Framework;
using Adam.Misc;

namespace Adam.Obstacles
{
    class MachineGun : Obstacle
    {
        bool _isFiring;
        Timer _firingTimer = new Timer();
        Timer _bulletSpacingTimer = new Timer();
        const int BulletInterval = 50;
        const int FiringInterval = 1000;
        bool _firingRight;
        SoundFx _firingSound;

        protected override Rectangle DrawRectangle
        {
            get
            {
                return CollRectangle;
            }
        }

        public MachineGun(Tile sourceTile)
        {
            AttackBox = new Rectangle(sourceTile.DrawRectangle.X, sourceTile.DrawRectangle.Y - Main.Tilesize * 5, Main.Tilesize, Main.Tilesize * 5);
            CollRectangle = sourceTile.DrawRectangle;

            _firingSound = new SoundFx("Sounds/Machine Gun/fire",this);
        }

        public override void Update()
        {
            if (_firingTimer.TimeElapsedInMilliSeconds > FiringInterval)
            {
                _firingTimer.Reset();
                _isFiring = !_isFiring;
            }

            if (_isFiring)
            {
                if (_bulletSpacingTimer.TimeElapsedInMilliSeconds > BulletInterval)
                {
                    if (_firingRight)
                    {
                        MachineGunParticle par = new MachineGunParticle(this, 8);
                        ExplosionParticle exp = new ExplosionParticle(CollRectangle.X + 8, CollRectangle.Y, Color.White, .7f);
                        GameWorld.Instance.Particles.Add(exp);
                        GameWorld.Instance.Particles.Add(par);
                        _firingSound.PlayNewInstanceOnce();
                        _firingSound.Reset();
                        _firingRight = !_firingRight;
                    }
                    else
                    {                      
                        MachineGunParticle par = new MachineGunParticle(this, 24);
                        ExplosionParticle exp = new ExplosionParticle(CollRectangle.X + 24, CollRectangle.Y, Color.White,.7f);
                        GameWorld.Instance.Particles.Add(exp);
                        GameWorld.Instance.Particles.Add(par);
                        _firingRight = !_firingRight;
                        _firingSound.PlayNewInstanceOnce();
                        _firingSound.Reset();
                    }
                    _bulletSpacingTimer.Reset();
                }
            }

            base.Update();
        }
    }
}
