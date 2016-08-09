using Adam.Levels;
using Adam.Misc.Helpers;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Timer = Adam.Misc.Timer;

namespace Adam.UI.Level_Editor
{
    internal class TileHolder : UiElement
    {
        private const int SpacingBetweenSquareAndTile = 3;
        private Vector2 _positionRelativeToContainer;
        private readonly Tile _tile;
        public Timer LastTimeUsed { get; set; } = new Timer(true);
        public static Rectangle SourceRectangle = new Rectangle(297, 189, 22, 23);

        private readonly Color _hoveredColor = new Color(69, 96, 198);
        private readonly Color _altHoveredColor = Color.LightGray;

        public delegate void TileHandler(TileHolder tile);

        public event TileHandler WasClicked;
        public event TileHandler WasReleased;
        private bool _isBeingMoved;
        private bool _isReturningToDefaultPos;
        private bool _isSteppingAside;
        public bool CanBeMoved { get; set; } = true;
        private Vector2 _mouseDifferential;
        private Vector2 _positionAtStartOfMovement;
        private Vector2 _containerPosition;
        private Vector2 _tileTextureDifferential;

        public TileHolder(int id)
        {
            DrawRectangle = new Rectangle(0, 0, CalcHelper.ApplyUiRatio(SourceRectangle.Width),
            CalcHelper.ApplyUiRatio(SourceRectangle.Height));
            _tile = new Tile(true) { Id = (byte)id };
            _tile.DefineTexture();
            Size = SourceRectangle.Width;
            AdjustTileInside();

            WasClicked += TileHolder_WasClicked;
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
        public override void SetPosition(float x, float y)
        {
            UpdateTilePosition(x, y);
            base.SetPosition(x, y);
        }

        private void UpdateTilePosition(float x, float y)
        {
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
            _tile.Update();

            if (SlotRectangle == new Rectangle(0, 0, 0, 0))
            {
                SlotRectangle = DrawRectangle;
            }

            _containerPosition = containerPos;

            if (!_isBeingMoved && !_isReturningToDefaultPos && !_isSteppingAside)
            {
                SetPosition(DefaultPosition);
            }

            if (_isReturningToDefaultPos)
            {
                if (!IsMovingToNewPosition)
                    _isReturningToDefaultPos = false;
            }


            if (_isBeingMoved)
            {
                Inventory.IsMovingTile = true;
                Rectangle mouse = InputHelper.MouseRectangle;
                SetPosition(mouse.X - (int)_mouseDifferential.X, mouse.Y - (int)_mouseDifferential.Y);

                if (InputHelper.IsLeftMouseReleased())
                {
                    ReturnToDefaultPosition();
                    _isBeingMoved = false;
                    Inventory.IsMovingTile = false;
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
            MoveTo(StepAsidePosition, 50);
            _isSteppingAside = true;
        }

        private Vector2 StepAsidePosition
            => new Vector2(_containerPosition.X +_positionRelativeToContainer.X, _positionRelativeToContainer.Y + _containerPosition.Y - DrawRectangle.Height / 2);

        /// <summary>
        /// Makes the tile return to its default position if it is not doing so already.
        /// </summary>
        public void ReturnToDefaultPosition(int duration = 100)
        {
            _isReturningToDefaultPos = true;
            _isSteppingAside = false;
            MoveTo(DefaultPosition, duration);
        }

        private Vector2 DefaultPosition => new Vector2(_containerPosition.X + _positionRelativeToContainer.X, _containerPosition.Y + _positionRelativeToContainer.Y);

        public void ChangeId(byte newId)
        {
            _tile.Destroy();
            _tile.Id = newId;
            _tile.DefineTexture();
            AdjustTileInside();
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            UpdateTilePosition(DrawRectangle.X, DrawRectangle.Y);
            if (Id != 0)
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
