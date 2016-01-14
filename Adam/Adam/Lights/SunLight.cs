using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam.Lights
{
    /// <summary>
    /// Not inherited from Light class because it needs to be as simple as possible to prevent lag.
    /// </summary>
    public class SunLight
    {
        Vector2 _position;
        static Texture2D _texture = GameWorld.SpriteSheet;
        static Rectangle _sourceRectangle = new Rectangle(256, 240, Width, Height);
        private Rectangle _drawRectangle;

        const int Width = 64;
        const int Height = 64;

        private const int ScaledWidth = Width*4;
        private const int ScaledHeight = Height*4;

        public SunLight(Rectangle tileDrawRectangle)
        {


            _drawRectangle = new Rectangle(tileDrawRectangle.Center.X - (ScaledWidth/2), tileDrawRectangle.Center.Y - (ScaledHeight/2), ScaledWidth, ScaledHeight);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _drawRectangle, _sourceRectangle, Color.White);
        }
    }
}
