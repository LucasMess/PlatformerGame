using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.PlayerCharacter;

namespace Adam.UI.Overlay_Elements
{
    public class Coin
    {
        Texture2D _texture;
        Rectangle _drawRectangle, _sourceRectangle;
        Vector2 _origin;
        Vector2 _frameCount;
        Vector2 _textPos;
        int _currentFrame, _switchFrame;
        double _timer;

        Animation _animation;
        GameTime _gameTime;
        int _score;
        bool _scoreChanged;

        public Coin(Vector2 position)
        {
            _texture = ContentHelper.LoadTexture("Menu/player_coin");
            _frameCount = new Vector2(7, 0);
            _drawRectangle = new Rectangle((int)position.X, (int)position.Y, 64, _texture.Height);
            _sourceRectangle = new Rectangle(0, 0, 64, 64);
            _origin = new Vector2(32, 32);

            _drawRectangle.X += (int)_origin.X;
            _drawRectangle.Y += (int)_origin.Y;
            _textPos = new Vector2(_drawRectangle.X + 64 + 10, _drawRectangle.Y - 16);

            _animation = new Animation(_texture, _drawRectangle, 100, 0, AnimationType.Loop);
        }

        public void Update(Player player, GameTime gameTime)
        {
            this._gameTime = gameTime;

            Animate();

            _scoreChanged = false;
            if (_score < player.Score)
            {
                _score++;
                _scoreChanged = true;
            }
            if (_score > player.Score)
            {
                _score--;
                _scoreChanged = true;
            }
        }

        private void Animate()
        {
            _switchFrame = 100;
            _timer += _gameTime.ElapsedGameTime.Milliseconds;

            if (_timer > _switchFrame)
            {
                _sourceRectangle.X += _sourceRectangle.Width;
                _currentFrame++;
                _timer = 0;
            }

            if (_currentFrame > _frameCount.X)
            {
                _sourceRectangle.X = 0;
                _currentFrame = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _drawRectangle, _sourceRectangle, Color.White, 0, _origin, SpriteEffects.None, 0);

            Color color = Color.White;
            if (_scoreChanged)
                color = Color.ForestGreen;
            FontHelper.DrawWithOutline(spriteBatch, Overlay.Font, _score.ToString(), _textPos, 5, color, Color.Black);
        }
    }
}
