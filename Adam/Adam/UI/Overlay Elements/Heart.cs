using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;
using Adam.PlayerCharacter;

namespace Adam.UI.Overlay_Elements
{
    public class Heart
    {
        Texture2D _aliveTexture, _deadTexture, _currentTexture;
        Rectangle _drawRectangle, _sourceRectangle;
        Vector2 _origin;
        Vector2 _heartFrameCount;
        bool _isHeartPumping;
        bool _heartHasPumped, _heartHasPumped2;
        double _pumpTimer;
        int _currentFrameHeart, _switchFrameHeart;
        double _heartTimer, _restartTimer;
        float _heartRotation;
        bool _isRotatingRight = true;
        Color _healthColor;
        bool _isHeartDead;
        int _currentHealth;
        Vector2 _textPos;
        int _maxHealth;
        int _oldCurrentHealth;

        SoundEffect _heartBeat1, _heartBeat2;
        SoundEffectInstance _h1I, _h2I;
        GameTime _gameTime;
        Player _player;

        public Heart(Vector2 position)
        {
            _drawRectangle = new Rectangle((int)position.X, (int)position.Y, 64, 64);
            _sourceRectangle = new Rectangle(0, 0, 64, 64);
            _origin = new Vector2(32, 32);

            _drawRectangle.X += (int)_origin.X;
            _drawRectangle.Y += (int)_origin.Y;


            _aliveTexture = ContentHelper.LoadTexture("Menu/live_heart");
            _deadTexture = ContentHelper.LoadTexture("Menu/dead_heart");
            _currentTexture = _aliveTexture;
            _heartFrameCount = new Vector2(3, 0);

            _textPos = new Vector2(_drawRectangle.X + 64 + 10, _drawRectangle.Y - 16);


            _heartBeat1 = ContentHelper.LoadSound("Sounds/Menu/heartbeat1");
            _heartBeat2 = ContentHelper.LoadSound("Sounds/Menu/heartbeat2");

            _h1I = _heartBeat1.CreateInstance();
            _h2I = _heartBeat2.CreateInstance();
        }

        public void Update(GameTime gameTime, Player player)
        {
            this._gameTime = gameTime;
            this._player = player;

            AnimateHeart();
            RotateHeart();
            PumpHeart();

            if (_currentHealth < player.Health)
            {
                _currentHealth++;
            }
            if (_currentHealth > player.Health)
            {
                _currentHealth--;
            }

            if (_maxHealth < player.MaxHealth)
            {
                _maxHealth++;
            }
            if (_maxHealth > player.MaxHealth)
            {
                _maxHealth--;
            }

            if (_oldCurrentHealth < player.Health)
            {
                GameWorld.ParticleSystem.Add(new SplashNumber(player, player.Health - _oldCurrentHealth, Color.Green));
                _oldCurrentHealth = player.Health;
            }

            if (_oldCurrentHealth > player.Health)
            {
                GameWorld.ParticleSystem.Add(new SplashNumber(player, player.Health - _oldCurrentHealth, Color.Red));
                _oldCurrentHealth = player.Health;
            }
        }

        private void PumpHeart()
        {
            if (_player.Health <= (_maxHealth * .2))
            {
                _isHeartPumping = true;
            }
            else
            {
                _isHeartPumping = false;
                _healthColor = Color.White;
            }

            if (_player.Health <= 0)
            {
                _isHeartPumping = false;
                _healthColor = Color.Red;
                _isHeartDead = true;
            }
            else _isHeartDead = false;

            if (_isHeartDead)
            {
                _currentTexture = _deadTexture;
                _heartRotation = 0;
            }
            else
            {
                _currentTexture = _aliveTexture;
                RotateHeart();
            }

            if (_isHeartPumping)
            {
                _pumpTimer += _gameTime.ElapsedGameTime.TotalMilliseconds;

                if (_pumpTimer > 500 && !_heartHasPumped)
                {
                    _drawRectangle.Width *= 2;
                    _drawRectangle.Height *= 2;
                    _pumpTimer = 0;
                    _heartHasPumped = true;
                    _h1I.Play();
                    _healthColor = Color.Red;
                }

                if (_pumpTimer > 100 && _heartHasPumped)
                {
                    _drawRectangle.Width /= 2;
                    _drawRectangle.Height /= 2;
                    _pumpTimer = 0;
                    _heartHasPumped = false;
                    _h2I.Play();
                    _healthColor = Color.White;
                }
            }
        }

        private void RotateHeart()
        {
            double limit = Math.PI / 8;

            if (_isRotatingRight)
            {
                if (_heartRotation > limit)
                {
                    _isRotatingRight = false;
                }
                else
                    _heartRotation += .01f;
            }

            if (!_isRotatingRight)
            {
                if (_heartRotation < -limit)
                {
                    _isRotatingRight = true;
                }
                else
                    _heartRotation -= .01f;
            }


        }

        private void AnimateHeart()
        {
            _switchFrameHeart = 75;
            _heartTimer += _gameTime.ElapsedGameTime.Milliseconds;
            int maxWait = 1000;

            if (_heartTimer > _switchFrameHeart)
            {
                _sourceRectangle.X += _sourceRectangle.Width;
                _currentFrameHeart++;
                _heartTimer = 0;
            }

            if (_currentFrameHeart >= _heartFrameCount.X)
            {
                _restartTimer += _gameTime.ElapsedGameTime.Milliseconds;
                _heartTimer = 0;
                if (_restartTimer > maxWait)
                {
                    _restartTimer = 0;
                    _sourceRectangle.X = 0;
                    _currentFrameHeart = 0;
                }
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            FontHelper.DrawWithOutline(spriteBatch, Overlay.Font, _currentHealth + "/" + _maxHealth, _textPos, 5, _healthColor, Color.Black);
            spriteBatch.Draw(_currentTexture, _drawRectangle, _sourceRectangle, Color.White, _heartRotation, _origin, SpriteEffects.None, 0);
        }
    }
}
