using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.Levels
{
    class Light
    {
        private static Rectangle _sourceRectangle = new Rectangle(320,240,64,64);
        private Vector2 _center;
        private float _radius;
        private Color _color;
        private Texture2D _texture = GameWorld.SpriteSheet;

        public Light(Vector2 center, float radius, Color color)
        {
            _center = center;
            _radius = radius;
            _color = color;
        }

        public void Update(Vector2 newCenter)
        {
            _center = newCenter;
        }

        public void Update()
        {

        }

        private Rectangle GetDrawRectangle()
        {
            return  new Rectangle((int)(_center.X - _radius), (int)(_center.Y - _radius), (int)(_radius * 2), (int)(_radius * 2));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, GetDrawRectangle(), _sourceRectangle, _color);
        }
    }
}
