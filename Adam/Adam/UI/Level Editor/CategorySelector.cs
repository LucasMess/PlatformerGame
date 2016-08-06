using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;
using Microsoft.Xna.Framework;
using Adam.UI.Elements;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI.Level_Editor
{
    class CategorySelector : UiElement
    {
        public const int Height = 32;
        private Rectangle _sourceRectangle = new Rectangle(148, 180, 57, Height);

        public CategorySelector()
        {
            DrawRectangle = new Rectangle(0, 0, CalcHelper.ApplyUiRatio(_sourceRectangle.Width)
            , CalcHelper.ApplyUiRatio(_sourceRectangle.Height));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameWorld.UiSpriteSheet, DrawRectangle, _sourceRectangle, Color.White);
        }

        public override void MoveTo(Vector2 position, int duration)
        {
            position.X -= CalcHelper.ApplyUiRatio(6);
            position.Y -= CalcHelper.ApplyUiRatio(8);
            base.MoveTo(position, duration);
        }
    }
}
