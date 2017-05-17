using ThereMustBeAnotherWay.Characters;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Misc.Helpers;
using ThereMustBeAnotherWay.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;

namespace ThereMustBeAnotherWay.UI.Information
{
    /// <summary>
    /// Dialog class for displaying information.
    /// 
    /// Created by: Lucas Message a long time ago.
    /// Cleaned up on 1/24/2016.
    /// Repurposed and buffed on 2/15/2017 :)
    /// </summary>
    public class Dialog
    {
        public delegate void EventHandler();

        public delegate void TextHandler(string code, int optionSelected);

        private readonly BitmapFont _font;
        private readonly SoundFx _letterPopSound;
        private readonly Timer _letterPopTimer = new Timer(true);
        private readonly char[] _pauseChars = { '!', '.', ',', '?' };
        private readonly Timer _selectBufferTimer = new Timer(true);
        private readonly Timer _skipTimer = new Timer(true);
        private int _currentLetterIndex;
        private DialogOptions _dialogOptions;
        private bool _dialogSkipPressedPressed;
        private string _fullText = "";
        private int _letterPopResetTime = 30;
        private string _nextDialogCode;
        private string _partialText = "";
        Container _dialogueContainer;
        Container _optionsContainer;

        /// <summary>
        ///     Displays text in dialog format to the player and lets them choose a response.
        /// </summary>
        public Dialog()
        {
            _dialogueContainer = new Container(850, 150);
            _dialogueContainer.SetPosition(TMBAW_Game.DefaultUiWidth / 2 - _dialogueContainer.Size.X / 2, 40);
            _dialogueContainer.ChangeStyle(Container.Style.GameUnique);

            _optionsContainer = new Container(850, 150);
            _optionsContainer.SetPosition(TMBAW_Game.DefaultUiWidth / 2 - _optionsContainer.Size.X / 2, TMBAW_Game.DefaultUiHeight - 200);
            _optionsContainer.ChangeStyle(Container.Style.GameUnique);

            var origin = new Vector2(_dialogueContainer.Size.X / 2f, _dialogueContainer.Size.Y / 2f);

            _font = ContentHelper.LoadFont("Fonts/x24");
            _letterPopSound = new SoundFx("Sounds/Menu/letterPop");
        }

        /// <summary>
        ///     Returns true if the dialog box is currently being displayed to the player.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        ///     Invoked whenever the player asks for the next dialog. It will send a code to all subscribers so that the right
        ///     dialog is showed.
        /// </summary>
        public event TextHandler NextDialog;

        /// <summary>
        ///     Changes the text displayed in the dialog box and shows it. If there are any strings in options, the player is able
        ///     to choose an answer, if there are none, the player can only continue to the next.
        /// </summary>
        /// <param name="text">The text that will be displayed.</param>
        /// <param name="nextDialogCode">
        ///     The specific and unique code that will allow the right character to respond to the player
        ///     input.
        /// </param>
        /// <param name="options">The options the player has to choose from when interacting with the character.</param>
        public void Say(string text, string nextDialogCode, string[] options, NonPlayableCharacter.Type character = NonPlayableCharacter.Type.None)
        {
            if (character != NonPlayableCharacter.Type.None)
            {

            }

            _nextDialogCode = nextDialogCode;
            _dialogOptions = new DialogOptions(options, _font, (int)_optionsContainer.Size.X - 60);
            Prepare(text);
        }

        /// <summary>
        ///     Resets the dialog box and variables so that the text can be shown.
        /// </summary>
        /// <param name="text"></param>
        private void Prepare(string text)
        {
            IsActive = true;
            _fullText = FontHelper.WrapText(_font, text, (int)_dialogueContainer.Size.X - 60);
            _skipTimer.Reset();
            _letterPopTimer.Reset();
            _partialText = "";
            _currentLetterIndex = 0;
        }

        public void Update()
        {

            if (IsActive)
            {
                // Checks to see if player wants to move on to the next dialog.
                if (_skipTimer.TimeElapsedInSeconds > .5)
                {
                    // If player presses button to skip dialog.
                    if (GameWorld.GetPlayer().IsContinueChatPressed() && !_dialogSkipPressedPressed)
                    {
                        _dialogSkipPressedPressed = true;
                        // If the dialog has not finished displaying the text, simply display the text.
                        if (IsWritingText())
                        {
                            SkipDisplayingTextCharByChar();
                        }
                        else
                        {
                            IsActive = false;
                            NextDialog?.Invoke(_nextDialogCode, _dialogOptions.SelectedOption);
                        }
                    }
                    if (!GameWorld.GetPlayer().IsContinueChatPressed()) _dialogSkipPressedPressed = false;
                }

                // Move the selector leaves around depending on keys pressed.
                if (!IsWritingText())
                {
                    if (InputHelper.IsKeyDown(Keys.S) && _selectBufferTimer.TimeElapsedInSeconds > .2)
                    {
                        _dialogOptions.IncrementSelectedIndex();
                        _selectBufferTimer.Reset();
                    }
                    if (InputHelper.IsKeyDown(Keys.W) && _selectBufferTimer.TimeElapsedInSeconds > .2)
                    {
                        _dialogOptions.DecrementSelectedIndex();
                        _selectBufferTimer.Reset();
                    }
                }
            }


            if (!IsActive)
            {
                _skipTimer.Reset();
            }

            DisplayTextCharByChar();
        }

        /// <summary>
        ///     Makes the text appear in the dialog box character by character like in Super Paper Mario.
        /// </summary>
        private void DisplayTextCharByChar()
        {
            if (_letterPopTimer.TimeElapsedInMilliSeconds > _letterPopResetTime &&
                IsWritingText())
            {
                var nextLetter = _fullText.ToCharArray()[_currentLetterIndex];
                _partialText += nextLetter;
                _currentLetterIndex++;

                var isPause = false;
                foreach (var pauseChar in _pauseChars)
                {
                    if (pauseChar == nextLetter)
                    {
                        _letterPopResetTime = 400;
                        _letterPopTimer.Reset();
                        _letterPopSound.PlayNewInstanceOnce();
                        _letterPopSound.Reset();
                        isPause = true;
                        break;
                    }
                }
                if (!isPause || nextLetter == ' ')
                {
                    _letterPopResetTime = 30;
                    _letterPopTimer.Reset();
                    _letterPopSound.PlayNewInstanceOnce();
                    _letterPopSound.Reset();
                }
            }
        }

        /// <summary>
        ///     This method will make the text appear instantly instead of character by character.
        /// </summary>
        private void SkipDisplayingTextCharByChar()
        {
            // This makes the program think it wrote all letters.
            _currentLetterIndex = _fullText.Length;
            _partialText = _fullText;
            // Plays sound one more time.
            _letterPopSound.PlayNewInstanceOnce();
            _letterPopSound.Reset();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                // Drawing for non-player dialog box.
                _dialogueContainer.Draw(spriteBatch);
                spriteBatch.DrawString(_font, _partialText,
                    new Vector2(_dialogueContainer.GetPosition().X + 30, _dialogueContainer.GetPosition().Y + 30),
                    Color.Black);

                if (!IsWritingText())
                {
                    // Displays options to choose from when the whole text has been displayed.
                    //spriteBatch.Draw(GameWorld.UiSpriteSheet, _playerDialogBox, _dialogBoxSourceRectangle, Color.White);
                    if (_dialogOptions.Count > 0)
                    {
                        _optionsContainer.Draw(spriteBatch);
                        _dialogOptions.Draw(spriteBatch, _font, (int)_optionsContainer.GetPosition().X + (int)_optionsContainer.Size.X / 2, (int)_optionsContainer.GetPosition().Y + 30);
                    }
                    else
                    {
                        // Displays default text to continue if there are no options.
                        string text = "Press space to continue";
                        spriteBatch.DrawString(_font, text,
                            new Vector2((int)_dialogueContainer.GetPosition().X + (int)_dialogueContainer.Size.X - _font.MeasureString(text).X - 5, (int)_dialogueContainer.GetPosition().Y + (int)_dialogueContainer.Size.Y - 30),
                            Color.Black);
                    }
                }
            }
        }

        /// <summary>
        ///     Returns true if the dialog is not done displaying all the text in the dialog box.
        /// </summary>
        /// <returns></returns>
        private bool IsWritingText()
        {
            return _currentLetterIndex < _fullText.Length;
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

        public DialogOptions(string[] options, BitmapFont font, int maxLineWidth)
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
                 _heights[i] = font.LineHeight * lineNumber[i];

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

        public void Draw(SpriteBatch spriteBatch, BitmapFont font, int centerX, int startingY)
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
                spriteBatch.DrawString(font, _options[i], new Vector2(x, y), Color.Black);
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
        public void DrawAroundText(SpriteBatch spriteBatch, string text, BitmapFont font, Vector2 positionOfText)
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