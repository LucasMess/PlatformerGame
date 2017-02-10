using Adam.Levels;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Timer = Adam.Misc.Timer;

namespace Adam.UI.Level_Editor
{
    /// <summary>
    ///     Contains all tiles and entities in grid format.
    /// </summary>
    class Inventory
    {
        public enum Category
        {
            Building, Wall, Special, Objects, Characters
        }
        public static Category CurrentCategory { get; private set; } = Category.Building;

        private const int SpacingBetweenTiles = 2 * 2;

        // The starting coordinates for the first tile in the grid.
        private const int DefaultX = 155 * 2;
        private const int DefaultY = 47 * 2;

        private const int TilesPerRow = 9;

        // This is used to keep track of where the backdrop was when the animation started.
        private static int _posAtStartOfAnimation;

        private static readonly Timer AnimationTimer = new Timer(true);
        private static Rectangle _backDrop;
        private readonly Rectangle _backDropSource = new Rectangle(0, 252, 305, 205);
        private CategorySelector _categorySelector = new CategorySelector();
        private List<Button> _categoryButtons = new List<Button>();

        // The position the backdrop should be in when open or closed.
        private readonly int _activeY;
        private readonly int _inactiveY;

        private List<TileHolder> _tileHolders = new List<TileHolder>();
        public static TileHolder TileBeingMoved { get; private set; } = new TileHolder(0);
        public static bool IsMovingTile { get; set; }
        private Rectangle _scissorRectangle = new Rectangle(150 * 2, 42 * 2, 236 * 2, 195 * 2);

        private byte[] _buildingTiles =
        {
            1, // Grass
            2, // Stone
            38, // Stone Brick
            5, // Sand
            39, // Ice
            40, // Snow Grass
            57, // Mud
            6, // Mesa
            41, // Void Tile
            4, // Hellrock
            10, // Gold Brick
            8, // Metal
            21, // Scaffolding
        };

        private byte[] _specialTiles =
        {
            3, // Marble Floor
            18, // Marble Column
            29, // Marble Ceiling
            47, // Wooden Platform
            14, // Vines
            15, // Ladder
            16, // Chain
            22, // Spikes
            46, // Void Ladder
            23, // Water
            24, // Lava
            25, // Poisoned Water
            64, // Player Detector
        };

        private byte[] _objects =
        {
            11, // Torch
            12, // Chandelier
            13, // Door
            19, // Chest
            36, // Sign
            58, // Portal
            37, // Checkpoint
            31, // Tree
            59, // Bed
            60, // Bookshelf
            61, // Painting
            20, // Tech -- OBSOLETE
            42, // Flame Spitter
            43, // Machine Gun
            41, // Void Fire Spitter
            45, // Mushroom Booster
            21, // Golden Apple
            22, // Health Apple
            32, // Small Rock
            34, // Medium Rock
            33, // Big Rock
            52, // Sapphire Crystal
            53, // Ruby Crystal
            54, // Emerald Crystal
            48, // Aquaant Crystal
            49, // Heliaura Crystal
            50, // Sentistract Sludge
        };

        private byte[] _wallTiles =
        {
            102, // Dirt Wall
            101, // Stone Wall
            107, // Stone Brick Wall
            105, // Sand Wall
            108, // Mesa Wall
            104, // Marble Wall
            109, // Wallpaper
            100, // Gold Brick Wall
            106, // Hellstone Wall
            103, // Fence
            110, // Black
        };

        private byte[] _characters =
        {
            200, // Player
            203, // NPC
            201, // Snake
            202, // Frog
            204, // Lost
            205, // Hellboar
            206, // Falling Boulder
            207, // Bay
            208, // Duck
            209, // Being of Sight
        };

        public Inventory()
        {
            _backDrop = new Rectangle(87 * 2, 38 * 2,
                305 * 2, 204 * 2);
            _inactiveY = _backDrop.Y - _backDrop.Height;
            _activeY = _backDrop.Y;
            _posAtStartOfAnimation = _backDrop.Y;

            CreateTileHolders();

            int buttonWidth = 46 * 2;
            int buttonHeight = 15 * 2;

            int spacingBetweenButtons = 6;

            // Category buttons on the side.
            Button button1 = new TextButton(new Vector2(99 * 2, 60 * 2), "Building", false);
            button1.MouseClicked += BuldingCatClicked;
            BuldingCatClicked(button1);
            _categoryButtons.Add(button1);

            Button button2 = new TextButton(new Vector2(button1.GetPosition().X, button1.GetPosition().Y + buttonHeight * 1 + spacingBetweenButtons * 2), "Wall", false);
            button2.MouseClicked += WallCatClicked;
            _categoryButtons.Add(button2);

            Button button3 = new TextButton(new Vector2(button1.GetPosition().X, button1.GetPosition().Y + buttonHeight * 2 + spacingBetweenButtons * 4), "Objects", false);
            button3.MouseClicked += ObjectsCatClicked; ;
            _categoryButtons.Add(button3);

            Button button4 = new TextButton(new Vector2(button1.GetPosition().X, button1.GetPosition().Y + buttonHeight * 3 + spacingBetweenButtons * 6), "Entities", false);
            button4.MouseClicked += CharactersCatClicked; ;
            _categoryButtons.Add(button4);

            Button button5 = new TextButton(new Vector2(button1.GetPosition().X, button1.GetPosition().Y + buttonHeight * 4 + spacingBetweenButtons * 8), "Special", false);
            button5.MouseClicked += SpecialCatClicked;
            _categoryButtons.Add(button5);

            foreach (var button in _categoryButtons)
            {
                button.BindTo(_backDrop);
                button.ChangeDimensions(new Rectangle(0, 0, (buttonWidth), (buttonHeight)));
                button.Color = new Color(95, 95, 95);
            }
        }

        private void SpecialCatClicked(Button button)
        {
            ChangeCategory(Category.Special);
            _categorySelector.MoveTo(button.GetPosition(), 100);
        }

        private void CharactersCatClicked(Button button)
        {
            ChangeCategory(Category.Characters);
            _categorySelector.MoveTo(button.GetPosition(), 100);
        }

        private void ObjectsCatClicked(Button button)
        {
            ChangeCategory(Category.Objects);
            _categorySelector.MoveTo(button.GetPosition(), 100);
        }

        private void BuldingCatClicked(Button button)
        {
            ChangeCategory(Category.Building);
            _categorySelector.MoveTo(button.GetPosition(), 100);
        }

        private void WallCatClicked(Button button)
        {
            ChangeCategory(Category.Wall);
            _categorySelector.MoveTo(button.GetPosition(), 100);
        }

        private void ChangeCategory(Category cat)
        {
            CurrentCategory = cat;
            CreateTileHolders();
        }

        private void CreateTileHolders()
        {
            _tileHolders = new List<TileHolder>();

            byte[] ids = { 0 };
            switch (CurrentCategory)
            {
                case Category.Building:
                    ids = _buildingTiles;
                    break;
                case Category.Wall:
                    ids = _wallTiles;
                    break;
                case Category.Special:
                    ids = _specialTiles;
                    break;
                case Category.Objects:
                    ids = _objects;
                    break;
                case Category.Characters:
                    ids = _characters;
                    break;

            }

            foreach (byte id in ids)
            {
                _tileHolders.Add(new TileHolder(id));
            }

            // Places the tile holders in their proper positions in the grid.
            var counter = 0;
            foreach (var tile in _tileHolders)
            {
                var x = (DefaultX) +
                        (counter % TilesPerRow) *
                        (tile.Size + SpacingBetweenTiles);
                var y = (DefaultY) +
                        (counter / TilesPerRow) *
                        (tile.Size + SpacingBetweenTiles);

                tile.SetPosition(x, y);
                tile.BindTo(new Vector2(_backDrop.X, _backDrop.Y));
                tile.WasClicked += OnTileClicked;
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
        public static void OpenOrClose()
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

            if (IsOpen)
            {
                if (InputHelper.IsLeftMousePressed() && !IsMovingTile)
                {
                    foreach (var tile in _tileHolders)
                    {
                        tile.CheckIfClickedOn();
                    }
                }
            }

            TileBeingMoved.Update(new Vector2(_backDrop.X, _backDrop.Y));
            foreach (var tile in _tileHolders)
            {
                tile.Update(new Vector2(_backDrop.X, _backDrop.Y));
            }

            foreach (var button in _categoryButtons)
            {
                button.Update(_backDrop);
            }

        }

        private void OnTileClicked(TileHolder tile)
        {
            TileBeingMoved = tile;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameWorld.UiSpriteSheet, _backDrop, _backDropSource, Color.White);

            // Sets the scrolling levels to disappear if they are not inside of this bounding box.
            Rectangle originalScissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            spriteBatch.GraphicsDevice.ScissorRectangle = _scissorRectangle;

            foreach (var tile in _tileHolders)
            {
                if (tile != TileBeingMoved || !IsMovingTile)
                    tile.Draw(spriteBatch);
            }

            // Returns the scissor rectangle to original.
            spriteBatch.GraphicsDevice.ScissorRectangle = originalScissorRectangle;


            foreach (var tile in _tileHolders)
            {
                if (!IsMovingTile)
                    tile.DrawToolTip(spriteBatch);
            }

            if (IsOpen)
                _categorySelector.Draw(spriteBatch);
            foreach (var button in _categoryButtons)
            {
                button.Draw(spriteBatch);
            }

        }

        public void DrawOnTop(SpriteBatch spriteBatch)
        {
            if (IsMovingTile)
                TileBeingMoved.Draw(spriteBatch);
        }

        public static void StartAnimation(Button button)
        {
            OpenOrClose();
        }
    }
}