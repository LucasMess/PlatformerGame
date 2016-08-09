namespace Adam.Misc
{
    /// <summary>
    /// Keeps track of whether the player can receive input if they are using an ability or doing another task.
    /// </summary>
    public class Countdown
    {        
        public delegate void EventHandler();
        public event EventHandler CountdownEnded;
    }
}
