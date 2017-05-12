using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace ThereMustBeAnotherWay.UI.Elements
{
    public class YesButton : Button
    {
        MessageBox _sender;

        public YesButton(Rectangle containerRectangle, MessageBox sender)
        {
            Text = "Yes";
            int width = 19 * 5;
            int height = 50;
            int x = containerRectangle.X + containerRectangle.Width / 2;
            int y = containerRectangle.Y + containerRectangle.Height;
            CollRectangle = new Rectangle(x - width / 2, y - height - 20, width, height);
            ChangeDimensions(new Vector2(width, height));
            this._sender = sender;
            Initialize();
        }

        protected void Initialize()
        {
            MouseHover += OnMouseHover;
            MouseOut += OnMouseOut;
            MouseClicked += YesButton_MouseClicked;
            SourceRectangle = new Rectangle(320, 20, 19, 6);
            Texture = GameWorld.SpriteSheet;
            Font = ContentHelper.LoadFont("Fonts/x8");
        }

        private void YesButton_MouseClicked(Button button)
        {
            _sender.IsActive = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, CollRectangle, SourceRectangle, Color);
           // spriteBatch.DrawString(Font, Text, new Vector2(CollRectangle.Center.X, CollRectangle.Center.Y),Color.White, 0, Font.MeasureString(Text) / 2, (float)(.5 / AdamGame.HeightRatio), 0);

        }
    }
}
