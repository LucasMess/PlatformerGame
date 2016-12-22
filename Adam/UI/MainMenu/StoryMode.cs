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
            levelSets.Add(new LevelGrid(levelOrder[currentLevelSet]));
        }
    }
}
