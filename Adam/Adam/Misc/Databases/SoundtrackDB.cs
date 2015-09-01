using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Misc.Databases
{
    public static class SoundtrackDB
    {
        /// <summary>
        /// Returns the appropriate song from the database according to the ID given.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static Song GetSong(byte ID)
        {
            switch (ID)
            {
                case 0: return ContentHelper.LoadSong("Music/Force Reunite"); // Main Menu
                case 1: return ContentHelper.LoadSong("Music/Party Time"); // Garden of Eden
                case 2: return ContentHelper.LoadSong("Music/Autumn"); // Level Editor 1
                case 3: return ContentHelper.LoadSong("Music/Silver Blue Light"); // Level Editor 2
                case 4: return ContentHelper.LoadSong("Music/Adam1"); // Nathan's Theme
                case 5: return ContentHelper.LoadSong("Music/Volatile Reaction"); // Chase scene maybe
                default: return ContentHelper.LoadSong("Music/Party Time"); // Garden of Eden;
            }
        }
    }
}
