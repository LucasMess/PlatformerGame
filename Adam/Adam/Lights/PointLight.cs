using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Lights
{
    /// <summary>
    /// Used to create a simple light that shines equally in all directions.
    /// </summary>
    class PointLight : Light
    {
        int size = DefaultSize;

        public PointLight(Entity source)
        {
            new PointLight(source, null, false, null);
        }

        public PointLight(Entity source, float? scale, bool isShaky, Color? color)
        {
            if (scale.HasValue)
                size = (int)(DefaultSize * scale);

            if (color.HasValue)
                this.color = color.Value;

            drawRectangle = new Rectangle(source.collRectangle.Center.X, source.collRectangle.Center.Y, size, size);
            origin = new Vector2(size / 2, size / 2);

            drawRectangle.X = drawRectangle.X - (int)origin.X;
            drawRectangle.Y = drawRectangle.Y - (int)origin.Y;
        }
    }
}
