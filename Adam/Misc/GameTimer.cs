﻿using System;

namespace ThereMustBeAnotherWay.Misc
{
    public class GameTimer
    {
        public static int ActiveTimers = 0;

        private double _currentTimeInSeconds;
        private double _currentTimeInMilliSeconds;
        private int _notificationTime;
        public bool IsInfinite { get; set; }

        public delegate void EventHandler();
        public event EventHandler SetTimeReached;

        public GameTimer(bool isInfinite = false, double initialTime = 0)
        {
            IsInfinite = isInfinite;
            if (IsInfinite)
            {
                TMBAW_Game.GameUpdateCalled += Increment;
                ActiveTimers++;
            }
            SetToInMilliseconds(initialTime);
        }

        /// <summary>
        /// Is set to true the timer will increment on each update tick.
        /// </summary>
        public bool IsOn { get; set; }

        /// <summary>
        /// Increments the timer by amount of time passed since last update.
        /// </summary>
        public void Increment()
        {
            _currentTimeInSeconds += TMBAW_Game.GameTime.ElapsedGameTime.TotalSeconds;
            _currentTimeInMilliSeconds += TMBAW_Game.GameTime.ElapsedGameTime.TotalMilliseconds;

            if (_currentTimeInMilliSeconds > _notificationTime)
            {
                SetTimeReached?.Invoke();
                if (!IsInfinite)
                {
                    //Destroy();
                }
            }

            //// Destroys timer if it is running for too long.
            //if (_notificationTime == 0 && _currentTimeInSeconds > 60 && !IsInfinite)
            //{
            //    Console.WriteLine("A timer timedout!");
            //    Destroy();
            //}
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
        public void ResetAndWaitFor(int time)
        {
            Reset();
            ChangeWaitTime(time);
        }

        /// <summary>
        /// Calls the function passed after a certain time once.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="action"></param>
        public void ResetAndWaitFor(int time, Action action, bool isRecurring = false)
        {
            ResetAndWaitFor(time);
            EventHandler onTimeReached = delegate ()
            {
                action?.Invoke();
                if (!isRecurring)
                    SetTimeReached -= SetTimeReached;
            };
            SetTimeReached += onTimeReached;
        }

        /// <summary>
        /// Used to change the time until the notification without resetting the timer.
        /// </summary>
        /// <param name="time"></param>
        public void ChangeWaitTime(int time)
        {
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

        /// <summary>
        /// Clears all references and events.
        /// </summary>
        public void Destroy()
        {
            if (IsInfinite)
            {
                TMBAW_Game.GameUpdateCalled -= Increment;
                ActiveTimers--;
            }
        }
    }
}