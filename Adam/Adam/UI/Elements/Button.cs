using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI
{
    public class Button
    {
        public bool IsActive { get; set; }

        protected Texture2D texture;
        protected Rectangle collRectangle;
        protected Rectangle sourceRectangle;
        protected SpriteFont font;
        protected SoundEffect errorSound, confirmSound, cursorSound, backSound;

        public delegate void EventHandler();

        public event EventHandler MouseClicked;
        public event EventHandler MouseOver;
        public event EventHandler MouseOut;

        public string Text { get; set; }
        protected Color color = Color.White;

        bool wasPressed, wasReleased;
        bool mouseIsOver;

        /// <summary>
        /// Creates a new default button.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="text"></param>
        public Button(Vector2 position, string text)
        {
            Text = text;
            collRectangle = new Rectangle((int)position.X, (int)position.Y, (int)(300/Main.WidthRatio), (int)(30/Main.HeightRatio));
            Initialize();
        }

        public Button() { }

        private void Initialize()
        {
            MouseOver += OnMouseOver;
            MouseOut += OnMouseOut;

            texture = ContentHelper.LoadTexture("Menu/menu_button_new") ;
            font = ContentHelper.LoadFont("Fonts/button");

            errorSound = ContentHelper.LoadSound("Sounds/Menu/error_style_2_001");
            confirmSound = ContentHelper.LoadSound("Sounds/Menu/confirm_style_4_001");
            cursorSound = ContentHelper.LoadSound("Sounds/Menu/cursor_style_2");
            backSound = ContentHelper.LoadSound("Sounds/Menu/back_style_2_001");
        }

        protected virtual void OnMouseOver()
        {
            if (!mouseIsOver)
            {
                cursorSound?.Play();
                mouseIsOver = true;
            }

            color = new Color(200, 200, 200);
        }

        protected virtual void OnMouseOut()
        {
            mouseIsOver = false;
            color = Color.White;
        }

        public virtual void Update()
        {            

            if (InputHelper.MouseRectangle.Intersects(collRectangle))
            {
                MouseOver();

                if (InputHelper.IsLeftMousePressed())
                {
                    wasPressed = true;
                    wasReleased = false;
                }
                if (InputHelper.IsLeftMouseReleased() && wasPressed)
                {
                    wasReleased = true;
                }
                if (wasPressed && wasReleased)
                {
                    MouseClicked();
                    wasReleased = false;
                    wasPressed = false;
                }
            }
            else MouseOut();

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, collRectangle, color);
            spriteBatch.DrawString(font, Text, new Vector2(collRectangle.Center.X, collRectangle.Center.Y), 
                Color.White, 0, font.MeasureString(Text) / 2, (float)(.5/Main.HeightRatio), SpriteEffects.None, 0);
        }

    }
}
