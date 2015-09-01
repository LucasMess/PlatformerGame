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
        Textbox textBox;

        /// <summary>
        /// A message box that allows the user to enter input.
        /// </summary>
        public TextInputBox()
        {
            textBox = new Textbox(DrawRectangle.X + DrawRectangle.Width / 2, DrawRectangle.Y + DrawRectangle.Height / 2, DrawRectangle.Width - BezelSize * 2);
            Button = new OKButton(DrawRectangle, this);
        }

        public override void Show(string message)
        {
            textBox.Reset();
            textBox.IsSelected = true;
            base.Show(message);
        }

        public override void Update()
        {
            if (IsActive)
            {
                if (InputHelper.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter))
                    IsActive = false;
                textBox.Update();
                base.Update();
            }
        }

        public string GetTextInput()
        {
            return textBox.Text;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                base.Draw(spriteBatch);
                textBox.Draw(spriteBatch);
            }
        }
    }
}
