using Adam.Levels;
using Adam.Misc;
using Adam.Misc.Helpers;
using Microsoft.Win32.SafeHandles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI
{
    public abstract class Button
    {
        public delegate void EventHandler();

        private bool _mouseIsOver;
        private bool _wasPressed, _wasReleased;
        protected Color Color = Color.White;
        protected Vector2 Origin = new Vector2();
        protected float Rotation = 0f;

        protected SoundFx ErrorFx = new SoundFx("Sounds/Menu/error_style_2_001");
        protected SoundFx ConfirmFx = new SoundFx("Sounds/Menu/confirm_style_4_001");
        protected SoundFx CursorFx = new SoundFx("Sounds/Menu/cursor_style_2");
        protected SoundFx BackFx = new SoundFx("Sounds/Menu/back_style_2_001");

        protected Button()
        {
            MouseHover += OnMouseHover;
            MouseOut += OnMouseOut;
            Origin = new Vector2(CollRectangle.Width / 2, CollRectangle.Height / 2);
        }

        public bool IsOn { get; set; }
        protected Texture2D Texture { get; set; }
        protected Rectangle CollRectangle;
        protected Rectangle SourceRectangle;
        protected SpriteFont Font { get; set; } = ContentHelper.LoadFont("Fonts/x16");
        public string Text { get; set; }
        public event EventHandler MouseClicked;
        public event EventHandler MouseHover;
        public event EventHandler MouseOut;

        protected virtual void OnMouseHover()
        {
            if (!_mouseIsOver)
            {
                CursorFx?.Play();
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
                MouseHover?.Invoke();

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
                    MouseClicked?.Invoke();
                    _wasReleased = false;
                    _wasPressed = false;
                }
            }
            else MouseOut?.Invoke();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameWorld.UiSpriteSheet, CollRectangle, SourceRectangle, Color);
            spriteBatch.DrawString(Font, Text, new Vector2(CollRectangle.Center.X, CollRectangle.Center.Y),
                Color.White, 0, Font.MeasureString(Text) / 2, (float)(.5 / Main.HeightRatio), SpriteEffects.None, 0);
        }
    }
}