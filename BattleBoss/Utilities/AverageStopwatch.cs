using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BattleBoss.Utilities
{
    /// <summary>
    /// Calculates an average time in ticks.
    /// </summary>
    public class AverageStopwatch
    {
        private List<long> _values = new List<long>(100);
        private Stopwatch _stopwatch = new Stopwatch();

        public void Start()
        {
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        public void Reset()
        {
            _stopwatch.Reset();
        }

        public void Restart()
        {
            _stopwatch.Restart();
        }

        public void Measure()
        {
            _stopwatch.Stop();

            if (_values.Count == 100)
            {
                _values.RemoveAt(0);
            }
            _values.Add(_stopwatch.ElapsedTicks);
        }

        public long GetAverage()
        {
            long sum = 0;
            for (int i = 0; i < _values.Count; i++)
            {
                sum += _values[i];
            }
            if (_values.Count == 0)
                return 0;
            return sum / _values.Count;
        }



    }
}
