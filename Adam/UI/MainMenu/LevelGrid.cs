using Adam.UI.Elements;
using Microsoft.Xna.Framework;
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

        }
    }
}
