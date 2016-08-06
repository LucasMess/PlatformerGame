using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI.Elements
{
    /// <summary>
    /// Button that has text inside of it describing its funciton.
    /// </summary>
    public class TextButton : Button
    {
        public const int Width = 89;
        public const int Height = 16;

        /// <summary>
        /// The button is created using game coordinates.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="text"></param>
        public TextButton(Vector2 position, string text)
        {
            Text = text;
            int width = CalcHelper.ApplyUiRatio(Width);
            CollRectangle = new Rectangle(CalcHelper.ApplyScreenScale((int)position.X) - width/2, CalcHelper.ApplyScreenScale((int)position.Y), width,
                CalcHelper.ApplyUiRatio(Height));
            SourceRectangle = new Rectangle(112, 32, Width, Height);
            Font = FontHelper.ChooseBestFont(CollRectangle.Height);
        }

    }
}
