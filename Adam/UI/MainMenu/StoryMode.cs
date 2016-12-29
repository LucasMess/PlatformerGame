using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Adam.UI.MainMenu
{
    class StoryMode
    {
        List<LevelGrid> levelSets = new List<LevelGrid>();

        string[] levelOrder = new string[]
        {
            "grasslands", "desert",
        };

        int currentLevelSet = 0;

        public StoryMode()
        {
            for (int i = 0; i < levelOrder.Length; i++)
            {
                levelSets.Add(new LevelGrid(levelOrder[i]));
            }

        }

        public void Update()
        {

        }

        /// <summary>
        /// Changes the current level set being shown to the player to the one that comes before it.
        /// </summary>
        public void ShowNextLevelSet()
        {
            currentLevelSet++;
            if (currentLevelSet >= levelSets.Count)
            {
                currentLevelSet = levelSets.Count - 1;
                //TODO: Add sound to show there isn't a next level set.
            }
        }

        /// <summary>
        /// Changes the current level set being shown to the player to the one that comes after it.
        /// </summary>
        public void ShowPreviousLevelSet()
        {
            currentLevelSet--;
            if (currentLevelSet < 0)
            {
                currentLevelSet = 0;
                //TODO: Add sound for invalid input.
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            levelSets[currentLevelSet].Draw(spriteBatch);
        }
    }
}
