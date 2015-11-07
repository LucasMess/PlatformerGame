using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Misc
{
    public class TimeFreeze
    {
        private Timer freezeTimer = new Timer();

        public TimeFreeze()
        {
            freezeTimer.SetTimeReached += FreezeTimer_SetTimeReached;
        }

        private void FreezeTimer_SetTimeReached()
        {
            isTimeFrozen = false;
        }

        public void AddFrozenTime(double time)
        {
            isTimeFrozen = true;
            freezeTimer.ResetAndWaitFor(time);
        }

        private bool isTimeFrozen;

        public bool IsTimeFrozen()
        {
            freezeTimer.Increment();
            return isTimeFrozen;
        }
    }
}
