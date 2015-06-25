using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI
{
    public class NewDialog
    {
        Texture2D texture;
        SpriteFont font;
        Rectangle drawRectangle;
        Vector2 origin;

        bool isActive;
        string text = "";
        StringBuilder sb;

        public delegate void EventHandler();
        public event EventHandler DialogOut;

        float opacity = 0;
        double skipTimer;

        int originalY;

        public NewDialog()
        {
            texture = ContentHelper.LoadTexture("Menu/dialog_box");
            drawRectangle = new Rectangle(Game1.UserResWidth / 2, 40, texture.Width * 2, texture.Height * 2);
            origin = new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2);
            drawRectangle.X -= (int)origin.X;

            originalY = drawRectangle.Y;
            drawRectangle.Y -= 40;

            font = ContentHelper.LoadFont("Fonts/dialog");
        }

        public void Show(string text)
        {
            isActive = true;
            this.text = FontHelper.WrapText(font, text, drawRectangle.Width - 60);
            skipTimer = 0;
            opacity = 0;
            drawRectangle.Y -= 40;
        }

        public void Update(GameTime gameTime)
        {
            float deltaOpacity = .03f;

            skipTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (skipTimer > .5)
            {
                if (InputHelper.IsAnyInputPressed())
                {
                    isActive = false;
                    //DialogOut();
                }
            }

            if (isActive)
            {
                float velocity =(originalY - drawRectangle.Y) / 10;
                drawRectangle.Y += (int)velocity;
                opacity += deltaOpacity;
            }
            else
            {
                float velocity = -3f;
                opacity -= deltaOpacity;
                drawRectangle.Y += (int)velocity;
            }

            if (opacity > 1)
                opacity = 1;
            if (opacity < 0)
                opacity = 0;
            if (drawRectangle.X < -100)
                drawRectangle.X = -100;
        }

        public void Draw(SpriteBatch spriteBatch)
        {            
            spriteBatch.Draw(texture, drawRectangle, Color.White * opacity);
            spriteBatch.DrawString(font, text, new Vector2(drawRectangle.X + 30, drawRectangle.Y + 30), Color.Black * opacity);
        }

    }
}
