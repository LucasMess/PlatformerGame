using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThereMustBeAnotherWay.Levels
{
    static class TerrainGeneration
    {
        static Perlin perlin;

        public static void Initialize(int seed = 0)
        {
            if (seed == 0)
            {
                seed = TMBAW_Game.Random.Next(Int32.MaxValue);
            }
            perlin = new Perlin(seed);
        }

        public static void GenerateWorld()
        {
            int[] tiles = new int[256 * 256];
            for (int i = 0; i < tiles.Length; i++)
            {

            }
        }

    }
}
