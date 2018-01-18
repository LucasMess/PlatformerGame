using ThereMustBeAnotherWay.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThereMustBeAnotherWay.UI.Elements
{
    public static class Cursor
    {
        private static Texture2D _texture = GameWorld.UiSpriteSheet;
        private static Rectangle _drawRectangle = new Rectangle(0, 0, 32, 32);
        private static Rectangle _sourceRectangle = new Rectangle(0,128,32,32);
        private static bool _isVisible = true;

        public enum Type
        {
            Build, Erase, Normal, Select,
        }


        public static void Update()
        {
            Rectangle mouse = InputSystem.GetMouseInUi();
            _drawRectangle.X = mouse.X - 5;
            _drawRectangle.Y = mouse.Y - 5;
        }

        public static void ChangeCursor(Type type)
        {
            switch (type)
            {
                case Type.Build:
                    _sourceRectangle = new Rectangle(0, 128, 32, 32);
                    break;
                case Type.Erase:
                    _sourceRectangle = new Rectangle(0, 160, 32, 32);
                    break;
                case Type.Normal:
                    _sourceRectangle = new Rectangle(32, 128, 32, 32);
                    break;
                case Type.Select:
                    _sourceRectangle = new Rectangle(32, 160, 32, 32);
                    break;
                default:
                    break;
            }
        }

        public static void Show()
        {
            _isVisible = true;
        }

        public static void Hide()
        {
            _isVisible = false;
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (_isVisible)
            spriteBatch.Draw(_texture, _drawRectangle, _sourceRectangle, Color.White);
        }
    }
}
