﻿using ThereMustBeAnotherWay.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ThereMustBeAnotherWay.UI.Elements
{
    /// <summary>
    /// Button that has text inside of it describing its funciton.
    /// </summary>
    public class TextButton : Button
    {
        public const int Width = 100;
        public const int Height = 24;

        /// <summary>
        /// The button is created using game coordinates.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <param name="convertCoordinates"></param>
        public TextButton(Vector2 position, string text, bool convertCoordinates = true)
        {
            Text = text;
            int width = Width;

            if (convertCoordinates)
            {
                position.X = (int)position.X * 2;
                position.Y = (int)position.Y * 2;
            }

            CollRectangle = new Rectangle((int)position.X, (int)position.Y, width,
                Height);
            SourceRectangle = new Rectangle(112, 32, Width, Height);
            Font = FontHelper.ChooseBestFont(CollRectangle.Height);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            //spriteBatch.DrawString(Font, Text, new Vector2(CollRectangle.Center.X, CollRectangle.Center.Y),
            // TextColor, 0, Font.MeasureString(Text) / 2, (float)(.5 / Main.HeightRatio), SpriteEffects.None, 0);

            float x = CollRectangle.Center.X - Font.MeasureString(Text).X / 2;
            float y = CollRectangle.Center.Y - Font.MeasureString(Text).Y / 2;

            FontHelper.DrawWithOutline(spriteBatch, Font, Text,
                new Vector2(x, y), 1, TextColor, Color.Black);

        }
    }
}
