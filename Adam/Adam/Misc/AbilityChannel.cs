using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Misc
{
    /// <summary>
    /// Keeps track of whether the player can receive input if they are using an ability or doing another task.
    /// </summary>
    public class AbilityChannel
    {
        bool isChanneling;

        public delegate void EventHandler();
        public event EventHandler AbilityCompleted;
        public event EventHandler AbilityStarted;
        public event EventHandler AbilityCancelled;
        
        public void Update()
        {

        }

        /// <summary>
        /// Returns whether there are any abilities or tasks queued for the player to do.
        /// </summary>
        /// <returns></returns>
        public bool CanCastAbility()
        {
            return !isChanneling;
        }

    }
}
