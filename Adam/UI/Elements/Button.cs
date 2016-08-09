using Adam.Misc;
using Adam.Misc.Helpers;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI
{
    public abstract class Button
    {
        public delegate void EventHandler();

        private bool _mouseIsOver;
        private bool _wasPressed, _wasReleased;
        public Color Color { get; set; } = Color.White;
        protected Color CurrentColor;
        protected Vector2 Origin = new Vector2();
        protected float Rotation = 0f;
        private Backdrop _backdrop;
        protected Backdrop Backdrop
        {
            get
            {
                if (_backdrop == null)
                {
                    _backdrop = new Backdrop(CollRectangle.X, CollRectangle.Y, CollRectangle.Width, CollRectangle.Height,
                        false);
                    _backdrop.DisableAnimation();
                }
                return _backdrop;
            }
        }

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
        public event ButtonHandler MouseClicked;
        public event EventHandler MouseHover;
        public event EventHandler MouseOut;
        public bool ShowBackground { get; set; } = true;
        public Color TextColor { get; set; } = Color.White;

        public delegate void ButtonHandler(Button button);

        protected Rectangle ContainerDiff;

        protected virtual void OnMouseHover()
        {
            if (!_mouseIsOver)
            {
                CursorFx?.Play();
                _mouseIsOver = true;
            }

            CurrentColor = new Color(Color.R - 50, Color.G - 50, Color.B - 50);
        }

        protected virtual void OnMouseOut()
        {
            _mouseIsOver = false;
            CurrentColor = Color;
        }

        /// <summary>
        /// Updates the position of the button according to where the container is.
        /// </summary>
        /// <param name="container"></param>
        public void Update(Rectangle container)
        {
            CollRectangle.X = container.X + ContainerDiff.X;
            CollRectangle.Y = container.Y + ContainerDiff.Y;
            Update();
        }

        public void BindTo(Rectangle container)
        {
            ContainerDiff.X = (int)GetPosition().X - container.X;
            ContainerDiff.Y = (int)GetPosition().Y - container.Y;
        }

        /// <summary>
        /// Makes the size of the collision rectangle of the button be indentical to the given rectangle.
        /// </summary>
        /// <param name="rectangle"></param>
        public void ChangeDimensions(Rectangle rectangle)
        {
            ChangeDimensions(new Vector2(rectangle.Width, rectangle.Height));
        }


        public void ChangeDimensions(Vector2 dimensions)
        {
            CollRectangle.Width = (int)dimensions.X;
            CollRectangle.Height = (int) dimensions.Y;
            _backdrop = null;
        }

        public virtual void Update()
        {
            Backdrop.SetPosition(new Vector2(CollRectangle.X, CollRectangle.Y));
            Backdrop.Color = CurrentColor;

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
                    MouseClicked?.Invoke(this);
                    _wasReleased = false;
                    _wasPressed = false;
                }
            }
            else MouseOut?.Invoke();
        }

        public Vector2 GetPosition()
        {
            return new Vector2(CollRectangle.X, CollRectangle.Y);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (ShowBackground)
                Backdrop.Draw(spriteBatch);
            //spriteBatch.Draw(GameWorld.UiSpriteSheet, CollRectangle, SourceRectangle, Color);
        }
    }
}