using ThereMustBeAnotherWay.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace ThereMustBeAnotherWay.UI
{
    public class LevelNameNotification
    {

        BitmapFont _font;
        Texture2D _texture;
        Rectangle _drawRectangle;
        string _text = "";
        bool _isActive;
        double _timer;
        float _opacity = 0;
        Vector2 _textPos, _original;

        public LevelNameNotification()
        {
            _texture = ContentHelper.LoadTexture("Tiles/black");
            _font = ContentHelper.LoadFont("Fonts/x64");
            _drawRectangle = new Rectangle(0, TMBAW_Game.UserResHeight - 180, TMBAW_Game.UserResWidth, 105);
        }

        public void Show(string text)
        {
            this._text = text;
            if (text == null) return;
            _isActive = true;
            _timer = 0;
            _textPos = new Vector2(TMBAW_Game.UserResWidth - _font.MeasureString(text).X - 30, _drawRectangle.Y);
            _original = _textPos;
            _textPos.X += _font.MeasureString(text).X / 2;
        }

        public void Update()
        {
            float deltaOpacity = .03f;

            if (_isActive)
            {
                _opacity += deltaOpacity;
                _timer += TMBAW_Game.GameTime.ElapsedGameTime.TotalSeconds;
                if (_textPos.X >= _original.X)
                {
                    _textPos.X += (_original.X - _textPos.X) / 10;
                }
                _textPos.X -= 1f;

                if (_timer > 3)
                {
                    _isActive = false;
                }
            }
            else
            {
                _opacity -= deltaOpacity;
                _textPos.X -= 6f;
            }

            if (_opacity > .7f)
                _opacity = .7f;
            if (_opacity < 0)
                _opacity = 0;

            if (_textPos.X < 0) _textPos.X = 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_text == null) return;
            spriteBatch.Draw(_texture, _drawRectangle, Color.White * _opacity);
            spriteBatch.DrawString(_font, _text, _textPos,
                Color.White * _opacity);

        }
    }
}
