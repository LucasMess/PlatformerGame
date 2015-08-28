using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Misc
{
    class Timer
    {
        double currentTimeInSeconds;
        double currentTimeInMilliSeconds;

        /// <summary>
        /// Increments the timer by amount of time passed since last update.
        /// </summary>
        public void Increment()
        {
            currentTimeInSeconds += GameWorld.Instance.gameTime.ElapsedGameTime.TotalSeconds;

            currentTimeInMilliSeconds += GameWorld.Instance.gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        /// <summary>
        /// Resets the timer to 0.
        /// </summary>
        public void Reset()
        {
            currentTimeInMilliSeconds = 0;
            currentTimeInSeconds = 0;
        }

        /// <summary>
        /// Sets the timer to a specific time.
        /// </summary>
        /// <param name="time"></param>
        public void SetToInMilliseconds(double time)
        {
            currentTimeInMilliSeconds = time;
            currentTimeInSeconds = time / 1000;
        }

        /// <summary>
        /// Returns the time passed in seconds since the last reset.
        /// </summary>
        public double TimeElapsedInSeconds
        {
            get
            {
                return currentTimeInSeconds;
            }
        }

        /// <summary>
        /// Returns the time passed in milliseconds since the last reset.
        /// </summary>
        public double TimeElapsedInMilliSeconds
        {
            get
            {
                return currentTimeInMilliSeconds;
            }
        }
    }
}