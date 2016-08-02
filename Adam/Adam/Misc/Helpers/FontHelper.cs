using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Adam.UI.Elements;
using Color = Microsoft.Xna.Framework.Color;

namespace Adam.Misc.Helpers
{
    public static class FontHelper
    {
        public static SpriteFont[] Fonts = {
            ContentHelper.LoadFont("Fonts/x16"),
            ContentHelper.LoadFont("Fonts/x32"),
            ContentHelper.LoadFont("Fonts/x64")
        };

        private static InGameWindow _window;

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
        public static void DrawWithOutline(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position,
            int outlineWidth, Color fontColor, Color outlineColor)
        {
            DrawWithOutline(spriteBatch, font, text, position, outlineWidth, fontColor, outlineColor, 1);
        }


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
        /// <param name="scale">"The size of the text"</param>
        public static void DrawWithOutline(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, int outlineWidth, Color fontColor, Color outlineColor, float scale)
        {
            if (text == null)
                text = "";

            spriteBatch.DrawString(font, text, new Vector2(position.X + 3, position.Y + 3), outlineColor, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, text, new Vector2(position.X, position.Y), fontColor, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);

            //int i = outlineWidth;
            //float lostWidth = font.MeasureString(text).X * scale / 2;
            //if (scale == 1) lostWidth = 0;

            //spriteBatch.DrawString(font, text, new Vector2(position.X + i - lostWidth, position.Y), outlineColor, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
            ////spriteBatch.DrawString(font, text, new Vector2(position.X - outlineWidth, position.Y), outlineColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
            //spriteBatch.DrawString(font, text, new Vector2(position.X - lostWidth, position.Y + i), outlineColor, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
            ////spriteBatch.DrawString(font, text, new Vector2(position.X, position.Y - outlineWidth), outlineColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
            //spriteBatch.DrawString(font, text, new Vector2(position.X + i - lostWidth, position.Y + i), outlineColor, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);

            //spriteBatch.DrawString(font, text, new Vector2(position.X - lostWidth, position.Y), fontColor, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Wraps text in a container.
        /// </summary>
        /// <param name="spriteFont">The font used.</param>
        /// <param name="text">The text to be wrapped.</param>
        /// <param name="maxLineWidth">The width of the container.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Chooses the best font to fit inside a container.
        /// </summary>
        /// <param name="height">The desired line height for the container.</param>
        /// <returns></returns>
        public static SpriteFont ChooseBestFont(int height)
        {
            for (int i = 1; i < Fonts.Length; i++)
            {
                if (height < Fonts[i].LineSpacing)
                    return Fonts[i - 1];
            }

            // Return largest font if the container is big enough.
            return Fonts[Fonts.Length - 1];
        }

        /// <summary>
        /// Draws tooltip above mouse cursor.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="text"></param>
        public static void DrawTooltip(SpriteBatch spriteBatch, string text)
        {

            if (text == null) text = "";
            var font = ChooseBestFont(CalcHelper.ApplyUiRatio(16));
            var mouse = InputHelper.MouseRectangle;

            _window = new InGameWindow(mouse.X - CalcHelper.ApplyUiRatio(4), mouse.Y - (int) font.MeasureString(text).Y - CalcHelper.ApplyUiRatio(2),
                (int) font.MeasureString(text).X + CalcHelper.ApplyUiRatio(8),
                (int) font.MeasureString(text).Y + CalcHelper.ApplyUiRatio(4), false);
            _window.Color = new Color(196,69,69);
            _window.DisableAnimation();
            _window.Draw(spriteBatch);

            //spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/black"),
            //    new Rectangle(mouse.X - 5, mouse.Y - (int)font.MeasureString(text).Y - 2, (int)font.MeasureString(text).X + 10,
            //        (int)font.MeasureString(text).Y + 4), Color.Black);

            DrawWithOutline(spriteBatch, font, text, new Vector2(mouse.X, mouse.Y - (int)font.MeasureString(text).Y), 1,
                Color.White, Color.Black);
        }
    }
}
