using Adam;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    public static class ContentHelper
    {
        public static Texture2D LoadTexture(string file)
        {
            return Game1.Content.Load<Texture2D>(file);
        }
        public static SoundEffect LoadSound(string file)
        {
            return Game1.Content.Load<SoundEffect>(file);
        }
        public static Song LoadSong(string file)
        {
            return Game1.Content.Load<Song>(file);
        }
        public static SpriteFont LoadFont(string file)
        {
            return Game1.Content.Load<SpriteFont>(file);
        }


    }
}
