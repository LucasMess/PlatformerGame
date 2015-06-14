using CodenameAdam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    class Chunk
    {
        Texture2D texture;
        Rectangle rectangle;
        Rectangle checkingRectangle;
        GraphicsDevice GraphicsDevice;

        public int pos;
        public int mapWidth;
        public int tileIndex;
        int tileSize;

        public int dimensions = 16;

        public Chunk(GraphicsDevice GraphicsDevice)
        {
            this.GraphicsDevice = GraphicsDevice;
            this.tileSize = Game1.Tilesize;
            rectangle = new Rectangle(0, 0, tileSize * dimensions, tileSize * dimensions);
        }

        public void Create(Tile[] tileArray, int colums)
        {
            checkingRectangle = new Rectangle(pos % colums * dimensions * tileSize, pos / colums * dimensions *tileSize, tileSize, tileSize);

            foreach (var tile in tileArray)
            {
                if (checkingRectangle.Intersects(tile.rectangle))
                    tileIndex = tile.TileIndex;
            }

            rectangle.X = tileArray[tileIndex].rectangle.X;
            rectangle.Y = tileArray[tileIndex].rectangle.Y;

            Color[] chunkColor = new Color[rectangle.Width * rectangle.Height];
            texture = new Texture2D(GraphicsDevice, rectangle.Width, rectangle.Height);

            //Check each of the 512 tiles in a chunk.
            for (int h = 0; h < dimensions; h++)
            {
                for (int w = 0; w < dimensions; w++)
                {
                    //detect where in the tilearray the tile to be drawn is
                    int tilePos = tileIndex + h * colums + w;
                    Tile tile = tileArray[tilePos];

                    Color[] tileColor = new Color[tileSize * tileSize];

                    if (tile.texture != null && tile.isSolid)
                        tile.texture.GetData<Color>(0, tile.sourceRectangle, tileColor, 0, tileSize * tileSize);

                    int startIndex = h * tileSize + w * dimensions * tileSize;
                    int currentPixel = 0;

                    for (int w1 = 0; w1 < tileSize; w1++)
                    {
                        for (int h1 = 0; h1 < tileSize; h1++)
                        {
                            chunkColor[startIndex + h1 + w1* rectangle.Width] = tileColor[currentPixel];
                            currentPixel++;
                        }
                    }
                }
            }

            texture.SetData<Color>(chunkColor);
        }

        public void Draw(Player player, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, Color.White);
            if (player != null)
                spriteBatch.Draw(texture, player.collRectangle, Color.White);
        }
    }
}
