using ThereMustBeAnotherWay.Characters.Non_Playable;
using ThereMustBeAnotherWay.GameData;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.UI;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace ThereMustBeAnotherWay.Levels
{
    /// <summary>
    /// The show must go on! This class is responsible for taking the player where he needs to go.
    /// </summary>
    public static class StoryTracker
    {
        public static PlayerProfile Profile { get; set; }
        public static GameTimer StoryTimer = new GameTimer();

        static COM comNPC = new COM(0, 0);

        public static bool IsInStoryMode = false;
        public static bool InCutscene = false;

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

        public static void OnLevelLoad()
        {
            if (Profile == null) return;
            switch (Profile.CurrentLevel)
            {
                case "GreenHills01":
                    MediaPlayer.Pause();
                    Overlay.BlackBars.Show();
                    InCutscene = true;
                    break;
                default:
                    break;
            }
        }

        public static void Update()
        {
            StoryTimer.Increment();
            switch (Profile.CurrentLevel)
            {
                case "GreenHills01":
                    if (GetVal("CaveMustBeOnlyWay") && !GetVal("CaveMustBeOnlyWay-2"))
                    {
                        AddTrigger("CaveMustBeOnlyWay-2");
                        comNPC.ShowDialog("greenHills01-cave", 0);
                    }
                    if (GetVal("Door") && !GetVal("Door-2"))
                    {
                        AddTrigger("Door-2");
                        comNPC.ShowDialog("greenHills01-door", 0);
                    }

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
                            GameWorld.GetPlayers()[0].IsFacingRight = false;
                            AddTrigger("resetTimer");
                        }
                        else
                        {
                            if (StoryTimer.TimeElapsedInMilliSeconds > 2000 && !GetVal("hasLookedRight"))
                            {
                                if (!GetVal("hasLookedLeft"))
                                {
                                    GameWorld.GetPlayers()[0].IsFacingRight = true;
                                    AddTrigger("hasLookedLeft");
                                    StoryTimer.Reset();
                                }
                                else if (!GetVal("hasLookedRight"))
                                {
                                    GameWorld.GetPlayers()[0].IsFacingRight = false;
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
                        if (GetVal("sentCoordinates"))
                        {
                            InCutscene = false;
                            Overlay.BlackBars.Hide();
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
            if (!_triggers.ContainsKey(key))
            {
                Console.WriteLine("Added trigger: " + key);
                _triggers.Add(key, true);
            }
        }

    }
}
