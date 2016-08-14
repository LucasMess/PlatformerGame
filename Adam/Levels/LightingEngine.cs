using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

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
                             GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), 10, Color.Red);
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

            Light sourceRed = new Light();
            Light sourceGreen = new Light();
            Light sourceBlue = new Light();
            int count = 0;
            int maxRed = 0;
            int maxGreen = 0;
            int maxBlue = 0;
            foreach (var light in lightsAround)
            {
                if (light != null)
                {
                    if (light.RedIntensity > maxRed)
                    {
                        maxRed = light.RedIntensity;
                        sourceRed = light;
                    }
                    if (light.GreenIntensity > maxGreen)
                    {
                        maxGreen = light.GreenIntensity;
                        sourceGreen = light;
                    }
                    if (light.BlueIntensity > maxBlue)
                    {
                        maxBlue = light.BlueIntensity;
                        sourceBlue = light;
                    }
                }
                count++;
            }

            int change = 1;
            if (!GameWorld.TileArray[i].IsTransparent)
                change += 2;


            int newRed = maxRed - change;
            int newGreen = maxGreen - change;
            int newBlue = maxBlue - change;

            if (newRed < 0) newRed = 0;
            if (newGreen < 0) newGreen = 0;
            if (newBlue < 0) newBlue = 0;
            if (newRed > Light.MaxLightLevel) newRed = Light.MaxLightLevel;
            if (newGreen > Light.MaxLightLevel) newGreen = Light.MaxLightLevel;
            if (newBlue > Light.MaxLightLevel) newBlue = Light.MaxLightLevel;

            Light thisLight = _lights[i];

            if (thisLight.RedIntensity < newRed)
            {
                thisLight.RedIntensity = newRed;
                if (sourceRed != null)
                    thisLight.RedSource = sourceRed;
                _needsToUpdateAgain = true;
            }
            if (thisLight.GreenIntensity < newGreen)
            {
                thisLight.GreenIntensity = newGreen;
                if (sourceGreen != null)
                    thisLight.GreenSource = sourceGreen;
                _needsToUpdateAgain = true;
            }
            if (thisLight.BlueIntensity < newBlue)
            {
                thisLight.BlueIntensity = newBlue;
                if (sourceBlue != null)
                    thisLight.BlueSource = sourceBlue;
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

                //_lights[index]?.DrawR(spriteBatch);
                //_lights[index]?.DrawG(spriteBatch);
                //_lights[index]?.DrawB(spriteBatch);
            }
        }

        public static void DrawGlows(SpriteBatch spriteBatch)
        {
            foreach (var index in GameWorld.ChunkManager.GetVisibleIndexes())
            {
                _lights[index]?.DrawGlow(spriteBatch);
                Light light = _lights[index];
                //string text = _lights?[index]?.LightLevel.ToString();
                //Color color = _lights[index].GetColor();
                StringBuilder text = new StringBuilder();
                text.Append(light.RedIntensity + ",");
                text.Append(light.GreenIntensity + ",");
                text.Append(light.BlueIntensity);
                FontHelper.DrawWithOutline(spriteBatch, FontHelper.Fonts[0], text.ToString(), new Vector2(GameWorld.TileArray[index].DrawRectangle.Center.X - FontHelper.Fonts[0].MeasureString(text).X / 2, GameWorld.TileArray[index].DrawRectangle.Y), 1, Color.White, Color.Black);
            }
        }
    }
}
