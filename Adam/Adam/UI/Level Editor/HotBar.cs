using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI.Level_Editor
{
    /// <summary>
    /// Used to keep useful tiles available one click away.
    /// </summary>
    class HotBar
    {
        private const int NumberOfVisible = 8;
        private const int SpacingBetweenTiles = 2;
        private const int StartingX = 148;
        private const int StartingY = 11;
        private readonly List<TileHolder> _tiles = new List<TileHolder>();

        public HotBar()
        {
            for (int i = 0; i < 8; i++)
            {
                TileHolder tile = new TileHolder(0);
                tile.SetPosition(CalcHelper.ApplyUiRatio(StartingX + (i * (tile.Size + SpacingBetweenTiles))),CalcHelper.ApplyUiRatio(StartingY));
                _tiles.Add(tile);
            }
        }

        public void Update()
        {
            foreach (var tileHolder in _tiles)
            {
                tileHolder.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var tileHolder in _tiles)
            {
                tileHolder.Draw(spriteBatch);
            }

            foreach (var tileHolder in _tiles)
            {
                tileHolder.DrawToolTip(spriteBatch);
            }
        }
    }
}
