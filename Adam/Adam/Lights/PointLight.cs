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
    class DynamicPointLight : Light
    {
        public DynamicPointLight(Entity source, Color color)
        {
            new DynamicPointLight(source, null, false, color);
        }

        public DynamicPointLight(Entity source, float? scale, bool isShaky, Color? color)
        {
            if (scale.HasValue)
                size = (int)(DefaultSize * scale);

            if (color.HasValue)
                this.color = color.Value;

            this.isShaky = isShaky;

            SetPosition(source.collRectangle);
     
        }

        public override void Update(Entity source)
        {
            SetPosition(source.collRectangle);

            if (isShaky) Shake();
        }
    }

    class FixedPointLight : Light
    {
        public FixedPointLight(Rectangle tileRectangle)
        {
            new FixedPointLight(tileRectangle, false, Color.White, null);
        }


        public FixedPointLight(Rectangle tileRectangle, bool isShaky, Color color, float? scale)
        {
            this.isShaky = isShaky;
            this.color = color;

            if (scale.HasValue)
                size = (int)(DefaultSize * scale);

            SetPosition(tileRectangle);

            lightHere = true;
        }

        public override void Update()
        {
            if (isShaky) Shake();
        }
    }
}
