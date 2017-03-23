using Adam.Misc.Helpers;
using Microsoft.Xna.Framework.Media;

namespace Adam.Misc.Databases
{
    public static class SoundtrackDb
    {
        /// <summary>
        /// Returns the appropriate song from the database according to the ID given.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Song GetSong(byte id)
        {
            switch (id)
            {
                case 0: return ContentHelper.LoadSong("Music/Sunny Day");
                case 1: return ContentHelper.LoadSong("Music/Party Time"); // Garden of Eden
                case 2: return ContentHelper.LoadSong("Music/Phantom from Space");
                case 3: return ContentHelper.LoadSong("Music/Invincible");
                case 4: return ContentHelper.LoadSong("Music/Anguish");
                case 5: return ContentHelper.LoadSong("Music/Volatile Reaction"); // Chase scene maybe
                case 6: return ContentHelper.LoadSong("Music/Sky View"); // Tutorial
                case 7: return ContentHelper.LoadSong("Music/Another Earth");
                case 8: return ContentHelper.LoadSong("Music/Heart of Nowhere");

                case 100: return ContentHelper.LoadSong("Music/Menu/Force Reunite"); // Main Menu

                case 101: return ContentHelper.LoadSong("Music/Level Editor/Autumn Day");
                case 102: return ContentHelper.LoadSong("Music/Level Editor/Silver Blue Light");
                case 103: return ContentHelper.LoadSong("Music/Level Editor/Adam1");
                case 104: return ContentHelper.LoadSong("Music/Level Editor/Unwritten Return");

                default: return ContentHelper.LoadSong("Music/Party Time"); // Garden of Eden;
            }
        }
    }
}
