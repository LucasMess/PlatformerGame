using Adam.Levels;
using Adam.Misc.Helpers;
using Adam.UI.Level_Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        private const int ButtonSize = 16;
        private readonly Texture2D _black = ContentHelper.LoadTexture("Tiles/black");
        private readonly ButtonImage _buttonImage;
        private readonly string _hoverText;
        private SpriteFont _font;
        private Rectangle _shadowSource = new Rectangle(128, 25, 16, 6);
        private bool _showHoverText;

        public IconButton(Vector2 position, Rectangle box, string hoverText, ButtonImage buttonImage)
        {
            Initialize();
            
            CollRectangle.X = CalcHelper.ApplyUiRatio((int) position.X) + box.X;
            CollRectangle.Y = CalcHelper.ApplyUiRatio((int) position.Y) + box.Y;
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
                    CollRectangle.Width = CalcHelper.ApplyUiRatio(SourceRectangle.Width);
                    CollRectangle.Height = CalcHelper.ApplyUiRatio(SourceRectangle.Height);
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
                    SourceRectangle.Width = SourceRectangle.Width*2;
                    CollRectangle.Width = CollRectangle.Width*2;
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
            CollRectangle = new Rectangle(0, 0, CalcHelper.ApplyUiRatio(ButtonSize), CalcHelper.ApplyUiRatio(ButtonSize));
            SourceRectangle = new Rectangle(0, 0, ButtonSize, ButtonSize);
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
                    spriteBatch.Draw(GameWorld.UiSpriteSheet, CollRectangle, SourceRectangle, Color);
                    spriteBatch.Draw(GameWorld.UiSpriteSheet,
                        new Rectangle(CollRectangle.X, CollRectangle.Y + CalcHelper.ApplyUiRatio(11),
                            CalcHelper.ApplyUiRatio(_shadowSource.Width), CalcHelper.ApplyUiRatio(_shadowSource.Height)),
                        _shadowSource, Color.White);
                    break;
                case ButtonImage.Expand:
                    if (IsOn)
                        spriteBatch.Draw(GameWorld.UiSpriteSheet, CollRectangle, SourceRectangle, Color, Rotation,
                            Origin, SpriteEffects.FlipVertically, 0);
                    else
                        spriteBatch.Draw(GameWorld.UiSpriteSheet, CollRectangle, SourceRectangle, Color, Rotation,
                            Origin, SpriteEffects.None, 0);
                    break;
            }
        }

        public void DrawOnTop(SpriteBatch spriteBatch)
        {
            if (_showHoverText)
            {
                var mouse = InputHelper.MouseRectangle;
                spriteBatch.Draw(_black,
                    new Rectangle(mouse.X - 5, mouse.Y - 52, (int) _font.MeasureString(_hoverText).X + 10,
                        (int) _font.MeasureString(_hoverText).Y + 4), Color.Black);
                FontHelper.DrawWithOutline(spriteBatch, _font, _hoverText, new Vector2(mouse.X, mouse.Y - 50), 1,
                    Color.White, Color.Black);
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