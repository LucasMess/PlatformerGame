using Adam.Misc.Helpers;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI
{
    /// <summary>
    ///     Displays a message to the user where they have to press OK to continue.
    /// </summary>
    public class MessageBox
    {
        protected const int BezelSize = 25;
        private readonly InGameWindow _window = new InGameWindow(125, 50);

        /// <summary>
        ///     Creates an instance of the message box that can be used to show a message to the player.
        /// </summary>
        public MessageBox()
        {
            Button = new OkButton(_window.Bounds);
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
        protected string Message { get; set; } = "Error 404:Text not found.";

        protected SpriteFont Font
        {
            get { return ContentHelper.LoadFont("Fonts/x16"); }
        }

        protected void Button_MouseClicked()
        {
            IsActive = false;
            _window.Hide();
            Button.MouseClicked -= Button_MouseClicked;
        }

        /// <summary>
        ///     Shows a message to the user in a message box with an OK button.
        /// </summary>
        /// <param name="message"></param>
        public virtual void Show(string message)
        {
            _window.Show();
            Button.MouseClicked += Button_MouseClicked;
            var wrapped = FontHelper.WrapText(Font, message, _window.Bounds.Width - BezelSize*2);
            Message = wrapped;
            IsActive = true;
        }

        public virtual void Update()
        {
            Button.Update(_window.Bounds);
            _window.Update();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            _window.Draw(spriteBatch);
            spriteBatch.DrawString(Font, Message,
                new Vector2(_window.Bounds.X + BezelSize, _window.Bounds.Y + BezelSize), Color.Black);
            Button.Draw(spriteBatch);
        }
    }
}