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

        public void Draw(SpriteBatch spriteBatch)
        {
            levelSets[currentLevelSet].Draw(spriteBatch);
        }
    }
}
