using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI.Level_Editor
{
    class TileHolder
    {
        private const int SpacingBetweenSquareAndTile = 3;
        private Vector2 _positionDifferential;
        private Tile _tile;
        public static Rectangle SourceRectangle = new Rectangle(297, 189, 22, 23);


        public TileHolder(int id)
        {
            _tile = new Tile(true) { Id = (byte)id };
            _tile.DefineTexture();


            _tile.DrawRectangle.Width = CalcHelper.ApplyUiRatio(16);
            _tile.DrawRectangle.Height = CalcHelper.ApplyUiRatio(16);
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

        public void Update(Vector2 position)
        {
            _tile.DrawRectangle.X = (int)(_positionDifferential.X + position.X);
            _tile.DrawRectangle.Y = (int)(_positionDifferential.Y + position.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawSquareBehindTile(_tile, spriteBatch);
            _tile.DrawByForce(spriteBatch);
        }

        private void DrawSquareBehindTile(Tile tile, SpriteBatch spriteBatch)
        {
            Color squareColor = Color.White;
            if (InputHelper.MouseRectangle.Intersects(tile.DrawRectangle))
                squareColor = Color.Gray;

            spriteBatch.Draw(GameWorld.UiSpriteSheet, new Rectangle(tile.DrawRectangle.X - CalcHelper.ApplyUiRatio(SpacingBetweenSquareAndTile), tile.DrawRectangle.Y - CalcHelper.ApplyUiRatio(SpacingBetweenSquareAndTile), CalcHelper.ApplyUiRatio(SourceRectangle.Width), CalcHelper.ApplyUiRatio(SourceRectangle.Height)), SourceRectangle, squareColor);
        }
    }
}
