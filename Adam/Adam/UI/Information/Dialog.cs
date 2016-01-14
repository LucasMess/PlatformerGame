using Adam.Misc;
using Adam.Misc.Helpers;
using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam.UI
{
    public class Dialog
    {
        Texture2D _texture;
        SpriteFont _font;
        Rectangle _drawRectangle;
        Rectangle _sourceRectangle;
        Vector2 _origin;

        bool _isActive;
        bool _isQuestion;
        string _fullText = "";
        private string _partialText = "";
        StringBuilder _sb;
        SoundFx _popSound;
        private SoundFx _letterPopSound;

        public delegate void EventHandler();
        public event EventHandler NextDialog;
        public event EventHandler CancelDialog;

        public event EventHandler YesResult;
        public event EventHandler NoResult;
        public event EventHandler OkResult;
        public event EventHandler CancelResult;


        float _opacity = 0;
        private Timer _skipTimer = new Timer();
        private Timer _letterPopTimer = new Timer();

        int _originalY;
        private int _currentLetterIndex;
        private int _letterPopResetTime = 30;
        private char[] _pauseChars = new[] {'!', '.', ',', '?'};

        Rectangle _yesBox;
        Rectangle _noBox;

        public Dialog()
        {
            _texture = GameWorld.SpriteSheet;
            _drawRectangle = new Rectangle(Main.UserResWidth / 2, 40, 600, 200);
            _sourceRectangle = new Rectangle(16 * 16, 14 * 16, 16 * 3, 16);
            _origin = new Vector2(_drawRectangle.Width / 2, _drawRectangle.Height / 2);
            _drawRectangle.X -= (int)_origin.X;

            _originalY = _drawRectangle.Y;
            _drawRectangle.Y -= 40;

            _font = ContentHelper.LoadFont("Fonts/x16");
            _popSound = new SoundFx("Sounds/message_show");
            _letterPopSound = new SoundFx("Sounds/Menu/letterPop");

            _yesBox = new Rectangle(_drawRectangle.X, _drawRectangle.Bottom, _drawRectangle.Width/2, 20);
            _noBox = new Rectangle(_drawRectangle.X + _drawRectangle.Width/2, _drawRectangle.Bottom, _drawRectangle.Width/2, 20);
        }

        /// <summary>
        /// Changes the text displayed in the dialog box and shows it.
        /// </summary>
        /// <param name="text"></param>
        public void Say(string text)
        {
            Prepare(text);
        }

        public void AskQuestion(string question)
        {
            _isQuestion = true;
            Prepare(question);
        }

        private void Prepare(string text)
        {
            _isActive = true;
            _fullText = FontHelper.WrapText(_font, text, _drawRectangle.Width - 60);
            _skipTimer.Reset();
            _letterPopTimer.Reset();
            _opacity = 0;
            _drawRectangle.Y -= 40;
            _popSound.Reset();
            _partialText = "";
            _currentLetterIndex = 0;
        }

        public void Update()
        {
            float deltaOpacity = .03f;
            if (_isActive)
            {
                _popSound.PlayOnce();
                // Checks to see if player wants to move on to the next dialog.
                if (_skipTimer.TimeElapsedInSeconds > .5)
                {
                    if (InputHelper.IsLeftMousePressed())
                    {
                        if (_isQuestion)
                        {
                            if (InputHelper.MouseRectangle.Intersects(_yesBox))
                            {
                                _isActive = false;
                                YesResult?.Invoke();
                            }
                            else if (InputHelper.MouseRectangle.Intersects(_noBox))
                            {
                                _isActive = false;
                                NoResult?.Invoke();
                            }
                        }
                        else
                        {
                            _isActive = false;
                            NextDialog?.Invoke();
                        }
                    }
                }
            }

            if (_isActive)
            {
                float velocity = (_originalY - _drawRectangle.Y) / 10;
                _drawRectangle.Y += (int)velocity;
                _opacity += deltaOpacity;
            }
            else
            {
                float velocity = -3f;
                _opacity -= deltaOpacity;
                _drawRectangle.Y += (int)velocity;
                _skipTimer.Reset();
            }

            if (_letterPopTimer.TimeElapsedInMilliSeconds > _letterPopResetTime && _currentLetterIndex < _fullText.Length)
            {
                char nextLetter = _fullText.ToCharArray()[_currentLetterIndex];
                _partialText += nextLetter;
                _currentLetterIndex++;

                bool _isPause = false;
                foreach (char pauseChar in _pauseChars)
                {
                    if (pauseChar == nextLetter)
                    {
                        _letterPopResetTime = 200;
                        _isPause = true;
                        break;
                    }
                }
                if (!_isPause || nextLetter == ' ')
                {
                    _letterPopResetTime = 30;
                    _letterPopTimer.Reset();
                    _letterPopSound.PlayNewInstanceOnce();
                    _letterPopSound.Reset();
                }
               
            }

            if (_opacity > 1)
                _opacity = 1;
            if (_opacity < 0)
                _opacity = 0;
            if (_drawRectangle.Y < -100)
                _drawRectangle.Y = -100;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_texture != null)
            spriteBatch.Draw(_texture, _drawRectangle, _sourceRectangle, Color.White * _opacity);
            spriteBatch.DrawString(_font, _partialText, new Vector2(_drawRectangle.X + 30, _drawRectangle.Y + 30), Color.Black * _opacity);
            if (_isActive && _skipTimer.TimeElapsedInSeconds > .5)
            {
                if (_isQuestion)
                {
                    FontHelper.DrawWithOutline(spriteBatch, _font, "Yes", new Vector2(_yesBox.X, _yesBox.Y), 2, Color.White, Color.Black);
                    FontHelper.DrawWithOutline(spriteBatch, _font, "No", new Vector2(_noBox.X, _noBox.Y), 2, Color.White, Color.Black);
                }
                string mousebutton = "Press left mouse button to continue";
                FontHelper.DrawWithOutline(spriteBatch, _font, mousebutton, new Vector2(_drawRectangle.Right - _font.MeasureString(mousebutton).X - 20, _drawRectangle.Bottom), 2, Color.White, Color.Black);
            }
        }

    }
}
