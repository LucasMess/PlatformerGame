using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        }

        public static void UpdateLightAt(int ind)
        {
            _lights[ind] = null;
            Tile tile = GameWorld.TileArray[ind];
            Tile wall = GameWorld.WallArray[ind];
            if (tile.IsTransparent && wall.IsTransparent)
            {
                _lights[ind] =
                    new Light(
                        new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                            GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), Main.Tilesize * 4, Color.White);
            }
            if (tile.Id == 11) // Torch
            {
                _lights[ind] =
                    new Light(
                        new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                            GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), Main.Tilesize * 6, Color.Orange);
            }

        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var index in GameWorld.ChunkManager.GetVisibleIndexes())
            {
                _lights[index]?.Draw(spriteBatch);
            }
        }
    }
}
