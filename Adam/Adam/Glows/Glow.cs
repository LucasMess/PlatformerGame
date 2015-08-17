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
    public class Glow
    {
        public Color Color { get; set; }
        Texture2D texture;
        Rectangle drawRectangle;
        Rectangle sourceRectangle;

        public Glow(Light light)
        {
            texture = GameWorld.SpriteSheet;
            drawRectangle = light.drawRectangle;
            sourceRectangle = new Rectangle(20* 16, 15 * 16, 64, 64);
            Color = light.color;
        }

        public void Update(Light light)
        {
            this.drawRectangle = light.drawRectangle;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color * .5f);
        }
    }
}
