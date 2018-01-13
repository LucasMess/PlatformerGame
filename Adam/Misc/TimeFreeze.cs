namespace ThereMustBeAnotherWay.Misc
{
    public class TimeFreeze
    {
        private GameTimer _freezeTimer = new GameTimer(true);

        public TimeFreeze()
        {
            _freezeTimer.SetTimeReached += FreezeTimer_SetTimeReached;
        }

        private void FreezeTimer_SetTimeReached()
        {
            _isTimeFrozen = false;
        }

        public void AddFrozenTime(int time)
        {
            _isTimeFrozen = true;
            _freezeTimer.ResetAndWaitFor(time);
        }

        private bool _isTimeFrozen;

        public bool IsTimeFrozen()
        {
            return _isTimeFrozen;
        }
    }
}
