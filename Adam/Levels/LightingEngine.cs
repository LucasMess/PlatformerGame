using Adam.Misc.Helpers;
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
                //_lights[ind] =
                //    new Light(
                //        new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                //            GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), Main.Tilesize * 4, Color.White, 1, false);
            }
            else if (tile.Id == 11) // Torch
            {
                _lights[ind] = new Light(new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                            GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), 15, Color.Orange);
                //_lights[ind] =
                //    new Light(
                //        new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                //            GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), Main.Tilesize * 6, Color.Orange);
            }
            else if (tile.Id == 12) // Chandelier
            {
                _lights[ind] =
                    new Light(
                        new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                            GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), Main.Tilesize * 9, Color.White);
            }
            else if (tile.Id == 24) // Lava
            {
                _lights[ind] =
                   new Light(
                       new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                           GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), Main.Tilesize * 2, Color.Yellow);
            }
            else if (tile.Id == 52) // Sapphire
            {
                _lights[ind] =
                   new Light(
                       new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                           GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), Main.Tilesize * 2, Color.Blue);
            }
            else if (tile.Id == 53) // Ruby
            {
                _lights[ind] =
                   new Light(
                       new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                           GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), Main.Tilesize * 2, Color.Red);
            }
            else if (tile.Id == 54) // Emerald
            {
                _lights[ind] =
                   new Light(
                       new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                           GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), Main.Tilesize * 2, Color.Green);
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
            int[] lightLevels = new int[4];
            if (i - width >= 0 && i - width < GameWorld.TileArray.Length)
            {
                lightLevels[0] = _lights[i - width].LightLevel;
            }
            if (i + width < GameWorld.TileArray.Length && i + width >= 0)
            {
                lightLevels[1] = _lights[i + width].LightLevel;
            }
            if (i - 1 >= 0 && i - 1 < GameWorld.TileArray.Length)
            {
                lightLevels[2] = _lights[i - 1].LightLevel;
            }
            if (i + 1 < GameWorld.TileArray.Length && i + 1 >= 0)
            {
                lightLevels[3] = _lights[i + 1].LightLevel;
            }


            int max = CalcHelper.GetMax(lightLevels);



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

        private static void SetLightLevelAt(int i, int newLight)
        {
            _lights[i].LightLevel = newLight;
        }

        private static int GetLightLevelAt(int i)
        {
            return _lights[i].LightLevel;
        }

        public static int[] LASTMODIFIEDINDICES;

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
            LASTMODIFIEDINDICES = indices.ToArray();
            return indices.ToArray();
        }

        /// <summary>
        /// Returns the indices of all tiles around this tile index.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private static int[] GetIndicesAround(int i)
        {
            int width = GameWorld.WorldData.LevelWidth;
            int[] indices = new[] {
                i-width,
                i-1,i+1,
                i+width,
                };

            for (int j = 0; j < indices.Length; j++)
            {
                if (indices[j] < 0)
                    indices[j] = 0;

                if (indices[j] >= GameWorld.TileArray.Length)
                    indices[j] = GameWorld.TileArray.Length;
            }

            return indices;
        }

        /// <summary>
        /// Returns the max light level around the tile with this index.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private static int GetMaxLightLevelAround(int i)
        {
            List<int> lightLevels = new List<int>();
            foreach (int index in GetIndicesAround(i))
            {
                lightLevels.Add(_lights[index].LightLevel);
            }
            return CalcHelper.GetMax(lightLevels.ToArray());
        }

        /// <summary>
        /// Gets the minimum light level of the tile with this index based on the light level of the tiles around it. It cannot be smaller than 0.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private static int GetCorrectLightLevelAt(int i)
        {
            int max = GetMaxLightLevelAround(i);

            int change = 1;
            if (!GameWorld.TileArray[i].IsTransparent)
                change += 2;
            int minimum = max - change;
            if (minimum < 0) minimum = 0;
            return minimum;
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

                //FontHelper.DrawWithOutline(spriteBatch, FontHelper.Fonts[0], _lights?[index]?.LightLevel.ToString(), new Vector2(GameWorld.TileArray[index].DrawRectangle.Center.X - FontHelper.Fonts[0].MeasureString(_lights?[index]?.LightLevel.ToString()).X / 2, GameWorld.TileArray[index].DrawRectangle.Y), 1, Color.White, Color.Black);
            }
        }
    }
}
