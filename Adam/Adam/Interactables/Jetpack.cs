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
        int currentFuel;
        int maxFuel = 10000;

        int depletionRate = 10;
        int refuelingRate = 50;

        double effectTimer;

        SoundEffect loop;
        SoundEffectInstance loopInstance;
        List<Particle> particles = new List<Particle>();

        public bool HasFuel { get; set; }
        public const float MaxSpeed  = -10f;

        public Jetpack()
        {
            currentFuel = maxFuel;
            loop = ContentHelper.LoadSound("Sounds/jetpack_engine");
            loopInstance = loop.CreateInstance();
        }

        public void Update(Player player, GameTime gameTime)
        {
            if (player.IsJumping)
            {
                effectTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                currentFuel -= depletionRate;
                if (loopInstance.State == SoundState.Stopped)
                    loopInstance.Play();

                if (effectTimer > 100 && InputHelper.IsLeftMousePressed() && HasFuel)
                {
                    Particle eff = new Particle();
                    eff.CreateJetPackSmokeParticle(player);
                    particles.Add(eff);
                    effectTimer = 0;
                }
            }
            else 
                currentFuel += refuelingRate;

            if (currentFuel <= 0)
            {
                HasFuel = false;
            }
            else HasFuel = true;

            if (currentFuel > maxFuel)
                currentFuel = maxFuel;

            foreach (var eff in particles)
            {
                eff.Update(gameTime);
                if (eff.ToDelete)
                {
                    particles.Remove(eff);
                    break;
                }
            }

            if (InputHelper.IsLeftMouseReleased() || !HasFuel)
                loopInstance.Stop();

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var par in particles)
                par.Draw(spriteBatch);
        }
    }
}
