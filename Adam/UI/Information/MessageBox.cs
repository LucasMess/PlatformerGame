﻿using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Misc.Helpers;
using ThereMustBeAnotherWay.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace ThereMustBeAnotherWay.UI
{
    /// <summary>
    ///     Displays a message to the user where they have to press OK to continue.
    /// </summary>
    public class MessageBox
    {
        protected const int BezelSize = 10;
        protected Container Window = new Container(250, 100);
        private static SoundFx _showSound = new SoundFx("Sounds/Menu/message_show");

        /// <summary>
        ///     Creates an instance of the message box that can be used to show a message to the player.
        /// </summary>
        public MessageBox()
        {
            int buttonWidth = 40;
            int buttonHeight = 20;
            int x = Window.DrawRectangle.Center.X - buttonWidth / 2;
            int y = Window.DrawRectangle.Bottom - buttonHeight - 4;

            Button = new TextButton(new Vector2(x, y), "Ok", false);
            Button.ChangeDimensions(new Vector2(buttonWidth, buttonHeight));
            Button.BindTo(Window.DrawRectangle);
            Button.Color = new Color(196, 69, 69);

            Window.ChangeStyle(Container.Style.SolidColorWithBevel);
        }

        /// <summary>
        ///     The main button of the message box.
        /// </summary>
        protected Button Button { get; set; }

        /// <summary>
        ///     Determines whether to draw the message box or not.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        ///     The message shown in the message box.
        /// </summary>
        protected string Message { get; set; } = "Error 404: Text not found.";

        protected SpriteFont Font
        {
            get { return ContentHelper.LoadFont("Fonts/x8"); }
        }

        protected void Button_MouseClicked(Button button)
        {
            IsActive = false;
            Window.Hide();
            Button.MouseClicked -= Button_MouseClicked;
        }

        /// <summary>
        ///     Shows a message to the user in a message box with an OK button.
        /// </summary>
        /// <param name="message"></param>
        public virtual void Show(string message)
        {
            Window.Show();
            _showSound.Play();
            Button.MouseClicked += Button_MouseClicked;
            var wrapped = FontHelper.WrapText(Font, message, Window.DrawRectangle.Width - BezelSize * 2);
            Message = wrapped;
            IsActive = true;
        }

        public virtual void Update()
        {
            Button.Update(Window.DrawRectangle);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Window.Draw(spriteBatch);
            spriteBatch.DrawString(Font, Message,
                new Vector2(Window.DrawRectangle.X + BezelSize, Window.DrawRectangle.Y + BezelSize), Color.Black);
            Button.Draw(spriteBatch);
        }
    }
}