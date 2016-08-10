using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
                UpdateLightAt(i);
            }

            DissipateAllLights();
        }

        public static void UpdateLightAt(int ind, bool update)
        {
            UpdateLightAt(ind);
            if (update)
                DissipateLightsAround(ind, new List<int>());
        }

        private static void UpdateLightAt(int ind)
        {
            _lights[ind] = null;
            Tile tile = GameWorld.TileArray[ind];
            Tile wall = GameWorld.WallArray[ind];

            _lights[ind] = new Light(new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                            GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), 0);

            if (tile.IsTransparent && wall.IsTransparent)
            {

                _lights[ind] = new Light(new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                            GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), 15);
                //_lights[ind] =
                //    new Light(
                //        new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                //            GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), Main.Tilesize * 4, Color.White, 1, false);
            }
            if (tile.Id == 11) // Torch
            {
                _lights[ind] = new Light(new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                            GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), 9);
                //_lights[ind] =
                //    new Light(
                //        new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                //            GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), Main.Tilesize * 6, Color.Orange);
            }
            if (tile.Id == 12) // Chandelier
            {
                _lights[ind] =
                    new Light(
                        new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                            GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), Main.Tilesize * 9, Color.White);
            }
            if (tile.Id == 24) // Lava
            {
                _lights[ind] =
                   new Light(
                       new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                           GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), Main.Tilesize * 2, Color.Yellow);
            }
            if (tile.Id == 52) // Sapphire
            {
                _lights[ind] =
                   new Light(
                       new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                           GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), Main.Tilesize * 2, Color.Blue);
            }
            if (tile.Id == 53) // Ruby
            {
                _lights[ind] =
                   new Light(
                       new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                           GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), Main.Tilesize * 2, Color.Red);
            }
            if (tile.Id == 54) // Emerald
            {
                _lights[ind] =
                   new Light(
                       new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                           GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), Main.Tilesize * 2, Color.Green);
            }


        }

        private static void DissipateAllLights()
        {
            bool hasChanged = false;

            for (int i = 0; i < _lights.Length; i++)
            {
                int width = GameWorld.WorldData.LevelWidth;
                byte[] lightLevels = new byte[4];
                if (i - width > 0 && i - width < GameWorld.TileArray.Length)
                {
                    lightLevels[0] = _lights[i - width].LightLevel;
                }
                if (i + width < GameWorld.TileArray.Length && i + width > 0)
                {
                    lightLevels[1] = _lights[i + width].LightLevel;
                }
                if (i - 1 > 0 && i - 1 < GameWorld.TileArray.Length)
                {
                    lightLevels[2] = _lights[i - 1].LightLevel;
                }
                if (i + 1 < GameWorld.TileArray.Length && i + 1 > 0)
                {
                    lightLevels[3] = _lights[i + 1].LightLevel;
                }

                byte max = CalcHelper.GetMax(lightLevels);

                byte change = 1;
                if (!GameWorld.TileArray[i].IsTransparent)
                    change++;

                if (max - change > _lights[i].LightLevel)
                {
                    hasChanged = true;
                    _lights[i].LightLevel = (byte)(max - change);
                }
            }


            if (hasChanged)
                DissipateAllLights();
        }

        /// <summary>
        /// Wrapper method for recursive dissipation.
        /// </summary>
        /// <param name="i"></param>
        private static void DissipateLightsAround(int i, List<int> changedIndices)
        { 

        }

        private static void SetLightLevelAt(int i, byte newLight)
        {
            _lights[i].LightLevel = newLight;
        }

        private static byte GetLightLevelAt(int i)
        {
            return _lights[i].LightLevel;
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
        private static byte GetMaxLightLevelAround(int i)
        {
            List<byte> lightLevels = new List<byte>();
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
        private static byte GetCorrectLightLevelAt(int i)
        {
            byte max = GetMaxLightLevelAround(i);

            int change = 1;
            if (!GameWorld.TileArray[i].IsTransparent)
                change++;
            int minimum = max - change;
            if (minimum < 0) minimum = 0;
            return (byte)minimum;
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

                FontHelper.DrawWithOutline(spriteBatch, FontHelper.Fonts[0], _lights?[index]?.LightLevel.ToString(), new Vector2(GameWorld.TileArray[index].DrawRectangle.Center.X - FontHelper.Fonts[0].MeasureString(_lights?[index]?.LightLevel.ToString()).X / 2, GameWorld.TileArray[index].DrawRectangle.Y), 1, Color.White, Color.Black);
            }
        }
    }
}
