using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    class Dialog
    {
        Texture2D texture;
        Rectangle rectangle;
        Vector2 origin;
        SpriteFont font;
        string text;
        public bool isVisible;
        Vector2 monitorRes;
        double visibleTimer, bufferTimer;

        public const int ExpirationTime = 2000;

        public enum Type { Notification, ActionRequired }
        Type type;

        public Dialog(ContentManager Content, Type type)
        {
            this.type = type;
            monitorRes = new Vector2(Game1.PrefferedResWidth, Game1.PrefferedResHeight);
            Load(Content);
        }

        public void AddText(string text)
        {
            rectangle = new Rectangle((int)monitorRes.X / 2, (int)monitorRes.Y / Game1.DefaultResHeight * (Game1.DefaultResHeight / 2 - 100), 1000, 200);
            this.text = text;
        }

        public void Load(ContentManager Content)
        {
            texture = Content.Load<Texture2D>("Menu/sign_damaged");
            font = Content.Load<SpriteFont>("dialog_box_font");
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isVisible)
            {
                spriteBatch.Draw(texture, rectangle, null, Color.White, 0, origin, SpriteEffects.None, 0);
               // spriteBatch.DrawString(font, text, new Vector2(rectangle.X, rectangle.Y), Color.Black, 0, font.MeasureString(text) / 2, 0.2f, SpriteEffects.None, 0);
            }
        }
    }
}
