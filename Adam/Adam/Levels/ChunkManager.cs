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
        Chunk[] _chunks;
        Chunk _activeChunk;
        private int _maxChunksX;
        private int _maxChunksY;
        private int _worldWidth;
        private int _worldHeight;

        public ChunkManager()
        {

        }

        public void ConvertToChunks(int worldWidth, int worldHeight)
        {
            // Validate data.
            if (worldWidth % Chunk.DefaultSize != 0 || worldHeight % Chunk.DefaultSize != 0)
            {
                throw new ArgumentException("This level file version is no longer supported. It has dimensions that cannot be divided into chunks.");
            }

            this._worldHeight = worldHeight;
            this._worldWidth = worldWidth;

            // Calculates the amount of chunks that need to be created.
            _maxChunksX = worldWidth / Chunk.DefaultSize;
            _maxChunksY = worldHeight / Chunk.DefaultSize;

            _chunks = new Chunk[_maxChunksX * _maxChunksY];

            for (int i = 0; i < _chunks.Length; i++)
            {
                _chunks[i] = new Chunk(i);
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
                            int currentTileInArray = (tileY * _worldWidth) + (tileX) + (chunkX * Chunk.DefaultSize) + (Chunk.DefaultSize * chunkY * _worldWidth);
                            int currentChunkInArray = chunkY * _maxChunksY + chunkX;
                            int currentTileInChunk = tileY * Chunk.DefaultSize + tileX;

                            _chunks[currentChunkInArray].SetData(currentTileInChunk, currentTileInArray);

                        }
                    }
                }
            }

            PreCalculateIndexes();
        }

        private void PreCalculateIndexes()
        {
            foreach (Chunk c in _chunks)
            {
                c.StoreIndexesOfSurroundingChunks(_chunks, _maxChunksX);
            }
        }

        /// <summary>
        /// Gathers all the indexes of the visible chunks around the camera.
        /// </summary>
        /// <returns>Visible indexes of tiles in chunks.</returns>
        public int[] GetVisibleIndexes()
        {
            Camera camera = GameWorld.Instance.Camera;
            if (camera == null)
                return new int[0];

            _activeChunk = GetChunk((int)camera.InvertedCoords.X, (int)camera.InvertedCoords.Y);
            // Chunk activeChunk = GetChunk(128 * Main.Tilesize, 128 * Main.Tilesize);
            return _activeChunk.GetSurroundIndexes();
        }

        /// <summary>
        /// Calculates the chunk at the specified location.
        /// </summary>
        /// <param name="x">X-coordinate.</param>
        /// <param name="y">Y-coordinate.</param>
        /// <returns>Chunk at location.</returns>
        public Chunk GetChunk(int x, int y)
        {
            int size = Chunk.DefaultSize * Main.Tilesize;
            int chunkIndex = (y / size) * _maxChunksX + (x / size);
            if (chunkIndex >= 0 && chunkIndex < _chunks.Length)
            {
                return _chunks[chunkIndex];
            }
            else return _chunks[0];
        }

        /// <summary>
        /// Calculates the chunks that are visible by the player.
        /// </summary>
        /// <returns>Visible chunks.</returns>
        private Chunk[] GetVisibleChunks()
        {
            if (GameWorld.Instance.Player == null || _chunks == null || GameWorld.Instance.Camera == null) return new Chunk[0];

            // Gets the chunk the camera is in.

            int cameraChunk = GetChunk((int)GameWorld.Instance.Camera.LastCameraLeftCorner.X, (int)GameWorld.Instance.Camera.LastCameraLeftCorner.Y).Index;

            // Defines how many chunks are visible in either direction.
            int visibleChunksY = (int)(3 / GameWorld.Instance.Camera.GetZoom());
            int visibleChunksX = (int)(3 / GameWorld.Instance.Camera.GetZoom());

            // Finds where the top left visible chunk is.
            int startingChunk = cameraChunk - (int)Math.Ceiling((double)(visibleChunksX / 2)) - 1;


            // Makes an array of visible chunks.
            List<Chunk> visibleChunks = new List<Chunk>();
            for (int h = 0; h < visibleChunksY; h++)
            {
                for (int w = 0; w < visibleChunksX; w++)
                {
                    int i = startingChunk + (h * _maxChunksX) + w;
                    if (i >= 0 && i < _chunks.Length)
                        visibleChunks.Add(_chunks[i]);
                }
            }
            return visibleChunks.ToArray();
        }

        public int GetNumberOfChunks()
        {
            if (_chunks == null)
                return 0;
            else return _chunks.Length;
        }

        public int GetActiveChunkIndex()
        {
            if (_activeChunk == null)
                return Int32.MaxValue;
            else return _activeChunk.Index;
        }
    }
}
