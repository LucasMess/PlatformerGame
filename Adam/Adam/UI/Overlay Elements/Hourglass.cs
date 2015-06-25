using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI.Overlay_Elements
{
    class Hourglass
    {
        Texture2D texture;
        Rectangle drawRectangle, sourceRectangle;
        Vector2 origin;
        Animation animation;
        Vector2 position;

        int currentTime;

        public Hourglass(Vector2 position)
        {
            texture = ContentHelper.LoadTexture("Menu/timer");
            drawRectangle = new Rectangle(Game1.UserResWidth * 9 / 12, Game1.UserResHeight * 1 / 12, 64, texture.Height);
            sourceRectangle = new Rectangle(0, 0, 64, 64);
            origin = new Vector2(32, 32);
            drawRectangle.X += (int)origin.X;
            drawRectangle.Y -= (int)origin.Y;
            animation = new Animation(texture, new Rectangle(drawRectangle.X, drawRectangle.Y, 64, 64), 200, 0, AnimationType.Loop);

            this.position = position;
        }

        public void Update(GameTime gameTime, Player player, GameWorld map)
        {
            animation.Update(gameTime);

            currentTime = map.timer.GetTime(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch);
            FontHelper.DrawWithOutline(spriteBatch, Overlay.Font, currentTime.ToString(), position, 5, Color.White, Color.Black);
        }
    }
}
