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
    public class JetpackPowerUp : Item
    {
        GameTime gameTime;
        bool isHovering;
        double hoverTimer;

        public JetpackPowerUp(int x, int y)
        {
            Texture = ContentHelper.LoadTexture("Objects/jetpack");
            loopSound = new Misc.SoundFx("Sounds/jetpack_engine");
            drawRectangle = new Rectangle(x, y, 32, 32);
            animation = new Animation(Texture, drawRectangle, 100, 0, AnimationType.Loop);
            velocity.Y = -10f;
        }

        public override void Update()
        {
            GameWorld gameWorld = GameWorld.Instance;
            gameTime = gameWorld.gameTime;

            animation.UpdateRectangle(drawRectangle);
            animation.Update(gameTime);

            Hover();

            effectTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (effectTimer > 100)
            {
                Particle eff = new Particle();
                eff.CreateJetPackSmokeParticle(this);
                GameWorld.Instance.particles.Add(eff);
                effectTimer = 0;
            }

            base.Update();
        }

        private void Hover()
        {
            if (isHovering)
            {
                hoverTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (hoverTimer > 100)
                {
                    velocity.Y = -velocity.Y;
                    hoverTimer = 0;
                }
            }
            else
            {
                if (velocity.Y < 0)
                {
                    velocity.Y += .3f;
                }
                else
                {
                    isHovering = true;
                    velocity.Y = 1f;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch);
        }
    }
}
