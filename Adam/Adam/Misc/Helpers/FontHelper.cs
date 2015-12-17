using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Misc.Helpers
{
    public static class FontHelper
    {
        /// <summary>
        /// This draws a string with a black outline around the letters.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch being used to draw the sprites.</param>
        /// <param name="font">The font the string will be drawn with.</param>
        /// <param name="text">The string.</param>
        /// <param name="position">The position of the top left corner.</param>
        /// <param name="outlineWidth">The width of the outline.</param>
        /// <param name="fontColor">The color of the font.</param>
        /// <param name="outlineColor">The color of the outline.</param>
        public static void DrawWithOutline(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, int outlineWidth, Color fontColor, Color outlineColor)
        {
            if (text == null)
                text = "";

            int i = outlineWidth;

            spriteBatch.DrawString(font, text, new Vector2(position.X + i, position.Y), outlineColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
            //spriteBatch.DrawString(font, text, new Vector2(position.X - outlineWidth, position.Y), outlineColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, text, new Vector2(position.X, position.Y + i), outlineColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
            //spriteBatch.DrawString(font, text, new Vector2(position.X, position.Y - outlineWidth), outlineColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, text, new Vector2(position.X + i, position.Y + i), outlineColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);

            spriteBatch.DrawString(font, text, new Vector2(position.X, position.Y), fontColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
        }

        public static string WrapText(SpriteFont spriteFont, string text, float maxLineWidth)
        {
            if (text == null) return "";
            string[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = spriteFont.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
        }
    }
}
