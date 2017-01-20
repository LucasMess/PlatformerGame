using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
            public int LevelID;
            public delegate void LevelHandler(LevelSquare square);
            public event LevelHandler OnLevelSelected;
            public LevelSquare(Vector2 position, string text, bool convertCoordinates = true) : base(position, text, convertCoordinates)
            {
                Int32.TryParse(text, out LevelID);
                MouseClicked += LevelSquare_MouseClicked;
            }

            private void LevelSquare_MouseClicked(Button button)
            {
                OnLevelSelected?.Invoke(this);
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
            x -= (LevelSquare.Width) + 5;
            for (int i = 0; i < 10; i++)
            {
                x += (LevelSquare.Width) + 5;
                if (x + (LevelSquare.Width) > AdamGame.DefaultUiWidth)
                {
                    x = 100;
                    y += (LevelSquare.Height) + 5;
                }
                LevelSquare square = new LevelSquare(new Vector2(x, y), i.ToString());
                square.OnLevelSelected += Square_OnLevelSelected;
                levelSquares.Add(square);
            }
        }

        private void Square_OnLevelSelected(LevelSquare square)
        {
            //TODO: Load level with level id == square.levelID.
        }

        public void Update()
        {
            // Updates the squares to check for input, since they are buttons.
            foreach (var levelSquare in levelSquares)
            {
                levelSquare.Update();
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
