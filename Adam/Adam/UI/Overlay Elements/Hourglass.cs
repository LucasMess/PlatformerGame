using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;
using Adam.PlayerCharacter;

namespace Adam.UI.Overlay_Elements
{
    public class Hourglass
    {
        Texture2D _texture;
        Rectangle _drawRectangle, _sourceRectangle;
        Vector2 _origin;
        Animation _animation;
        Vector2 _position;

        int _currentTime;

        public Hourglass(Vector2 position)
        {
            _texture = ContentHelper.LoadTexture("Menu/timer");
            _drawRectangle = new Rectangle(Main.UserResWidth * 9 / 12, Main.UserResHeight * 1 / 12, 64, _texture.Height);
            _sourceRectangle = new Rectangle(0, 0, 64, 64);
            _origin = new Vector2(32, 32);
            _drawRectangle.X += (int)_origin.X;
            _drawRectangle.Y -= (int)_origin.Y;
            _animation = new Animation(_texture, new Rectangle(_drawRectangle.X, _drawRectangle.Y, 64, 64), 200, 0, AnimationType.Loop);

            this._position = position;
        }

        public void Update()
        {
            _animation.Update(Main.GameTime);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _animation.Draw(spriteBatch);
            FontHelper.DrawWithOutline(spriteBatch, Overlay.Font, _currentTime.ToString(), _position, 5, Color.White, Color.Black);
        }
    }
}
