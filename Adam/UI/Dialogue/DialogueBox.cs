using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThereMustBeAnotherWay.Graphics;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Misc.Helpers;
using ThereMustBeAnotherWay.UI.Elements;

namespace ThereMustBeAnotherWay.UI.Dialogue
{
    public class DialogueBox : UiElement
    {
        private const int LINE_WIDTH = 800;
        private const int BOX_PADDING = 25;
        private const int BOX_HEIGHT = 150;
        private const int BOX_MARGIN = 50;

        private const int LETTER_POP_INTERVAL = 60;
        private const int LETTER_POP_INTERVAL_PAUSE = 400;

        private readonly char[] PAUSE_CHARS = { '!', '.', ',', '?' };

        private Container _container;
        private SpriteFont _smallFont;
        private SpriteFont _bigFont;
        private string _wrappedText;
        private string _singleLineText;
        private string _characterName;

        // Variables used for popping letters one at a time in the dialogue box.
        private GameTimer _letterPopTimer = new GameTimer();
        private SoundFx _letterPopSound;
        private string _partialText;
        private int _currentLetterIndex = 0;
        private int _currentLetterPopInterval = 0;
        
        /// <summary>
        /// Returns true if the dialogue box is currently being shown to the user.
        /// </summary>
        public bool IsActive { get; private set; }

        public DialogueBox()
        {
            _container = new Container(LINE_WIDTH + BOX_PADDING * 2, 150);
            _container.ChangeStyle(Container.Style.GameUnique);
            _container.Opacity = .8f;
            _smallFont = ContentHelper.LoadFont("Fonts/x16");
            _bigFont = ContentHelper.LoadFont("Fonts/x32");
            _letterPopSound = new SoundFx("Sounds/Menu/letterPop");

            GraphicsRenderer.OnResolutionChanged += SetShownAndHiddenPositions;
            SetShownAndHiddenPositions(0, 0);
        }

        /// <summary>
        /// Calculate the shown and hidden positions based on the current resolution.
        /// </summary>
        /// <param name="width">Current resolution width.</param>
        /// <param name="height">Current resolution height.</param>
        private void SetShownAndHiddenPositions(int width, int height)
        {
            // Set the positions again due to changing resolutions.
            _container.SetShownPosition(new Vector2(TMBAW_Game.UserResWidth / 2, TMBAW_Game.UserResHeight - BOX_HEIGHT / 2 - BOX_MARGIN));
            _container.SetHiddenPosition(new Vector2(TMBAW_Game.UserResWidth / 2, TMBAW_Game.UserResHeight + BOX_HEIGHT / 2));
            _container.ResetPosition();
        }

        public void Show(string characterName, string text)
        {            
            _singleLineText = text;
            _container.Show();
            _characterName = characterName;
            Prepare(text);
        }

        /// <summary>
        /// Wraps the text.
        /// </summary>
        /// <param name="text"></param>
        private void Prepare(string text)
        {
            IsActive = true;
            _wrappedText = FontHelper.WrapText(_smallFont, text, LINE_WIDTH);
            _letterPopTimer.Reset();
            _partialText = "";
            _currentLetterIndex = 0;
            _currentLetterPopInterval = LETTER_POP_INTERVAL;
        }

        public void Update()
        {
            if (IsActive)
            {
                DisplayTextCharByChar();
            }
        }

        /// <summary>
        /// Shows one character at a time.
        /// </summary>
        private void DisplayTextCharByChar()
        {
            _letterPopTimer.Increment();
            if (_letterPopTimer.TimeElapsedInMilliSeconds > _currentLetterPopInterval &&
                IsWritingText())
            {
                var nextLetter = _wrappedText.ToCharArray()[_currentLetterIndex];
                _partialText += nextLetter;
                _currentLetterIndex++;

                var isPause = false;
                foreach (var pauseChar in PAUSE_CHARS)
                {
                    if (pauseChar == nextLetter)
                    {
                        _currentLetterPopInterval = LETTER_POP_INTERVAL_PAUSE;
                        _letterPopTimer.Reset();
                        _letterPopSound.PlayNewInstanceOnce();
                        _letterPopSound.Reset();
                        isPause = true;
                        break;
                    }
                }
                if (!isPause || nextLetter == ' ')
                {
                    _currentLetterPopInterval = LETTER_POP_INTERVAL;
                    _letterPopTimer.Reset();
                    _letterPopSound.PlayNewInstanceOnce();
                    _letterPopSound.Reset();
                }
            }
        }

        /// <summary>
        /// This method will make the text appear instantly instead of character by character.
        /// </summary>
        public void SkipDialogue()
        {
            if (IsWritingText())
            {
                // This makes the program think it wrote all letters.
                _currentLetterIndex = _singleLineText.Length;
                _partialText = _wrappedText;
                // Plays sound one more time.
                _letterPopSound.PlayNewInstanceOnce();
                _letterPopSound.Reset();
            }
            else
            {
                IsActive = false;
                _container.Hide();
            }
        }

        /// <summary>
        /// Returns true if the dialog is not done displaying all the text in the dialog box.
        /// </summary>
        /// <returns></returns>
        private bool IsWritingText()
        {
            return _currentLetterIndex < _wrappedText.Length;
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                // Drawing for non-player dialog box.
                _container.Draw(spriteBatch);
                spriteBatch.DrawString(_smallFont, _partialText,
                    new Vector2(_container.GetPosition().X + BOX_PADDING, _container.GetPosition().Y + BOX_PADDING),
                    new Color(51, 51, 51));
                FontHelper.DrawWithOutline(spriteBatch, _bigFont, _characterName, _container.GetPosition() + new Vector2(20, -_bigFont.MeasureString(_characterName).Y), 2, Color.White, Color.Black);

                //if (!IsWritingText())
                //{
                //    // Displays options to choose from when the whole text has been displayed.
                //    //spriteBatch.Draw(GameWorld.UiSpriteSheet, _playerDialogBox, _dialogBoxSourceRectangle, Color.White);
                //    if (_dialogOptions.Count > 0)
                //    {
                //        _optionsContainer.Draw(spriteBatch);
                //        _dialogOptions.Draw(spriteBatch, _font, (int)_optionsContainer.GetPosition().X + (int)_optionsContainer.Size.X / 2, (int)_optionsContainer.GetPosition().Y + 30);
                //    }
                //    else
                //    {
                //        // Displays default text to continue if there are no options.
                //        string text = "Press space to continue";
                //        spriteBatch.DrawString(_font, text,
                //            new Vector2((int)_dialogueContainer.GetPosition().X + (int)_dialogueContainer.Size.X - _font.MeasureString(text).X - 5, (int)_dialogueContainer.GetPosition().Y + (int)_dialogueContainer.Size.Y - 30),
                //            new Color(51, 51, 51));
                //    }
                //}
            }
        }


        /// <summary>
        ///     Helper class to organize the options and display them correctly.
        /// </summary>
        public class DialogOptions
        {
            private readonly LeafSelectors _leafSelectors = new LeafSelectors();
            private readonly SoundFx _selectorSound = new SoundFx("Sounds/Menu/cursor_style_2");
            private readonly string[] _options;
            private readonly float[] _heights;

            public DialogOptions(string[] options, SpriteFont font, int maxLineWidth)
            {
                _options = options ?? new string[0];

                // Wrap the options so that they fit inside the box.
                var i = 0;
                var lineNumber = new int[_options.Length];
                _heights = new float[_options.Length];
                var wrapped = new string[_options.Length];
                foreach (var option in _options)
                {
                    wrapped[i] = FontHelper.WrapText(font, option, maxLineWidth);

                    // Counts how many lines there are.
                    lineNumber[i] = wrapped[i].Split('\n').Length;

                    // Determines the height of each element.
                    _heights[i] = font.LineSpacing * lineNumber[i];

                    i++;
                }

                _options = wrapped;
            }

            /// <summary>
            ///     The options being hovered by the player currently.
            /// </summary>
            public int SelectedOption { get; private set; }

            /// <summary>
            ///     The number of options available currently.
            /// </summary>
            public int Count => _options?.Length ?? 0;

            public void IncrementSelectedIndex()
            {
                SelectedOption++;
                if (SelectedOption >= _options.Length)
                {
                    SelectedOption = 0;
                }
                _selectorSound.PlayNewInstanceOnce();
                _selectorSound.Reset();
            }

            public void DecrementSelectedIndex()
            {
                SelectedOption--;
                if (SelectedOption < 0)
                    SelectedOption = _options.Length - 1;
                _selectorSound.PlayNewInstanceOnce();
                _selectorSound.Reset();
            }

            public void Draw(SpriteBatch spriteBatch, SpriteFont font, int centerX, int startingY)
            {
                for (var i = 0; i < _options.Length; i++)
                {
                    float deltaY = 0;
                    for (var h = 0; h < i; h++)
                    {
                        deltaY += _heights[h] + 10;
                    }

                    var x = centerX - font.MeasureString(_options[i]).X / 2;
                    var y = startingY + deltaY;
                    spriteBatch.DrawString(font, _options[i], new Vector2(x, y), new Color(51, 51, 51));
                    if (SelectedOption == i)
                    {
                        _leafSelectors.DrawAroundText(spriteBatch, _options[i], font, new Vector2(x, y));
                    }
                }
            }
        }

        /// <summary>
        ///     The leaves that appear around things to select them without a mouse.
        /// </summary>
        public class LeafSelectors
        {
            private const int Size = 24;
            private readonly Rectangle _sourceRectangle = new Rectangle(128, 288, 16, 16);
            private readonly Texture2D _texture = GameWorld.SpriteSheet;
            private Rectangle _drawRectangle;

            public void Animate()
            {
                //TODO: Add animation to selector leaves.
            }

            /// <summary>
            ///     Wraps the text with leaves around them.
            /// </summary>
            /// <param name="spriteBatch"></param>
            /// <param name="text"></param>
            /// <param name="font"></param>
            /// <param name="positionOfText"></param>
            public void DrawAroundText(SpriteBatch spriteBatch, string text, SpriteFont font, Vector2 positionOfText)
            {
                _drawRectangle = new Rectangle((int)positionOfText.X - Size - Size / 2, (int)positionOfText.Y + 4, Size,
                    Size);
                spriteBatch.Draw(_texture, _drawRectangle, _sourceRectangle, Color.White);

                _drawRectangle.X += Size + (int)font.MeasureString(text).X + 4;
                spriteBatch.Draw(_texture, _drawRectangle, _sourceRectangle, Color.White, 0, Vector2.Zero,
                    SpriteEffects.FlipHorizontally, 0);
            }
        }
    }
}
