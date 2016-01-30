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
        private const int Width = 89;
        private const int Height = 16;

        public TextButton(Vector2 position, string text)
        {
            Text = text;
            CollRectangle = new Rectangle((int)position.X, (int)position.Y, (int)(Width*2 / Main.WidthRatio),
                (int)(Height*2 / Main.HeightRatio));
            SourceRectangle = new Rectangle(112, 32, Width, Height);
            Font = FontHelper.ChooseBestFont(CollRectangle.Height);
        }

    }
}
