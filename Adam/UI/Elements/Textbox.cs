using Adam.Misc;
using Adam.Misc.Helpers;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;

namespace Adam.UI
{
    class Textbox : UiElement
    {
        Texture2D _white;
        BitmapFont _font;
        Timer _flashingTimer = new Timer(true);

        bool _editLineFlashing;

        KeyboardState _currentKbState;
        KeyboardState _oldKbState;

        /// <summary>
        /// Creates a textbox in the specified location that takes in input when selected.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">THe width of the textbox.</param>
        public Textbox(int x, int y, int width, int height)
        {
            _white = ContentHelper.LoadTexture("Tiles/white");
            _font = FontHelper.ChooseBestFont(height);
            DrawRectangle = new Rectangle(x, y, width, height);
        }

        public override void Update(Rectangle container)
        {
            _currentKbState = Keyboard.GetState();

            CheckIfSelected();

            if (IsSelected)
            {
                ConvertInput();
                FlashEditLine();
            }

            _oldKbState = _currentKbState;
            base.Update(container);
        }


        /// <summary>
        /// When the mouse is clicked, the textbox checks to see if it is being selected or deselected.
        /// </summary>
        private void CheckIfSelected()
        {
            if (InputHelper.IsLeftMousePressed())
            {
                if (InputHelper.GetMouseInUi().Intersects(DrawRectangle))
                {
                    _isSelected = true;
                }
                else
                {
                    _isSelected = false;
                }
            }
        }

        /// <summary>
        /// Flashes the editing line in the textbox.
        /// </summary>
        private void FlashEditLine()
        {

            if (_flashingTimer.TimeElapsedInMilliSeconds > 500)
            {
                _editLineFlashing = !_editLineFlashing;
                _flashingTimer.Reset();
            }
        }

        /// <summary>
        /// Erases contents of textbox.
        /// </summary>
        public void Reset()
        {
            Text = "";
        }

        /// <summary>
        /// Checks for keyboard input and converts it into text in the textbox.
        /// </summary>
        private void ConvertInput()
        {
            // Inputs new characters.
            InputHelper.TryLinkToKeyboardInput(ref _text, _currentKbState, _oldKbState);

            // Resets flashing line if anything is being pressed.
            if (InputHelper.IsAnyInputPressed())
            {
                _flashingTimer.Reset();
                _editLineFlashing = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int spacing = 2;
            Rectangle mouse = InputHelper.GetMouseInUi();
            spriteBatch.Draw(_white, new Rectangle(mouse.X, mouse.Y, 10, 10), Color.Black);
            spriteBatch.Draw(_white, new Rectangle(DrawRectangle.X - spacing, DrawRectangle.Y - spacing, DrawRectangle.Width + spacing * 2, DrawRectangle.Height + spacing * 2), new Color(153, 153, 153));
            spriteBatch.Draw(_white, DrawRectangle, new Color(224, 224, 224));



            // Sets the scissor rectangle so that the text is only drawn in the textbox.
            Rectangle currentScissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(DrawRectangle.X + spacing, DrawRectangle.Y, DrawRectangle.Width - spacing * 2, DrawRectangle.Height);

            if (_editLineFlashing)
            {
                spriteBatch.DrawString(_font, Text + "|", new Vector2(DrawRectangle.X + spacing, DrawRectangle.Center.Y - _font.LineHeight / 2), Color.Black);
            }
            else
            {
                spriteBatch.DrawString(_font, Text, new Vector2(DrawRectangle.X + spacing, DrawRectangle.Center.Y - _font.LineHeight / 2), Color.Black);
            }

            // Returns the scissor rectangle to its original size.
            spriteBatch.GraphicsDevice.ScissorRectangle = currentScissorRectangle;
        }

        bool _isSelected;
        /// <summary>
        /// The textbox is currently selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

        string _text;
        /// <summary>
        /// The text currently in the textbox;
        /// </summary>
        public string Text
        {
            get
            {
                if (_text == null)
                    return "";
                else return _text;
            }

            set
            {
                _text = value;
            }
        }




    }
}
