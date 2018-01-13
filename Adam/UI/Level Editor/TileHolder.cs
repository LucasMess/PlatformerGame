using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Misc.Helpers;
using ThereMustBeAnotherWay.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static ThereMustBeAnotherWay.TMBAW_Game;
using GameTimer = ThereMustBeAnotherWay.Misc.GameTimer;

namespace ThereMustBeAnotherWay.UI.Level_Editor
{
    internal class TileHolder : UiElement
    {
        private const int SpacingBetweenSquareAndTile = 3 * 2;
        private Vector2 _positionRelativeToContainer;
        private Tile _tile;
        public GameTimer LastTimeUsed { get; set; } = new GameTimer(true);
        public static Rectangle SourceRectangle = new Rectangle(297, 189, 22, 23);

        private readonly Color _hoveredColor = new Color(69, 96, 198);
        private readonly Color _altHoveredColor = Color.LightGray;

        public delegate void TileHandler(TileHolder tile);

        public event TileHandler WasClicked;
        public event TileHandler WasReleased;
        private bool _isBeingMoved;
        private bool _isReturningToDefaultPos;
        private bool _isSteppingAside;
        private bool _wasMouseReleased;
        public bool CanBeMoved { get; set; } = true;
        private Vector2 _mouseDifferential;
        private Vector2 _containerPosition;
        private Vector2 _tileTextureDifferential;
        private static SoundFx _pickUpSound = new SoundFx("Sounds/Level Editor/pickup_tileholder");
        public static SoundFx ReturnSound = new SoundFx("Sounds/Level Editor/return_tileholder");

        public TileHolder(TileType id)
        {
            DrawRectangle = new Rectangle(0, 0, (SourceRectangle.Width) * 2,
            SourceRectangle.Height * 2);
            _tile = new Tile(true) { Id = id };
            _tile.DefineTexture();
            Size = SourceRectangle.Width * 2;
            AdjustTileInside();

            WasClicked += TileHolder_WasClicked;
            WasReleased += TileHolder_WasReleased;
        }

        private void TileHolder_WasReleased(TileHolder tile)
        {

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

            int width = ((int)(16 * wRatio));
            int height = ((int)(16 * hRatio));

            int widthDiff = ((16) - width) / 2;
            int heightDiff = ((16) - height) / 2;
            _tileTextureDifferential = new Vector2(widthDiff, heightDiff);
            _tile.DrawRectangle.Width = width * 2;
            _tile.DrawRectangle.Height = height * 2;
        }

        private void TileHolder_WasClicked(TileHolder tile)
        {
            if (CanBeMoved && _wasMouseReleased)
            {
                _pickUpSound.Play();
                _isBeingMoved = true;
                _wasMouseReleased = false;
                Rectangle mouse = InputHelper.GetMouseInUi();
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
            _tile.DrawRectangle.X = (int)(x + _tileTextureDifferential.X + (SpacingBetweenSquareAndTile));
            _tile.DrawRectangle.Y = (int)(y + _tileTextureDifferential.Y + (SpacingBetweenSquareAndTile));
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
                Rectangle mouse = InputHelper.GetMouseInUi();
                SetPosition(mouse.X - (int)_mouseDifferential.X, mouse.Y - (int)_mouseDifferential.Y);

                if (InputHelper.IsLeftMousePressed() && _wasMouseReleased)
                {
                    _wasMouseReleased = false;
                    ReturnToDefaultPosition();
                    _isBeingMoved = false;
                    Inventory.IsMovingTile = false;
                    HotBar.ReplaceHotBar(this);
                    WasReleased?.Invoke(this);
                }
            }

            if (InputHelper.IsLeftMouseReleased())
                _wasMouseReleased = true;
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
            return InputHelper.GetMouseInUi().Intersects(DrawRectangle);
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
        public TileType Id => _tile.Id;

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
            => new Vector2(_containerPosition.X + _positionRelativeToContainer.X, _positionRelativeToContainer.Y + _containerPosition.Y - DrawRectangle.Height / 2);

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

        public void ChangeId(TileType newId)
        {
            _tile.ResetToDefault();
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
                name = _tile.Id.ToString();
                FontHelper.DrawTooltip(spriteBatch, name);
            }
        }

    }
}
