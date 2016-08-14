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
        public int RedIntensity { get; set; } = 0;
        public int GreenIntensity { get; set; } = 0;
        public int BlueIntensity { get; set; } = 0;
        private static Rectangle _sourceRectangle = new Rectangle(320, 240, 64, 64);
        private Vector2 _center;
        public Color _color = Color.Black;
        public float Red { get; set; }
        public float Green { get; set; }
        public float Blue { get; set; }
        public Light RedSource { get; set; }
        public Light GreenSource { get; set; }
        public Light BlueSource { get; set; }
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
                RedIntensity = (int)(Light.MaxLightLevel * color.R / 255f);
                GreenIntensity = (int)(Light.MaxLightLevel * color.G / 255f);
                BlueIntensity = (int)(Light.MaxLightLevel * color.B / 255f);
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
            //float colorR = (GetRedSourceColor().R / 255 * RedIntensity / MaxLightLevel + GetGreenSourceColor().R / 255 * GreenIntensity / MaxLightLevel + GetBlueSourceColor().R / 255 * BlueIntensity / MaxLightLevel) / 3;
            //float colorG = (GetRedSourceColor().G / 255 * RedIntensity / MaxLightLevel + GetGreenSourceColor().G / 255 * GreenIntensity / MaxLightLevel + GetBlueSourceColor().G / 255 * BlueIntensity / MaxLightLevel) / 3;
            //float colorB = (GetRedSourceColor().B / 255 * RedIntensity / MaxLightLevel + GetGreenSourceColor().B / 255 * GreenIntensity / MaxLightLevel + GetBlueSourceColor().B / 255 * BlueIntensity / MaxLightLevel) / 3;

            //spriteBatch.Draw(_texture, DrawRectangle, _sourceRectangle, new Color(colorR, colorG, colorB, GetOpacity()), 0, new Vector2(0, 0), SpriteEffects.None, _layerDepth);

            //spriteBatch.Draw(_texture, DrawRectangle, _sourceRectangle, GetRedSourceColor() * (GetOpacity() / MaxLightLevel), 0, new Vector2(0, 0), SpriteEffects.None, _layerDepth);
            //spriteBatch.Draw(_texture, DrawRectangle, _sourceRectangle, GetGreenSourceColor() * (GetOpacity() / MaxLightLevel), 0, new Vector2(0, 0), SpriteEffects.None, _layerDepth);
            //spriteBatch.Draw(_texture, DrawRectangle, _sourceRectangle, GetBlueSourceColor() * (GetOpacity() / MaxLightLevel), 0, new Vector2(0, 0), SpriteEffects.None, _layerDepth);


            spriteBatch.Draw(_texture, DrawRectangle, _sourceRectangle, new Color((float)RedIntensity / MaxLightLevel, (float)GreenIntensity / MaxLightLevel, (float)BlueIntensity / MaxLightLevel, ((float)GetOpacity() / MaxLightLevel)), 0, new Vector2(0, 0), SpriteEffects.None, _layerDepth);

            _hasIncremented = false;
        }

        public void DrawR(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(_texture, DrawRectangle, _sourceRectangle, GetRedSourceColor() * ((float)RedIntensity / MaxLightLevel), 0, new Vector2(0, 0), SpriteEffects.None, 0);
            spriteBatch.Draw(_texture, DrawRectangle, _sourceRectangle, new Color((float)RedIntensity / MaxLightLevel, 0, 0, ((float)GetOpacity() / MaxLightLevel)), 0, new Vector2(0, 0), SpriteEffects.None, 0);

        }

        public void DrawG(SpriteBatch spriteBatch)
        {
            // spriteBatch.Draw(_texture, DrawRectangle, _sourceRectangle, GetGreenSourceColor() * ((float)GreenIntensity / MaxLightLevel), 0, new Vector2(0, 0), SpriteEffects.None, 1);
            spriteBatch.Draw(_texture, DrawRectangle, _sourceRectangle, new Color(0, (float)GreenIntensity / MaxLightLevel, 0, ((float)GetOpacity() / MaxLightLevel)), 0, new Vector2(0, 0), SpriteEffects.None, 0);

        }

        public void DrawB(SpriteBatch spriteBatch)
        {
            // spriteBatch.Draw(_texture, DrawRectangle, _sourceRectangle, GetBlueSourceColor() * ((float)BlueIntensity / MaxLightLevel), 0, new Vector2(0, 0), SpriteEffects.None, 2);
            spriteBatch.Draw(_texture, DrawRectangle, _sourceRectangle, new Color(0, 0, (float)BlueIntensity / MaxLightLevel, ((float)GetOpacity() / MaxLightLevel)), 0, new Vector2(0, 0), SpriteEffects.None, 0);

        }

        public void DrawGlow(SpriteBatch spriteBatch)
        {
            if (_hasGlow)
                spriteBatch.Draw(_texture, GlowRectangle, _sourceRectangle, _color * .5f);
        }

        public float GetOpacity()
        {
            int max = 0;
            if (RedIntensity > max)
                max = RedIntensity;
            if (GreenIntensity > max)
                max = GreenIntensity;
            if (BlueIntensity > max)
                max = BlueIntensity;
            return max;
        }

        public Color GetRedSourceColor()
        {
            if (IsLightSource || RedSource == null)
                return _color;
            else return RedSource.GetRedSourceColor();
        }

        public Color GetGreenSourceColor()
        {
            if (IsLightSource || GreenSource == null)
                return _color;
            else return GreenSource.GetGreenSourceColor();
        }

        public Color GetBlueSourceColor()
        {
            if (IsLightSource || BlueSource == null)
                return _color;
            else return BlueSource.GetBlueSourceColor();
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
