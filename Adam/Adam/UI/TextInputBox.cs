using Adam.UI.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI
{
    public class TextInputBox : MessageBox
    {
        Textbox _textBox;

        public delegate void TextInputHandler(TextInputArgs e);
        public event TextInputHandler OnInputEntered;

        /// <summary>
        /// A message box that allows the user to enter input.
        /// </summary>
        public TextInputBox()
        {
            _textBox = new Textbox(DrawRectangle.X + DrawRectangle.Width / 2, DrawRectangle.Y + DrawRectangle.Height / 2, DrawRectangle.Width - BezelSize * 2);
            Button = new OkButton(DrawRectangle);
        }

        public override void Show(string message)
        {
            _textBox.Reset();
            _textBox.IsSelected = true;
            base.Show(message);
        }

        public override void Update()
        {
            if (IsActive)
            {
                if (InputHelper.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter))
                {
                    Button_MouseClicked();
                }
                else
                {
                    _textBox.Update();
                    base.Update();
                }

                // If state changed, tell subscriber that input has been entered.
                if (!IsActive)
                {
                    OnInputEntered(new TextInputArgs(_textBox.Text));
                }


            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                base.Draw(spriteBatch);
                _textBox.Draw(spriteBatch);
            }
        }

        public void SetTextTo(string text)
        {
            _textBox.Text = text;
        }
    }

    public class TextInputArgs : EventArgs
    {
        public TextInputArgs(string input)
        {
            Input = input;
        }

        public string Input
        {
            get; set;
        }
    }
}
