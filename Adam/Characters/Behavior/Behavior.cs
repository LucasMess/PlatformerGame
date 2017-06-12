using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ThereMustBeAnotherWay.Characters.Behavior
{
    public class Behavior
    {
        protected Entity Entity;
        private List<Misc.Timer> _waitTimers = new List<Misc.Timer>();

        public virtual void Initialize(Entity entity)
        {
            this.Entity = entity;
        }

        public virtual void Update(Entity entity)
        {
            foreach (var timer in _waitTimers)
            {
                timer.Increment();
            }
        }

        public void Run()
        {
            Entity = Entity.Get();
        }
        /// <summary>
        /// Blocks execution of code until the time duration has passed.
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        protected void WaitThen(int duration, Action action)
        {
            Misc.Timer timer = new Misc.Timer();
            _waitTimers.Add(timer);
            timer.ResetAndWaitFor(duration, delegate ()
             {
                 action?.Invoke();
                 _waitTimers.Remove(timer);
             });
        }


        /// <summary>
        /// Performs the given action after the given duration.
        /// </summary>
        /// <param name="waitDuration"></param>
        /// <param name="action"></param>
        /// <returns>A timer that has the set time</returns>
        protected Misc.Timer CreateTimedAction(int duration, Action action)
        {
            Misc.Timer timer = new Misc.Timer();
            _waitTimers.Add(timer);
            timer.ResetAndWaitFor(duration, delegate ()
            {
                timer.Reset();
                action?.Invoke();
            }, true);
            return timer;
        }

    }
}