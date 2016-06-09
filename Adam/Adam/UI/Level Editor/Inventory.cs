using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using Adam.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI.Level_Editor
{
    /// <summary>
    /// Contains all tiles and entities in grid format, and also contains a hotbar for quick selection of items.
    /// </summary>
    class Inventory
    {
        private const int SpacingBetweenTiles = 2;
        private const int SpacingBetweenSquareAndTile = 3;
        private const int defaultX = 159;
        private const int activeY = 51;
        private const int tilesPerRow = 9;

        /// <summary>
        /// Returns true if the inventory is visible and active.
        /// </summary>
        public bool IsOpen { get; set; }

        List<Tile> _grid = new List<Tile>();
        List<Rectangle> _gridSquares = new List<Rectangle>();
        private Rectangle _gridRectSource = new Rectangle(297, 189, 22, 23);
        private Rectangle _backDrop;
        private Rectangle _backDropSource = new Rectangle(0, 252, 305, 205);

        public Inventory()
        {
            _backDrop = new Rectangle(CalcHelper.ApplyUiRatio(87), CalcHelper.ApplyUiRatio(38), CalcHelper.ApplyUiRatio(305), CalcHelper.ApplyUiRatio(204));

            for (int i = 1; i < 60; i++)
            {
                Tile tile = new Tile(true) { Id = (byte)i };
                tile.DefineTexture();
                _grid.Add(tile);
            }

            int counter = 0;
            foreach (var tile in _grid)
            {
                tile.DrawRectangle.Width = CalcHelper.ApplyUiRatio(16);
                tile.DrawRectangle.Height = CalcHelper.ApplyUiRatio(16);

                tile.DrawRectangle.X = CalcHelper.ApplyUiRatio(defaultX) + (counter % tilesPerRow) * CalcHelper.ApplyUiRatio(_gridRectSource.Width + SpacingBetweenTiles);
                tile.DrawRectangle.Y = CalcHelper.ApplyUiRatio(activeY) + (counter / tilesPerRow) * CalcHelper.ApplyUiRatio(_gridRectSource.Height + SpacingBetweenTiles);

                counter++;
            }

        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameWorld.UiSpriteSheet, _backDrop, _backDropSource, Color.White);

            foreach (var tile in _grid)
            {
                DrawSquareBehindTile(tile, spriteBatch);
                tile.DrawByForce(spriteBatch);

            }
        }

        private void DrawSquareBehindTile(Tile tile, SpriteBatch spriteBatch)
        {
            Color squareColor = Color.White;
            if (InputHelper.MouseRectangle.Intersects(tile.DrawRectangle))
                squareColor = Color.Gray;

            spriteBatch.Draw(GameWorld.UiSpriteSheet, new Rectangle(tile.DrawRectangle.X - CalcHelper.ApplyUiRatio(SpacingBetweenSquareAndTile), tile.DrawRectangle.Y - CalcHelper.ApplyUiRatio(SpacingBetweenSquareAndTile), CalcHelper.ApplyUiRatio(_gridRectSource.Width), CalcHelper.ApplyUiRatio(_gridRectSource.Height)), _gridRectSource, squareColor);
        }


    }
}
