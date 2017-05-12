using ThereMustBeAnotherWay.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ThereMustBeAnotherWay.UI
{
    public class TextInputBox : MessageBox
    {
        Textbox _textBox;
        private string lastMessage;

        public delegate void TextInputHandler(TextInputArgs e);
        public event TextInputHandler OnInputEntered;

        /// <summary>
        /// A message box that allows the user to enter input.
        /// </summary>
        public TextInputBox()
        {
            _textBox = new Textbox(Window.DrawRectangle.X + (BezelSize), Window.DrawRectangle.Y + Window.DrawRectangle.Height / 2 - (20), Window.DrawRectangle.Width - (BezelSize*2), 10);
            _textBox.BindTo(Window.DrawRectangle);

            int buttonWidth = (40);
            int buttonHeight = (20);
            int x = Window.DrawRectangle.Center.X - buttonWidth / 2;
            int y = Window.DrawRectangle.Bottom - buttonHeight - (4);

            Button = new TextButton(new Vector2(x, y), "Ok", false);
            Button.ChangeDimensions(new Vector2(buttonWidth, buttonHeight));
            Button.BindTo(Window.DrawRectangle);
            Button.Color = new Color(196, 69, 69);
        }

        public override void Show(string message)
        {
            _textBox.Reset();
            _textBox.IsSelected = true;
            lastMessage = message;
            base.Show(message);
        }

        /// <summary>
        /// Used to make the text input box appear once again as it was before.
        /// </summary>
        public void ShowSameMessage()
        {
            Show(lastMessage);
        }

        public override void Update()
        {
            if (IsActive)
            {
                if (InputHelper.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter))
                {
                    Button_MouseClicked(Button);
                }
                else
                {
                    _textBox.Update(Window.DrawRectangle);
                    _textBox.ForceStayRelativeToContainer();
                    base.Update();
                }

                // If state changed, tell subscriber that input has been entered.
                if (!IsActive)
                {
                    OnInputEntered?.Invoke(new TextInputArgs(_textBox.Text));
                }


            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                base.Draw(spriteBatch);
                if (_textBox.DrawRectangle.Bottom < TMBAW_Game.UserResHeight)
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
