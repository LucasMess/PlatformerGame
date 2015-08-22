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
        Light source;

        public Glow(Light light)
        {
            texture = GameWorld.SpriteSheet;
            drawRectangle = light.drawRectangle;
            int width = light.drawRectangle.Width - 64;
            int height = light.drawRectangle.Height - 64;
            drawRectangle.X += (light.drawRectangle.Width - width) / 2;
            drawRectangle.Y += (light.drawRectangle.Height - height) / 2;
            drawRectangle.Width = width;
            drawRectangle.Height = height;
            sourceRectangle = new Rectangle(20 * 16, 15 * 16, 64, 64);
            Color = light.color;
            source = light;
        }

        public void Update(Light light)
        {
            source = light;
            drawRectangle = light.drawRectangle;
            int width = light.drawRectangle.Width - 64;
            int height = light.drawRectangle.Height - 64;
            drawRectangle.X += (light.drawRectangle.Width - width) / 2;
            drawRectangle.Y += (light.drawRectangle.Height - height) / 2;
            drawRectangle.Width = width;
            drawRectangle.Height = height;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color * source.glowIntensity);
        }
    }
}
