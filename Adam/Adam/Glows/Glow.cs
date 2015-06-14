using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    class Glow : Entity
    {
        public Color Color { get; set; }


        public Glow(Light light)
        {
            //All glows come from light sources.
            texture = Game1.Content.Load<Texture2D>("Lighting/glow_200x200");
            drawRectangle = light.rectangle;
            origin = new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2);
            sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
        }

        public void Update(Rectangle rectangle)
        {
            this.drawRectangle = rectangle;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color * opacity, 0, origin, SpriteEffects.None, 0);
        }
    }
}
