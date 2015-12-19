using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    public class LoadingScreen
    {
        Texture2D _background, _customObject, _circle;
        Rectangle _rectangle, _customRect, _circleRect;
        GameTime _gameTime;
        Vector2 _monitorRes;
        SpriteFont _font;
        Random _randGen = new Random();

        bool _hasAnimated, _hasGoneUp;
        bool _textChosen;
        public bool IsReady;

        string _loadingText, _randomText, _normalLoadingText;

        double _bufferTimer, _textTimer, _loadingTextTimer;
        float _velocity;
        float _rotation;
        int _count = 0, _maxCount;

        public LoadingScreen(Vector2 monitorRes, ContentManager content)
        {
            _background = ContentHelper.LoadTexture("Backgrounds/Loading Screen/loading_background");
            _customObject = ContentHelper.LoadTexture("Backgrounds/Loading Screen/loading_tree");
            _circle = ContentHelper.LoadTexture("Backgrounds/Loading Screen/loading_circle");
            _font = content.Load<SpriteFont>("Fonts/x64");

            _randomText = "";
            _normalLoadingText = "";
            _loadingText = "";

            this._monitorRes = monitorRes;

            _rectangle = new Rectangle(0, 0, (int)monitorRes.X, (int)monitorRes.Y);
            _customRect = new Rectangle(0, (int)monitorRes.Y, (int)monitorRes.X, (int)monitorRes.Y);
            _circleRect = new Rectangle((int)monitorRes.X / 2, (int)monitorRes.Y / 2 - 100, _circle.Width, _circle.Height);
        }

        public void Update(GameTime gameTime)
        {
            this._gameTime = gameTime;
            UpdateTimers();
            LoadingDots();

            _rotation += .2f;

            if (!_textChosen)
                ChooseText();

            if (!_hasAnimated)
            {
                if (!_hasGoneUp)
                {
                    _customRect.Y += (int)_velocity;
                    _velocity = -10f;
                    if (_customRect.Y < 10)
                    {
                        _hasGoneUp = true;
                        _velocity = 0;
                    }
                }
                else
                {
                    _customRect.Y += (int)_velocity;
                    _velocity = 1f;
                    if (_customRect.Y >= 30)
                    {
                        _hasAnimated = true;
                        _customRect.Y = 30;
                    }
                }
            }

            if (_velocity < -10)
                _velocity = -10;
            if (_velocity > 5)
                _velocity = 5;

        }

        public void UpdateTimers()
        {
            _bufferTimer += _gameTime.ElapsedGameTime.TotalSeconds;

            if (_bufferTimer > 0)
                IsReady = true;

            _textTimer += _gameTime.ElapsedGameTime.TotalSeconds;

            if (_textTimer > 3)
            {
                _textTimer = 0;
                _textChosen = false;
            }

            _loadingTextTimer += _gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_loadingTextTimer > 250)
            {
                _count++;
                _loadingTextTimer = 0;
                if (_count > _maxCount)
                    _count = 0;
            }

        }

        public void LoadingDots()
        {
            switch (_count)
            {
                case 0:
                    _loadingText = "Loading";
                    break;
                case 1:
                    _loadingText = "Loading.";
                    break;
                case 2:
                    _loadingText = "Loading..";
                    break;
                case 3:
                    _loadingText = "Loading...";
                    break;
            }
            _maxCount = 3;
            _normalLoadingText = "Loading...";
        }

        public void Restart()
        {
            IsReady = false;
            _hasAnimated = false;
            _hasGoneUp = false;
            _rectangle = new Rectangle(0, 0, (int)_monitorRes.X, (int)_monitorRes.Y);
            _customRect = new Rectangle(0, (int)_monitorRes.Y, (int)_monitorRes.X, (int)_monitorRes.Y);
            _bufferTimer = 0;
            _textTimer = 10000;
        }

        public void ChooseText()
        {
            switch (_randGen.Next(0, 36))
            {
                case 0:
                    _randomText = "Hello there.";
                    break;
                case 1:
                    _randomText = "Walnuts are yummy.";
                    break;
                case 2:
                    _randomText = "Stephen smells like roses.";
                    break;
                case 3:
                    _randomText = "Salmon is a good source of protein.";
                    break;
                case 4:
                    _randomText = "Indeeeeeeeeeed.";
                    break;
                case 5:
                    _randomText = "Loading the loading screen.";
                    break;
                case 6:
                    _randomText = "Reticulating spines.";
                    break;
                case 7:
                    _randomText = "Error 404: Level not found.";
                    break;
                case 8:
                    _randomText = "You are beautiful.";
                    break;
                case 9:
                    _randomText = "Trying to find lost keys.";
                    break;
                case 10:
                    _randomText = "Deleting system memory.";
                    break;
                case 11:
                    _randomText = "Windows update incoming.";
                    break;
                case 12:
                    _randomText = "You have lost connection to the internet.";
                    break;
                case 13:
                    _randomText = "Lighting the darkness.";
                    break;
                case 14:
                    _randomText = "Moving immovable objects.";
                    break;
                case 15:
                    _randomText = "Stopping unstoppable force.";
                    break;
                case 16:
                    _randomText = "Nerfing Irelia.";
                    break;
                case 17:
                    _randomText = "Meow.";
                    break;
                case 18:
                    _randomText = "Upgrading antiviruses.";
                    break;
                case 19:
                    _randomText = "Opening Internet Explorer.";
                    break;
                case 20:
                    _randomText = "Putting out the firewall.";
                    break;
                case 21:
                    _randomText = "Giving Satan a massage.";
                    break;
                case 22:
                    _randomText = "Doing Satan's pedicure.";
                    break;
                case 23:
                    _randomText = "Far Lands or Bust!";
                    break;
                case 24:
                    _randomText = "Shaving bears.";
                    break;
                case 25:
                    _randomText = "Drinking tea.";
                    break;
                case 26:
                    _randomText = "Starting pillow fight.";
                    break;
                case 27:
                    _randomText = "Reloading the unloadable.";
                    break;
                case 28:
                    _randomText = "Checking out pictures folder.";
                    break;
                case 29:
                    _randomText = "Taking a break.";
                    break;
                case 30:
                    _randomText = "Loading assets.";
                    break;
                case 31:
                    _randomText = "Googling for solution.";
                    break;
                case 32:
                    _randomText = "Oh oh, we might blue screen!";
                    break;
                case 33:
                    _randomText = "Deleting user files.";
                    break;
                case 34:
                    _randomText = "Drinking water.";
                    break;
                case 35:
                    _randomText = "Pretending to load.";
                    break;
            }
            _textChosen = true;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_background, _rectangle, Color.White);
            spriteBatch.Draw(_customObject, _customRect, Color.White);

            Vector2 randomTextOrigin = new Vector2(_font.MeasureString(_randomText).X / 2, _font.LineSpacing / 2);
            Vector2 loadingTextOrigin = new Vector2(_font.MeasureString(_normalLoadingText).X / 2, _font.LineSpacing / 2);
            Vector2 circleOrigin = new Vector2(_circle.Width / 2, _circle.Height / 2);

            spriteBatch.DrawString(_font, _randomText, _monitorRes / 2, new Color(51, 63, 80), 0, randomTextOrigin, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(_circle, new Vector2(_circleRect.X, _circleRect.Y), null, Color.White, _rotation, circleOrigin, .08f, SpriteEffects.None, 0);
            spriteBatch.DrawString(_font, _loadingText, new Vector2(_monitorRes.X / 2, _monitorRes.Y / 2 - 200), new Color(51, 63, 80), 0, loadingTextOrigin, 1, SpriteEffects.None, 0);
        }

    }
}
