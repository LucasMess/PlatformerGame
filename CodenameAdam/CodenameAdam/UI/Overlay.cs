using Adam;
using CodenameAdam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodenameAdam
{
    class Overlay
    {
        Texture2D heart;
        Rectangle heartRect, heartSource;
        Vector2 heartOrigin;
        Vector2 heartFrameCount;
        int currentFrameHeart, switchFrameHeart;
        double heartTimer, restartTimer;
        float heartRotation;
        bool isRotatingRight = true;

        Texture2D coin;
        Rectangle coinRect, coinSource;
        Vector2 coinOrigin;
        Vector2 coinFrameCount;
        int currentFrameCoin, switchFrameCoin;
        double coinTimer;

        Texture2D time;
        Rectangle timeRect, timeSource;
        Vector2 timeOrigin;
        Animation timeAnimation;

        Animation chrono;
        Animation armor;

        SpriteFont font;
        GameTime gameTime;

        int screenWidth;
        int screenHeight;
        int score, currentHealth, maxHealth, currentTime;
        int oldCurrentHealth;

        List<SplashDamage> splashDamage = new List<SplashDamage>();

        Vector2 healthPos, scorePos, timePos;
        ContentManager Content;

        public Overlay()
        {
            screenWidth = Game1.PREFFERED_RESWIDTH;
            screenHeight = Game1.PREFFERED_RESHEIGHT;
        }

        public void Load()
        {
            this.Content = Game1.Content;

            heart = Content.Load<Texture2D>("Menu/player_heart");
            heartFrameCount = new Vector2(3, 0);

            coin = Content.Load<Texture2D>("Menu/player_coin");
            coinFrameCount = new Vector2(7, 0);

            chrono = new Animation(Content.Load<Texture2D>("Menu/chronoshift"), new Rectangle(screenWidth - 100, screenHeight* 2/12, 64, 64), 50, 0, AnimationType.Loop);

            heartRect = new Rectangle(screenWidth * 1 / 12, screenHeight * 1 / 12, 64, heart.Height);
            heartSource = new Rectangle(0, 0, 64, 64);
            heartOrigin = new Vector2(32, 32);

            armor = new Animation(Content.Load<Texture2D>("Menu/menu_armor"), new Rectangle(heartRect.X, heartRect.Y + 50, 32*10, 32), 0, 0, AnimationType.Loop);

            coinRect = new Rectangle(screenWidth * 11 / 24, screenHeight * 1 / 12, 64, coin.Height);
            coinSource = new Rectangle(0, 0, 64, 64);
            coinOrigin = new Vector2(32, 32);

            time = ContentHelper.LoadTexture("Menu/timer");
            timeRect = new Rectangle(screenWidth * 9 / 12, screenHeight * 1 / 12, 64, time.Height);
            timeSource = new Rectangle(0, 0, 64, 64);
            timeOrigin = new Vector2(32, 32);
            timeRect.X += (int)timeOrigin.X;
            timeRect.Y -= (int)timeOrigin.Y;
            timeAnimation = new Animation(time, new Rectangle(timeRect.X, timeRect.Y, 64,64), 200, 0, AnimationType.Loop);

            font = Content.Load<SpriteFont>("Fonts/overlay");

            healthPos = new Vector2(heartRect.X + 64 + 10, heartRect.Y - heart.Height / 2);
            scorePos = new Vector2(coinRect.X + 64 + 10, coinRect.Y - coin.Height / 2);
            timePos = new Vector2(timeRect.X + 64 + 50, scorePos.Y);

        }

        public void Update(GameTime gameTime, Player player, Map map)
        {
            this.gameTime = gameTime;

            AnimateCoin();
            AnimateHeart();
            RotateHeart();

            chrono.Update(gameTime);
            timeAnimation.Update(gameTime);

            currentTime = map.timer.GetTime(gameTime);
            if (currentTime < 100)
                map.WarnRunningOutOfTime();

            armor.sourceRectangle.X = armor.texture.Width / 2 - (player.armorPoints / 100 * armor.texture.Width / 2);

            if (player.isChronoshifting)
                chrono.switchFrame = 100;
            else chrono.switchFrame = 600;

            score = player.score;
            currentHealth = player.health;
            maxHealth = player.maxHealth;

            if (oldCurrentHealth < currentHealth)
            {
                splashDamage.Add(new SplashDamage(currentHealth - oldCurrentHealth, screenWidth, screenHeight, Content));
                oldCurrentHealth = currentHealth;
            }

            if (oldCurrentHealth > currentHealth)
            {
                splashDamage.Add(new SplashDamage(currentHealth - oldCurrentHealth, screenWidth, screenHeight, Content));
                oldCurrentHealth = currentHealth;
            }

            foreach (var spl in splashDamage)
            {
                spl.Update(gameTime);
            }
            foreach (var spl in splashDamage)
            {
                if (spl.toDelete)
                {
                    splashDamage.Remove(spl);
                    break;
                }
            }

        }

        protected void AnimateHeart()
        {
            switchFrameHeart = 75;
            heartTimer += gameTime.ElapsedGameTime.Milliseconds;
            int maxWait = 1000;

            if (heartTimer > switchFrameHeart)
            {
                heartSource.X += heartSource.Width;
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
                    heartSource.X = 0;
                    currentFrameHeart = 0;
                }
            }

        }

        protected void AnimateCoin()
        {
            switchFrameCoin = 100;
            coinTimer += gameTime.ElapsedGameTime.Milliseconds;

            if (coinTimer > switchFrameCoin)
            {
                coinSource.X += coinSource.Width;
                currentFrameCoin++;
                coinTimer = 0;
            }

            if (currentFrameCoin > coinFrameCount.X)
            {
                coinSource.X = 0;
                currentFrameCoin = 0;
            }
        }

        protected void RotateHeart()
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

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(heart, heartRect, heartSource, Color.White, heartRotation, heartOrigin, SpriteEffects.None, 0);
            spriteBatch.Draw(coin, coinRect, coinSource, Color.White, 0, coinOrigin, SpriteEffects.None, 0);
            //chrono.Draw(spriteBatch);
            //armor.Draw(spriteBatch);
            timeAnimation.Draw(spriteBatch);

            //spriteBatch.DrawString(font, currentHealth + "/" + maxHealth, healthPos, Color.White);

            FontHelper.DrawWithOutline(spriteBatch, font, currentHealth + "/" + maxHealth, healthPos, 5, Color.White, Color.Black);
            FontHelper.DrawWithOutline(spriteBatch, font, score.ToString(), scorePos, 5, Color.White, Color.Black);
            FontHelper.DrawWithOutline(spriteBatch, font, currentTime.ToString(), timePos, 5, Color.White, Color.Black);

            //spriteBatch.DrawString(font, score.ToString(), new Vector2(scorePos.X+5, scorePos.Y), Color.Black);
            //spriteBatch.DrawString(font, score.ToString(), new Vector2(scorePos.X-5, scorePos.Y), Color.Black);
            //spriteBatch.DrawString(font, score.ToString(), new Vector2(scorePos.X, scorePos.Y+5), Color.Black);
            //spriteBatch.DrawString(font, score.ToString(), new Vector2(scorePos.X, scorePos.Y-5), Color.Black);

            //spriteBatch.DrawString(font, score.ToString(), scorePos, Color.White);

            foreach (var spl in splashDamage)
            {
                spl.Draw(spriteBatch);
            }
        }
    }

    public class SplashDamage
    {
        SpriteFont font;
        Vector2 position;
        string text;
        bool isNegative;
        public bool toDelete;
        bool hasExpanded;
        float opacity = 2;
        int damage;
        float scale, normScale;
        Vector2 velocity, origin;

        public SplashDamage(int damage, int screenWidth, int screenHeight, ContentManager Content)
        {
            this.damage = damage;
            text = damage.ToString();
            position = new Vector2(screenWidth / 2, screenHeight / 2);

            if (damage < 0)
                isNegative = true;

            int absDamage = Math.Abs(damage);
            scale = .1f;
            if (absDamage > 10)
                scale = .15f;
            if (absDamage > 20)
                scale = .2f;
            if (absDamage > 40)
                scale = .3f;
            if (absDamage > 80)
                scale = .5f;

            font = Content.Load<SpriteFont>("Fonts/splash_damage");
            velocity = new Vector2(0, -5);

            origin = font.MeasureString(text) / 2;
            normScale = scale;
        }


        public void Update(GameTime gameTime)
        {
            opacity -= .05f;
            position += velocity;

            velocity.Y = velocity.Y * 0.9f;

            if (scale > normScale * 2)
            {
                hasExpanded = true;
            }

            if (!hasExpanded)
            {
                scale += .05f;
            }
            else
            {
                scale -= .01f;
            }

            if (opacity < 0)
                toDelete = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isNegative)
            {
                spriteBatch.DrawString(font, text, position, Color.Red * opacity, 0, origin, scale, SpriteEffects.None, 0);
            }
            else spriteBatch.DrawString(font, "+" + text, position, Color.Green * opacity, 0, origin, scale, SpriteEffects.None, 0);
        }
    }

    public static class FontHelper
    {
        public static void DrawWithOutline(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, int outlineWidth, Color fontColor, Color outlineColor)
        {
            spriteBatch.DrawString(font, text, new Vector2(position.X + outlineWidth, position.Y), outlineColor);
            spriteBatch.DrawString(font, text, new Vector2(position.X - outlineWidth, position.Y), outlineColor);
            spriteBatch.DrawString(font, text, new Vector2(position.X, position.Y + outlineWidth), outlineColor);
            spriteBatch.DrawString(font, text, new Vector2(position.X, position.Y - outlineWidth), outlineColor);

            spriteBatch.DrawString(font, text, new Vector2(position.X,position.Y+10), fontColor);
        }
    }

}
