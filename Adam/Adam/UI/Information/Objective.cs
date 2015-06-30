using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI.Information
{
    public class Objective
    {
        SpriteFont font;
        Texture2D texture;
        Rectangle drawRectangle;
        float opacity = 0;
        public bool isActive;
        public bool isComplete;
        bool completeAnimationDone;
        int newY;
        int goalX;
        public int ID;
        Color color = Color.Black;

        string text = "";

        public Objective()
        {
            texture = ContentHelper.LoadTexture("Tiles/white");
            font = ContentHelper.LoadFont("Fonts/dialog");
            drawRectangle = new Rectangle(Game1.UserResWidth, 0, 300, 125);
            goalX = Game1.UserResWidth - drawRectangle.Width;
        }

        public void Create(string text, int ID)
        {
            this.text = FontHelper.WrapText(font, text, drawRectangle.Width - 10);
            this.ID = ID;
            isActive = true;
            drawRectangle.Y = (30);
            newY = drawRectangle.Y;
        }

        public void SetPosition(int index)
        {
            drawRectangle.Y = (30 + (130 * index));
            newY = drawRectangle.Y;
        }

        public void TransitionIntoNewPosition(int index)
        {
            newY = (30 + (155 * index));
        }

        public void Update(GameTime gameTime)
        {
            float deltaOpacity = .03f;
            if (isActive)
            {
                if (!isComplete)
                {
                    color = Color.Black;
                    opacity += deltaOpacity;
                    drawRectangle.Y += (newY - drawRectangle.Y) / 10;
                    drawRectangle.X += (goalX - drawRectangle.X) / 10;
                }
                else
                {
                    int completeX = goalX - 100;
                    drawRectangle.X += (completeX - drawRectangle.X) / 10;
                    color = Color.Green;
                    opacity -= deltaOpacity / 3;
                    if (opacity <= 0)
                        isActive = false;
                }

                if (opacity > .8f)
                    opacity = .8f;
                if (opacity < 0)
                    opacity = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, drawRectangle, color * opacity);
            spriteBatch.DrawString(font, "Objective:", new Vector2(drawRectangle.X + 5, drawRectangle.Y + 5), Color.Yellow * opacity);
            spriteBatch.DrawString(font, text, new Vector2(drawRectangle.X + 5, drawRectangle.Y + 30), Color.White * opacity,0,new Vector2(0,0),1,SpriteEffects.None,0);
        }
    }
}
