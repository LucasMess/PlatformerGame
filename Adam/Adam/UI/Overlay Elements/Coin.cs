using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI.Overlay_Elements
{
    public class Coin
    {
        Texture2D texture;
        Rectangle drawRectangle, sourceRectangle;
        Vector2 origin;
        Vector2 frameCount;
        Vector2 textPos;
        int currentFrame, switchFrame;
        double timer;

        Animation animation;
        GameTime gameTime;
        int score;

        public Coin(Vector2 position)
        {
            texture = ContentHelper.LoadTexture("Menu/player_coin");
            frameCount = new Vector2(7, 0);
            drawRectangle = new Rectangle((int)position.X, (int)position.Y, 64, texture.Height);
            sourceRectangle = new Rectangle(0, 0, 64, 64);
            origin = new Vector2(32, 32);

            drawRectangle.X += (int)origin.X;
            drawRectangle.Y += (int)origin.Y;
            textPos = new Vector2(drawRectangle.X + 64 + 10, drawRectangle.Y - 16);

            animation = new Animation(texture, drawRectangle, 100, 0, AnimationType.Loop);
        }

        public void Update(Player player, GameTime gameTime)
        {
            this.gameTime = gameTime;

            Animate();

            if (score < player.Score)
            {
                score++;
            }
            if (score > player.Score)
            {
                score--;
            }
        }

        private void Animate()
        {
            switchFrame = 100;
            timer += gameTime.ElapsedGameTime.Milliseconds;

            if (timer > switchFrame)
            {
                sourceRectangle.X += sourceRectangle.Width;
                currentFrame++;
                timer = 0;
            }

            if (currentFrame > frameCount.X)
            {
                sourceRectangle.X = 0;
                currentFrame = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.White, 0, origin, SpriteEffects.None, 0);

            FontHelper.DrawWithOutline(spriteBatch, Overlay.Font, score.ToString(), textPos, 5, Color.White, Color.Black);
        }
    }
}
