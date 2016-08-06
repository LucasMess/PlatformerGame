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
        private Rectangle _sourceRectangle = new Rectangle(148,180,57,32);

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameWorld.UiSpriteSheet, DrawRectangle, _sourceRectangle, Color.White);
        }
    }
}
