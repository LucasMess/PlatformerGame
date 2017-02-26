using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.BitmapFonts;
using System;

namespace Adam.Misc.Helpers
{
    /// <summary>
    /// Provides simple loading of assets that is easier to type.
    /// </summary>
    public static class ContentHelper
    {
        /// <summary>
        /// Loads the Texture2D at the specified file path.
        /// </summary>
        /// <param name="file">The file path of the texture.</param>
        /// <returns>Loaded texture.</returns>
        public static Texture2D LoadTexture(string file)
        {
            try
            {
                return AdamGame.Content.Load<Texture2D>(file);
            }
            catch (ContentLoadException)
            {
                Console.WriteLine("Texture2D location ({0}) could not be found. Make sure the file path is spelled correctly or that the file exists.", file);
                throw;
                return AdamGame.DefaultTexture;
            }
        }

        /// <summary>
        /// Loads the SoundEffect at the specified file path.
        /// </summary>
        /// <param name="file">The file path of the sound effect.</param>
        /// <returns>Loaded sound effect.</returns>
        public static SoundEffect LoadSound(string file)
        {
            try
            {
                return AdamGame.Content.Load<SoundEffect>(file);
            }
            catch (ContentLoadException)
            {
                Console.WriteLine("SoundEffect location ({0}) could not be found. Make sure the file path is spelled correctly or that the file exists.", file);
                throw;
            }
        }

        /// <summary>
        /// Loads the Song at the specified file path.
        /// </summary>
        /// <param name="file">The file path of the song.</param>
        /// <returns>Loaded song.</returns>
        public static Song LoadSong(string file)
        {
            try
            {
                return AdamGame.Content.Load<Song>(file);
            }
            catch (ContentLoadException)
            {
                Console.WriteLine("Song location ({0}) could not be found. Make sure the file path is spelled correctly or that the file exists.", file);
                throw;
            }
        }

        /// <summary>
        /// Loads the BitmapFont at the specified file path.
        /// </summary>
        /// <param name="file">The file path of the font.</param>
        /// <returns>Loaded font.</returns>
        public static BitmapFont LoadFont(string file)
        {
            try
            {
                return AdamGame.Content.Load<BitmapFont>(file);
            }
            catch (ContentLoadException)
            {
                Console.WriteLine("BitmapFont location ({0}) could not be found. Make sure the file path is spelled correctly or that the file exists.", file);
                throw;
            }
        }

        /// <summary>
        /// Loads the BitmapFont at the specified file path.
        /// </summary>
        /// <param name="file">The file path of the font.</param>
        /// <returns>Loaded font.</returns>
        public static Effect LoadEffect(string file)
        {
            try
            {
                return AdamGame.Content.Load<Effect>(file);
            }
            catch (ContentLoadException)
            {
                Console.WriteLine("Effect location ({0}) could not be found. Make sure the file path is spelled correctly or that the file exists.", file);
                throw;
            }
        }


    }
}
