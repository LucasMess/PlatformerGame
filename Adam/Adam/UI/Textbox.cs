using Adam.Misc;
using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI
{
    class Textbox
    {
        Rectangle _bounds;
        Texture2D _black;
        SpriteFont _font;
        Timer _flashingTimer = new Timer();

        bool _editLineFlashing;

        KeyboardState _currentKbState;
        KeyboardState _oldKbState;

        /// <summary>
        /// Creates a textbox in the specified location that takes in input when selected.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">THe width of the textbox.</param>
        public Textbox(int x, int y, int width)
        {
            _black = ContentHelper.LoadTexture("Tiles/white");
            _font = ContentHelper.LoadFont("Fonts/x32");
            _bounds = new Rectangle(x - width / 2, y - _font.LineSpacing / 2, width, _font.LineSpacing);
        }

        public void Update()
        {
            _currentKbState = Keyboard.GetState();

            CheckIfSelected();

            if (IsSelected)
            {
                ConvertInput();
                FlashEditLine();
            }

            _oldKbState = _currentKbState;
        }

        /// <summary>
        /// When the mouse is clicked, the textbox checks to see if it is being selected or deselected.
        /// </summary>
        private void CheckIfSelected()
        {
            if (InputHelper.IsLeftMousePressed())
            {
                if (InputHelper.MouseRectangle.Intersects(_bounds))
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
            _flashingTimer.Increment();
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
            float opacity;
            if (IsSelected)
                opacity = .7f;
            else opacity = .3f;

            spriteBatch.Draw(_black, new Rectangle(InputHelper.MouseRectangle.X, InputHelper.MouseRectangle.Y, 10, 10), Color.Black);
            spriteBatch.Draw(_black, _bounds, Color.Black * opacity);


            // Sets the scissor rectangle so that the text is only drawn in the textbox.
            Rectangle currentScissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            spriteBatch.GraphicsDevice.ScissorRectangle = _bounds;

            if (_editLineFlashing)
            {
                spriteBatch.DrawString(_font, Text + "|", new Vector2(_bounds.X, _bounds.Y), Color.White);
            }
            else
            {
               spriteBatch.DrawString(_font, Text, new Vector2(_bounds.X, _bounds.Y), Color.White);
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
