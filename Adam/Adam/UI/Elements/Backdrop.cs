
using System.Windows.Forms;
using Adam.Levels;
using Adam.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.UI.Elements
{
    /// <summary>
    /// Used to contain UI elements.
    /// </summary>
    public class Backdrop : UiElement
    {
        private static Rectangle[] _sourceRectangles =
        {
            new Rectangle(320, 197, 4, 4), // top left
            new Rectangle(325, 197, 1, 4), // top
            new Rectangle(327, 197, 4, 4), // top right
            new Rectangle(320, 202, 11, 4), // middle fill
            new Rectangle(320, 207, 4, 5), // bot left
            new Rectangle(325, 207, 1, 5), // bot
            new Rectangle(327, 207, 4, 5), // bot right
        };

        private Vector2 _hiddenPos;
        private Vector2 _shownPos;
        public Vector2 Size { get; private set; }
        public bool IsHidden { get; private set; } = true;
        private Texture2D _texture = GameWorld.UiSpriteSheet;
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Makes a window with these game dimensions at the center of the screen.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Backdrop(int width, int height)
        {
            width = CalcHelper.ApplyUiRatio(width);
            height = CalcHelper.ApplyUiRatio(height);

            Size = new Vector2(width, height);
            float x = (Main.UserResWidth / 2) - width / 2;
            float y = (Main.UserResHeight / 2) - height / 2;
            DrawRectangle = new Rectangle((int)x, (int)y, width, height);

            _shownPos = new Vector2(x, y);
            _hiddenPos = new Vector2(x, Main.UserResHeight);

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
        public Backdrop(int x, int y, int width, int height, bool convertCoords)
        {
            if (convertCoords)
            {
                width = CalcHelper.ApplyUiRatio(width);
                height = CalcHelper.ApplyUiRatio(height);
                x = CalcHelper.ApplyUiRatio(x);
                y = CalcHelper.ApplyUiRatio(y);
            }

            Size = new Vector2(width, height);
            DrawRectangle = new Rectangle((int)x, (int)y, width, height);

            _shownPos = new Vector2(x, y);
            _hiddenPos = new Vector2(x, Main.UserResHeight);

            SetPosition(_hiddenPos);
        }

        public void DisableAnimation()
        {
            DrawRectangle = new Rectangle((int)_shownPos.X, (int)_shownPos.Y, DrawRectangle.Width, DrawRectangle.Height);
        }

        public void Show()
        {
            if (IsHidden)
            {
                IsHidden = false;
                MoveTo(_shownPos, 100);
            }
        }

        public void Hide()
        {
            if (!IsHidden)
            {
                IsHidden = true;
                MoveTo(_hiddenPos, 100);
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            int cornerSize = CalcHelper.ApplyUiRatio(_sourceRectangles[0].Width);
            spriteBatch.Draw(_texture, new Rectangle(DrawRectangle.X, DrawRectangle.Y, cornerSize, cornerSize), _sourceRectangles[0], Color);
            int topBlankWidth = (int)(Size.X - cornerSize * 2);
            spriteBatch.Draw(_texture, new Rectangle((DrawRectangle.X + cornerSize), DrawRectangle.Y, topBlankWidth, cornerSize), _sourceRectangles[1], Color);
            spriteBatch.Draw(_texture, new Rectangle((DrawRectangle.X + cornerSize + topBlankWidth), DrawRectangle.Y, cornerSize, cornerSize), _sourceRectangles[2], Color);

            int midBlankHeight = (int)(Size.Y - cornerSize * 2);
            spriteBatch.Draw(_texture, new Rectangle(DrawRectangle.X, (DrawRectangle.Y + cornerSize), (int)Size.X, midBlankHeight), _sourceRectangles[3], Color);

            int yBot = (DrawRectangle.Y + midBlankHeight + cornerSize);
            spriteBatch.Draw(_texture, new Rectangle(DrawRectangle.X, yBot, cornerSize, cornerSize), _sourceRectangles[4], Color);
            spriteBatch.Draw(_texture, new Rectangle((DrawRectangle.X + cornerSize), yBot, topBlankWidth, cornerSize), _sourceRectangles[5], Color);
            spriteBatch.Draw(_texture, new Rectangle((DrawRectangle.X + cornerSize + topBlankWidth), yBot, cornerSize, cornerSize), _sourceRectangles[6], Color);
        }

    }
}
