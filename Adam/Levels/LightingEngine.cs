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
            if (_lights == null) return;
            _lights[ind] = null;
            Tile tile = GameWorld.TileArray[ind];
            Tile wall = GameWorld.WallArray[ind];
            if (tile.IsTransparent && wall.IsTransparent)
            {
                _lights[ind] =
                    new Light(
                        new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                            GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), Main.Tilesize * 4, Color.White, 1, false);
            }
            if (tile.Id == 11) // Torch
            {
                _lights[ind] =
                    new Light(
                        new Vector2(GameWorld.TileArray[ind].GetDrawRectangle().Center.X,
                            GameWorld.TileArray[ind].GetDrawRectangle().Center.Y), Main.Tilesize * 6, Color.Orange);
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
            }
        }
    }
}
