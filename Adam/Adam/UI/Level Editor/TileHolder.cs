using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms;
using Adam.Levels;
using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timer = Adam.Misc.Timer;

namespace Adam.UI.Level_Editor
{
    internal class TileHolder
    {
        private const int SpacingBetweenSquareAndTile = 3;
        private Vector2 _positionRelativeToContainer;
        private readonly Tile _tile;
        public static Rectangle SourceRectangle = new Rectangle(297, 189, 22, 23);

        public Rectangle DrawRectangle = new Rectangle(0, 0, CalcHelper.ApplyUiRatio(SourceRectangle.Width),
            CalcHelper.ApplyUiRatio(SourceRectangle.Height));

        private readonly Color _hoveredColor = new Color(69, 96, 198);
        private readonly Color _altHoveredColor = Color.LightGray;

        public delegate void TileHandler(TileHolder tile);

        public event TileHandler WasClicked;
        public event TileHandler WasReleased;
        public event TileHandler WasReturned;
        private bool _isBeingMoved;
        private bool _isReturningToDefaultPos;
        private bool _isSteppingAside;
        public bool CanBeMoved { get; set; } = true;
        private Vector2 _mouseDifferential;
        private Vector2 _positionAtStartOfMovement;
        private Vector2 _containerPosition;
        private readonly Timer _movementTimer = new Timer();
        private Vector2 _tileTextureDifferential;

        public TileHolder(int id)
        {
            _tile = new Tile(true) {Id = (byte) id};
            _tile.DefineTexture();
            Size = SourceRectangle.Width;
            AdjustTileInside();

            WasClicked += TileHolder_WasClicked;
            WasReleased += TileHolder_WasReleased;
            WasReturned += TileHolder_WasReturned;
        }

        private void AdjustTileInside()
        {
            Rectangle tileSource = _tile.SourceRectangle;
            float hRatio = 1f, wRatio = 1f;
            if (tileSource.Width != 16 || tileSource.Height != 16)
            {
                bool widthIsBigger = tileSource.Width > tileSource.Height;
                bool sameSize = tileSource.Width == tileSource.Height;

                if (!sameSize)
                {
                    if (widthIsBigger)
                    {
                        hRatio = (float)tileSource.Height / (float)tileSource.Width;
                    }
                    else
                    {
                        wRatio = (float)tileSource.Width / (float)tileSource.Height;
                    }
                }

            }

            int width = CalcHelper.ApplyUiRatio((int)(16 * wRatio));
            int height = CalcHelper.ApplyUiRatio((int)(16 * hRatio));

            int widthDiff = (CalcHelper.ApplyUiRatio(16) - width) / 2;
            int heightDiff = (CalcHelper.ApplyUiRatio(16) - height) / 2;
            _tileTextureDifferential = new Vector2(widthDiff, heightDiff);
            _tile.DrawRectangle.Width = width;
            _tile.DrawRectangle.Height = height;
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
            if (CanBeMoved)
            {
                _isBeingMoved = true;
                Rectangle mouse = InputHelper.MouseRectangle;
                float x = mouse.X - Position.X;
                float y = mouse.Y - Position.Y;
                _mouseDifferential = new Vector2(x, y);
            }
        }

        /// <summary>
        /// Sets the position of the tileholder instantly.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetPosition(float x, float y)
        {
            DrawRectangle.X = (int)(x);
            DrawRectangle.Y = (int)(y);
            _tile.DrawRectangle.X = (int)(x + _tileTextureDifferential.X + CalcHelper.ApplyUiRatio(SpacingBetweenSquareAndTile));
            _tile.DrawRectangle.Y = (int)(y + _tileTextureDifferential.Y + CalcHelper.ApplyUiRatio(SpacingBetweenSquareAndTile));
        }

        /// <summary>
        /// Sets the x coordinate of the tileholder instantly.
        /// </summary>
        /// <param name="x"></param>
        public void SetX(float x)
        {
            SetPosition(x, DrawRectangle.Y);
        }

        /// <summary>
        /// Sets the y coordinate of the tileholder instantly.
        /// </summary>
        /// <param name="y"></param>
        public void SetY(float y)
        {
            SetPosition(DrawRectangle.X, y);

        }

        public void BindTo(Vector2 position)
        {
            float x = Math.Abs(position.X - DrawRectangle.X);
            float y = Math.Abs(position.Y - DrawRectangle.Y);
            _positionRelativeToContainer = new Vector2(x, y);
        }

        public void Update(Vector2 containerPos)
        {
            if (SlotRectangle == new Rectangle(0, 0, 0, 0))
            {
                SlotRectangle = DrawRectangle;
            }

            _containerPosition = containerPos;
            if (_isSteppingAside)
            {
                float deltaY = _positionRelativeToContainer.Y + containerPos.Y - DrawRectangle.Height -
                               CalcHelper.ApplyUiRatio(SpacingBetweenSquareAndTile) - _positionAtStartOfMovement.Y;
                SetY(CalcHelper.EaseInAndOut((float) _movementTimer.TimeElapsedInMilliSeconds,
                    _positionAtStartOfMovement.Y, deltaY, 50));

                if (DrawRectangle.Y == (int) (containerPos.Y + _positionRelativeToContainer.Y + deltaY))
                {
                    _isSteppingAside = false;
                }
            }
            else if (_isReturningToDefaultPos)
            {
                float deltaX = _positionRelativeToContainer.X + containerPos.X - _positionAtStartOfMovement.X;
                float deltaY = _positionRelativeToContainer.Y + containerPos.Y - _positionAtStartOfMovement.Y;

                SetX(CalcHelper.EaseInAndOut((float) _movementTimer.TimeElapsedInMilliSeconds,
                    _positionAtStartOfMovement.X, deltaX, 200));
                SetY(CalcHelper.EaseInAndOut((float) _movementTimer.TimeElapsedInMilliSeconds,
                    _positionAtStartOfMovement.Y, deltaY, 200));

                if (IsAtDefaultPosition())
                    WasReturned?.Invoke(this);
            }
            else
            {
                SetX(_positionRelativeToContainer.X + containerPos.X);
                SetY(_positionRelativeToContainer.Y + containerPos.Y);
                //}
                Update();
            }
        }

        public void Update()
        {
            if (_isBeingMoved)
            {
                Rectangle mouse = InputHelper.MouseRectangle;
                SetPosition(mouse.X - (int)_mouseDifferential.X, mouse.Y - (int)_mouseDifferential.Y);

                if (InputHelper.IsLeftMouseReleased())
                {
                    WasReleased?.Invoke(this);
                    HotBar.ReplaceHotBar(this);
                }
            }
        }

        /// <summary>
        /// Invokes click event if the tile is being clicked on.
        /// </summary>
        public void CheckIfClickedOn()
        {
            if (IsHovered() && InputHelper.IsLeftMousePressed())
            {
                WasClicked?.Invoke(this);
            }
        }

        /// <summary>
        /// Returns true if the tile is being hovered.
        /// </summary>
        /// <returns></returns>
        private bool IsHovered()
        {
            return InputHelper.MouseRectangle.Intersects(DrawRectangle);
        }

        /// <summary>
        /// Returns size of the square behind tiles.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Returns in Vector2 format the coordinates of the drawing rectangle.
        /// </summary>
        private Vector2 Position => new Vector2(DrawRectangle.X, DrawRectangle.Y);

        /// <summary>
        /// Returns the id of the tile the holder has.
        /// </summary>
        public byte Id => _tile.Id;

        /// <summary>
        /// The rectangle representing the space the tile is supposed to be occupying by default.
        /// </summary>
        public Rectangle SlotRectangle { get; private set; }

        /// <summary>
        /// Returns the draw rectangle since they are equal.
        /// </summary>
        public Rectangle CollRectangle => DrawRectangle;

        /// <summary>
        /// Returns true if the tile is intersecting with the middle of the slot of another tile.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public bool IsIntersectingWithSlotOf(TileHolder tile)
        {
            Rectangle r = new Rectangle(DrawRectangle.X + DrawRectangle.Width / 2 - 1, DrawRectangle.Y, 1,
                DrawRectangle.Height);
            return r.Intersects(tile.SlotRectangle);
        }

        /// <summary>
        /// Makes the tile step aside.
        /// </summary>
        public void StepAside()
        {
            _isSteppingAside = true;
            _isReturningToDefaultPos = false;
            _positionAtStartOfMovement = new Vector2(Position.X, Position.Y);
            _movementTimer.Reset();
        }

        /// <summary>
        /// Makes the tile return to its default position is it is not doing so already.
        /// </summary>
        public void ReturnToDefaultPosition()
        {
            if (!_isReturningToDefaultPos)
            {
                _isReturningToDefaultPos = true;
                _isSteppingAside = false;
                _positionAtStartOfMovement = new Vector2(Position.X, Position.Y);
                _movementTimer.Reset();
            }
        }

        /// <summary>
        /// Returns true if the tile is in the position it is supposed to be.
        /// </summary>
        /// <returns></returns>
        private bool IsAtDefaultPosition()
        {
            return (Math.Abs(DrawRectangle.X - (_positionRelativeToContainer.X + _containerPosition.X)) < 1) &&
                    (Math.Abs(DrawRectangle.Y - (_positionRelativeToContainer.Y + _containerPosition.Y)) < 1);
        }

        public void ChangeId(byte newId)
        {
            _tile.Id = newId;
            _tile.DefineTexture();
            _tile.DrawRectangle.Width = CalcHelper.ApplyUiRatio(16);
            _tile.DrawRectangle.Height = CalcHelper.ApplyUiRatio(16);
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            //if (Id != 0)
            {

                DrawSquareBehindTile(spriteBatch);
                _tile.DrawShadowVersion(spriteBatch);
                _tile.DrawByForce(spriteBatch);
            }
        }

        private void DrawSquareBehindTile(SpriteBatch spriteBatch)
        {
            Color squareColor = Color.White;
            if (_isBeingMoved || _isReturningToDefaultPos || _isSteppingAside)
            {
                squareColor = _hoveredColor;
            }
            if (!Inventory.IsMovingTile && IsHovered())
            {
                squareColor = _hoveredColor;
            }

            if (IsHovered() && !CanBeMoved)
            {
                squareColor = _altHoveredColor;
            }

            spriteBatch.Draw(GameWorld.UiSpriteSheet, DrawRectangle, SourceRectangle, squareColor);
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

    }
}
