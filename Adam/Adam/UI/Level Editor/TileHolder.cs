using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;
using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI.Level_Editor
{
    class TileHolder
    {
        private const int SpacingBetweenSquareAndTile = 3;
        private Vector2 _positionDifferential;
        private readonly Tile _tile;
        public static Rectangle SourceRectangle = new Rectangle(297, 189, 22, 23);
        private readonly Color _selectedColor = new Color(69, 96, 198);
        private readonly Texture2D _black = ContentHelper.LoadTexture("Tiles/black");
        private readonly SpriteFont _font = FontHelper.ChooseBestFont(CalcHelper.ApplyUiRatio(16));

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
            DrawSquareBehindTile(spriteBatch);
            _tile.DrawByForce(spriteBatch);
        }

        private void DrawSquareBehindTile(SpriteBatch spriteBatch)
        {
            Color squareColor = Color.White;
            if (InputHelper.MouseRectangle.Intersects(_tile.DrawRectangle))
                squareColor = _selectedColor;

            spriteBatch.Draw(GameWorld.UiSpriteSheet, new Rectangle(_tile.DrawRectangle.X - CalcHelper.ApplyUiRatio(SpacingBetweenSquareAndTile), _tile.DrawRectangle.Y - CalcHelper.ApplyUiRatio(SpacingBetweenSquareAndTile), CalcHelper.ApplyUiRatio(SourceRectangle.Width), CalcHelper.ApplyUiRatio(SourceRectangle.Height)), SourceRectangle, squareColor);
        }

        public void DrawToolTip(SpriteBatch spriteBatch)
        {
            if (IsHovered())
            {
                string name = "Name not found";
                Tile.Names.TryGetValue(_tile.Id, out name);
                FontHelper.DrawTooltip(spriteBatch, name);
            }
        }

        private bool IsHovered()
        {
            return InputHelper.MouseRectangle.Intersects(_tile.DrawRectangle);
        }
    }
}
