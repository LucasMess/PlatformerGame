using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Misc
{
    public class TimeFreeze
    {
        private Timer _freezeTimer = new Timer();

        public TimeFreeze()
        {
            _freezeTimer.SetTimeReached += FreezeTimer_SetTimeReached;
        }

        private void FreezeTimer_SetTimeReached()
        {
            _isTimeFrozen = false;
        }

        public void AddFrozenTime(double time)
        {
            _isTimeFrozen = true;
            _freezeTimer.ResetAndWaitFor(time);
        }

        private bool _isTimeFrozen;

        public bool IsTimeFrozen()
        {
            _freezeTimer.Increment();
            return _isTimeFrozen;
        }
    }
}
