using Adam.Characters.Non_Playable;
using Adam.GameData;
using Adam.Misc;
using Adam.Misc.Sound;
using Adam.UI;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace Adam.Levels
{
    /// <summary>
    /// The show must go on! This class is responsible for taking the player where he needs to go.
    /// </summary>
    public static class StoryTracker
    {
        public static PlayerProfile Profile { get; set; }
        public static Timer StoryTimer = new Timer();

        static COM comNPC = new COM(0, 0);

        public static bool IsInStoryMode = false;

        public static bool trigger1 = false;
        public static bool trigger2 = false;
        public static bool trigger3 = false;
        public static bool trigger4 = false;
        public static bool trigger5 = false;

        private static class GreenHills
        {
            public static bool FadedOut = false;
        }

        /// <summary>
        /// Sets up the game progression.
        /// </summary>
        public static void Initialize()
        {



        }

        public static void ResumeFromSavePoint()
        {
            IsInStoryMode = true;

            // It's a new game, so go to the tutorial.
            if (!Profile.HasPlayedTutorial)
            {

            }
            else
            {
                Overlay.FadeToBlack();
                DataFolder.PlayStoryLevel(Profile.CurrentLevel);
            }
        }

        public static void Update()
        {
            StoryTimer.Increment();
            switch (Profile.CurrentLevel)
            {
                case "GreenHills01":
                    if (StoryTimer.TimeElapsedInMilliSeconds > 1000 && !GetVal("hasStartedWakeupDialog"))
                    {
                        AddTrigger("hasStartedWakeupDialog");
                        comNPC.ShowDialog("greenhills01-wakeup", 0);
                    }
                    if (GetVal("hasWokenUp"))
                    {
                        if (!GetVal("resetTimer"))
                        {
                            MediaPlayer.Resume();
                            StoryTimer.Reset();
                            GameWorld.GetPlayer().IsFacingRight = false;
                            AddTrigger("resetTimer");
                        }
                        else
                        {
                            if (StoryTimer.TimeElapsedInMilliSeconds > 2000 && !GetVal("hasLookedRight"))
                            {
                                if (!GetVal("hasLookedLeft"))
                                {
                                    GameWorld.GetPlayer().IsFacingRight = true;
                                    AddTrigger("hasLookedLeft");
                                    StoryTimer.Reset();
                                }
                                else if (!GetVal("hasLookedRight"))
                                {
                                    GameWorld.GetPlayer().IsFacingRight = false;
                                    AddTrigger("hasLookedRight");
                                    StoryTimer.Reset();
                                }
                            }
                        }
                        if (GetVal("hasLookedRight") && !GetVal("whatHappened"))
                        {
                            comNPC.ShowDialog("greenhills01-whathappened", 0);
                            AddTrigger("whatHappened");
                        }
                    }

                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Loads the tutorial level and takes the player there.
        /// </summary>
        private static void StartTutorial()
        {
            DataFolder.PlayStoryLevel("tutorial");
        }

        /// <summary>
        /// Switches active profiles.
        /// </summary>
        public static void SwitchProfile()
        {
            // TODO: Allow player to switch profiles.
        }

        public static Dictionary<string, bool> _triggers = new Dictionary<string, bool>();

        public static bool GetVal(string key)
        {
            if (_triggers.ContainsKey(key))
            {
                return _triggers[key];
            }
            else return false;
        }

        public static void AddTrigger(string key)
        {
            _triggers.Add(key, true);
        }

    }
}
