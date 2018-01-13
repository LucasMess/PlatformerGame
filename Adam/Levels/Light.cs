using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThereMustBeAnotherWay.Levels
{
    public class Light
    {
        public Color Color { get; set; } = Color.White;
        public int Radius { get; set; } = 100;
        public float Intensity { get; set; } = 1f;

        private static Texture2D Texture = GameWorld.SpriteSheet;
        private static Rectangle SourceRectangle = new Rectangle(320, 240, 64, 64);
        private Rectangle _drawRectangle = new Rectangle(0, 0, 100, 100);
        private Rectangle _glowRectangle = new Rectangle(0, 0, 50, 50);

        public Light()
        {

        }

        public Light(Vector2 center, Color color, int radius)
        {
            Color = color;
            Radius = radius;
            _drawRectangle.Width = Radius * 2;
            _drawRectangle.Height = Radius * 2;
            _glowRectangle.Width = Radius;
            _glowRectangle.Height = Radius;
            Update(center);
        }

        public void Update(Vector2 center)
        {
            _drawRectangle.X = (int)center.X - Radius;
            _drawRectangle.Y = (int)center.Y - Radius;
            _glowRectangle.X = (int)center.X - Radius / 2;
            _glowRectangle.Y = (int)center.Y - Radius / 2;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, _drawRectangle, SourceRectangle, Color * Intensity);
        }

        public void DrawGlow(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, _glowRectangle, SourceRectangle, Color * Intensity * .5f);
        }
    }
}
