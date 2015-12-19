using Adam.Misc;
using Adam.Misc.Helpers;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam.UI
{
    /// <summary>
    /// Displays a message to the user where they have to press OK to continue.
    /// </summary>
    public class MessageBox
    {
        SoundFx _attentionSound;
        protected const int BezelSize = 25;

        /// <summary>
        /// The main button of the message box.
        /// </summary>
        protected Button Button
        {
            get; set;
        }

        /// <summary>
        /// Determines whether to draw the message box or not.
        /// </summary>
        public bool IsActive
        {
            get; set;
        }

        /// <summary>
        /// The message shown in the message box.
        /// </summary>
        protected string Message
        {
            get; set;
        }

        protected Texture2D Texture
        {
            get
            {
                return GameWorld.SpriteSheet;
            }
        }

        protected SpriteFont Font
        {
            get
            {
                return ContentHelper.LoadFont("Fonts/x32");
            }
        }

        protected Rectangle DrawRectangle
        {
            get
            {
                int width = 60 * 10;
                int height = 20 * 10;
                return new Rectangle(Main.UserResWidth / 2 - width / 2, Main.UserResHeight / 2 - height / 2, width, height);
            }
        }

        protected Rectangle SourceRectangle
        {
            get
            {
                int width = 60;
                int height = 20;
                return new Rectangle(320, 0, width, height);
            }
        }

        /// <summary>
        /// Creates an instance of the message box that can be used to show a message to the player.
        /// </summary>
        public MessageBox()
        {
            Button = new OkButton(DrawRectangle);
            _attentionSound = new SoundFx("Sounds/message_show");
        }

        protected void Button_MouseClicked()
        {
            IsActive = false;
            Button.MouseClicked -= Button_MouseClicked;
        }


        /// <summary>
        /// Shows a message to the user in a message box with an OK button.
        /// </summary>
        /// <param name="message"></param>
        public virtual void Show(string message)
        {
            Button.MouseClicked += Button_MouseClicked;
            _attentionSound.PlayIfStopped();
            string wrapped = FontHelper.WrapText(Font, message, DrawRectangle.Width - BezelSize * 2);
            Message = wrapped;
            IsActive = true;
        }

        public virtual void Update()
        {
            Button.Update();

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                spriteBatch.Draw(ContentHelper.LoadTexture("Tiles/white"), new Rectangle(0, 0, Main.UserResWidth, Main.UserResHeight), Color.Black * .7f);
                spriteBatch.Draw(Texture, DrawRectangle, SourceRectangle, Color.White);
                spriteBatch.DrawString(Font, Message, new Vector2(DrawRectangle.X + BezelSize, DrawRectangle.Y + BezelSize), Color.Black);
                Button.Draw(spriteBatch);
            }
        }


    }
}
