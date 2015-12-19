using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam
{
    public class Glow
    {
        public Color Color { get; set; }
        Texture2D _texture;
        Rectangle _drawRectangle;
        Rectangle _sourceRectangle;
        Light _source;

        public Glow(Light light)
        {
            _texture = GameWorld.SpriteSheet;
            _drawRectangle = light.DrawRectangle;
            int width = light.DrawRectangle.Width - 64;
            int height = light.DrawRectangle.Height - 64;
            _drawRectangle.X += (light.DrawRectangle.Width - width) / 2;
            _drawRectangle.Y += (light.DrawRectangle.Height - height) / 2;
            _drawRectangle.Width = width;
            _drawRectangle.Height = height;
            _sourceRectangle = new Rectangle(20 * 16, 15 * 16, 64, 64);
            Color = light.Color;
            _source = light;
        }

        public void Update(Light light)
        {
            _source = light;
            _drawRectangle = light.DrawRectangle;
            int width = light.DrawRectangle.Width - 64;
            int height = light.DrawRectangle.Height - 64;
            _drawRectangle.X += (light.DrawRectangle.Width - width) / 2;
            _drawRectangle.Y += (light.DrawRectangle.Height - height) / 2;
            _drawRectangle.Width = width;
            _drawRectangle.Height = height;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _drawRectangle, _sourceRectangle, Color * _source.GlowIntensity);
        }
    }
}
