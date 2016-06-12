using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using Adam.Levels;
using Adam.Misc;
using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI.Level_Editor
{
    class TileHolder
    {
        private const int SpacingBetweenSquareAndTile = 3;
        private Vector2 _positionDifferential;
        private readonly Tile _tile;
        public static Rectangle SourceRectangle = new Rectangle(297, 189, 22, 23);
        private readonly Color _selectedColor = new Color(69, 96, 198);
        private readonly Texture2D _black = ContentHelper.LoadTexture("Tiles/black");
        private readonly SpriteFont _font = FontHelper.ChooseBestFont(CalcHelper.ApplyUiRatio(16));

        public delegate void TileHandler(TileHolder tile);
        public event TileHandler WasClicked;
        public event TileHandler WasReleased;
        private bool _isBeingMoved;
        private bool _isReturningToDefaultPos;
        private Vector2 _mouseDifferential;
        private Vector2 _positionAtStartOfMovement;
        private readonly Timer _movementTimer = new Timer();

        public TileHolder(int id)
        {
            _tile = new Tile(true) { Id = (byte)id };
            _tile.DefineTexture();
            Size = SourceRectangle.Width;

            _tile.DrawRectangle.Width = CalcHelper.ApplyUiRatio(16);
            _tile.DrawRectangle.Height = CalcHelper.ApplyUiRatio(16);

            WasClicked += TileHolder_WasClicked;
            WasReleased += TileHolder_WasReleased;
        }

        private void TileHolder_WasReleased(TileHolder tile)
        {
            _isBeingMoved = false;
            _isReturningToDefaultPos = true;
            _positionAtStartOfMovement = new Vector2(Position.X, Position.Y);
            _movementTimer.Reset();
        }

        private void TileHolder_WasClicked(TileHolder tile)
        {
            _isBeingMoved = true;
            Rectangle mouse = InputHelper.MouseRectangle;
            float x = mouse.X - Position.X;
            float y = mouse.Y - Position.Y;
            _mouseDifferential = new Vector2(x, y);
        }

        public void SetPosition(int x, int y)
        {
            _tile.DrawRectangle.X = x;
            _tile.DrawRectangle.Y = y;
        }

        public void BindTo(Vector2 position)
        {
            float x = Math.Abs(position.X - _tile.DrawRectangle.X);
            float y = Math.Abs(position.Y - _tile.DrawRectangle.Y);
            _positionDifferential = new Vector2(x, y);
        }

        public void Update(Vector2 position)
        {
            if (_isReturningToDefaultPos)
            {
                float deltaX = _positionDifferential.X + position.X - _positionAtStartOfMovement.X;
                float deltaY = _positionDifferential.Y + position.Y - _positionAtStartOfMovement.Y;

                _tile.DrawRectangle.X = (int)CalcHelper.EaseInAndOut((float)_movementTimer.TimeElapsedInMilliSeconds,
                    _positionAtStartOfMovement.X, deltaX, 200);
                _tile.DrawRectangle.Y = (int)CalcHelper.EaseInAndOut((float)_movementTimer.TimeElapsedInMilliSeconds,
                    _positionAtStartOfMovement.Y, deltaY, 200);

                if (_tile.DrawRectangle.X == (int)(_positionDifferential.X + position.X) &&
                    _tile.DrawRectangle.Y == (int)(_positionDifferential.Y + position.Y))
                    _isReturningToDefaultPos = false;
            }
            else
            {
                _tile.DrawRectangle.X = (int)(_positionDifferential.X + position.X);
                _tile.DrawRectangle.Y = (int)(_positionDifferential.Y + position.Y);
            }
            Update();
        }

        public void Update()
        {
            if (_isBeingMoved)
            {
                Rectangle mouse = InputHelper.MouseRectangle;
                SetPosition(mouse.X - (int)_mouseDifferential.X, mouse.Y - (int)_mouseDifferential.Y);

                if (InputHelper.IsLeftMouseReleased())
                    WasReleased?.Invoke(this);
            }
        }

        public void CheckIfClickedOn()
        {
            if (IsHovered())
            {
                WasClicked?.Invoke(this);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawSquareBehindTile(spriteBatch);
            _tile.DrawByForce(spriteBatch);
        }

        private void DrawSquareBehindTile(SpriteBatch spriteBatch)
        {
            Color squareColor = Color.White;
            if (_isBeingMoved || _isReturningToDefaultPos)
            {
                squareColor = _selectedColor;
            }
            if (!Inventory.IsMovingTile && InputHelper.MouseRectangle.Intersects(_tile.DrawRectangle))
            {
                squareColor = _selectedColor;
            }

            spriteBatch.Draw(GameWorld.UiSpriteSheet, new Rectangle(_tile.DrawRectangle.X - CalcHelper.ApplyUiRatio(SpacingBetweenSquareAndTile), _tile.DrawRectangle.Y - CalcHelper.ApplyUiRatio(SpacingBetweenSquareAndTile), CalcHelper.ApplyUiRatio(SourceRectangle.Width), CalcHelper.ApplyUiRatio(SourceRectangle.Height)), SourceRectangle, squareColor);
        }

        public void DrawToolTip(SpriteBatch spriteBatch)
        {
            if (IsHovered() && !_isReturningToDefaultPos)
            {
                string name = "Name not found";
                Tile.Names.TryGetValue(_tile.Id, out name);
                FontHelper.DrawTooltip(spriteBatch, name);
            }
        }

        private bool IsHovered()
        {
            return InputHelper.MouseRectangle.Intersects(_tile.DrawRectangle);
        }

        /// <summary>
        /// Returns size of the square behind tiles.
        /// </summary>
        public int Size { get; private set; }

        private Vector2 Position => new Vector2(_tile.DrawRectangle.X, _tile.DrawRectangle.Y);

        /// <summary>
        /// Returns the id of the tile the holder has.
        /// </summary>
        public byte Id => _tile.Id;
    }
}
