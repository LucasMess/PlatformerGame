using Adam.Misc.Databases;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam.Misc
{
    public static class SoundtrackManager
    {

        public static byte CurrentId
        {
            get; private set;
        }

        static Song _currentSong;

        /// <summary>
        /// Plays the specified track.
        /// </summary>
        /// <param name="id"></param>
        public static void PlayTrack(byte id, bool repeating)
        {
            if (id != CurrentId)
            {
                MediaPlayer.Stop();
                CurrentId = id;
                _currentSong = SoundtrackDb.GetSong(CurrentId);
                MediaPlayer.Play(_currentSong);
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
            byte newId = 0;
            if (CurrentId < 101 || CurrentId > 103)
            {
                newId = (byte)GameWorld.RandGen.Next(101, 103);
                PlayTrack(newId, false);
            }
            else if (MediaPlayer.State == MediaState.Stopped)
            {
                newId = (byte)GameWorld.RandGen.Next(101, 103);
                PlayTrack(newId, false);
            }
        }
    }
}
