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
        private Rectangle _squareRectangle;
        private readonly Color _selectedColor = new Color(69, 96, 198);
        private readonly Texture2D _black = ContentHelper.LoadTexture("Tiles/black");
        private readonly SpriteFont _font = FontHelper.ChooseBestFont(CalcHelper.ApplyUiRatio(16));

        public delegate void TileHandler(TileHolder tile);
        public event TileHandler WasClicked;
        public event TileHandler WasReleased;
        public event TileHandler WasReturned;
        private bool _isBeingMoved;
        private bool _isReturningToDefaultPos;
        private bool _isSteppingAside;
        private Vector2 _mouseDifferential;
        private Vector2 _positionAtStartOfMovement;
        private Vector2 _containerPosition;
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
            WasReturned += TileHolder_WasReturned;
        }

        private void TileHolder_WasReturned(TileHolder tile)
        {
            _isReturningToDefaultPos = false;
        }

        private void TileHolder_WasReleased(TileHolder tile)
        {
            _isBeingMoved = false;
            ReturnToDefaultPosition();
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

        public void Update(Vector2 containerPos)
        {
            if (SlotRectangle == new Rectangle(0,0,0,0))
            {
                SlotRectangle = _squareRectangle;
            }

            _containerPosition = containerPos;
            if (_isSteppingAside)
            {
                float deltaY = _positionDifferential.Y + containerPos.Y - _squareRectangle.Height - CalcHelper.ApplyUiRatio(SpacingBetweenSquareAndTile) - _positionAtStartOfMovement.Y;
                _tile.DrawRectangle.Y = (int)CalcHelper.EaseInAndOut((float)_movementTimer.TimeElapsedInMilliSeconds,
                   _positionAtStartOfMovement.Y, deltaY, 50);

                if (_tile.DrawRectangle.Y == (int) (containerPos.Y + _positionDifferential.Y + deltaY))
                {
                    _isSteppingAside = false;
                }
            }
            else if (_isReturningToDefaultPos)
            {
                float deltaX = _positionDifferential.X + containerPos.X - _positionAtStartOfMovement.X;
                float deltaY = _positionDifferential.Y + containerPos.Y - _positionAtStartOfMovement.Y;

                _tile.DrawRectangle.X = (int)CalcHelper.EaseInAndOut((float)_movementTimer.TimeElapsedInMilliSeconds,
                    _positionAtStartOfMovement.X, deltaX, 200);
                _tile.DrawRectangle.Y = (int)CalcHelper.EaseInAndOut((float)_movementTimer.TimeElapsedInMilliSeconds,
                    _positionAtStartOfMovement.Y, deltaY, 200);

                if (IsAtDefaultPosition())
                    WasReturned?.Invoke(this);
            }
            else
            {
                _tile.DrawRectangle.X = (int)(_positionDifferential.X + containerPos.X);
                _tile.DrawRectangle.Y = (int)(_positionDifferential.Y + containerPos.Y);
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

            _squareRectangle = new Rectangle(_tile.DrawRectangle.X - CalcHelper.ApplyUiRatio(SpacingBetweenSquareAndTile), _tile.DrawRectangle.Y - CalcHelper.ApplyUiRatio(SpacingBetweenSquareAndTile), CalcHelper.ApplyUiRatio(SourceRectangle.Width), CalcHelper.ApplyUiRatio(SourceRectangle.Height));
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
            if (_isBeingMoved || _isReturningToDefaultPos || _isSteppingAside)
            {
                squareColor = _selectedColor;
            }
            if (!Inventory.IsMovingTile && IsHovered())
            {
                squareColor = _selectedColor;
            }


            spriteBatch.Draw(GameWorld.UiSpriteSheet, _squareRectangle, SourceRectangle, squareColor);
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
            return InputHelper.MouseRectangle.Intersects(_squareRectangle);
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

        public Rectangle SlotRectangle { get; private set; }
        public Rectangle CollRectangle => _squareRectangle;

        public bool IsIntersectingWithSlotOf(TileHolder tile)
        {
            Rectangle r = new Rectangle(_squareRectangle.X + _squareRectangle.Width/2 - 1, _squareRectangle.Y, 1,
                _squareRectangle.Height);
            return r.Intersects(tile.SlotRectangle);
        }

        public void StepAside()
        {
            _isSteppingAside = true;
            _isReturningToDefaultPos = false;
            _positionAtStartOfMovement = new Vector2(Position.X, Position.Y);
            _movementTimer.Reset();
        }

        public void ReturnToDefaultPosition()
        {
            if (!IsAtDefaultPosition() && !_isReturningToDefaultPos)
            {
                _isReturningToDefaultPos = true;
                _isSteppingAside = false;
                _positionAtStartOfMovement = new Vector2(Position.X, Position.Y);
                _movementTimer.Reset();
            }
        }

        private bool IsAtDefaultPosition()
        {
            return (_tile.DrawRectangle.X == (int)(_positionDifferential.X + _containerPosition.X) &&
                    _tile.DrawRectangle.Y == (int)(_positionDifferential.Y + _containerPosition.Y));
        }
    }
}
