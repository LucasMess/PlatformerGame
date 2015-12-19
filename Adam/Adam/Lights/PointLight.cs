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
    /// Used to create a simple light that shines equally in all directions and follows an entity.
    /// </summary>
    public class DynamicPointLight : Light
    {

        public DynamicPointLight(Entity source, float? scale, bool isShaky, Color? color, float glowIntensity)
        {
            if (scale.HasValue)
                Size = (int)(DefaultSize * scale);

            if (color.HasValue)
                this.Color = color.Value;

            this.GlowIntensity = glowIntensity;
            this.IsShaky = isShaky;

            SetPosition(source.GetCollRectangle());
            Glow = new Glow(this);
            this.source = source;

            LightHere = true;

        }

        public override void Update(Entity source)
        {
            SetPosition(source.GetCollRectangle());

            if (IsShaky) Shake();

            base.Update();
        }
    }

    /// <summary>
    /// Used to create a simple light that shines equally in all directions and stays fixed.
    /// </summary>
    public class FixedPointLight : Light
    {
        public FixedPointLight(Rectangle tileRectangle, bool isShaky, Color color, float? scale, float glowIntensity)
        {
            this.IsShaky = isShaky;
            this.Color = color;

            if (scale.HasValue)
                Size = (int)(DefaultSize * scale);

            SetPosition(tileRectangle);
            this.GlowIntensity = glowIntensity;
            Glow = new Glow(this);

            LightHere = true;
        }

        public override void Update()
        {
            if (IsShaky) Shake();

            base.Update();
        }
    }
}
