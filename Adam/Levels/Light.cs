using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.Levels
{
    class Light
    {
        public const byte MaxLightLevel = 15;
        public byte LightLevel { get; set; } = 0;
        private static Rectangle _sourceRectangle = new Rectangle(320, 240, 64, 64);
        private Vector2 _center;
        private float _radius = 48;
        private Color _color = Color.White;
        private Texture2D _texture = GameWorld.SpriteSheet;
        private float _layerDepth;
        private bool _hasGlow;
        public bool IsLightSource { get; set; } = true;

        public Light(Vector2 center, byte lightLevel)
        {
            Update(center);
            LightLevel = lightLevel;
            if (lightLevel != 0)
            {
                IsLightSource = true;
            }
        }


        public Light(Vector2 center, float radius, Color color, float layerDepth = 0, bool hasGlow = true)
        {
            _radius = radius;
            _color = color;
            _layerDepth = layerDepth;
            Update(center);
            _hasGlow = hasGlow;
        }

        public void Update(Vector2 newCenter)
        {
            _center = newCenter;
            DrawRectangle = new Rectangle((int)(_center.X - _radius), (int)(_center.Y - _radius), (int)(_radius * 2), (int)(_radius * 2));
            GlowRectangle = new Rectangle((int)(_center.X - _radius / 4), (int)(_center.Y - _radius / 4), (int)(_radius / 2), (int)(_radius / 2));
        }

        public Rectangle DrawRectangle { get; private set; }
        public Rectangle GlowRectangle { get; private set; }

        public void DrawLight(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, DrawRectangle, _sourceRectangle, _color * GetOpacity(), 0, new Vector2(0, 0), SpriteEffects.None, _layerDepth);
        }

        public void DrawGlow(SpriteBatch spriteBatch)
        {
            if (_hasGlow)
                spriteBatch.Draw(_texture, GlowRectangle, _sourceRectangle, _color * .5f);
        }

        public float GetOpacity()
        {
            return (float)LightLevel / MaxLightLevel;
        }
    }
}
