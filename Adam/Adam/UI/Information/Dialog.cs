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
        string _text = "";
        StringBuilder _sb;
        SoundFx _popSound;
        ITalkable _currentSender;

        public delegate void EventHandler();
        public event EventHandler NextDialog;
        public event EventHandler CancelDialog;

        public event EventHandler YesResult;
        public event EventHandler NoResult;
        public event EventHandler OkResult;
        public event EventHandler CancelResult;


        float _opacity = 0;
        double _skipTimer;

        int _originalY;

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

            _yesBox = new Rectangle(_drawRectangle.X, _drawRectangle.Bottom, _drawRectangle.Width/2, 20);
            _noBox = new Rectangle(_drawRectangle.X + _drawRectangle.Width/2, _drawRectangle.Bottom, _drawRectangle.Width/2, 20);
        }

        public void Say(string text, ITalkable sender)
        {
            _currentSender = sender;
            sender.CurrentConversation++;

            Prepare(text);
        }

        public void Show(string text)
        {
            _currentSender = null;

            Prepare(text);
        }

        public void AskQuestion(string question)
        {
            _currentSender = null;
            _isQuestion = true;
            Prepare(question);
        }

        private void Prepare(string text)
        {
            _isActive = true;
            this._text = FontHelper.WrapText(_font, text, _drawRectangle.Width - 60);
            _skipTimer = 0;
            _opacity = 0;
            _drawRectangle.Y -= 40;
            _popSound.Reset();
        }

        public void Update(GameTime gameTime)
        {
            float deltaOpacity = .03f;
            if (_isActive)
            {
                _popSound.PlayOnce();
                _skipTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (_skipTimer > .5)
                {
                    
                    if (InputHelper.IsLeftMousePressed())
                    {
                        if (_isQuestion)
                        {
                            if (InputHelper.MouseRectangle.Intersects(_yesBox))
                            {
                                _isActive = false;
                                YesResult();
                            }
                            else if (InputHelper.MouseRectangle.Intersects(_noBox))
                            {
                                _isActive = false;
                                NoResult();
                            }
                        }
                        else
                        {
                            _isActive = false;
                            if (_currentSender != null)
                                _currentSender.OnNextDialog();
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
                _skipTimer = 0;
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
            spriteBatch.DrawString(_font, _text, new Vector2(_drawRectangle.X + 30, _drawRectangle.Y + 30), Color.Black * _opacity);
            if (_isActive && _skipTimer > .5)
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
