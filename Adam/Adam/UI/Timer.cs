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
        public double totalTime;
        int currentTime;
        public bool IsOver { get; set; }

        public GameTimer(int duration)
        {
            totalTime = duration * 1000;
        }

        public int GetTime(GameTime gameTime)
        {
            totalTime -= gameTime.ElapsedGameTime.TotalMilliseconds;
            currentTime = (int)totalTime / 1000;

            if (currentTime <= 0)
                IsOver = true;

            return currentTime;
        }




    }
}
