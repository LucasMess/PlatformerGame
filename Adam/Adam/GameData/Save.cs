using Adam;
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
        Level currentLevel;
        PlayerStats playerStats;

        public Save()
        {
            playerStats = new PlayerStats();
            playerStats.SetToDefault();
            currentLevel = Level.Level1and1;
            progress = 0;
        }

        public PlayerStats PlayerStats
        {
            get { return playerStats; }
            set { playerStats = value; }
        }

        public Level CurrentLevel
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
