using Adam.Misc.Databases;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Misc
{
    public static class SoundtrackManager
    {

        public static byte CurrentID
        {
            get; private set;
        }

        static Song currentSong;

        /// <summary>
        /// Plays the specified track.
        /// </summary>
        /// <param name="ID"></param>
        public static void PlayTrack(byte ID, bool repeating)
        {
            if (ID != CurrentID)
            {
                MediaPlayer.Stop();
                CurrentID = ID;
                currentSong = SoundtrackDB.GetSong(CurrentID);
                MediaPlayer.Play(currentSong);
                MediaPlayer.IsRepeating = repeating;
            }
        }

        public static void Stop()
        {
            MediaPlayer.Stop();
        }

        /// <summary>
        /// Plays the main theme of the game.
        /// </summary>
        public static void PlayMainTheme()
        {
            PlayTrack(100, true);
        }

        /// <summary>
        /// Plays a random song from the level editor playlist.
        /// </summary>
        public static void PlayLevelEditorTheme()
        {
            byte newID = 0;
            if (CurrentID < 101 || CurrentID > 103)
            {
                newID = (byte)GameWorld.RandGen.Next(101, 103);
                PlayTrack(newID, false);
            }
            else if (MediaPlayer.State == MediaState.Stopped)
            {
                newID = (byte)GameWorld.RandGen.Next(101, 103);
                PlayTrack(newID, false);
            }
        }
    }
}
