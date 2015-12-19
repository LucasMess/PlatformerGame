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
        double _progress;
        double _maxprogress = 1000;
        GameMode _currentLevel;
        PlayerStats _playerStats;
        ObjectiveTracker _objTracker;

        public Save()
        {
            _objTracker = new ObjectiveTracker();
            _playerStats = new PlayerStats();
            _playerStats.SetToDefault();
            _currentLevel = GameMode.None;
            _progress = 0;
        }

        public PlayerStats PlayerStats
        {
            get { return _playerStats; }
            set { _playerStats = value; }
        }

        public GameMode CurrentLevel
        {
            get { return _currentLevel; }
            set { _currentLevel = value; }
        }

        public string Completeness
        {
            get
            {
                double percent = (_progress / _maxprogress) * 100;
                return percent + "%";
            }
        }

        public double Progress
        {
            set
            {
                _progress = value;
                if (_progress > _maxprogress)
                    throw new Exception("Progress set is bigger than maximum allowed progress!");
            }
        }

        public ObjectiveTracker ObjTracker
        {
            get { return _objTracker; }
            set { _objTracker = value; }
        }
    }

    public class PlayerStats
    {
        byte _lives;
        public byte Lives
        {
            get { return _lives; }
            set { _lives = value; }
        }

        int _score;
        public int Score
        {
            get { return _score; }
            set { _score = value; }
        }

        int _numberOfDeaths;
        public int NumberOfDeaths
        {
            get { return _numberOfDeaths; }
            set { _numberOfDeaths = value; }
        }

        int _damageTaken;
        public int DamageTaken
        {
            get { return _damageTaken; }
            set { _damageTaken = value; }
        }

        int _damageInflicted;
        public int DamageInflicted
        {
            get { return _damageInflicted; }
            set { _damageInflicted = value; }
        }

        public void SetToDefault()
        {
            _lives = 5;
        }

    }
}
