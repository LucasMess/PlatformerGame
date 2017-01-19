﻿using Adam.Misc;
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
            _center = center;
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
            Vector2 offset = new Vector2();
            float radius = GetRadius();
            DrawRectangle = new Rectangle((int)(_center.X - radius + offset.X), (int)(_center.Y - radius + offset.Y), (int)(radius * 2), (int)(radius * 2));
            GlowRectangle = new Rectangle((int)(_center.X + offset.X - radius / 4), (int)(_center.Y + radius / 4 + offset.Y), (int)(radius / 2), (int)(radius / 2));
        }

        public Rectangle DrawRectangle { get; private set; }
        public Rectangle GlowRectangle { get; private set; }

        public void DrawLight(SpriteBatch spriteBatch)
        {
            Update(_center);
            spriteBatch.Draw(_texture, DrawRectangle, _sourceRectangle, new Color((float)RedIntensity / MaxLightLevel, (float)GreenIntensity / MaxLightLevel, (float)BlueIntensity / MaxLightLevel, ((float)GetOpacity() / MaxLightLevel)), 0, new Vector2(0, 0), SpriteEffects.None, 0);
            _hasIncremented = false;
        }

        public void DrawR(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, DrawRectangle, _sourceRectangle, new Color((float)RedIntensity / MaxLightLevel, 0, 0, ((float)GetOpacity() / MaxLightLevel)), 0, new Vector2(0, 0), SpriteEffects.None, 0);

        }

        public void DrawG(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, DrawRectangle, _sourceRectangle, new Color(0, (float)GreenIntensity / MaxLightLevel, 0, ((float)GetOpacity() / MaxLightLevel)), 0, new Vector2(0, 0), SpriteEffects.None, 0);

        }

        public void DrawB(SpriteBatch spriteBatch)
        {
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
            int offR = 0;
            int offG = 0;
            int offB = 0;

            if (IsLightSource)
                return new Vector2();

            if (IsLightSource || SourceLight == null)
            {
                return Vector2.Zero;
            }
            return SourceLight.GetOffset();
        }

        private const int SizeVariant = 3;
        private bool _hasIncremented;
        private float GetRadius()
        {
            float r = 0;
            float g = 0;
            float b = 0;

            if (IsLightSource)
            {
                if (ChangesSize)
                {
                    if (!_hasIncremented)
                    {
                        _hasIncremented = true;
                        _changeSizeTimer.Increment();
                        if (_changeSizeTimer.TimeElapsedInMilliSeconds > RateOfChange)
                        {
                            oldChangeSize = AdamGame.Random.Next(-ShakeOffset, ShakeOffset + 1);
                            _changeSizeTimer.Reset();
                        }
                    }

                    return 80 + oldChangeSize;
                }
                return 80;
            }

            if (RedIntensity > BlueIntensity)
            {
                if (RedIntensity > GreenIntensity)
                {
                    if (RedSource != null) return RedSource.GetRadius();
                }
                if (GreenSource != null) return GreenSource.GetRadius();
            }
            if (BlueIntensity > GreenIntensity)
                if (BlueSource != null) return BlueSource.GetRadius();
            if (GreenSource != null) return GreenSource.GetRadius();
            return 80;

        }
    }
}