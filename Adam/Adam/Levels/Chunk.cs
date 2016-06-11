using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Levels
{
    /// <summary>
    /// Contains the tiles inside of it.
    /// </summary>
    public class Chunk
    {
        // At 48 pixels tilesize, 20x12 tiles are visible.

        public const int DefaultSize = 4;
        private int[] _indexes = new int[DefaultSize * DefaultSize];
        private int[] _indexesAround;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">index of this chunk.</param>
        public Chunk(int index)
        {
            Index = index;
        }

        public void SetData(int index, int tileIndex)
        {
            _indexes[index] = tileIndex;
        }

        public int[] GetTileIndexes()
        {
            return _indexes;
        }

        /// <summary>
        /// Calculates the tile indexes that need to be loaded when this chunk is active.
        /// </summary>
        /// <param name="chunks"></param>
        /// <param name="maxChunksX"></param>
        /// 
        public void StoreIndexesOfSurroundingChunks(Chunk[] chunks, int maxChunksX)
        {
            // Put the surrounding chunks in a list.
            List<Chunk> surroundingChunks = new List<Chunk>();
            int startingChunk = Index - maxChunksX * 2 - 3;
            int width = 12;
            int height = 7;

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    int current = startingChunk + (h * maxChunksX + w);
                    if (current >= 0 && current < chunks.Length)
                        surroundingChunks.Add(chunks[current]);
                }
            }

            // Make one array containing all indexes of surrounding chunks.
            int chunkCapacity = Chunk.DefaultSize * Chunk.DefaultSize;
            List<int> indexesSurroundingChunk = new List<int>();
            foreach (Chunk c in surroundingChunks)
            {
                int[] chunkIndexes = c.GetTileIndexes();
                foreach (int i in chunkIndexes)
                {
                    indexesSurroundingChunk.Add(i);
                }
            }
            _indexesAround = indexesSurroundingChunk.ToArray();
        }

        /// <summary>
        /// The index of this chunk in the chunk array.
        /// </summary>
        public int Index
        {
            get; set;
        }

        public int[] GetSurroundIndexes()
        {
            return _indexesAround;
        }
    }
}
