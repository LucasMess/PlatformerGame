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
        const int MaxSize = 12;
        const int MinSize = 1;

        public int Size = 1;
        int _index;
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
                        _selectionSquares = new Image[Size * Size];
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
                        _selectionSquares = new Image[Size * Size];
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
            
            for (int i = 0; i < _selectionSquares.Length; i++)
            {
                if (SelectedIndexes[i] >= 0 && SelectedIndexes[i] < gameWorld.TileArray.Length)
                {
                    //Create grid
                    _selectionSquares[i] = new Image();
                    Tile hovered = gameWorld.TileArray[SelectedIndexes[i]];
                    _selectionSquares[i].Rectangle = hovered.DrawRectangle;
                    _selectionSquares[i].SourceRectangle = new Microsoft.Xna.Framework.Rectangle(21 * 16, 7 * 16, 16, 16);
                    _selectionSquares[i].Texture = GameWorld.SpriteSheet;

                    //Create transparent tiles to show selected tile
                    Tile fakeTile = new Tile(true); 
                    fakeTile.Id = gameWorld.LevelEditor.SelectedId;
                    fakeTile.DrawRectangle = hovered.DrawRectangle;
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
            foreach(Tile t in _selectedBrushTiles)
            {
                if (t != null)
                spriteBatch.Draw(t.Texture, t.DrawRectangle, t.SourceRectangle, t.Color * .5f);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_selectionSquares == null) return;
            foreach(Image i in _selectionSquares)
            {
                if (i.Texture != null)
                spriteBatch.Draw(i.Texture, i.Rectangle, i.SourceRectangle, Color.White);
            }

        }
    }

}
