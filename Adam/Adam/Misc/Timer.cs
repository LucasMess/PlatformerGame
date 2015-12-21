using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adam.Misc
{
    public class Timer
    {
        double _currentTimeInSeconds;
        double _currentTimeInMilliSeconds;
        double _notificationTime;

        public delegate void EventHandler();
        public event EventHandler SetTimeReached;

        public Timer()
        {
            Main.GameUpdateCalled += Increment;
        }

        /// <summary>
        /// Is set to true the timer will increment on each update tick.
        /// </summary>
        public bool IsOn { get; set; }

        /// <summary>
        /// Increments the timer by amount of time passed since last update.
        /// </summary>
        private void Increment(GameTime gameTime)
        {
            _currentTimeInSeconds += gameTime.ElapsedGameTime.TotalSeconds;
            _currentTimeInMilliSeconds += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (SetTimeReached != null && _currentTimeInMilliSeconds > _notificationTime)
            {
                SetTimeReached();
            }
        }

        /// <summary>
        /// Resets the timer to 0.
        /// </summary>
        public void Reset()
        {
            _currentTimeInMilliSeconds = 0;
            _currentTimeInSeconds = 0;
        }

        /// <summary>
        /// Sets the timer to a specific time.
        /// </summary>
        /// <param name="time"></param>
        public void SetToInMilliseconds(double time)
        {
            _currentTimeInMilliSeconds = time;
            _currentTimeInSeconds = time / 1000;
        }

        /// <summary>
        /// Resets the timer and sets a time in which the timer will fire an event.
        /// </summary>
        /// <param name="time"></param>
        public void ResetAndWaitFor(double time)
        {
            Reset();
            _notificationTime = time;
        }

        /// <summary>
        /// Returns the time passed in seconds since the last reset.
        /// </summary>
        public double TimeElapsedInSeconds
        {
            get
            {
                return _currentTimeInSeconds;
            }
        }

        /// <summary>
        /// Returns the time passed in milliseconds since the last reset.
        /// </summary>
        public double TimeElapsedInMilliSeconds
        {
            get
            {
                return _currentTimeInMilliSeconds;
            }
        }
    }
}