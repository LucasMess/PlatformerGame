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
        SpriteFont textFont, headFont;
        Texture2D texture;
        Rectangle drawRectangle;
        float opacity = 0;
        public bool isActive;
        public bool isComplete;
        bool completeAnimationDone;
        int newY;
        int goalX;
        public int ID;
        double lifespan;
        Color color = Color.Black;

        string text = "";

        public Objective()
        {
            texture = ContentHelper.LoadTexture("Tiles/white");
            textFont = ContentHelper.LoadFont("Fonts/x32");
            headFont = ContentHelper.LoadFont("Fonts/x64");
            drawRectangle = new Rectangle(Main.UserResWidth, 0, 300, 125);
            goalX = Main.UserResWidth - drawRectangle.Width;
        }

        public void Create(string text, int ID)
        {
            this.text = FontHelper.WrapText(textFont, text, drawRectangle.Width - 20);
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
                    color = Color.White;
                    opacity += deltaOpacity;
                    drawRectangle.Y += (newY - drawRectangle.Y) / 10;
                    drawRectangle.X += (goalX - drawRectangle.X) / 10;
                }
                else
                {
                    drawRectangle.Width = 400;

                    if (lifespan > 2)
                    {
                        int completeX = Main.UserResWidth + 100;
                        drawRectangle.X += (completeX - drawRectangle.X) / 10;
                        if (drawRectangle.X >= Main.UserResWidth)
                            isActive = false;
                    }
                    else
                    {
                        int completeX = goalX - 100;
                        drawRectangle.X += (completeX - drawRectangle.X) / 10;
                        color = Color.Green;
                        lifespan += gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }

                if (opacity > .8f)
                    opacity = .8f;
                if (opacity < 0)
                    opacity = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(texture, drawRectangle, color * opacity);
            //spriteBatch.DrawString(font, "Objective:", new Vector2(drawRectangle.X + 5, drawRectangle.Y + 5), Color.Yellow * opacity);
            string headText = "Objective:";
            if (isComplete) headText = "Objective Complete!";
            FontHelper.DrawWithOutline(spriteBatch, headFont, headText, new Vector2(drawRectangle.X + 5, drawRectangle.Y + 5), 2, Color.Yellow, Color.Black);
            //spriteBatch.DrawString(textFont, text, new Vector2(drawRectangle.X + 5, drawRectangle.Y + 60), Color.Black * opacity, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
            FontHelper.DrawWithOutline(spriteBatch, textFont, text, new Vector2(drawRectangle.X + 5, drawRectangle.Y + 60), 2, new Color(240,240,240), Color.Black);
        }
    }
}
