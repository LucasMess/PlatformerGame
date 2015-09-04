using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Levels
{
    /// <summary>
    /// Organizes the gameworld into chunks.
    /// </summary>
    public class ChunkManager
    {
        Chunk[] chunks;
        private int _maxChunksX;
        private int _maxChunksY;
        private int worldWidth;
        private int worldHeight;

        public ChunkManager()
        {

        }

        public void ConvertToChunks(int worldWidth, int worldHeight)
        {
            // Validate data.
            if (worldWidth % Chunk.DefaultSize != 0 || worldHeight % Chunk.DefaultSize != 0)
            {
                throw new ArgumentException("The world size cannot be divided into chunks because it is not divisible by the default chunk size.");
            }

            this.worldHeight = worldHeight;
            this.worldWidth = worldWidth;

            // Calculates the amount of chunks that need to be created.
            _maxChunksX = worldWidth / Chunk.DefaultSize;
            _maxChunksY = worldHeight / Chunk.DefaultSize;

            chunks = new Chunk[_maxChunksX * _maxChunksY];

            for (int i = 0; i < chunks.Length; i++)
            {
                chunks[i] = new Chunk(i);
            }

            SplitTiles();
        }

        /// <summary>
        /// Split tiles into their own chunks.
        /// </summary>
        private void SplitTiles()
        {
            for (int chunkY = 0; chunkY < _maxChunksY; chunkY++)
            {
                for (int chunkX = 0; chunkX < _maxChunksX; chunkX++)
                {
                    for (int tileY = 0; tileY < Chunk.DefaultSize; tileY++)
                    {
                        for (int tileX = 0; tileX < Chunk.DefaultSize; tileX++)
                        {
                            int currentTileInArray = chunkX * Chunk.DefaultSize + chunkY * worldWidth * Chunk.DefaultSize + tileY * Chunk.DefaultSize + tileX;
                            int currentChunkInArray = chunkY * _maxChunksY + chunkX;
                            int currentTileInChunk = tileY * Chunk.DefaultSize + tileX;

                            chunks[currentChunkInArray].SetData(currentTileInChunk, currentTileInChunk);

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gathers all the indexes of the visible chunks.
        /// </summary>
        /// <returns>Visible indexes of tiles in chunks.</returns>
        public int[] GetVisibleIndexes()
        {
            Chunk[] visibleChunks = GetVisibleChunks();
            int amountOfTilesInside = Chunk.DefaultSize * Chunk.DefaultSize;
            int[] visibleIndexes = new int[(visibleChunks.Length + 2) * amountOfTilesInside];

            // Transfer the indexes stored in the chunks to a single array.
            foreach (Chunk chunk in visibleChunks)
            {
                int[] indexesInChunk = chunk.GetTileIndexes();
                indexesInChunk.CopyTo(visibleIndexes, amountOfTilesInside * chunk.Index);
            }


            return visibleIndexes;
        }

        /// <summary>
        /// Calculates the chunk at the specified location.
        /// </summary>
        /// <param name="x">X-coordinate.</param>
        /// <param name="y">Y-coordinate.</param>
        /// <returns>Chunk at location.</returns>
        public Chunk GetChunk(int x, int y)
        {
            int chunkIndex = (y / Chunk.DefaultSize * _maxChunksX) + (int)(x / Chunk.DefaultSize);
            if (chunkIndex >= 0 && chunkIndex < chunks.Length)
                return chunks[chunkIndex];
            else return new Chunk(0);
        }

        /// <summary>
        /// Calculates the chunks that are visible by the player.
        /// </summary>
        /// <returns>Visible chunks.</returns>
        private Chunk[] GetVisibleChunks()
        {
            if (GameWorld.Instance.player == null || chunks == null || GameWorld.Instance.camera == null) return new Chunk[0];

            // Gets the chunk the camera is in.

            int playerChunk = GetChunk((int)GameWorld.Instance.camera.lastCameraLeftCorner.X, (int)GameWorld.Instance.camera.lastCameraLeftCorner.Y).Index;

            // Defines how many chunks are visible in either direction.
            int visibleChunksY = (int)(3 / GameWorld.Instance.camera.GetZoom());
            int visibleChunksX = (int)(3 / GameWorld.Instance.camera.GetZoom());

            // Finds where the top left visible chunk is.
            int startingChunk = playerChunk - (int)Math.Ceiling((double)(visibleChunksX / 2)) - 1;

            // Makes an array of visible chunks.
            List<Chunk> visibleChunks = new List<Chunk>();
            for (int h = 0; h < visibleChunksY; h++)
            {
                for (int w = 0; w < visibleChunksX; w++)
                {
                    int i = startingChunk + (h * _maxChunksX) + w;
                    if (i >= 0 && i < chunks.Length)
                        visibleChunks.Add(chunks[i]);
                }
            }
            return visibleChunks.ToArray();
        }
    }
}
