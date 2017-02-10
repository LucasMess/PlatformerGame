using Adam.Levels;
using Adam.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Adam.UI.Level_Editor
{
    /// <summary>
    /// Used to keep useful tiles available one click away.
    /// </summary>
    static class HotBar
    {
        private const int NumberOfVisible = 8;
        private const int SpacingBetweenTiles = 2 * 2;
        private const int StartingX = 145 * 2;
        private const int StartingY = 8 * 2;
        private static List<TileHolder> _tileHolders = new List<TileHolder>();
        private static Rectangle _selectorSourceRect = new Rectangle(283, 142, 26, 26);
        private static Rectangle _selectorDrawRect;
        public static TileHolder SelectedTile { get; private set; }
        private static TileHolder _deletedTile = new TileHolder(0);
        private static SoundFx _replaceSound = new SoundFx("Sounds/Level Editor/replace_hotbar");
        private static SoundFx _swipeSound = new SoundFx("Sounds/Level Editor/swipe_hotbar");

        public static void Initialize()
        {
            for (int i = 0; i < 8; i++)
            {
                TileHolder tileHolder = new TileHolder(0);
                tileHolder.SetPosition(StartingX + (i * (tileHolder.Size + SpacingBetweenTiles)), StartingY);
                tileHolder.BindTo(new Vector2(0, 0));
                tileHolder.WasClicked += Tile_WasClicked;
                tileHolder.CanBeMoved = false;
                _tileHolders.Add(tileHolder);
            }

            _tileHolders[0].ChangeId(1);
            Tile_WasClicked(_tileHolders[0]);
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
            tile.LastTimeUsed.Reset();
        }


        public static void AddToHotBarFromWorld(byte id)
        {
            // Checks if already on hotbar.
            foreach (var tileHolder in _tileHolders)
            {
                if (tileHolder.Id == id)
                {
                    Tile_WasClicked(tileHolder);
                    return;
                }
            }


            // Checks for an empty slot, otherwise replaces least used tile.
            foreach (var tileHolder in _tileHolders)
            {
                if (tileHolder.Id == 0)
                {
                    ReplaceHotBarWithMiddleMouse(tileHolder, id);
                    Tile_WasClicked(tileHolder);
                    return;
                }
            }

            TileHolder tileHolderWithHighestTime = new TileHolder(0);
            foreach (var tileHolder in _tileHolders)
            {
                if (tileHolder.LastTimeUsed.TimeElapsedInMilliSeconds > tileHolderWithHighestTime.LastTimeUsed.TimeElapsedInMilliSeconds)
                {
                    tileHolderWithHighestTime = tileHolder;
                }
            }

            ReplaceHotBarWithMiddleMouse(tileHolderWithHighestTime, id);
            Tile_WasClicked(tileHolderWithHighestTime);

        }

        public static void ReplaceHotBarWithMiddleMouse(TileHolder tileHolder, byte newId)
        {
            _deletedTile = new TileHolder(tileHolder.Id);
            _deletedTile.SetPosition(tileHolder.GetPosition());
            _deletedTile.MoveTo(new Vector2(_deletedTile.GetPosition().X, -_deletedTile.DrawRectangle.Height), 200);

            tileHolder.ChangeId(newId);
            tileHolder.SetPosition(tileHolder.GetPosition().X, -tileHolder.DrawRectangle.Height);
            tileHolder.ReturnToDefaultPosition();

            _swipeSound.Play();
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
                    // It's all an optical illusion. The tile in the hot bar replaces the tile being moved, and a temporary tile is created to animate the deletion of the tile in the hotbar.

                    _deletedTile = new TileHolder(_tileHolders[i].Id);
                    _deletedTile.SetPosition(_tileHolders[i].GetPosition());
                    _deletedTile.MoveTo(new Vector2(_deletedTile.GetPosition().X, -_deletedTile.DrawRectangle.Height), 200);

                    _tileHolders[i].ChangeId(tileHolder.Id);
                    _tileHolders[i].SetPosition(tileHolder.GetPosition());
                    _tileHolders[i].ReturnToDefaultPosition();

                    tileHolder.SetPosition(AdamGame.UserResWidth, AdamGame.UserResHeight);
                    tileHolder.ReturnToDefaultPosition(500);

                    _swipeSound.Play();
                    _replaceSound.Play();
                    return;
                }
            }
            TileHolder.ReturnSound.Play();
        }

        public static void Update()
        {
            if (SelectedTile != null)
            {
                _selectorDrawRect = SelectedTile.CollRectangle;
                _selectorDrawRect.X -= 2 * 2;
                _selectorDrawRect.Y -= 2 * 2;
                _selectorDrawRect.Width = _selectorSourceRect.Width * 2;
                _selectorDrawRect.Height = _selectorSourceRect.Height * 2;
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

            _deletedTile.Draw(spriteBatch);

            spriteBatch.Draw(GameWorld.UiSpriteSheet, _selectorDrawRect, _selectorSourceRect, Color.White);

            foreach (var tileHolder in _tileHolders)
            {
                tileHolder.DrawToolTip(spriteBatch);
            }
        }
    }
}
