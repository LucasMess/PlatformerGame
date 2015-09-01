using Adam.Misc.Databases;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Misc
{
    public class SoundtrackManager
    {

        public byte CurrentID
        {
            get; private set;
        }

        Song currentSong;

        public SoundtrackManager()
        {

        }

        /// <summary>
        /// Plays the specified track.
        /// </summary>
        /// <param name="ID"></param>
        public void PlayTrack(byte ID)
        {
            CurrentID = ID;
            currentSong = SoundtrackDB.GetSong(ID);
            MediaPlayer.Play(currentSong);
            MediaPlayer.IsRepeating = true;
        }

        /// <summary>
        /// Plays the main theme of the game.
        /// </summary>
        public void PlayMainTheme()
        {
            PlayTrack(0);
        }

        /// <summary>
        /// Plays a random song from the level editor playlist.
        /// </summary>
        public void PlayLevelEditorTheme()
        {
        }
    }
}
