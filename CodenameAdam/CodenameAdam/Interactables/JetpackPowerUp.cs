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
    class JetpackPowerUp : PowerUp
    {
        GameTime gameTime;
        bool isHovering;
        double hoverTimer;

        public JetpackPowerUp(int x, int y)
        {
            texture = ContentHelper.LoadTexture("Objects/jetpack");
            loop = ContentHelper.LoadSound("Sounds/jetpack_engine");
            loopInstance = loop.CreateInstance();
            drawRectangle = new Rectangle(x, y, 32, 32);
            animation = new Animation(texture, drawRectangle, 100, 0, AnimationType.Loop);
            velocity.Y = -10f;
        }

        public override void Update(GameTime gameTime, Player player, Map map)
        {
            base.Update(gameTime, player, map);
            this.gameTime = gameTime;

            animation.UpdateRectangle(drawRectangle);
            animation.Update(gameTime);

            Hover();

            effectTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (effectTimer > 100)
            {
                Particle eff = new Particle();
                eff.CreateJetPackSmokeParticle(this);
                effects.Add(eff);
                effectTimer = 0;
            }

            elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (player.collRectangle.Intersects(drawRectangle) && elapsedTime > 500)
            {
                wasPickedUp = true;
                toDelete = true;
            }

            if (wasPickedUp)
            {
                player.Chronoshift(Evolution.Future);
                loopInstance.Stop();
            }

            if (loopInstance.State == SoundState.Stopped)
                loopInstance.Play();

            float xDist = player.collRectangle.Center.X - drawRectangle.Center.X;
            float yDist = player.collRectangle.Center.Y - drawRectangle.Center.Y;
            float distanceTo = CalcHelper.GetPythagoras(xDist, yDist);
            if (distanceTo > 1000)
                loopInstance.Volume = 0;
            else loopInstance.Volume = .5f - (distanceTo / 1000) / 2;
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

            foreach (var eff in effects)
                eff.Draw(spriteBatch);
        }
    }
}
