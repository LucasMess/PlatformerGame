using CodenameAdam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    class Room
    {
        int mapWidth, mapHeight;

        public Room(int mapWidth, int mapHeight)
        {
            this.mapHeight = mapHeight;
            this.mapWidth = mapWidth;
        }

        public void Add(int currentPos, int size, ref Tile[] tileArray)
        {
            for (int h = 0; h <= size; h++)
            {
                for (int w = 0; w <= size; w++)
                {
                    if (currentPos + w + h * mapWidth < tileArray.Length &&
                        currentPos + w + h * mapWidth > 0)
                    {
                        tileArray[currentPos + w + (h * mapWidth)].ID = 0;
                        tileArray[currentPos + w + (h * mapWidth)].isSolid = false;
                    }

                }
            }
        }

        public void AddExit(int currentPos, int size, Tile[] tileArray)
        {
            for (int h = 0; h <= size; h++)
            {
                for (int w = 0; w <= size; w++)
                {
                    tileArray[currentPos + w + (h * mapWidth)].ID = 4;
                    tileArray[currentPos + w + (h * mapWidth)].isSolid = false;
                }
            }
        }
    }
}
