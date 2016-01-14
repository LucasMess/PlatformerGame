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

        protected Texture2D Texture;
        protected Rectangle CollRectangle;
        protected Rectangle SourceRectangle;
        protected SpriteFont Font;
        protected SoundEffect ErrorSound, ConfirmSound, CursorSound, BackSound;

        public delegate void EventHandler();

        public event EventHandler MouseClicked;
        public event EventHandler MouseOver;
        public event EventHandler MouseOut;

        public string Text { get; set; }
        protected Color Color = Color.White;

        bool _wasPressed, _wasReleased;
        bool _mouseIsOver;

        /// <summary>
        /// Creates a new default button.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="text"></param>
        public Button(Vector2 position, string text)
        {
            Text = text;
            CollRectangle = new Rectangle((int)position.X, (int)position.Y, (int)(300/Main.WidthRatio), (int)(30/Main.HeightRatio));
            Initialize();
        }

        public Button() { }

        private void Initialize()
        {
            MouseOver += OnMouseOver;
            MouseOut += OnMouseOut;

            Texture = ContentHelper.LoadTexture("Menu/menu_button_new") ;
            Font = ContentHelper.LoadFont("Fonts/x16");

            ErrorSound = ContentHelper.LoadSound("Sounds/Menu/error_style_2_001");
            ConfirmSound = ContentHelper.LoadSound("Sounds/Menu/confirm_style_4_001");
            CursorSound = ContentHelper.LoadSound("Sounds/Menu/cursor_style_2");
            BackSound = ContentHelper.LoadSound("Sounds/Menu/back_style_2_001");
        }

        protected virtual void OnMouseOver()
        {
            if (!_mouseIsOver)
            {
                CursorSound?.Play();
                _mouseIsOver = true;
            }

            Color = new Color(200, 200, 200);
        }

        protected virtual void OnMouseOut()
        {
            _mouseIsOver = false;
            Color = Color.White;
        }

        public virtual void Update()
        {            

            if (InputHelper.MouseRectangle.Intersects(CollRectangle))
            {
                MouseOver();

                if (InputHelper.IsLeftMousePressed())
                {
                    _wasPressed = true;
                    _wasReleased = false;
                }
                if (InputHelper.IsLeftMouseReleased() && _wasPressed)
                {
                    _wasReleased = true;
                }
                if (_wasPressed && _wasReleased)
                {
                    MouseClicked();
                    _wasReleased = false;
                    _wasPressed = false;
                }
            }
            else MouseOut();

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, CollRectangle, Color);
            spriteBatch.DrawString(Font, Text, new Vector2(CollRectangle.Center.X, CollRectangle.Center.Y), 
                Color.White, 0, Font.MeasureString(Text) / 2, (float)(.5/Main.HeightRatio), SpriteEffects.None, 0);
        }

    }
}
