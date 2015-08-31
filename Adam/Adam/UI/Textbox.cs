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
        Rectangle bounds;
        Texture2D black;
        SpriteFont font;
        Timer flashingTimer = new Timer();

        bool editLineFlashing;

        KeyboardState currentKBState;
        KeyboardState oldKBState;

        /// <summary>
        /// Creates a textbox in the specified location that takes in input when selected.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">THe width of the textbox.</param>
        public Textbox(int x, int y, int width)
        {
            black = ContentHelper.LoadTexture("Tiles/white");
            font = ContentHelper.LoadFont("Fonts/objectiveText");
            bounds = new Rectangle(x,y,width,font.LineSpacing);
        }

        public void Update()
        {
            currentKBState = Keyboard.GetState();

            CheckIfSelected();

            if (IsSelected)
            {
                ConvertInput();
                FlashEditLine();
            }

            oldKBState = currentKBState;
        }

        /// <summary>
        /// When the mouse is clicked, the textbox checks to see if it is being selected or deselected.
        /// </summary>
        private void CheckIfSelected()
        {
            if (InputHelper.IsLeftMousePressed())
            {
                if (InputHelper.MouseRectangle.Intersects(bounds))
                {
                    isSelected = true;
                }
                else
                {
                    isSelected = false;
                }
            }
        }

        /// <summary>
        /// Flashes the editing line in the textbox.
        /// </summary>
        private void FlashEditLine()
        {
            flashingTimer.Increment();
            if (flashingTimer.TimeElapsedInMilliSeconds > 500)
            {
                editLineFlashing = !editLineFlashing;
                flashingTimer.Reset();
            }
        }

        /// <summary>
        /// Checks for keyboard input and converts it into text in the textbox.
        /// </summary>
        private void ConvertInput()
        {
            // Inputs new characters.
            char newChar;
            if (InputHelper.TryConvertKeyboardInput(currentKBState, oldKBState, out newChar))
                text += newChar;

            // Deletes characters.
            if (InputHelper.IsBackSpacePressed(currentKBState,oldKBState))
            {
                if (Text.Length != 0)
                    text = text.Remove(text.Length - 1, 1);
            }

            // Resets flashing line if anything is being pressed.
            if (InputHelper.IsAnyInputPressed())
            {
                flashingTimer.Reset();
                editLineFlashing = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float opacity;
            if (IsSelected)
                opacity = .7f;
            else opacity = .3f;

            spriteBatch.Draw(black, new Rectangle(InputHelper.MouseRectangle.X, InputHelper.MouseRectangle.Y, 10, 10), Color.Black);
            spriteBatch.Draw(black, bounds, Color.Black * opacity);


            // Sets the scissor rectangle so that the text is only drawn in the textbox.
            Rectangle currentScissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            spriteBatch.GraphicsDevice.ScissorRectangle = bounds;

            if (editLineFlashing)
            {
                spriteBatch.DrawString(font, Text + "|", new Vector2(bounds.X, bounds.Y), Color.White);
            }
            else
            {
                spriteBatch.DrawString(font, Text, new Vector2(bounds.X, bounds.Y), Color.White);
            }

            // Returns the scissor rectangle to its original size.
            spriteBatch.GraphicsDevice.ScissorRectangle = currentScissorRectangle;
        }

        bool isSelected;
        /// <summary>
        /// The textbox is currently selected.
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
        }

        string text;
        /// <summary>
        /// The text currently in the textbox;
        /// </summary>
        public string Text
        {
            get
            {
                if (text == null)
                    return "";
                else return text;
            }

            private set
            {
                text = value;
            }
        }




    }
}
