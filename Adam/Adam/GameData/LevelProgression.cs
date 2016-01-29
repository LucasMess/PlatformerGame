using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.GameData
{
    /// <summary>
    /// Class containing primitive variables that are saved to the player's computer and loaded. It defines where in the game
    /// the player is at, as well stats.
    /// </summary>
    [Serializable]
    public class LevelProgression
    {

        public int StoryProgress = 0;

        // Garden of Eden
        public bool HasRetrievedMap = false;
        public bool HasStartedMainQuest = false;
        public bool HasStartedCharlieCollectingQuest = false;

        // Player stats
        public int PlayerHealth = 0;
        public int PlayerGold = 0;
        public int PlayerExp = 0;
        public int NumberOfEnemiesKilled = 0;

    }
}
