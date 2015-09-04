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
        public const int DefaultSize = 64;
        private int[] indexes = new int[DefaultSize * DefaultSize];

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
            indexes[index] = tileIndex;
        }

        public int[] GetTileIndexes()
        {
            return indexes;
        }

        /// <summary>
        /// The index of this chunk in the chunk array.
        /// </summary>
        public int Index
        {
            get; set;
        }
    }
}
