using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using Adam.Levels;
using Adam.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI.Level_Editor
{
    /// <summary>
    /// Contains all tiles and entities in grid format, and also contains a hotbar for quick selection of items.
    /// </summary>
    class Inventory
    {
        private const int SpacingBetweenTiles = 2;
        private const int defaultX = 159;
        private const int defaultY = 51;
        private int _activeY;
        private int _inactiveY;
        private static int posAtStartOfAnimation;
        private int _midway;
        private const int tilesPerRow = 9;

        private const float Acceleration = 13f;
        private const float Deceleration = 13f;
        private static Timer _animationTimer = new Timer();

        /// <summary>
        /// Returns true if the inventory is visible and active.
        /// </summary>
        public static bool IsOpen { get; set; }

        private List<TileHolder> _tileHolders = new List<TileHolder>();
        private static Rectangle _backDrop;
        private static float _velocityY;
        private Rectangle _backDropSource = new Rectangle(0, 252, 305, 205);

        public Inventory()
        {
            _backDrop = new Rectangle(CalcHelper.ApplyUiRatio(87), CalcHelper.ApplyUiRatio(38), CalcHelper.ApplyUiRatio(305), CalcHelper.ApplyUiRatio(204));
            _inactiveY = _backDrop.Y - _backDrop.Height;
            _activeY = _backDrop.Y;
            _midway = _inactiveY + _backDrop.Height/2;
            posAtStartOfAnimation = _backDrop.Y;

            for (int i = 1; i < 60; i++)
            {
                _tileHolders.Add(new TileHolder(i));
            }

            int counter = 0;
            foreach (var tile in _tileHolders)
            {
                int x = CalcHelper.ApplyUiRatio(defaultX) +
                        (counter % tilesPerRow) * CalcHelper.ApplyUiRatio(TileHolder.SourceRectangle.Width + SpacingBetweenTiles);
                int y = CalcHelper.ApplyUiRatio(defaultY) +
                        (counter / tilesPerRow) * CalcHelper.ApplyUiRatio(TileHolder.SourceRectangle.Height + SpacingBetweenTiles);

                tile.SetPosition(x, y);
                tile.BindTo(new Vector2(_backDrop.X, _backDrop.Y));

                counter++;
            }

        }

        public static void StartAnimation()
        {
            IsOpen = !IsOpen;
            _animationTimer.Reset();
            posAtStartOfAnimation = _backDrop.Y;
        }

        private void Animate()
        {
            if (IsOpen)
            {
                _backDrop.Y =
                    (int)
                        CalcHelper.EaseInAndOut((int) _animationTimer.TimeElapsedInMilliSeconds, posAtStartOfAnimation,
                            Math.Abs(posAtStartOfAnimation - _activeY), 200);
                // CalcHelper.SharpAnimationY(_inactiveY, _activeY, ref _backDrop, ref _velocityY);
                //if (_backDrop.Y < _activeY)
                //{
                //    if (_backDrop.Y < _midway)
                //        _velocityY += Acceleration;
                //    else _velocityY -= Deceleration;
                //}
                //else
                //{
                //    _velocityY = 0;
                //    _backDrop.Y = _activeY;
                //}
            }
            else
            {
                _backDrop.Y =
                    (int)
                        CalcHelper.EaseInAndOut((int)_animationTimer.TimeElapsedInMilliSeconds, posAtStartOfAnimation,
                            -Math.Abs(posAtStartOfAnimation - _inactiveY), 200);
                //CalcHelper.SharpAnimationY(_activeY, _inactiveY, ref _backDrop, ref _velocityY);

                //if (_backDrop.Y > _inactiveY)
                //{
                //    if (_backDrop.Y > _midway)
                //        _velocityY -= Acceleration;
                //    else _velocityY += Deceleration;
                //}
                //else
                //{
                //    _velocityY = 0;
                //    _backDrop.Y = _inactiveY;
                //}
            }
        }

        public void Update()
        {
            Animate();
            _backDrop.Y += (int)_velocityY;

            foreach (var tile in _tileHolders)
            {
                tile.Update(new Vector2(_backDrop.X, _backDrop.Y));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameWorld.UiSpriteSheet, _backDrop, _backDropSource, Color.White);

            foreach (var tile in _tileHolders)
            {
                tile.Draw(spriteBatch);
            }
        }




    }
}
