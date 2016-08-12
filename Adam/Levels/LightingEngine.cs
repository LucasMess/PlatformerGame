using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Adam.Levels
{
    public static class LightingEngine
    {
        //TODO: Work on lower performance lighting system.

        private static Light[] _lights;

        public static void GenerateLights()
        {
            _lights = new Light[GameWorld.WorldData.LevelWidth * GameWorld.WorldData.LevelHeight];
            for (int i = 0; i < _lights.Length; i++)
            {
                ResetLightAt(i);
            }

            UpdateAllLights();
        }

        public static void UpdateLightingAt(int ind, bool update)
        {
            ResetLightAt(ind);
            if (update)
                UpdateLightsAround(ind);
        }

        private static void ResetLightAt(int ind)
        {
            if (ind < 0 || ind >= _lights.Length) return;
            _lights[ind] = null;
            Tile tile = GameWorld.TileArray[ind];
            Tile wall = GameWorld.WallArray[ind];

            _lights[ind] = new Light(new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                            GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), 0, Color.White);

            if (tile.IsTransparent && wall.IsTransparent)
            {

                _lights[ind] = new Light(new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                            GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), Light.MaxLightLevel, Color.White);
            }
            else if (tile.Id == 11) // Torch
            {
                _lights[ind] = new Light(new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                            GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), 15, Color.Orange)
                {
                    ChangesSize = true,
                };
            }
            else if (tile.Id == 12) // Chandelier
            {
                _lights[ind] = new Light(new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                            GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), Light.MaxLightLevel, Color.White);
            }
            else if (tile.Id == 24) // Lava
            {
                _lights[ind] = new Light(new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                             GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), Light.MaxLightLevel, Color.Red);
            }
            else if (tile.Id == 52) // Sapphire
            {
                _lights[ind] = new Light(new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                            GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), 6, Color.Blue);
            }
            else if (tile.Id == 53) // Ruby
            {
                _lights[ind] = new Light(new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                            GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), 6, Color.Red);
            }
            else if (tile.Id == 54) // Emerald
            {
                _lights[ind] = new Light(new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                             GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), 6, Color.Green);
            }
            else
            {
                _lights[ind].IsLightSource = false;
            }


        }

        static int count = 0;
        private static bool _needsToUpdateAgain = false;
        private static void UpdateAllLights()
        {
            _needsToUpdateAgain = false;
            for (int i = 0; i < _lights.Length; i++)
            {
                UpdateLight(i);
            }

            count++;
            Console.WriteLine("Times called:" + count);
            if (_needsToUpdateAgain)
                UpdateAllLights();
        }

        private static void UpdateLight(int i)
        {
            if (i < 0 || i >= _lights.Length) return;
            int width = GameWorld.WorldData.LevelWidth;
            Light[] lightsAround = new Light[4];
            if (i - width >= 0 && i - width < GameWorld.TileArray.Length)
            {
                lightsAround[0] = _lights[i - width];
            }
            if (i + width < GameWorld.TileArray.Length && i + width >= 0)
            {
                lightsAround[1] = _lights[i + width];
            }
            if (i - 1 >= 0 && i - 1 < GameWorld.TileArray.Length)
            {
                lightsAround[2] = _lights[i - 1];
            }
            if (i + 1 < GameWorld.TileArray.Length && i + 1 >= 0)
            {
                lightsAround[3] = _lights[i + 1];
            }

            int max = 0;
            Light sourceLight = new Light();
            Color[] colors = new Color[4];
            int count = 0;
            foreach (var light in lightsAround)
            {
                if (light != null)
                {
                    if (light.LightLevel > max)
                    {
                        max = light.LightLevel;
                        sourceLight = light;
                    }
                    //Color source = light.GetSourceColor();
                    //colors[count] = new Color(source.R, source.G, source.B) * ((float)light.LightLevel / Light.MaxLightLevel);

                }
                count++;
            }

            //int r = 0, g = 0, b = 0;
            //foreach (var color in colors)
            //{
            //    r += color.R;
            //    g += color.G;
            //    b += color.B;
            //}
            //_lights[i]._color = new Color(r/colors.Length, g / colors.Length, b / colors.Length, 255);

            int change = 1;
            if (!GameWorld.TileArray[i].IsTransparent)
                change += 2;

            int newLightLevel = max - change;
            if (newLightLevel < 0)
                newLightLevel = 0;
            if (newLightLevel > Light.MaxLightLevel)
                newLightLevel = Light.MaxLightLevel;

            if (_lights[i].LightLevel < newLightLevel)
            {
                _lights[i].LightLevel = newLightLevel;
                if (sourceLight != null)
                    _lights[i].SourceLight = sourceLight;
                _needsToUpdateAgain = true;
            }
        }

        private static void UpdateLightsAround(int i)
        {
            // Clear all non-sources
            int[] indices = GetIndicesOfAllLightsInRange(i);
            foreach (var ind in indices)
            {
                if (ind < 0 || ind >= _lights.Length) return;
                ResetLightAt(ind);
            }

            _needsToUpdateAgain = true;
            while (_needsToUpdateAgain)
            {
                _needsToUpdateAgain = false;

                foreach (var ind in indices)
                {
                    UpdateLight(ind);
                }
            }
        }

        private static int[] GetIndicesOfAllLightsInRange(int i)
        {
            int halfSize = Light.MaxLightLevel;
            int width = GameWorld.WorldData.LevelWidth;
            int starting = i - halfSize - halfSize * width;
            List<int> indices = new List<int>();
            for (int h = 0; h < Light.MaxLightLevel * 2; h++)
            {
                for (int w = 0; w < Light.MaxLightLevel * 2; w++)
                {
                    int index = starting + h * width + w;
                    indices.Add(index);
                }
            }
            return indices.ToArray();
        }

        public static void DrawLights(SpriteBatch spriteBatch)
        {
            foreach (var index in GameWorld.ChunkManager.GetVisibleIndexes())
            {
                _lights[index]?.DrawLight(spriteBatch);
            }
        }

        public static void DrawGlows(SpriteBatch spriteBatch)
        {
            foreach (var index in GameWorld.ChunkManager.GetVisibleIndexes())
            {
                _lights[index]?.DrawGlow(spriteBatch);

                //string text = _lights?[index]?.LightLevel.ToString();
                //Color color = _lights[index].GetColor();
                //string text = "R:" + color.R;
                //FontHelper.DrawWithOutline(spriteBatch, FontHelper.Fonts[0], text, new Vector2(GameWorld.TileArray[index].DrawRectangle.Center.X - FontHelper.Fonts[0].MeasureString(text).X / 2, GameWorld.TileArray[index].DrawRectangle.Y), 1, Color.White, Color.Black);
            }
        }
    }
}
