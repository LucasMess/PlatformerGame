﻿using ThereMustBeAnotherWay.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using System.Text;
using ThereMustBeAnotherWay.Misc.Helpers;
using MonoGame.Extended.BitmapFonts;

namespace ThereMustBeAnotherWay.Levels
{
    public class Light
    {
        public const sbyte MaxLightLevel = 16;
        public sbyte RedIntensity { get; set; } = 0;
        public sbyte GreenIntensity { get; set; } = 0;
        public sbyte BlueIntensity { get; set; } = 0;
        public sbyte SunlightIntensity { get; set; } = 0;
 

        private const int DefaultRadius = 64;
        private static Rectangle _sourceRectangle = new Rectangle(320, 240, 64, 64);
        private static Rectangle _sourceRectangleFullWhite = new Rectangle(336, 224, 16, 16);
        private Vector2 _center;
        public Light RedSource { get; set; }
        public Light GreenSource { get; set; }
        public Light BlueSource { get; set; }
        public List<Color> ColorOfSources = new List<Color>();
        private Texture2D _texture = GameWorld.SpriteSheet;
        public Light SourceLight { get; set; }
        public bool IsLightSource { get; set; } = true;
        public bool IsShaky { get; set; }
        public bool ChangesSize { get; set; }
        public int RateOfChange { get; set; } = 100;
        private Timer _changeSizeTimer = new Timer();
        private int oldChangeSize;
        public bool IsSunlight { get; set; }

        public Light() { }

        public Light(Vector2 center, int lightLevel, Color color, bool isSunlight = false)
        {
            _center = center;
            IsSunlight = isSunlight;
            if (isSunlight)
            {
                SunlightIntensity = MaxLightLevel;
                IsLightSource = false;
            }
            else
            {
                if (lightLevel != 0)
                {
                    IsLightSource = true;
                    RedIntensity = (sbyte)(MaxLightLevel * color.R / 255f);
                    GreenIntensity = (sbyte)(MaxLightLevel * color.G / 255f);
                    BlueIntensity = (sbyte)(MaxLightLevel * color.B / 255f);
                }
            }
            Update(_center);
        }

        public float Scale { get; set; } = 1;

        public void Update(Vector2 newCenter)
        {
            _center = newCenter;
            Vector2 offset = new Vector2();
            float radius = GetRadius() * Scale;
            DrawRectangle = new Rectangle((int)(_center.X - radius + offset.X), (int)(_center.Y - radius + offset.Y), (int)(radius * 2), (int)(radius * 2));
            GlowRectangle = new Rectangle((int)(_center.X + offset.X - radius / 4), (int)(_center.Y + radius / 4 + offset.Y), (int)(radius / 2), (int)(radius / 2));
        }

        public Rectangle DrawRectangle { get; private set; }
        public Rectangle GlowRectangle { get; private set; }

        public void DrawSunlight(SpriteBatch spriteBatch)
        {
            if (SunlightIntensity == 0) return;
            spriteBatch.Draw(_texture, DrawRectangle, _sourceRectangle, Color.White * ((float)SunlightIntensity / Light.MaxLightLevel), 0, new Vector2(0, 0), SpriteEffects.None, 0);
        }

        public void DrawLight(SpriteBatch spriteBatch)
        {
             Update(_center);
            spriteBatch.Draw(_texture, DrawRectangle, _sourceRectangle, new Color((float)RedIntensity / MaxLightLevel, (float)GreenIntensity / MaxLightLevel, (float)BlueIntensity / MaxLightLevel, (GetOpacity() / MaxLightLevel)), 0, new Vector2(0, 0), SpriteEffects.None, 0);
            _hasIncremented = false;
        }

        public float GetOpacity()
        {
            if (IsLightSource) return MaxLightLevel;
            int max = 0;
            if (RedIntensity > max)
                max = RedIntensity;
            if (GreenIntensity > max)
                max = GreenIntensity;
            if (BlueIntensity > max)
                max = BlueIntensity;
            return max;
        }

        private const int ShakeOffset = 10;
        private Vector2 GetOffset()
        {
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
                            oldChangeSize = TMBAW_Game.Random.Next(-ShakeOffset, ShakeOffset + 1);
                            _changeSizeTimer.Reset();
                        }
                    }

                    return DefaultRadius + oldChangeSize;
                }
                return DefaultRadius;
            }

            if (RedIntensity > BlueIntensity)
            {
                if (RedIntensity > GreenIntensity)
                {
                    if (RedSource != null)
                        return RedSource.GetRadius();
                }
                else if (GreenSource != null)
                    return GreenSource.GetRadius();
            }
            else if (BlueIntensity > GreenIntensity)
            {
                if (BlueSource != null)
                    return BlueSource.GetRadius();
            }
            else if (GreenSource != null)
                return GreenSource.GetRadius();


            return DefaultRadius;

        }

        internal void DrawLightAsGlow(SpriteBatch spriteBatch)
        {
            if (IsLightSource)
            {
                float old = Scale;
                Scale /= 2;
                Update(_center);
                Scale = old;
                spriteBatch.Draw(_texture, DrawRectangle, _sourceRectangle, new Color((float)RedIntensity / MaxLightLevel, (float)GreenIntensity / MaxLightLevel, (float)BlueIntensity / MaxLightLevel, ((float)GetOpacity() / MaxLightLevel)) * .5f, 0, new Vector2(0, 0), SpriteEffects.None, 0);
            }
        }

        public void DrawDebug(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(FontHelper.Fonts[0], RedIntensity.ToString(), new Vector2(DrawRectangle.Center.X, DrawRectangle.Center.Y), Color.Red);
            spriteBatch.DrawString(FontHelper.Fonts[0], GreenIntensity.ToString(), new Vector2(DrawRectangle.Center.X, DrawRectangle.Center.Y + 8), Color.Green);
            spriteBatch.DrawString(FontHelper.Fonts[0], BlueIntensity.ToString(), new Vector2(DrawRectangle.Center.X, DrawRectangle.Center.Y + 16), Color.Blue);
        }
    }
}
