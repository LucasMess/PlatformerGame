using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
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
                tile.BindTo(new Vector2(0,0));
                _tiles.Add(tile);
            }

            Inventory.TileBeingMoved.WasReleased += ReplaceHotBar;
        }

        private void ReplaceHotBar(TileHolder tile)
        {
            foreach (var tileHolder in _tiles)
            {
                if (tile.IsIntersectingWithSlotOf(tileHolder))
                {
                    
                }
            }
        }

        public void Update()
        {
            foreach (var tileHolder in _tiles)
            {
                tileHolder.Update(new Vector2(0,0));
                if (Inventory.TileBeingMoved.IsIntersectingWithSlotOf(tileHolder))
                {
                    tileHolder.StepAside();
                }
                else
                {
                    tileHolder.ReturnToDefaultPosition();
                }
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
