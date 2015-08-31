using Adam.Misc.Helpers;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI
{
    /// <summary>
    /// Displays a message to the user where they have to press OK to continue.
    /// </summary>
    public class MessageBox
    {
        /// <summary>
        /// Determines whether to draw the message box or not.
        /// </summary>
        public bool IsActive
        {
            get; private set;
        }

        /// <summary>
        /// The message shown in the message box.
        /// </summary>
        string Message
        {
            get; set;
        }

        OKButton button
        {
            get
            {
                return new OKButton(DrawRectangle.X + DrawRectangle.Width / 2, DrawRectangle.Y + DrawRectangle.Height);
            }
        }

        Texture2D Texture
        {
            get
            {
                return Main.DefaultTexture;
            }
        }

        SpriteFont Font
        {
            get
            {
                return ContentHelper.LoadFont("Fonts/objectiveText");
            }
        }

        Rectangle DrawRectangle
        {
            get
            {
                int width = 300;
                int height = 100;
                return new Rectangle(Main.UserResWidth / 2 - width / 2, Main.UserResHeight/2 - height / 2, width, height);
            }
        }

        Rectangle SourceRectangle
        {
            get
            {
                int width = 75;
                int height = 25;
                return new Rectangle(0, 0, width, height);
            }
        }


        /// <summary>
        /// Shows a message to the user in a message box with an OK button.
        /// </summary>
        /// <param name="message"></param>
        public void Show(string message)
        {
            string wrapped = FontHelper.WrapText(Font, message, DrawRectangle.Width);
            Message = wrapped;
            IsActive = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                spriteBatch.Draw(Texture, DrawRectangle, Color.White);
                spriteBatch.DrawString(Font, Message, new Vector2(DrawRectangle.X + 5, DrawRectangle.Y + 10), Color.White);
            }
        }


    }
}
