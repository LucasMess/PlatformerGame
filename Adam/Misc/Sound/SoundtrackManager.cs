using ThereMustBeAnotherWay.Misc.Databases;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace ThereMustBeAnotherWay.Misc.Sound
{
    public static class SoundtrackManager
    {
        static bool isPaused = false;

        public static byte CurrentId
        {
            get; private set;
        }

        static Song _currentSong;

        /// <summary>
        /// Plays the specified track.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="repeating"></param>
        public static void PlayTrack(byte id, bool repeating)
        {
            if (TMBAW_Game.IsMusicMuted)
                return;
            if (id != CurrentId)
            {
                MediaPlayer.Stop();
                CurrentId = id;
                _currentSong = SoundtrackDb.GetSong(CurrentId);
                MediaPlayer.Play(_currentSong);
                MediaPlayer.IsRepeating = repeating;
            }
        }

        public static void Update()
        {
            if (isPaused)
            {
                MediaPlayer.Pause();
            }
            else
            {
                MediaPlayer.Resume();
            }
            isPaused = false;
        }

        public static void Pause()
        {
            isPaused = true;
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
                newId = (byte)TMBAW_Game.Random.Next(101, 103);
                PlayTrack(newId, false);
            }
            else if (MediaPlayer.State == MediaState.Stopped)
            {
                newId = (byte)TMBAW_Game.Random.Next(101, 103);
                PlayTrack(newId, false);
            }
        }
    }
}
