using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam.UI.Elements
{
    public class KeyPopUp
    {
        bool _playerOn;
        Rectangle _drawRectangle;
        Rectangle _sourceRectangle;
        Texture2D _texture;

        /// <summary>
        /// To display the 'W' key above the object.
        /// </summary>
        public KeyPopUp()
        {
            _texture = GameWorld.SpriteSheet;
            _sourceRectangle = new Rectangle(16 * 20, 16 * 7, 16, 16);
        }

        public void Update(Rectangle collRectangle)
        {
            _drawRectangle = new Rectangle(collRectangle.X , collRectangle.Y - 48, 32, 32);

            if (GameWorld.Player.GetCollRectangle().Intersects(collRectangle))
                _playerOn = true;
            else _playerOn = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_playerOn)
                spriteBatch.Draw(_texture, _drawRectangle, _sourceRectangle, Color.White, 0, new Vector2(0,0), Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
        }
    }

}
