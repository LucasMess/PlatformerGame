using Adam;
using Adam.UI.Information;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.GameData
{
    public class Save
    {
        double progress;
        double maxprogress = 1000;
        GameMode currentLevel;
        PlayerStats playerStats;
        ObjectiveTracker objTracker;

        public Save()
        {
            objTracker = new ObjectiveTracker();
            playerStats = new PlayerStats();
            playerStats.SetToDefault();
            currentLevel = GameMode.None;
            progress = 0;
        }

        public PlayerStats PlayerStats
        {
            get { return playerStats; }
            set { playerStats = value; }
        }

        public GameMode CurrentLevel
        {
            get { return currentLevel; }
            set { currentLevel = value; }
        }

        public string Completeness
        {
            get
            {
                double percent = (progress / maxprogress) * 100;
                return percent + "%";
            }
        }

        public double Progress
        {
            set
            {
                progress = value;
                if (progress > maxprogress)
                    throw new Exception("Progress set is bigger than maximum allowed progress!");
            }
        }

        public ObjectiveTracker ObjTracker
        {
            get { return objTracker; }
            set { objTracker = value; }
        }
    }

    public class PlayerStats
    {
        byte lives;
        public byte Lives
        {
            get { return lives; }
            set { lives = value; }
        }

        int score;
        public int Score
        {
            get { return score; }
            set { score = value; }
        }

        int numberOfDeaths;
        public int NumberOfDeaths
        {
            get { return numberOfDeaths; }
            set { numberOfDeaths = value; }
        }

        int damageTaken;
        public int DamageTaken
        {
            get { return damageTaken; }
            set { damageTaken = value; }
        }

        int damageInflicted;
        public int DamageInflicted
        {
            get { return damageInflicted; }
            set { damageInflicted = value; }
        }

        public void SetToDefault()
        {
            lives = 5;
        }

    }
}
