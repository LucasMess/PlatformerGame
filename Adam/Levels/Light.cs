using Adam.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Adam.Levels
{
    class Light
    {
        public const int MaxLightLevel = 16;
        public int LightLevel { get; set; } = 0;
        private static Rectangle _sourceRectangle = new Rectangle(320, 240, 64, 64);
        private Vector2 _center;
        public Color _color = Color.Black;
        public List<Color> ColorOfSources = new List<Color>();
        private Texture2D _texture = GameWorld.SpriteSheet;
        public Light SourceLight { get; set; }
        private float _layerDepth;
        private bool _hasGlow;
        public bool IsLightSource { get; set; } = true;
        public bool IsShaky { get; set; }
        public bool ChangesSize { get; set; }
        public int RateOfChange { get; set; } = 100;
        private Timer _changeSizeTimer = new Timer();
        private int oldChangeSize;

        public Light() { }

        public Light(Vector2 center, int lightLevel, Color color)
        {
            _color = color;
            Update(center);
            LightLevel = lightLevel;
            if (lightLevel != 0)
            {
                IsLightSource = true;
            }
        }
        public void Update(Vector2 newCenter)
        {
            _center = newCenter;
            Vector2 offset = GetOffset();
            float radius = GetRadius();
            DrawRectangle = new Rectangle((int)(_center.X - radius + offset.X), (int)(_center.Y - radius + offset.Y), (int)(radius * 2), (int)(radius * 2));
            GlowRectangle = new Rectangle((int)(_center.X + offset.X - radius / 4), (int)(_center.Y + radius / 4 + offset.Y), (int)(radius / 2), (int)(radius / 2));
        }

        public Rectangle DrawRectangle { get; private set; }
        public Rectangle GlowRectangle { get; private set; }

        public void DrawLight(SpriteBatch spriteBatch)
        {
            Update(_center);

            //foreach (Color color in ColorOfSources)
            //{
            //    spriteBatch.Draw(_texture, DrawRectangle, _sourceRectangle, color * GetOpacity(), 0, new Vector2(0, 0), SpriteEffects.None, _layerDepth);
            //}
            spriteBatch.Draw(_texture, DrawRectangle, _sourceRectangle, GetSourceColor() * GetOpacity(), 0, new Vector2(0, 0), SpriteEffects.None, _layerDepth);
            _hasIncremented = false;
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

        public Color GetSourceColor()
        {
            //if (IsLightSource || ColorOfSources.Count == 0)
            //{
            //    return _color;
            //}


            //float r = 0, g = 0, b = 0;
            //foreach (var color in ColorOfSources)
            //{
            //    r += color.R;
            //    g += color.G;
            //    b += color.B;

            //}

            //r = MathHelper.Min(r, 255);
            //g= MathHelper.Min(g, 255);
            //b = MathHelper.Min(b, 255);

            //return new Color(r / 255, g / 255, b / 255);

            //return _color;

            if (IsLightSource || SourceLight == null)
            {
                return _color;
            }
            else return SourceLight.GetSourceColor();
        }

        private const int ShakeOffset = 10;
        private Vector2 GetOffset()
        {
            if (IsLightSource || SourceLight == null)
            {
                //if (IsShaky)
                //{
                //    int x = Main.Random.Next(-ShakeOffset, ShakeOffset + 1);
                //    int y = Main.Random.Next(-ShakeOffset, ShakeOffset + 1);
                //    return new Vector2(x, y);
                //}
                return Vector2.Zero;
            }
            return SourceLight.GetOffset();
        }

        private const int SizeVariant = 3;
        private bool _hasIncremented;
        private float GetRadius()
        {
            if (IsLightSource || SourceLight == null)
            {
                if (ChangesSize)
                {
                    if (!_hasIncremented)
                    {
                        _hasIncremented = true;
                        _changeSizeTimer.Increment();
                        if (_changeSizeTimer.TimeElapsedInMilliSeconds > RateOfChange)
                        {
                            oldChangeSize = Main.Random.Next(-ShakeOffset, ShakeOffset + 1);
                            _changeSizeTimer.Reset();
                        }
                    }

                    return 80 + oldChangeSize;
                }
                return 80;
            }
            return SourceLight.GetRadius();
        }
    }
}
