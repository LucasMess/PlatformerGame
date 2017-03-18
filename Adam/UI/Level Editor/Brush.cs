using Adam.Levels;
using Adam.Misc;
using Adam.UI.Level_Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Adam.UI
{
    public class Brush
    {
        Timer gridOpacityTimer = new Timer(true);

        public enum BrushMode { Build, Erase }
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
        public int[] SelectedIndexes;

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

        private void CheckIfSizeChanged()
        {
            MouseState mouse = Mouse.GetState();
            int scrollWheel = mouse.ScrollWheelValue;

            // If brush is big and a tile that cannot have a brush > 1 is selected, resize brush.
            if (Size > MaxSize)
            {
                Size = MaxSize;
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
                        //_selectionSquares = new Image[Size * Size];
                        _selectedBrushTiles = new Tile[Size * Size];
                        SizeChanged();
                    }
                }
                if (scrollWheel < _lastScrollWheel)
                {
                    Size--;
                    if (Size < MinSize) Size = MinSize;
                    else
                    {
                        //_selectionSquares = new Image[Size * Size];
                        _selectedBrushTiles = new Tile[Size * Size];
                        SizeChanged();
                    }
                }
            }

            _lastScrollWheel = scrollWheel;
        }

        private void CreateBrush()
        {
            SelectedIndexes = GetTilesCoveredByBrush();

            // Create grid.
            if (SelectedIndexes[0] >= 0 && SelectedIndexes[0] < GameWorld.TileArray.Length)
            {
                for (int i = 0; i < _selectedBrushTiles.Length; i++)
                {
                    if (SelectedIndexes[i] >= 0 && SelectedIndexes[i] < GameWorld.TileArray.Length)
                    {
                        //Create transparent tiles to show selected tile
                        Tile hoveredTile = GameWorld.TileArray[SelectedIndexes[i]];
                        Tile fakeTile = new Tile(true);
                        fakeTile.IsBrushTile = true;
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
