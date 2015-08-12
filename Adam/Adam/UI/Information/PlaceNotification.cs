using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI
{
    public class PlaceNotification
    {

        SpriteFont font;
        Texture2D texture;
        Rectangle drawRectangle;
        string text = "";
        bool isActive;
        double timer;
        float opacity = 0;
        Vector2 textPos, original;

        public PlaceNotification()
        {
            texture = ContentHelper.LoadTexture("Tiles/black");
            font = ContentHelper.LoadFont("Fonts/placeNotification");
            drawRectangle = new Rectangle(0, Game1.UserResHeight - 180, Game1.UserResWidth, 105);
        }

        public void Show(string text)
        {
            this.text = text;
            isActive = true;
            timer = 0;
            textPos = new Vector2(Game1.UserResWidth - font.MeasureString(text).X - 30, drawRectangle.Y);
            original = textPos;
            textPos.X += font.MeasureString(text).X / 2;
        }

        public void Update(GameTime gameTime)
        {
            float deltaOpacity = .03f;

            if (isActive)
            {
                opacity += deltaOpacity;
                timer += gameTime.ElapsedGameTime.TotalSeconds;
                if (textPos.X >= original.X)
                {
                    textPos.X += (original.X - textPos.X) / 10;
                }
                textPos.X -= 1f;

                if (timer > 3)
                {
                    isActive = false;
                }
            }
            else
            {
                opacity -= deltaOpacity;
                textPos.X -= 6f;
            }

            if (opacity > .7f)
                opacity = .7f;
            if (opacity < 0)
                opacity = 0;

            if (textPos.X < 0) textPos.X = 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, drawRectangle, Color.White * opacity);
            spriteBatch.DrawString(font, text, textPos,
                Color.White * opacity);

        }
    }
}
