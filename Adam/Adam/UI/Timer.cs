using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    public class GameTimer
    {
        //This will countdown the time left for a level
        public double TotalTime;
        int _currentTime;
        public bool IsOver { get; set; }

        public GameTimer(int duration)
        {
            TotalTime = duration * 1000;
        }

        public int GetTime(GameTime gameTime)
        {
            TotalTime -= gameTime.ElapsedGameTime.TotalMilliseconds;
            _currentTime = (int)TotalTime / 1000;

            if (_currentTime <= 0)
                IsOver = true;

            return _currentTime;
        }




    }
}
