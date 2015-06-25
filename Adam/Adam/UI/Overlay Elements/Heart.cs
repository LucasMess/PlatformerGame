using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI.Overlay_Elements
{
    class Heart
    {
        Texture2D aliveTexture, deadTexture, currentTexture;
        Rectangle drawRectangle, sourceRectangle;
        Vector2 origin;
        Vector2 heartFrameCount;
        bool isHeartPumping;
        bool heartHasPumped, heartHasPumped2;
        double pumpTimer;
        int currentFrameHeart, switchFrameHeart;
        double heartTimer, restartTimer;
        float heartRotation;
        bool isRotatingRight = true;
        Color healthColor;
        bool isHeartDead;
        int currentHealth;
        Vector2 textPos;
        int maxHealth;
        int oldCurrentHealth;

        SoundEffect heartBeat1, heartBeat2;
        SoundEffectInstance h1i, h2i;
        GameTime gameTime;
        Player player;
        GameWorld gameWorld;

        public Heart(Vector2 position)
        {
            drawRectangle = new Rectangle((int)position.X, (int)position.Y, 64, 64);
            sourceRectangle = new Rectangle(0, 0, 64, 64);
            origin = new Vector2(32, 32);

            drawRectangle.X += (int)origin.X;
            drawRectangle.Y += (int)origin.Y;


            aliveTexture = ContentHelper.LoadTexture("Menu/live_heart");
            deadTexture = ContentHelper.LoadTexture("Menu/dead_heart");
            currentTexture = aliveTexture;
            heartFrameCount = new Vector2(3, 0);

            textPos = new Vector2(drawRectangle.X + 64 + 10, drawRectangle.Y - 16);


            heartBeat1 = ContentHelper.LoadSound("Sounds/Menu/heartbeat1");
            heartBeat2 = ContentHelper.LoadSound("Sounds/Menu/heartbeat2");

            h1i = heartBeat1.CreateInstance();
            h2i = heartBeat2.CreateInstance();
        }

        public void Update(GameTime gameTime, Player player, GameWorld map, List<SplashDamage> splashDamages)
        {
            this.gameTime = gameTime;
            this.player = player;
            this.gameWorld = map;

            AnimateHeart();
            RotateHeart();
            PumpHeart();

            if (currentHealth < player.health)
            {
                currentHealth++;
            }
            if (currentHealth > player.health)
            {
                currentHealth--;
            }

            if (maxHealth < player.maxHealth)
            {
                maxHealth++;
            }
            if (maxHealth > player.maxHealth)
            {
                maxHealth--;
            }

            if (oldCurrentHealth < player.health)
            {
                splashDamages.Add(new SplashDamage(player.health - oldCurrentHealth));
                oldCurrentHealth = player.health;
            }

            if (oldCurrentHealth > player.health)
            {
                splashDamages.Add(new SplashDamage(player.health - oldCurrentHealth));
                oldCurrentHealth = player.health;
            }
        }

        private void PumpHeart()
        {
            if (player.health <= (maxHealth * .2))
            {
                isHeartPumping = true;
            }
            else
            {
                isHeartPumping = false;
                healthColor = Color.White;
            }

            if (player.health <= 0)
            {
                isHeartPumping = false;
                healthColor = Color.Red;
                isHeartDead = true;
            }
            else isHeartDead = false;

            if (isHeartDead)
            {
                currentTexture = deadTexture;
                heartRotation = 0;
            }
            else
            {
                currentTexture = aliveTexture;
                RotateHeart();
            }

            if (isHeartPumping)
            {
                pumpTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                if (pumpTimer > 500 && !heartHasPumped)
                {
                    drawRectangle.Width *= 2;
                    drawRectangle.Height *= 2;
                    pumpTimer = 0;
                    heartHasPumped = true;
                    h1i.Play();
                    healthColor = Color.Red;
                }

                if (pumpTimer > 100 && heartHasPumped)
                {
                    drawRectangle.Width /= 2;
                    drawRectangle.Height /= 2;
                    pumpTimer = 0;
                    heartHasPumped = false;
                    h2i.Play();
                    healthColor = Color.White;
                }
            }
        }

        private void RotateHeart()
        {
            double limit = Math.PI / 8;

            if (isRotatingRight)
            {
                if (heartRotation > limit)
                {
                    isRotatingRight = false;
                }
                else
                    heartRotation += .01f;
            }

            if (!isRotatingRight)
            {
                if (heartRotation < -limit)
                {
                    isRotatingRight = true;
                }
                else
                    heartRotation -= .01f;
            }


        }

        private void AnimateHeart()
        {
            switchFrameHeart = 75;
            heartTimer += gameTime.ElapsedGameTime.Milliseconds;
            int maxWait = 1000;

            if (heartTimer > switchFrameHeart)
            {
                sourceRectangle.X += sourceRectangle.Width;
                currentFrameHeart++;
                heartTimer = 0;
            }

            if (currentFrameHeart >= heartFrameCount.X)
            {
                restartTimer += gameTime.ElapsedGameTime.Milliseconds;
                heartTimer = 0;
                if (restartTimer > maxWait)
                {
                    restartTimer = 0;
                    sourceRectangle.X = 0;
                    currentFrameHeart = 0;
                }
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            FontHelper.DrawWithOutline(spriteBatch, Overlay.Font, currentHealth + "/" + maxHealth, textPos, 5, healthColor, Color.Black);
            spriteBatch.Draw(currentTexture, drawRectangle, sourceRectangle, Color.White, heartRotation, origin, SpriteEffects.None, 0);
        }
    }
}
