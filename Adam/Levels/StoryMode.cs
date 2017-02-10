using Adam.GameData;
using Adam.UI;

namespace Adam.Levels
{
    /// <summary>
    /// The show must go on! This class is responsible for taking the player where he needs to go.
    /// </summary>
    public static class StoryMode
    {
        public static PlayerProfile Profile { get; private set; }

        /// <summary>
        /// Sets up the game progression.
        /// </summary>
        public static void Initialize()
        {
            // Look for most recent profile in the data folder.

            // If there is no profile, offer to create a new one.


        }

        public static void ResumeFromSavePoint()
        {
            // It's a new game, so go to the tutorial.
            if (Profile.CurrentLevel == 0)
            {

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

    }
}
