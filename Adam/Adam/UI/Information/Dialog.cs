using Adam.Misc;
using Adam.Misc.Helpers;
using Adam.Misc.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI
{
    public class Dialog
    {
        Texture2D texture;
        SpriteFont font;
        Rectangle drawRectangle;
        Rectangle sourceRectangle;
        Vector2 origin;

        bool isActive = false;
        string text = "";
        StringBuilder sb;
        SoundFx popSound;
        ITalkable CurrentSender;

        public delegate void EventHandler();
        public event EventHandler NextDialog;
        public event EventHandler CancelDialog;

        float opacity = 0;
        double skipTimer;

        int originalY;

        public Dialog()
        {
            texture = GameWorld.SpriteSheet;
            drawRectangle = new Rectangle(Game1.UserResWidth / 2, 40, 600, 200);
            sourceRectangle = new Rectangle(16 * 16, 14 * 16, 16 * 3, 16);
            origin = new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2);
            drawRectangle.X -= (int)origin.X;

            originalY = drawRectangle.Y;
            drawRectangle.Y -= 40;

            font = ContentHelper.LoadFont("Fonts/dialog");
            popSound = new SoundFx("Sounds/message_show");
        }

        public void Say(string text, ITalkable sender)
        {
            CurrentSender = sender;
            sender.CurrentConversation++;

            Prepare(text);
        }

        public void Show(string text)
        {
            CurrentSender = null;

            Prepare(text);
        }

        private void Prepare(string text)
        {
            isActive = true;
            this.text = FontHelper.WrapText(font, text, drawRectangle.Width - 60);
            skipTimer = 0;
            opacity = 0;
            drawRectangle.Y -= 40;
            popSound.Reset();
        }

        public void Update(GameTime gameTime)
        {
            float deltaOpacity = .03f;
            if (isActive)
            {
                popSound.PlayOnce();
                skipTimer += gameTime.ElapsedGameTime.TotalSeconds;
                GameWorld.Instance.player.manual_hasControl = false;
                if (skipTimer > .5)
                {
                    if (InputHelper.IsLeftMousePressed())
                    {
                        isActive = false;
                        GameWorld.Instance.player.manual_hasControl = true;
                        if (CurrentSender != null)
                            CurrentSender.OnNextDialog();
                    }
                }
            }

            if (isActive)
            {
                float velocity = (originalY - drawRectangle.Y) / 10;
                drawRectangle.Y += (int)velocity;
                opacity += deltaOpacity;
            }
            else
            {
                float velocity = -3f;
                opacity -= deltaOpacity;
                drawRectangle.Y += (int)velocity;
                skipTimer = 0;
            }

            if (opacity > 1)
                opacity = 1;
            if (opacity < 0)
                opacity = 0;
            if (drawRectangle.Y < -100)
                drawRectangle.Y = -100;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, drawRectangle, sourceRectangle, Color.White * opacity);
            spriteBatch.DrawString(font, text, new Vector2(drawRectangle.X + 30, drawRectangle.Y + 30), Color.Black * opacity);
            if (isActive && skipTimer > .5)
            {
                string mousebutton = "Press left mouse button to continue";
               // spriteBatch.DrawString(font, mousebutton, new Vector2(drawRectangle.Right - font.MeasureString(mousebutton).X, drawRectangle.Bottom), Color.Black);
                FontHelper.DrawWithOutline(spriteBatch, font, mousebutton, new Vector2(drawRectangle.Right - font.MeasureString(mousebutton).X - 20, drawRectangle.Bottom), 2, Color.White, Color.Black);
            }
        }

    }
}
