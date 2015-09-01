using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Adam.UI.Elements
{
    public class YesButton : Button
    {
        MessageBox sender;

        public YesButton(Rectangle containerRectangle, MessageBox sender)
        {
            Text = "Yes";
            int width = 19 * 5;
            int height = 6 * 5;
            int x = containerRectangle.X + containerRectangle.Width / 2;
            int y = containerRectangle.Y + containerRectangle.Height;
            collRectangle = new Rectangle(x - width / 2, y - height - 20, width, height);
            this.sender = sender;
            Initialize();
        }

        protected void Initialize()
        {
            MouseOver += OnMouseOver;
            MouseOut += OnMouseOut;
            MouseClicked += YesButton_MouseClicked;
            sourceRectangle = new Rectangle(320, 20, 19, 6);
            texture = GameWorld.SpriteSheet;
            font = ContentHelper.LoadFont("Fonts/objectiveText");
        }

        private void YesButton_MouseClicked()
        {
            sender.IsActive = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, collRectangle, sourceRectangle, color);
            spriteBatch.DrawString(font, Text, new Vector2(collRectangle.Center.X, collRectangle.Center.Y),
                Color.White, 0, font.MeasureString(Text) / 2, (float)(.5 / Main.HeightRatio), SpriteEffects.None, 0);

        }
    }

    public class OKButton : Button
    {
        MessageBox sender;

        public OKButton(Rectangle containerRectangle, MessageBox sender)
        {
            Text = "OK";
            int width = 19 * 5;
            int height = 6 * 5;
            int x = containerRectangle.X + containerRectangle.Width / 2;
            int y = containerRectangle.Y + containerRectangle.Height;
            collRectangle = new Rectangle(x - width / 2, y - height - 20, width, height);
            this.sender = sender;
            Initialize();
        }

        protected void Initialize()
        {
            MouseOver += OnMouseOver;
            MouseOut += OnMouseOut;
            MouseClicked += OKButton_MouseClicked;
            sourceRectangle = new Rectangle(320, 20, 19, 6);
            texture = GameWorld.SpriteSheet;
            font = ContentHelper.LoadFont("Fonts/objectiveText");
        }

        private void OKButton_MouseClicked()
        {
            sender.IsActive = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, collRectangle, sourceRectangle, color);
            spriteBatch.DrawString(font, Text, new Vector2(collRectangle.Center.X, collRectangle.Center.Y),
                Color.White, 0, font.MeasureString(Text) / 2, (float)(.5 / Main.HeightRatio), SpriteEffects.None, 0);

        }


    }
}
