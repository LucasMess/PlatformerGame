using ThereMustBeAnotherWay.Misc;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThereMustBeAnotherWay.Particles;

namespace ThereMustBeAnotherWay.Levels
{
    /// <summary>
    /// Spawns snow and rain particle among other things.
    /// </summary>
    public static class Weather
    {

        private static GameTimer spawnTimer = new GameTimer();
        private static int[] lastIndices = new int[0];

        public static void Update()
        {
            spawnTimer.Increment();

            if (GameWorld.WorldData.IsSnowing)
            {
                SpawnSnow();
            }
            if (GameWorld.WorldData.IsRaining)
            {
                SpawnRain();
            }
        }

        private static void SpawnSnow()
        {

            if (spawnTimer.TimeElapsedInMilliSeconds > 500)
            {
                spawnTimer.Reset();
                int[] indices = GameWorld.ChunkManager.GetVisibleIndexes();
                if (indices == null)
                    return;

                int width = GameWorld.ChunkManager.GetVisibileWidth();
                int starting = indices[0];

                // Do the top row as always.
                for (int i = starting; i < starting + width; i++)
                {
                    Tile tile = GameWorld.GetTile(i);
                    ParticleSystem.Add(Particles.ParticleType.Snow, CalcHelper.GetRandXAndY(tile.DrawRectangle), new Vector2(0, 1), Color.White);
                }

                var newIndices = indices.Except(lastIndices);

                foreach (var i in newIndices)
                {
                    Tile tile = GameWorld.GetTile(i);
                    ParticleSystem.Add(Particles.ParticleType.Snow, CalcHelper.GetRandXAndY(tile.DrawRectangle), new Vector2(0, 1), Color.White);
                }

                lastIndices = indices;

            }
        }
        private static void SpawnRain()
        {

            if (spawnTimer.TimeElapsedInMilliSeconds > 50)
            {
                spawnTimer.Reset();
                int[] indices = GameWorld.ChunkManager.GetVisibleIndexes();
                if (indices == null)
                    return;

                int width = GameWorld.ChunkManager.GetVisibileWidth();
                int starting = indices[0];

                // Do the top row as always.
                for (int i = starting; i < starting + width; i++)
                {
                    Tile tile = GameWorld.GetTile(i);
                    ParticleSystem.Add(Particles.ParticleType.Rain, CalcHelper.GetRandXAndY(tile.DrawRectangle), new Vector2(0, 8), Color.Blue);
                }

                // This spawns particles on areas that have just been put into view.
                var newIndices = indices.Except(lastIndices);

                foreach (var i in newIndices)
                {
                    Tile tile = GameWorld.GetTile(i);
                    ParticleSystem.Add(Particles.ParticleType.Rain, CalcHelper.GetRandXAndY(tile.DrawRectangle), new Vector2(0, 8), Color.Blue);
                }

                lastIndices = indices;

            }
        }

    }
}
