using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Misc
{
    public class Timer
    {
        double currentTimeInSeconds;
        double currentTimeInMilliSeconds;
        double notificationTime;

        public delegate void EventHandler();
        public event EventHandler SetTimeReached;

        /// <summary>
        /// Increments the timer by amount of time passed since last update.
        /// </summary>
        public void Increment()
        {
            currentTimeInSeconds += Main.GameTime.ElapsedGameTime.TotalSeconds;
            currentTimeInMilliSeconds += Main.GameTime.ElapsedGameTime.TotalMilliseconds;

            if (SetTimeReached != null && currentTimeInMilliSeconds > notificationTime)
            {
                SetTimeReached();
            }
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
        /// Resets the timer and sets a time in which the timer will fire an event.
        /// </summary>
        /// <param name="time"></param>
        public void ResetAndWaitFor(double time)
        {
            Reset();
            notificationTime = time;
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