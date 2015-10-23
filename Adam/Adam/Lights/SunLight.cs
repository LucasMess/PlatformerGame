using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Lights
{
    /// <summary>
    /// Not inherited from Light class because it needs to be as simple as possible to prevent lag.
    /// </summary>
    public class SunLight
    {
        Vector2 position;
        static Texture2D texture = GameWorld.SpriteSheet;
        static Rectangle sourceRectangle =new Rectangle(380, 0, Width, Height);

        const int Width = 256;
        const int Height = 256;

        public SunLight(Rectangle tileDrawRectangle)
        {
            position = new Vector2(tileDrawRectangle.Center.X, tileDrawRectangle.Center.Y);
            position.X -= Width/2;
            position.Y -= Height /2;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, sourceRectangle, Color.White);
        }
    }
}
