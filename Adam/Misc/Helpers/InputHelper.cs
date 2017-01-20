using Adam.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Adam
{
    public static class InputHelper
    {
        static Timer _keyPressedTimer = new Timer(true);

        public static bool IsAnyInputPressed()
        {
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.A) ||
                keyboard.IsKeyDown(Keys.W) ||
                keyboard.IsKeyDown(Keys.S) ||
                keyboard.IsKeyDown(Keys.D) ||
                keyboard.IsKeyDown(Keys.Space))
            {
                return true;
            }
            else return false;
        }

        public static bool IsKeyDown(Keys key)
        {
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(key))
                return true;
            else return false;
        }
        public static bool IsKeyUp(Keys key)
        {
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyUp(key))
                return true;
            else return false;
        }
        public static bool IsRightMousePressed()
        {
            MouseState mouse = Mouse.GetState();
            if (mouse.RightButton == ButtonState.Pressed)
                return true;
            else return false;
        }
        public static bool IsMiddleMousePressed()
        {
            MouseState mouse = Mouse.GetState();
            if (mouse.MiddleButton == ButtonState.Pressed)
                return true;
            else return false;
        }
        public static bool IsLeftMousePressed()
        {
            MouseState mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Pressed)
                return true;
            else return false;
        }
        public static bool IsLeftMouseReleased()
        {
            MouseState mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Released)
                return true;
            else return false;
        }

        static MouseState MouseState { get; set; }

        /// <summary>
        /// Returns the rectangle of the mouse when screen is scaled.
        /// </summary>
        public static Rectangle GetMouseInUi()
        {
            MouseState = Mouse.GetState();
            return new Rectangle((int)(MouseState.X * AdamGame.UiWidthRatio), (int)(MouseState.Y * AdamGame.UiHeightRatio), 1, 1);
        }

        ///// <summary>
        ///// Returns the rectangle of the mouse in screen coordinates.
        ///// </summary>
        //public static Rectangle MouseRectangle
        //{
        //    get
        //    {
        //        MouseState mouseState = Mouse.GetState();
        //        Rectangle mouseRect = new Rectangle((int)(mouseState.X), (int)(mouseState.Y), 1, 1);
        //        return mouseRect;
        //    }
        //}

        /// <summary>
        /// Returns the rectangle of the mouse in GameWorld coordinates.
        /// </summary>
        public static Rectangle GetMouseRectGameWorld()
        {
            MouseState = Mouse.GetState();
            Rectangle rectangle = new Rectangle((int)(MouseState.X * AdamGame.WidthRatio), (int)(MouseState.Y * AdamGame.HeightRatio), 1, 1);
            rectangle.X = (int)(rectangle.X / AdamGame.Camera.GetZoom());
            rectangle.Y = (int)(rectangle.Y / AdamGame.Camera.GetZoom());
            rectangle.X -= (int)(AdamGame.Camera.LastCameraLeftCorner.X);
            rectangle.Y -= (int)(AdamGame.Camera.LastCameraLeftCorner.Y);
            return rectangle;
        }

        /// <summary>
        /// Returns true if the backspace key is pressed.
        /// </summary>
        /// <returns></returns>
        public static bool IsBackSpacePressed(KeyboardState newKb, KeyboardState oldKb)
        {
            // Prevents multiple deletion.
            if (oldKb == newKb)
                return false;
            if (IsKeyDown(Keys.Back))
                return true;
            else return false;
        }



        /// <summary>
        /// Tries to convert keyboard input to characters and prevents repeatedly returning the 
        /// same character if a key was pressed last frame, but not yet unpressed this frame.
        /// 
        /// Code courtesy of: http://roy-t.nl/index.php/2010/02/11/code-snippet-converting-keyboard-input-to-text-in-xna/
        /// </summary>
        /// <param name="keyboard">The current KeyboardState</param>
        /// <param name="oldKeyboard">The KeyboardState of the previous frame</param>
        /// <param name="key">When this method returns, contains the correct character if conversion succeeded.
        /// Else contains the null, (000), character.</param>
        /// <returns>True if conversion was successful</returns>
        public static bool TryLinkToKeyboardInput(ref string text, KeyboardState keyboard, KeyboardState oldKeyboard)
        {
            Keys[] keysPressed = keyboard.GetPressedKeys();
            List<char> charactersList = new List<char>();

            if (keysPressed.Length > 0)
            {
                for (int i = 0; i < keysPressed.Length; i++)
                {
                    if (!oldKeyboard.IsKeyDown(keysPressed[i]) || _keyPressedTimer.TimeElapsedInMilliSeconds > 500)
                    {
                        char tempChar;
                        FindPressedKey(keysPressed[i], out tempChar);
                        if (tempChar != 0)
                            charactersList.Add(tempChar);

                        // If backspace is pressed, delete last character.
                        if (InputHelper.IsKeyDown(Keys.Back))
                        {
                            if (text.Length != 0)
                                text = text.Remove(text.Length - 1, 1);
                        }
                    }
                }

                // Check for long key presses.
                if (keysPressed.Length == 1)
                {
                }
                else
                {
                    _keyPressedTimer.Reset();
                }
            }
            else
            {
                // No input pressed.
                _keyPressedTimer.Reset();
                return false;
            }

            // Add pressed characters to string.
            for (int i = 0; i < charactersList.Count; i++)
            {
                text += charactersList[i];
            }



            return true;

        }

        /// <summary>
        /// Converts Keys class enum into char.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        public static bool FindPressedKey(Keys key, out char character)
        {
            bool shift = IsKeyDown(Keys.LeftShift) || IsKeyDown(Keys.RightShift);

            switch (key)
            {
                //Alphabet keys
                case Keys.A: if (shift) { character = 'A'; } else { character = 'a'; } return true;
                case Keys.B: if (shift) { character = 'B'; } else { character = 'b'; } return true;
                case Keys.C: if (shift) { character = 'C'; } else { character = 'c'; } return true;
                case Keys.D: if (shift) { character = 'D'; } else { character = 'd'; } return true;
                case Keys.E: if (shift) { character = 'E'; } else { character = 'e'; } return true;
                case Keys.F: if (shift) { character = 'F'; } else { character = 'f'; } return true;
                case Keys.G: if (shift) { character = 'G'; } else { character = 'g'; } return true;
                case Keys.H: if (shift) { character = 'H'; } else { character = 'h'; } return true;
                case Keys.I: if (shift) { character = 'I'; } else { character = 'i'; } return true;
                case Keys.J: if (shift) { character = 'J'; } else { character = 'j'; } return true;
                case Keys.K: if (shift) { character = 'K'; } else { character = 'k'; } return true;
                case Keys.L: if (shift) { character = 'L'; } else { character = 'l'; } return true;
                case Keys.M: if (shift) { character = 'M'; } else { character = 'm'; } return true;
                case Keys.N: if (shift) { character = 'N'; } else { character = 'n'; } return true;
                case Keys.O: if (shift) { character = 'O'; } else { character = 'o'; } return true;
                case Keys.P: if (shift) { character = 'P'; } else { character = 'p'; } return true;
                case Keys.Q: if (shift) { character = 'Q'; } else { character = 'q'; } return true;
                case Keys.R: if (shift) { character = 'R'; } else { character = 'r'; } return true;
                case Keys.S: if (shift) { character = 'S'; } else { character = 's'; } return true;
                case Keys.T: if (shift) { character = 'T'; } else { character = 't'; } return true;
                case Keys.U: if (shift) { character = 'U'; } else { character = 'u'; } return true;
                case Keys.V: if (shift) { character = 'V'; } else { character = 'v'; } return true;
                case Keys.W: if (shift) { character = 'W'; } else { character = 'w'; } return true;
                case Keys.X: if (shift) { character = 'X'; } else { character = 'x'; } return true;
                case Keys.Y: if (shift) { character = 'Y'; } else { character = 'y'; } return true;
                case Keys.Z: if (shift) { character = 'Z'; } else { character = 'z'; } return true;

                //Decimal keys
                case Keys.D0: if (shift) { character = ')'; } else { character = '0'; } return true;
                case Keys.D1: if (shift) { character = '!'; } else { character = '1'; } return true;
                case Keys.D2: if (shift) { character = '@'; } else { character = '2'; } return true;
                case Keys.D3: if (shift) { character = '#'; } else { character = '3'; } return true;
                case Keys.D4: if (shift) { character = '$'; } else { character = '4'; } return true;
                case Keys.D5: if (shift) { character = '%'; } else { character = '5'; } return true;
                case Keys.D6: if (shift) { character = '^'; } else { character = '6'; } return true;
                case Keys.D7: if (shift) { character = '&'; } else { character = '7'; } return true;
                case Keys.D8: if (shift) { character = '*'; } else { character = '8'; } return true;
                case Keys.D9: if (shift) { character = '('; } else { character = '9'; } return true;

                //Decimal numpad keys
                case Keys.NumPad0: character = '0'; return true;
                case Keys.NumPad1: character = '1'; return true;
                case Keys.NumPad2: character = '2'; return true;
                case Keys.NumPad3: character = '3'; return true;
                case Keys.NumPad4: character = '4'; return true;
                case Keys.NumPad5: character = '5'; return true;
                case Keys.NumPad6: character = '6'; return true;
                case Keys.NumPad7: character = '7'; return true;
                case Keys.NumPad8: character = '8'; return true;
                case Keys.NumPad9: character = '9'; return true;

                //Special keys
                case Keys.OemTilde: if (shift) { character = '~'; } else { character = '`'; } return true;
                case Keys.OemSemicolon: if (shift) { character = ':'; } else { character = ';'; } return true;
                case Keys.OemQuotes: if (shift) { character = '"'; } else { character = '\''; } return true;
                case Keys.OemQuestion: if (shift) { character = '?'; } else { character = '/'; } return true;
                case Keys.OemPlus: if (shift) { character = '+'; } else { character = '='; } return true;
                case Keys.OemPipe: if (shift) { character = '|'; } else { character = '\\'; } return true;
                case Keys.OemPeriod: if (shift) { character = '>'; } else { character = '.'; } return true;
                case Keys.OemOpenBrackets: if (shift) { character = '{'; } else { character = '['; } return true;
                case Keys.OemCloseBrackets: if (shift) { character = '}'; } else { character = ']'; } return true;
                case Keys.OemMinus: if (shift) { character = '_'; } else { character = '-'; } return true;
                case Keys.OemComma: if (shift) { character = '<'; } else { character = ','; } return true;
                case Keys.Space: character = ' '; return true;
            }
            character = (char)0;
            return false;
        }

    }
}
