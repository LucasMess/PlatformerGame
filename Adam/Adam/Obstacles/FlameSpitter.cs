using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;
using Microsoft.Xna.Framework;
using Adam.Misc;

namespace Adam.Obstacles
{
    class FlameSpitter : Obstacle
    {
        Timer _firingTimer = new Timer();
        Timer _particleTimer = new Timer();
        bool _isFlaming;
        Rectangle _sourceRectangleOfParticle;
        SoundFx _flameSound;

        protected override Rectangle DrawRectangle
        {
            get
            {
                return CollRectangle;
            }
        }

        public FlameSpitter(Tile sourceTile)
        {
            _flameSound = new SoundFx("Sounds/Flame Spitter/flame", this);
            CollRectangle = sourceTile.DrawRectangle;
        }

        public override void Update()
        {
            CheckIfChangingState();

            AttackBox = new Rectangle(CollRectangle.X, CollRectangle.Y - (Main.Tilesize * 4), Main.Tilesize, Main.Tilesize * 4);

            if (_isFlaming)
            {
                
                if (_particleTimer.TimeElapsedInMilliSeconds > 100)
                {
                    FlameParticle par = new FlameParticle(this, Color.LightBlue);
                    FlameParticle par2 = new FlameParticle(this, Color.Red);
                    FlameParticle par3 = new FlameParticle(this, Color.Yellow);
                    GameWorld.Instance.Particles.Add(par);
                    GameWorld.Instance.Particles.Add(par2);
                    GameWorld.Instance.Particles.Add(par3);
                    _particleTimer.Reset();
                    _flameSound.PlayIfStopped();
                }
            }
            else
            {
                _flameSound.Stop();
            }

            Player.Player player = GameWorld.Instance.Player;
            if (_isFlaming && AttackBox.Intersects(player.GetCollRectangle()))
            {
                player.IsOnFire = true;
            }


            base.Update();
        }

        /// <summary>
        /// Updates timer and determines whether the flamespitter is firing or not.
        /// </summary>
        private void CheckIfChangingState()
        {
        
            if (!_isFlaming)
            {
                if (_firingTimer.TimeElapsedInMilliSeconds > 3000)
                {
                    _isFlaming = true;
                    _firingTimer.Reset();
                }
            }
            else
            {
                if (_firingTimer.TimeElapsedInMilliSeconds > 3000)
                {
                    _isFlaming = false;
                    _firingTimer.Reset();
                }
            }
        }


    }
}
