using Adam.Levels;
using Adam.Misc.Helpers;
using Adam.UI.Level_Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace Adam.UI.Elements
{
    /// <summary>
    ///     The image that will be used as the button texture.
    /// </summary>
    public enum ButtonImage
    {
        Play,
        Save,
        Open,
        New,
        Delete,
        Wall,
        Rename,
        Edit,
        Back,
        Expand,
        Brush,
        Eraser,
        Undo,
        Settings
    }

    /// <summary>
    /// Buttons that have an icon instead of text.
    /// </summary>
    public class IconButton : Button
    {
        public const int Size = 32;
        private readonly Texture2D _black = ContentHelper.LoadTexture("Tiles/black");
        private readonly ButtonImage _buttonImage;
        private readonly string _hoverText;
        private BitmapFont _font;
        private Rectangle _shadowSource = new Rectangle(128, 25, 16, 6);
        private bool _showHoverText;

        public IconButton(Vector2 position, Rectangle box, string hoverText, ButtonImage buttonImage)
        {
            Initialize();

            CollRectangle.X = (int)position.X + box.X;
            CollRectangle.Y = (int)position.Y + box.Y;
            _buttonImage = buttonImage;
            _hoverText = hoverText;

            switch (_buttonImage)
            {
                case ButtonImage.New:
                    SourceRectangle.X = 16;
                    SourceRectangle.Y = 0;
                    break;
                case ButtonImage.Edit:
                    SourceRectangle.X = 80;
                    SourceRectangle.Y = 16;
                    break;
                case ButtonImage.Open:
                    SourceRectangle.X = 48;
                    SourceRectangle.Y = 0;
                    break;
                case ButtonImage.Expand:
                    SourceRectangle.X = 144;
                    SourceRectangle.Y = 25;
                    SourceRectangle.Width = 10;
                    SourceRectangle.Height = 5;
                    CollRectangle.Width = SourceRectangle.Width * 2;
                    CollRectangle.Height = SourceRectangle.Height * 2;
                    break;
                case ButtonImage.Delete:
                    SourceRectangle.X = 64;
                    SourceRectangle.Y = 0;
                    break;
                case ButtonImage.Rename:
                    SourceRectangle.X = 80;
                    SourceRectangle.Y = 0;
                    break;
                case ButtonImage.Wall:
                    SourceRectangle.X = 64;
                    SourceRectangle.Y = 16;
                    break;
                case ButtonImage.Play:
                    SourceRectangle.X = 0;
                    SourceRectangle.Y = 0;
                    break;
                case ButtonImage.Back:
                    SourceRectangle.X = 80;
                    SourceRectangle.Y = 32;
                    SourceRectangle.Width = SourceRectangle.Width * 2;
                    CollRectangle.Width = CollRectangle.Width * 2;
                    break;
                case ButtonImage.Save:
                    SourceRectangle.X = 32;
                    SourceRectangle.Y = 0;
                    break;
                case ButtonImage.Brush:
                    SourceRectangle.X = 96;
                    SourceRectangle.Y = 0;
                    break;
                case ButtonImage.Eraser:
                    SourceRectangle.X = 96;
                    SourceRectangle.Y = 16;
                    break;
                case ButtonImage.Undo:
                    SourceRectangle.X = 64;
                    SourceRectangle.Y = 32;
                    break;
                case ButtonImage.Settings:
                    SourceRectangle.X = 48;
                    SourceRectangle.Y = 16;
                    break;
            }
        }

        /// <summary>
        /// Sets default variables for the button. This needs to be called by every function button.
        /// </summary>
        private void Initialize()
        {
            MouseHover += OnMouseHover;
            MouseOut += OnMouseOut;
            CollRectangle = new Rectangle(0, 0, Size, Size);
            SourceRectangle = new Rectangle(0, 0, 16, 16);
            _font = ContentHelper.LoadFont("Fonts/x32");
        }

        /// <summary>
        /// Checks for button events and special interactions.
        /// </summary>
        public override void Update()
        {
            base.Update();

            switch (_buttonImage)
            {
                case ButtonImage.Expand:
                    IsOn = Inventory.IsOpen;
                    break;
            }
        }

        /// <summary>
        /// Draws button along with its shadow.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            switch (_buttonImage)
            {
                default:
                    spriteBatch.Draw(GameWorld.UiSpriteSheet, CollRectangle, SourceRectangle, CurrentColor);
                    spriteBatch.Draw(GameWorld.UiSpriteSheet,
                        new Rectangle(CollRectangle.X, CollRectangle.Y + 11 * 2,
                            _shadowSource.Width * 2, _shadowSource.Height * 2),
                        _shadowSource, Color.White);
                    break;
                case ButtonImage.Expand:
                    if (IsOn)
                        spriteBatch.Draw(GameWorld.UiSpriteSheet, CollRectangle, SourceRectangle, Color, Rotation,
                            Origin, SpriteEffects.FlipVertically, 0);
                    else
                        spriteBatch.Draw(GameWorld.UiSpriteSheet, CollRectangle, SourceRectangle, CurrentColor, Rotation,
                            Origin, SpriteEffects.None, 0);
                    break;
            }
        }

        public void DrawTooltip(SpriteBatch spriteBatch)
        {
            if (_showHoverText)
            {
                FontHelper.DrawTooltip(spriteBatch, _hoverText);
            }
        }

        protected override void OnMouseHover()
        {
            base.OnMouseHover();

            _showHoverText = true;
        }

        protected override void OnMouseOut()
        {
            base.OnMouseOut();

            _showHoverText = false;
        }
    }
}