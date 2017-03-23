using Adam.Levels;
using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI.Elements
{
    public static class KeyPopUp
    {
        private static bool _isActive;
        private static Rectangle _drawRectangle = new Rectangle(0, 0, 32, 32);
        private static Rectangle _sourceRectangle = new Rectangle(320, 112, 16, 16);
        private static Texture2D _texture = GameWorld.SpriteSheet;
        private static string _displayCharacter;

        public static void Show(string key, Rectangle collRectangle)
        {
            _displayCharacter = key;
            _isActive = true;
            _drawRectangle.X = collRectangle.Center.X - 16;
            _drawRectangle.Y = collRectangle.Y - 40;
        }

        public static void Update()
        {
            _isActive = false;
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (_isActive)
            {
                spriteBatch.Draw(_texture, _drawRectangle, _sourceRectangle, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                FontHelper.DrawWithOutline(spriteBatch, FontHelper.Fonts[1], _displayCharacter, new Vector2(_drawRectangle.Center.X - FontHelper.Fonts[1].MeasureString(_displayCharacter).X / 2, _drawRectangle.Y + 8), 1, Color.Black, Color.DarkGray);
            }
        }
    }

}
