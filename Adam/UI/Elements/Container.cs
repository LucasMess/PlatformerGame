using ThereMustBeAnotherWay.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ThereMustBeAnotherWay.Graphics;

namespace ThereMustBeAnotherWay.UI.Elements
{
    /// <summary>
    /// Used to contain UI elements.
    /// </summary>
    public class Container : UiElement
    {
        public enum Style
        {
            SolidColor,
            SolidColorWithBevel,
            GameUnique,
        }

        private Style _currentContainerStyle = Style.SolidColor;

        /// <summary>
        /// Gets the source rectangles depending on what style is being used.
        /// </summary>
        /// <returns></returns>
        private Rectangle[] GetSourceRectangles()
        {
            switch (_currentContainerStyle)
            {
                case Style.SolidColor:
                    return _sourceRectanglesSolidColor;
                case Style.SolidColorWithBevel:
                    return _sourceRectanglesSolidColorWithBevel;
                case Style.GameUnique:
                    return _sourceRectanglesSpecial;
            }
            return _sourceRectanglesSolidColor;
        }

        private static Rectangle[] _sourceRectanglesSolidColor =
        {
            new Rectangle(320, 197, 4, 4), // top left
            new Rectangle(325, 197, 1, 4), // top
            new Rectangle(327, 197, 4, 4), // top right
            new Rectangle(320, 202, 4, 4), // middle left
            new Rectangle(325, 202, 1, 4), // middle fill
            new Rectangle(327, 202, 4, 4), // middle right
            new Rectangle(320, 207, 4, 5), // bot left
            new Rectangle(325, 207, 1, 5), // bot
            new Rectangle(327, 207, 4, 5), // bot right
        };


        private static Rectangle[] _sourceRectanglesSolidColorWithBevel =
        {
            new Rectangle(320 + 12, 197, 4, 4), // top left
            new Rectangle(325 + 12, 197, 1, 4), // top
            new Rectangle(327 + 12, 197, 4, 4), // top right
            new Rectangle(320 + 12, 202, 4, 4), // middle left
            new Rectangle(325 + 12, 202, 1, 4), // middle fill
            new Rectangle(327 + 12, 202, 4, 4), // middle right
            new Rectangle(320 + 12, 207, 4, 5), // bot left
            new Rectangle(325 + 12, 207, 1, 5), // bot
            new Rectangle(327 + 12, 207, 4, 5), // bot right
        };

        private static Rectangle[] _sourceRectanglesSpecial =
        {
            new Rectangle(344, 193, 6, 6), // top left
            new Rectangle(351, 193, 1, 6), // top
            new Rectangle(353, 193, 6, 6), // top right
            new Rectangle(344, 200, 6, 4), // middle left
            new Rectangle(351, 200, 1, 4), // middle fill
            new Rectangle(353, 200, 6, 4), // middle right
            new Rectangle(344, 205, 6, 7), // bot left
            new Rectangle(351, 205, 1, 7), // bot
            new Rectangle(353, 205, 6, 7), // bot right
        };



        private Vector2 _hiddenPos;
        private Vector2 _shownPos;
        public Vector2 Size { get; private set; }
        public bool IsHidden { get; private set; } = true;
        private Texture2D _texture = GameWorld.UiSpriteSheet;
        public Color Color { get; set; } = Color.White;
        public float Opacity { get; set; } = 1f;


        private Container()
        {
        }

        /// <summary>
        /// Resets the position of the container if the resolution changes.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void ResetPosition()
        {
            // Moves the container around instantly on a resolution change.
            if (IsHidden)
                SetPosition(_hiddenPos);
            else SetPosition(_shownPos);
        }

        /// <summary>
        /// Makes a window with these game dimensions at the center of the screen.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Container(int width, int height) : base()
        {
            Size = new Vector2(width, height);
            float x = (TMBAW_Game.UserResWidth / 2) - width / 2;
            float y = (TMBAW_Game.UserResHeight / 2) - height / 2;
            DrawRectangle = new Rectangle((int)x, (int)y, width, height);

            _shownPos = new Vector2(x, y);
            _hiddenPos = new Vector2(x, TMBAW_Game.UserResHeight);

            SetPosition(_hiddenPos);
        }


        /// <summary>
        /// Makes a window in game dimensions at the game coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="convertCoords"></param>
        public Container(int x, int y, int width, int height, bool convertCoords) : base()
        {

            Size = new Vector2(width, height);
            DrawRectangle = new Rectangle((int)x, (int)y, width, height);

            _shownPos = new Vector2(x, y);
            _hiddenPos = new Vector2(x, TMBAW_Game.UserResHeight);

            SetPosition(_hiddenPos);
        }

        public void ChangeStyle(Style newStyle)
        {
            _currentContainerStyle = newStyle;
        }

        public void DisableAnimation()
        {
            DrawRectangle = new Rectangle((int)_shownPos.X, (int)_shownPos.Y, DrawRectangle.Width, DrawRectangle.Height);
        }

        public void Show()
        {
            IsHidden = false;
            MoveTo(_shownPos, 100);
        }

        public void Hide()
        {
            IsHidden = true;
            MoveTo(_hiddenPos, 100);
        }

        /// <summary>
        /// Overrides the default shown position with the given one.
        /// </summary>
        /// <param name="position">The center of the shown position.</param>
        public void SetShownPosition(Vector2 position)
        {
            Vector2 leftCorner = new Vector2();
            leftCorner.X = position.X - Width / 2;
            leftCorner.Y = position.Y - Height / 2;
            _shownPos = leftCorner;
        }

        /// <summary>
        /// Overrides the default hidden position with the given one.
        /// </summary>
        /// <param name="position">The center of the hidden position.</param>
        public void SetHiddenPosition(Vector2 position)
        {
            Vector2 leftCorner = new Vector2();
            leftCorner.X = position.X - Width / 2;
            leftCorner.Y = position.Y - Height / 2;
            _hiddenPos = leftCorner;
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            int cornerWidth = GetSourceRectangles()[0].Width;
            int cornerHeight = GetSourceRectangles()[0].Height;
            int botCornerWidth = GetSourceRectangles()[6].Width;
            int botCornerHeight = GetSourceRectangles()[6].Height;

            spriteBatch.Draw(_texture, new Rectangle(DrawRectangle.X, DrawRectangle.Y, cornerWidth, cornerHeight), GetSourceRectangles()[0], Color * Opacity);
            int topBlankWidth = (int)(Size.X - cornerWidth * 2);
            spriteBatch.Draw(_texture, new Rectangle((DrawRectangle.X + cornerWidth), DrawRectangle.Y, topBlankWidth, cornerHeight), GetSourceRectangles()[1], Color * Opacity);
            spriteBatch.Draw(_texture, new Rectangle((DrawRectangle.X + cornerWidth + topBlankWidth), DrawRectangle.Y, cornerWidth, cornerHeight), GetSourceRectangles()[2], Color * Opacity);

            int midBlankWidth = (int)(Size.X - cornerWidth * 2);
            int midBlankHeight = (int)(Size.Y - cornerHeight - botCornerHeight);
            spriteBatch.Draw(_texture, new Rectangle(DrawRectangle.X, (DrawRectangle.Y + cornerHeight), cornerWidth, midBlankHeight), GetSourceRectangles()[3], Color * Opacity);
            spriteBatch.Draw(_texture, new Rectangle(DrawRectangle.X + cornerWidth, (DrawRectangle.Y + cornerHeight), midBlankWidth, midBlankHeight), GetSourceRectangles()[4], Color * Opacity);
            spriteBatch.Draw(_texture, new Rectangle(DrawRectangle.X + cornerWidth + midBlankWidth, (DrawRectangle.Y + cornerHeight), cornerWidth, midBlankHeight), GetSourceRectangles()[5], Color * Opacity);

            int yBot = (DrawRectangle.Y + midBlankHeight + cornerHeight);
            spriteBatch.Draw(_texture, new Rectangle(DrawRectangle.X, yBot, botCornerWidth, botCornerHeight), GetSourceRectangles()[6], Color * Opacity);
            spriteBatch.Draw(_texture, new Rectangle((DrawRectangle.X + botCornerWidth), yBot, topBlankWidth, botCornerHeight), GetSourceRectangles()[7], Color * Opacity);
            spriteBatch.Draw(_texture, new Rectangle((DrawRectangle.X + botCornerWidth + topBlankWidth), yBot, botCornerWidth, botCornerHeight), GetSourceRectangles()[8], Color * Opacity);
        }

    }
}
