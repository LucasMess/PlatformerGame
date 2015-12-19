using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Interactables
{
    public class Jetpack
    {
        int _currentFuel;
        int _maxFuel = 10000;

        int _depletionRate = 10;
        int _refuelingRate = 50;

        double _effectTimer;

        SoundEffect _loop;
        SoundEffectInstance _loopInstance;
        List<Particle> _particles = new List<Particle>();

        public bool HasFuel { get; set; }
        public const float MaxSpeed  = -10f;

        public Jetpack()
        {
            _currentFuel = _maxFuel;
            _loop = ContentHelper.LoadSound("Sounds/jetpack_engine");
            _loopInstance = _loop.CreateInstance();
        }

        public void Update(Player.Player player, GameTime gameTime)
        {
            if (player.IsJumping)
            {
                _effectTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                _currentFuel -= _depletionRate;
                if (_loopInstance.State == SoundState.Stopped)
                    _loopInstance.Play();

                if (_effectTimer > 100 && InputHelper.IsLeftMousePressed() && HasFuel)
                {
                    Particle eff = new Particle();
                    eff.CreateJetPackSmokeParticle(player);
                    _particles.Add(eff);
                    _effectTimer = 0;
                }
            }
            else 
                _currentFuel += _refuelingRate;

            if (_currentFuel <= 0)
            {
                HasFuel = false;
            }
            else HasFuel = true;

            if (_currentFuel > _maxFuel)
                _currentFuel = _maxFuel;

            foreach (var eff in _particles)
            {
                eff.Update(gameTime);
                if (eff.ToDelete)
                {
                    _particles.Remove(eff);
                    break;
                }
            }

            if (InputHelper.IsLeftMouseReleased() || !HasFuel)
                _loopInstance.Stop();

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var par in _particles)
                par.Draw(spriteBatch);
        }
    }
}
