using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Adam
{
    class Button
    {
        public enum SettingsState {ON, OFF}
        public SettingsState currentSettingsState = SettingsState.OFF;
        public bool wasPressed;

        Texture2D texture;
        Rectangle leaf1, leaf2;
        SpriteFont font;
        Rectangle sourceRectangle, rectangle;
        Vector2 textPos, textOrigin;
        Vector2 leafOrigin;
        Color textColor;
        string text;
        float scale = .3f;
        float r1, r2;
        bool t1, t2;

        public Button() { }

        public void Load(ContentManager Content)
        {
            texture = Content.Load<Texture2D>("Menu/Leaf");
            font = Content.Load<SpriteFont>("Fonts/button");
            sourceRectangle = new Rectangle(0, 0, 230, 130);
        }

        public bool IsPressed()
        {
            MouseState mouseState = Mouse.GetState();
            Rectangle mouseRect = new Rectangle((int)(mouseState.X), (int)(mouseState.Y), 1, 1);
            if (mouseState.LeftButton == ButtonState.Pressed && mouseRect.Intersects(rectangle))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public void Update()
        {
            MouseState mouseState = Mouse.GetState();
            Rectangle mouseRect = new Rectangle(mouseState.X * (Game1.DefaultResWidth/Game1.PrefferedResWidth), mouseState.Y * (Game1.DefaultResHeight/Game1.PrefferedResHeight), 1, 1);
            if (mouseRect.Intersects(rectangle))
            {
                sourceRectangle.X = 230;
                textColor = new Color(97, 34, 34);;

                if (t1 == true)
                    r1 += .1f;
                else r1 -= .1f;

                if (t2 == true)
                    r2 -= .1f;
                else r2 += .1f;

                if (r1 > Math.PI / 4)
                    t1 = true;

                if (r1 < 0)
                    t1 = false;

                if (r2 > Math.PI / 4)
                    t1 = false;

                if (r2 < 0)
                    t1 = true;

            }
            else
            {
                sourceRectangle.X = 0;
                textColor = new Color(196, 69, 69);;
            }
        }

        public void SetPosition(Vector2 pos)
        {
            rectangle = new Rectangle((int)pos.X, (int)pos.Y, 300, 100);
        }

        public void SetText(string text)
        {
            this.text = text;
            textOrigin = font.MeasureString(text) / 2;
            textPos = new Vector2(rectangle.Center.X, rectangle.Center.Y);

            float textX = textPos.X - textOrigin.X*scale;
            float textY = textPos.Y - textOrigin.Y*scale;

            leaf1 = new Rectangle((int)(textX - 32), (int)textY, 32, 32);
            leaf2 = new Rectangle((int)(textX + textOrigin.X * 2 * scale), (int)textY, 32, 32);

            leafOrigin = new Vector2(16,16);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Debug
            //spriteBatch.Draw(texture, rectangle, Color.White);


            spriteBatch.Draw(texture, leaf1, null, Color.White,0, new Vector2(0,0), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, leaf2, null, Color.White, 0, new Vector2(0,0), SpriteEffects.FlipHorizontally, 0);
            spriteBatch.DrawString(font, text, textPos, textColor, 0, textOrigin, scale, SpriteEffects.None, 0);
        }



    }
}
