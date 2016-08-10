using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Adam.Levels
{
    public static class LightingEngine
    {

        private static Light[] _lights;

        public static void GenerateLights()
        {
            _lights = new Light[GameWorld.WorldData.LevelWidth * GameWorld.WorldData.LevelHeight];
            for (int i = 0; i < _lights.Length; i++)
            {
                DefineLight(i);
            }
            UpdateAllLights();
        }

        private static void DefineLight(int i)
        {
            _lights[i] = new Light(i);

            // Sunlight
            if (GameWorld.TileArray[i].IsTransparent && GameWorld.WallArray[i].IsTransparent)
            {
                _lights[i].MakeSource(25);
            }
        }

        private static void UpdateAllLights()
        {
            bool hasChanged = false;
            do
            {
                hasChanged = false;
                int width = GameWorld.WorldData.LevelWidth;
                for (int i = 0; i < _lights.Length; i++)
                {
                    hasChanged = UpdateLightAt(i);
                }
            }
            while (hasChanged);
        }

        private static bool UpdateLightAt(int i)
        {
            int width = GameWorld.WorldData.LevelWidth;
            int[] indices = new[] {
                    i - width - 1,
                    i - width,
                    i - width + 1,
                    i - 1,
                    i + 1,
                    i + width - 1,
                    i + width,
                    i + width + 1 };
            List<byte> strengths = new List<byte>();
            foreach (var ind in indices)
            {
                if (ind >= 0 && ind < GameWorld.TileArray.Length)
                {
                    strengths.Add((byte)_lights[ind].Strength);
                }
            }

            int max = CalcHelper.GetMax(strengths.ToArray());
            int change = 1;
            if (!GameWorld.TileArray[i].IsTransparent)
                change += 3;
            if (_lights[i].Strength < max - change)
            {
                _lights[i].Strength = max - change;
                return true;
            }
            return false;
        }

        public static void UpdateLightingAround(int i)
        {
            List<Light> sources = new List<Light>();
            int width = GameWorld.WorldData.LevelWidth;
            int startingIndex = i - width * (int)Math.Ceiling(Light.MaxIntensity / 2.0) - (int)Math.Ceiling(Light.MaxIntensity / 2.0);
            for (int h = 0; h < Light.MaxIntensity; h++)
            {
                for (int w = 0; w < Light.MaxIntensity; w++)
                {
                    int index = startingIndex + w + h * width;
                    if (index >= 0 && index < GameWorld.TileArray.Length)
                    {
                        Light light = _lights[index];
                        if (!light.IsSource)
                        {
                            light.Strength = 0;
                        }

                    }
                }
            }
            for (int h = 0; h < Light.MaxIntensity; h++)
            {
                for (int w = 0; w < Light.MaxIntensity; w++)
                {
                    int index = startingIndex + w + h * width;
                    if (index >= 0 && index < GameWorld.TileArray.Length)
                    {
                        Light light = _lights[index];
                        if (!light.IsSource)
                        {
                            UpdateLightAt(index);
                        }

                    }
                }
            }


        }

        private static void FloodSources(List<Light> sources)
        {
            foreach (var source in sources)
            {
                int i = source.Index;
                int width = GameWorld.WorldData.LevelWidth;
                int[] indices = new[] {
                    i - width - 1,
                    i - width,
                    i - width + 1,
                    i - 1,
                    i + 1,
                    i + width - 1,
                    i + width,
                    i + width + 1,
                };

                foreach (var ind in indices)
                {
                    FloodLightRecursive(source.Strength, ind);
                }

            }
        }

        private static void FloodLightRecursive(int currentLightLevel, int index)
        {
            if (currentLightLevel < 0)
                return;

            currentLightLevel--;

            int i = index;
            int width = GameWorld.WorldData.LevelWidth;

            int[] indices = new[] {
                    i - width - 1,
                    i - width,
                    i - width + 1,
                    i - 1,
                    i + 1,
                    i + width - 1,
                    i + width,
                    i + width + 1,
                };

            if (i >= 0 && i < GameWorld.TileArray.Length)
            {
                if (currentLightLevel > _lights[i].Strength)
                    _lights[i].Strength = currentLightLevel;
                foreach (var ind in indices)
                {
                    FloodLightRecursive(currentLightLevel, ind);
                }
            }

        }


        public static void DrawLights(GraphicsDevice graphics, SpriteBatch spriteBatch)
        {
            Texture2D lightMap = new Texture2D(graphics, GameWorld.WorldData.LevelWidth, GameWorld.WorldData.LevelHeight);
            Color[] pixels = new Color[_lights.Length];

            for (int i = 0; i < pixels.Length; i++)
            {
                int strength = _lights[i].Strength;
                //if (strength != 15)
                //    strength = 0;
                pixels[i] = new Color(0f, 0f, 0f, 1 - (strength / (float)Light.MaxIntensity));
            }

            lightMap.SetData<Color>(pixels);

            Vector2 cameraPos = Main.Camera.LeftTopGameCoords;
            int width = (int)(((float)Main.DefaultResWidth / (GameWorld.WorldData.LevelWidth * Main.Tilesize)) * lightMap.Width);
            int height = (int)(((float)Main.DefaultResHeight / (GameWorld.WorldData.LevelHeight * Main.Tilesize)) * lightMap.Height);

            //spriteBatch.Draw(lightMap, new Rectangle(0, 0, Main.DefaultResWidth, Main.DefaultResHeight), Color.White);

            //spriteBatch.Draw(lightMap, new Rectangle(0, 0, Main.DefaultResWidth, Main.DefaultResHeight), new Rectangle((int)cameraPos.X, (int)cameraPos.Y, width, height), Color.White);

            spriteBatch.Draw(lightMap, new Rectangle(0, 0, (GameWorld.WorldData.LevelWidth * Main.Tilesize), (GameWorld.WorldData.LevelHeight * Main.Tilesize)), Color.White);
        }

    }



}
