using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
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
        private const int StartingX = 145;
        private const int StartingY = 8;
        private static List<TileHolder> _tileHolders = new List<TileHolder>();
        private static Rectangle _selectorSourceRect = new Rectangle(283, 142, 26, 26);
        private static Rectangle _selectorDrawRect;
        public static TileHolder SelectedTile { get; private set; }


        public static void Initialize()
        {
            for (int i = 0; i < 8; i++)
            {
                TileHolder tileHolder = new TileHolder(0);
                tileHolder.SetPosition(CalcHelper.ApplyUiRatio(StartingX + (i * (tileHolder.Size + SpacingBetweenTiles))), CalcHelper.ApplyUiRatio(StartingY));
                tileHolder.BindTo(new Vector2(0, 0));
                tileHolder.WasClicked += Tile_WasClicked;
                tileHolder.CanBeMoved = false;
                _tileHolders.Add(tileHolder);
            }
        }

        /// <summary>
        /// Change brush to clicked tile.
        /// </summary>
        /// <param name="tile"></param>
        private static void Tile_WasClicked(TileHolder tile)
        {
            if (tile.Id == 0)
                return;

            SelectedTile = tile;
            LevelEditor.SelectedId = tile.Id;
        }

        /// <summary>
        /// Replace the tile in the hot bar with the tile hovered above it.
        /// </summary>
        /// <param name="tileHolder"></param>
        public static void ReplaceHotBar(TileHolder tileHolder)
        {
            // Check for a tile of the same id already on the hotbar and delete it.
            for (int i = 0; i < _tileHolders.Count; i++)
            {
                if (tileHolder.Id == _tileHolders[i].Id)
                {
                    _tileHolders[i].ChangeId(0);
                }
            }

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
            if (SelectedTile != null)
            {
                _selectorDrawRect = SelectedTile.CollRectangle;
                _selectorDrawRect.X -= CalcHelper.ApplyUiRatio(2);
                _selectorDrawRect.Y -= CalcHelper.ApplyUiRatio(2);
                _selectorDrawRect.Width = CalcHelper.ApplyUiRatio(_selectorSourceRect.Width);
                _selectorDrawRect.Height = CalcHelper.ApplyUiRatio(_selectorSourceRect.Height);
            }


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

            spriteBatch.Draw(GameWorld.UiSpriteSheet, _selectorDrawRect, _selectorSourceRect, Color.White);

            foreach (var tileHolder in _tileHolders)
            {
                tileHolder.DrawToolTip(spriteBatch);
            }
        }
    }
}
