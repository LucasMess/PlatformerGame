using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam.UI
{
    public class Brush
    {
        protected int MaxSize
        {
            get
            {
                if (_selectedBrushTiles[0]?.GetSize() == new Vector2(1, 1))
                {
                    return 12;
                }
                return 1;
            }
        }

        const int MinSize = 1;

        public int Size = 1;
        int _index;
        Image hoverBrushSquare = new Image();
        Image[] _selectionSquares = new Image[1];
        Tile[] _selectedBrushTiles = new Tile[1];
        public int[] SelectedIndexes;

        int _lastScrollWheel;

        public delegate void EventHandler();
        public event EventHandler SizeChanged;

        public void Update()
        {
            CreateBrush();
            CheckIfSizeChanged();
        }

        private void CheckIfSizeChanged()
        {
            MouseState mouse = Mouse.GetState();
            int scrollWheel = mouse.ScrollWheelValue;

            if (!GameWorld.Instance.LevelEditor.OnInventory)
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
            GameWorld gameWorld = GameWorld.Instance;
            SelectedIndexes = GetTilesCoveredByBrush();

            // Create grid.
            Tile hoveredTile = gameWorld.TileArray[SelectedIndexes[0]];
            hoverBrushSquare.Rectangle = new Rectangle(hoveredTile.GetDrawRectangle().X, hoveredTile.GetDrawRectangle().Y, Size * (int)hoveredTile.GetSize().X * Main.Tilesize, Size * (int)hoveredTile.GetSize().Y * Main.Tilesize);
            hoverBrushSquare.SourceRectangle = new Rectangle(21 * 16, 7 * 16, 16, 16);
            hoverBrushSquare.Texture = GameWorld.SpriteSheet;

            for (int i = 0; i < _selectedBrushTiles.Length; i++)
            {
                if (SelectedIndexes[i] >= 0 && SelectedIndexes[i] < gameWorld.TileArray.Length)
                {
                    //Create transparent tiles to show selected tile
                    hoveredTile = gameWorld.TileArray[SelectedIndexes[i]];
                    Tile fakeTile = new Tile(true);
                    fakeTile.Id = gameWorld.LevelEditor.SelectedId;
                    fakeTile.DrawRectangle = hoveredTile.DrawRectangle;
                    fakeTile.IsBrushTile = true;
                    fakeTile.DefineTexture();
                    fakeTile.Texture = GameWorld.SpriteSheet;
                    _selectedBrushTiles[i] = fakeTile;

                }
            }

        }

        private int[] GetTilesCoveredByBrush()
        {
            _index = GameWorld.Instance.LevelEditor.IndexOfMouse;

            GameWorld gameWorld = GameWorld.Instance;

            //Get indexes around brush
            List<int> indexes = new List<int>();
            for (int h = 0; h < Size; h++)
            {
                for (int w = 0; w < Size; w++)
                {
                    int startingIndex = _index - (int)(Math.Truncate((double)(Size / 2))) - (int)(Math.Truncate((double)(Size / 2)) * gameWorld.WorldData.LevelWidth);
                    int i = startingIndex + (h * gameWorld.WorldData.LevelHeight) + w;
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
                if (t != null)
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
            if (hoverBrushSquare.Texture != null)
                spriteBatch.Draw(hoverBrushSquare.Texture, hoverBrushSquare.Rectangle, hoverBrushSquare.SourceRectangle, Color.White);

        }
    }

}
