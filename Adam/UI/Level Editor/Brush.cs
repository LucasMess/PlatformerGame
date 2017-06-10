using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.UI.Elements;
using ThereMustBeAnotherWay.UI.Level_Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ThereMustBeAnotherWay.UI
{
    public class Brush
    {
        Timer gridOpacityTimer = new Timer(true);

        public enum BrushMode { Build, Erase, Select }
        public BrushMode CurrentBrushMode;

        protected int MaxSize
        {
            get
            {
                if (_selectedBrushTiles[0]?.GetSize() == new Point(32, 32))
                {
                    return 12;
                }
                return 1;
            }
        }

        const int MinSize = 1;

        public int Size = 1;
        int _index;
        private Image grid;
        private Rectangle bright, dim, middle;
        Image[] _selectionSquares = new Image[1];
        Tile[] _selectedBrushTiles = new Tile[1];
        public int[] SelectedIndices;
        private int[] _lastSelectedIndices;

        int _lastScrollWheel;

        public delegate void EventHandler();
        public event EventHandler SizeChanged;

        public Brush()
        {
            bright = new Rectangle(412, 0, 36, 36);
            middle = new Rectangle(412 + 36, 0, 36, 36);
            dim = new Rectangle(412 + 36 + 36, 0, 36, 36);
        }

        public void Update()
        {
            CreateBrush();
            CheckIfSizeChanged();
        }


        public void ChangeBrushMode(BrushMode mode)
        {
            CurrentBrushMode = mode;
            switch (mode)
            {
                case BrushMode.Build:
                    LevelEditor.ButtonBar.OnBrushButtonClicked();
                    break;
                case BrushMode.Erase:
                    LevelEditor.ButtonBar.OnEraserButtonClicked();
                    break;
                case BrushMode.Select:
                    LevelEditor.ButtonBar.OnSelectButtonClicked();
                    break;
                default:
                    break;
            }
        }
        private void CheckIfSizeChanged()
        {
            MouseState mouse = Mouse.GetState();
            int scrollWheel = mouse.ScrollWheelValue;

            // If brush is big and a tile that cannot have a brush > 1 is selected, resize brush.
            if (Size > MaxSize)
            {
                Size = MaxSize;
                DeleteOldTiles();
                _selectedBrushTiles = new Tile[Size * Size];
            }

            if (!Inventory.IsOpen)
            {
                if (scrollWheel > _lastScrollWheel)
                {
                    Size++;
                    if (Size > MaxSize) Size = MaxSize;
                    else
                    {
                        DeleteOldTiles();
                        _selectedBrushTiles = new Tile[Size * Size];
                        SizeChanged?.Invoke();
                    }
                }
                if (scrollWheel < _lastScrollWheel)
                {
                    Size--;
                    if (Size < MinSize) Size = MinSize;
                    else
                    {
                        DeleteOldTiles();
                        _selectedBrushTiles = new Tile[Size * Size];
                        SizeChanged?.Invoke();
                    }
                }
            }

            _lastScrollWheel = scrollWheel;
        }

        /// <summary>
        /// Delete the tiles first, to prevent residual lights and interactables.
        /// </summary>
        private void DeleteOldTiles()
        {
            if (_selectedBrushTiles != null)
            {
                foreach (var tile in _selectedBrushTiles)
                {
                    tile?.ResetToDefault();
                }
            }
        }

        private void CreateBrush()
        {
            SelectedIndices = GetTilesCoveredByBrush();

            // Check if the indices are the same as the last tick. This prevents the flickering for random subIds.
            if (_lastSelectedIndices != null)
            {
                var union = from a in SelectedIndices
                            join b in _lastSelectedIndices on a equals b
                            select a;

                // Return if two arrays are identical.
                if (SelectedIndices.Length == _lastSelectedIndices.Length && union.Count() == SelectedIndices.Length)
                    return;
            }

            DeleteOldTiles();

            // Create grid.
            if (SelectedIndices[0] >= 0 && SelectedIndices[0] < GameWorld.TileArray.Length)
            {
                for (int i = 0; i < _selectedBrushTiles.Length; i++)
                {
                    if (SelectedIndices[i] >= 0 && SelectedIndices[i] < GameWorld.TileArray.Length)
                    {
                        //Create transparent tiles to show selected tile
                        Tile hoveredTile = GameWorld.TileArray[SelectedIndices[i]];
                        Tile fakeTile = new Tile(true)
                        {
                            IsBrushTile = true
                        };
                        if (Size == 1)
                        {

                            fakeTile.Id = LevelEditor.SelectedId;
                            fakeTile.SetOriginalPosition(hoveredTile.DrawRectangle.X, hoveredTile.DrawRectangle.Y);
                            fakeTile.DefineTexture();
                            if (hoveredTile.GetSize() != new Point(32, 32))
                            {
                                fakeTile.Id = hoveredTile.Id;
                                fakeTile.DefineTexture();
                                fakeTile.DrawRectangle = hoveredTile.DrawRectangle;
                            }
                        }
                        else
                        {
                            fakeTile.Id = LevelEditor.SelectedId;
                            fakeTile.SetOriginalPosition(hoveredTile.DrawRectangle.X, hoveredTile.DrawRectangle.Y);
                            fakeTile.DefineTexture();
                        }

                        _selectedBrushTiles[i] = fakeTile;

                    }
                }
            }

            // Setting size of grid.
            Tile firstTile = _selectedBrushTiles[0];
            int width, height;

            if (Size == 1)
            {
                width = Size * (int)firstTile.GetSize().X + 8;
                height = Size * (int)firstTile.GetSize().Y + 8;
            }
            else
            {
                width = Size * 32 + 8;
                height = Size * 32 + 8;
            }

            grid.Rectangle = new Rectangle(firstTile.GetDrawRectangle().X - 4, firstTile.GetDrawRectangle().Y - 4, width, height);
            grid.Texture = GameWorld.UiSpriteSheet;

            _lastSelectedIndices = new int[SelectedIndices.Length];
            SelectedIndices.CopyTo(_lastSelectedIndices, 0);

        }

        private int[] GetTilesCoveredByBrush()
        {
            Rectangle mouse = InputHelper.GetMouseRectGameWorld();
            _index = CalcHelper.GetIndexInGameWorld(mouse.X, mouse.Y);

            //Get indexes around brush
            List<int> indexes = new List<int>();
            for (int h = 0; h < Size; h++)
            {
                for (int w = 0; w < Size; w++)
                {
                    int startingIndex = _index - (int)(Math.Truncate((double)(Size / 2))) - (int)(Math.Truncate((double)(Size / 2)) * GameWorld.WorldData.LevelWidth);
                    int i = startingIndex + (h * GameWorld.WorldData.LevelHeight) + w;
                    indexes.Add(i);
                }
            }

            return indexes.ToArray();
        }

        public void DrawBehind(SpriteBatch spriteBatch)
        {
            if (_selectedBrushTiles == null) return;
            foreach (Tile t in _selectedBrushTiles)
            {
                if (t != null && t.Texture != null)
                    spriteBatch.Draw(t.Texture, t.DrawRectangle, t.SourceRectangle, t.Color * .5f);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //if (_selectionSquares == null) return;
            //foreach (Image i in _selectionSquares)
            //{
            //    if (i.Texture != null)
            //        spriteBatch.Draw(i.Texture, i.Rectangle, i.SourceRectangle, Color.White);
            //}

            float opacity = (float)(.6 + .4 * (Math.Sin(gridOpacityTimer.TimeElapsedInSeconds * 5)));
            Color gridColor = Color.White * opacity;

            if (grid.Texture != null)
                spriteBatch.Draw(grid.Texture, grid.Rectangle, dim, gridColor);
            if (grid.Texture != null)
                spriteBatch.Draw(grid.Texture, grid.Rectangle, middle, gridColor);
            if (grid.Texture != null)
                spriteBatch.Draw(grid.Texture, grid.Rectangle, bright, gridColor);

        }
    }

}
