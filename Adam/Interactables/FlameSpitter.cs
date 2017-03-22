using Adam.Levels;
using Adam.Misc;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adam.Interactables
{
    class FlameSpitter : Interactable
    {
        // This timer is infinite so that all spitters are in sync.
        private Timer _spitTimer = new Timer();
        private Timer _fireBallTimer = new Timer();

        private bool _isSpittingFire;
        private Vector2 _leftCornerTile;
        private Rectangle _collRectangle;

        private static SoundFx _flameSound = new SoundFx("Sounds/Tiles/flame_spitter_fire");

        private const int FireTime = 3000;
        private const int IdleTime = 5000;
        private const int ParticleTime = 100;

        private const int Damage = 5;

        public FlameSpitter(Tile tile)
        {
            tile.AnimationStopped = true;
            _leftCornerTile = new Vector2(tile.GetDrawRectangle().X, tile.GetDrawRectangle().Y);
            _collRectangle = new Rectangle(tile.GetDrawRectangle().X + 3, tile.GetDrawRectangle().Y - AdamGame.Tilesize * 3 - 3, AdamGame.Tilesize - 6, AdamGame.Tilesize * 3 - 6);
        }

        public override void Update(Tile tile)
        {
            // Entity collision and damage.
            // TODO: Change collision detection so that only entities in the same chunk get checked.
            if (_isSpittingFire)
            {
                foreach (var entity in GameWorld.Entities)
                {
                    if (entity.CollRectangle.Intersects(_collRectangle))
                    {
                        entity.TakeDamage(null, Damage);
                    }
                }

                if (GameWorld.GetPlayer().CollRectangle.Intersects(_collRectangle))
                {
                    GameWorld.GetPlayer().TakeDamage(null, Damage);
                }
            }

            // Particles and visual effects.
            _spitTimer.Increment();
            if (_isSpittingFire)
            {
                tile.AnimationStopped = false;
                _flameSound.PlayIfStopped();
                if (_spitTimer.TimeElapsedInMilliSeconds < FireTime)
                {
                    _fireBallTimer.Increment();
                    if (_fireBallTimer.TimeElapsedInMilliSeconds > ParticleTime)
                    {
                        _fireBallTimer.Reset();
                        GameWorld.ParticleSystem.Add(Particles.ParticleType.FireBall, _leftCornerTile + new Vector2(3,0) * 2, CalcHelper.GetRandXAndY(new Rectangle(-10, -50, 20, 30)) / 10f, Color.White);
                        GameWorld.ParticleSystem.Add(Particles.ParticleType.FireBall, _leftCornerTile + new Vector2(12,0) * 2, CalcHelper.GetRandXAndY(new Rectangle(-10, -50, 20, 30)) / 10f, Color.White);
                    }
                }
                else
                {
                    _isSpittingFire = false;
                    _spitTimer.Reset();
                }
            }
            else
            {
                _flameSound.Stop();
                tile.AnimationStopped = true;
                tile.CurrentFrame = 1;
                if (_spitTimer.TimeElapsedInMilliSeconds > IdleTime)
                {
                    _isSpittingFire = true;
                    _spitTimer.Reset();
                }
            }

            base.Update(tile);
        }
    }
}
