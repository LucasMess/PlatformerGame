using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Adam.UI.MainMenu
{
    /// <summary>
    /// Used to show the levels in squares numbered 0-n in a grid layout.
    /// </summary>
    class LevelGrid
    {
        class LevelSquare : TextButton
        {
            public LevelSquare(Vector2 position, string text, bool convertCoordinates = true) : base(position, text, convertCoordinates)
            {

            }
        }

        List<LevelSquare> levelSquares = new List<LevelSquare>();

        /// <summary>
        /// Creates a new level grid from the files in a folder with the given name.
        /// </summary>
        /// <param name="levelTheme"></param>
        public LevelGrid(string levelTheme)
        {
            int y = 100;
            int x = 100;
            x -= CalcHelper.ApplyUiRatio(LevelSquare.Width) + 5;
            for (int i = 0; i < 10; i++)
            {
                x += CalcHelper.ApplyUiRatio(LevelSquare.Width) + 5;
                if (x + CalcHelper.ApplyUiRatio(LevelSquare.Width) > AdamGame.DefaultResWidth)
                {
                    x = 100;
                    y += CalcHelper.ApplyUiRatio(LevelSquare.Height) + 5;
                }
                levelSquares.Add(new LevelSquare(new Vector2(x, y), i.ToString()));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var square in levelSquares)
            {
                square.Draw(spriteBatch);
            }
        }
    }
}
