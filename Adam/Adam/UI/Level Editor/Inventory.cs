using System;
using System.Collections.Generic;
using Adam.Levels;
using Adam.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Adam.UI.Level_Editor
{
    /// <summary>
    ///     Contains all tiles and entities in grid format.
    /// </summary>
    internal class Inventory
    {
        private const int SpacingBetweenTiles = 2;

        // The starting coordinates for the first tile in the grid.
        private const int DefaultX = 159;
        private const int DefaultY = 51;

        private const int TilesPerRow = 9;

        // This is used to keep track of where the backdrop was when the animation started.
        private static int _posAtStartOfAnimation;

        private static readonly Timer AnimationTimer = new Timer();
        private static Rectangle _backDrop;
        private readonly Rectangle _backDropSource = new Rectangle(0, 252, 305, 205);

        // The position the backdrop should be in when open or closed.
        private readonly int _activeY;
        private readonly int _inactiveY;

        private readonly List<TileHolder> _tileHolders = new List<TileHolder>();
        private TileHolder _tileBeingMoved = new TileHolder(0);
        public static bool IsMovingTile { get; private set; }

        public Inventory()
        {
            _backDrop = new Rectangle(CalcHelper.ApplyUiRatio(87), CalcHelper.ApplyUiRatio(38),
                CalcHelper.ApplyUiRatio(305), CalcHelper.ApplyUiRatio(204));
            _inactiveY = _backDrop.Y - _backDrop.Height;
            _activeY = _backDrop.Y;
            _posAtStartOfAnimation = _backDrop.Y;

            // Testing with every tile.
            for (var i = 1; i < 60; i++)
            {
                _tileHolders.Add(new TileHolder(i));
            }

            // Places the tile holders in their proper positions in the grid.
            var counter = 0;
            foreach (var tile in _tileHolders)
            {
                var x = CalcHelper.ApplyUiRatio(DefaultX) +
                        (counter % TilesPerRow) *
                        CalcHelper.ApplyUiRatio(TileHolder.SourceRectangle.Width + SpacingBetweenTiles);
                var y = CalcHelper.ApplyUiRatio(DefaultY) +
                        (counter / TilesPerRow) *
                        CalcHelper.ApplyUiRatio(TileHolder.SourceRectangle.Height + SpacingBetweenTiles);

                tile.SetPosition(x, y);
                tile.BindTo(new Vector2(_backDrop.X, _backDrop.Y));
                tile.WasClicked += OnTileClicked;
                tile.WasReleased += OnTileReleased;
                counter++;
            }
        }

        /// <summary>
        ///     Returns true if the inventory is visible and active.
        /// </summary>
        public static bool IsOpen { get; private set; }

        /// <summary>
        /// Changes the state of the inventory and triggers the animation to start.
        /// </summary>
        public static void StartAnimation()
        {
            IsOpen = !IsOpen;
            AnimationTimer.Reset();
            _posAtStartOfAnimation = _backDrop.Y;
        }

        /// <summary>
        /// Provides animation for the backdrop and the tiles depending on whether the inventory is open or not.
        /// </summary>
        private void Animate()
        {
            if (IsOpen)
            {
                _backDrop.Y =
                    (int)
                        CalcHelper.EaseInAndOut((int)AnimationTimer.TimeElapsedInMilliSeconds, _posAtStartOfAnimation,
                            Math.Abs(_posAtStartOfAnimation - _activeY), 200);
            }
            else
            {
                _backDrop.Y =
                    (int)
                        CalcHelper.EaseInAndOut((int)AnimationTimer.TimeElapsedInMilliSeconds, _posAtStartOfAnimation,
                            -Math.Abs(_posAtStartOfAnimation - _inactiveY), 200);
            }
        }

        /// <summary>
        /// Returns the space occupied by this element where it cannot be clicked through.
        /// </summary>
        /// <returns></returns>
        public Rectangle GetCollRectangle()
        {
            return _backDrop;
        }

        /// <summary>
        /// Updates animations and tiles.
        /// </summary>
        public void Update()
        {
            Animate();
            foreach (var tile in _tileHolders)
            {
                tile.Update(new Vector2(_backDrop.X, _backDrop.Y));
            }

            if (InputHelper.IsLeftMousePressed() && !IsMovingTile)
            {
                foreach (var tile in _tileHolders)
                {
                    tile.CheckIfClickedOn();
                }
            }
        }

        private void OnTileClicked(TileHolder tile)
        {
            IsMovingTile = true;
            _tileBeingMoved = tile;
        }

        private void OnTileReleased(TileHolder tile)
        {
            IsMovingTile = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameWorld.UiSpriteSheet, _backDrop, _backDropSource, Color.White);

            foreach (var tile in _tileHolders)
            {
                if (tile != _tileBeingMoved)
                    tile.Draw(spriteBatch);
            }

            _tileBeingMoved.Draw(spriteBatch);

            foreach (var tile in _tileHolders)
            {
                if (!IsMovingTile)
                    tile.DrawToolTip(spriteBatch);
            }
        }
    }
}