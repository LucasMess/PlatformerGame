using Adam.Misc;
using Adam.Misc.Helpers;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI
{
    /// <summary>
    /// Simple base class for all UI elements that can be clicked on.
    /// </summary>
    public abstract class Button
    {
        public delegate void EventHandler();

        private bool _mouseIsOver;
        private bool _wasPressed;
        private bool _wasReleased;

        public Color Color { get; set; } = Color.White;


        protected Color CurrentColor;

        protected Vector2 Origin = new Vector2();
        protected float Rotation = 0f;
        private Container _container;

        /// <summary>
        /// The container that holds this button in place. The button will follow the container if it moves.
        /// </summary>
        protected Container Container
        {
            get
            {
                if (_container == null)
                {
                    _container = new Container(CollRectangle.X, CollRectangle.Y, CollRectangle.Width, CollRectangle.Height,
                        false);
                    _container.DisableAnimation();
                }
                return _container;
            }
        }

        protected SoundFx ErrorSound = new SoundFx("Sounds/Menu/error_style_2_001");
        protected SoundFx ConfirmSound = new SoundFx("Sounds/Menu/confirm_style_4_001");
        protected SoundFx CursorSound = new SoundFx("Sounds/Menu/cursor_style_2");
        protected SoundFx BackSound = new SoundFx("Sounds/Menu/back_style_2_001");

        protected Button()
        {
            MouseHover += OnMouseHover;
            MouseOut += OnMouseOut;
            Origin = new Vector2(CollRectangle.Width / 2, CollRectangle.Height / 2);
        }

        /// <summary>
        /// Used to keep track of a setting, not used by all buttons.
        /// </summary>
        public bool IsOn { get; set; }
        protected Texture2D Texture { get; set; }
        protected Rectangle CollRectangle;
        protected Rectangle SourceRectangle;
        protected SpriteFont Font { get; set; } = ContentHelper.LoadFont("Fonts/x16");
        public string Text { get; set; }

        /// <summary>
        /// Called when user clicks the button.
        /// </summary>
        public event ButtonHandler MouseClicked;

        /// <summary>
        /// Called when user hovers over the button.
        /// </summary>
        public event EventHandler MouseHover;

        /// <summary>
        /// Called when user was hovering over the button and stops hovering it.
        /// </summary>
        public event EventHandler MouseOut;

        /// <summary>
        /// If set to true, the texture of the button will show.
        /// </summary>
        public bool ShowBackground { get; set; } = true;

        public Color TextColor { get; set; } = Color.White;

        public delegate void ButtonHandler(Button button);

        /// <summary>
        /// The difference in coordinates between the container and the button.
        /// </summary>
        protected Rectangle ContainerDiff;

        /// <summary>
        /// Default behavior for when the button is hovered. Override this method to remove default sound effects and color changes.
        /// </summary>
        protected virtual void OnMouseHover()
        {
            if (!_mouseIsOver)
            {
                CursorSound?.Play();
                _mouseIsOver = true;
            }

            CurrentColor = new Color(Color.R - 50, Color.G - 50, Color.B - 50);
        }

        /// <summary>
        /// Default behavior for when the button stops being hovered. Override this method to remove default sound effects and color changes.
        /// </summary>
        protected virtual void OnMouseOut()
        {
            _mouseIsOver = false;
            CurrentColor = Color;
        }

        /// <summary>
        /// Updates the position of the button according to where the container is, and also calls Update().
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

        /// <summary>
        /// Changes the size of the collision rectangle of the button to the given width and height values.
        /// </summary>
        /// <param name="dimensions">Vector2 where x = width, and y = height.</param>
        public void ChangeDimensions(Vector2 dimensions)
        {
            CollRectangle.Width = (int)dimensions.X;
            CollRectangle.Height = (int)dimensions.Y;
            _container = null;
        }

        /// <summary>
        /// Checks for collision with cursor and invokes events based on user actions.
        /// </summary>
        public virtual void Update()
        {
            Container.SetPosition(new Vector2(CollRectangle.X, CollRectangle.Y));
            Container.Color = CurrentColor;

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

        /// <summary>
        /// Returns the coordinates of the collision rectangle of the button.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPosition()
        {
            return new Vector2(CollRectangle.X, CollRectangle.Y);
        }


        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (ShowBackground)
                Container.Draw(spriteBatch);
            //spriteBatch.Draw(GameWorld.UiSpriteSheet, CollRectangle, SourceRectangle, Color);
        }
    }
}