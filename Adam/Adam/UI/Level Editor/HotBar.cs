using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI.Level_Editor
{
    /// <summary>
    /// Used to keep useful tiles available one click away.
    /// </summary>
    static class HotBar
    {
        private const int NumberOfVisible = 8;
        private const int SpacingBetweenTiles = 2;
        private const int StartingX = 148;
        private const int StartingY = 11;
        private static List<TileHolder> _tileHolders = new List<TileHolder>();

        public static void Initialize()
        {
            for (int i = 0; i < 8; i++)
            {
                TileHolder tile = new TileHolder(0);
                tile.SetPosition(CalcHelper.ApplyUiRatio(StartingX + (i * (tile.Size + SpacingBetweenTiles))),CalcHelper.ApplyUiRatio(StartingY));
                tile.BindTo(new Vector2(0,0));
                tile.WasClicked += Tile_WasClicked;
                _tileHolders.Add(tile);
            }
        }

        /// <summary>
        /// Change brush to clicked tile.
        /// </summary>
        /// <param name="tile"></param>
        private static void Tile_WasClicked(TileHolder tile)
        {
            LevelEditor.SelectedId = tile.Id;
        }

        /// <summary>
        /// Replace the tile in the hot bar with the tile hovered above it.
        /// </summary>
        /// <param name="tileHolder"></param>
        public static void ReplaceHotBar(TileHolder tileHolder)
        {
            for (int i = 0; i < _tileHolders.Count; i++)
            {
                if (tileHolder.IsIntersectingWithSlotOf(_tileHolders[i]))
                {
                    _tileHolders[i].ChangeId(tileHolder.Id);
                    break;
                }
            }
        }

        public static void Update()
        {
            foreach (var tileHolder in _tileHolders)
            {
                if (Inventory.IsOpen && Inventory.TileBeingMoved.IsIntersectingWithSlotOf(tileHolder))
                {
                    tileHolder.StepAside();
                }
                else
                {
                    tileHolder.ReturnToDefaultPosition();
                }

                tileHolder.CheckIfClickedOn();
                tileHolder.Update(new Vector2(0, 0));
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var tileHolder in _tileHolders)
            {
                tileHolder.Draw(spriteBatch);
            }

            foreach (var tileHolder in _tileHolders)
            {
                tileHolder.DrawToolTip(spriteBatch);
            }
        }
    }
}
