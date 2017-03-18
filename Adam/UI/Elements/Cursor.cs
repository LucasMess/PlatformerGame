using Adam.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adam.UI.Elements
{
    public static class Cursor
    {
        private static Texture2D _texture = GameWorld.UiSpriteSheet;
        private static Rectangle _drawRectangle = new Rectangle(0, 0, 32, 32);
        private static Rectangle _sourceRectangle = new Rectangle(0,128,32,32);

        public enum CursorType
        {
            Build, Erase,
        }


        public static void Update()
        {
            Rectangle mouse = InputHelper.GetMouseInUi();
            _drawRectangle.X = mouse.X - 5;
            _drawRectangle.Y = mouse.Y - 5;
        }

        public static void ChangeCursor(CursorType type)
        {
            switch (type)
            {
                case CursorType.Build:
                    _sourceRectangle = new Rectangle(0, 128, 32, 32);
                    break;
                case CursorType.Erase:
                    _sourceRectangle = new Rectangle(0, 160, 32, 32);
                    break;
                default:
                    break;
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _drawRectangle, _sourceRectangle, Color.White);
        }
    }
}
