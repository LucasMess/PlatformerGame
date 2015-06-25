using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Levels
{
    class WorldData
    {
        public Texture2D mainMap;
        public Texture2D wallMap;
        public Song song;
        public string levelName = "";
        public bool wantClouds;

        public WorldData(Level CurrentLevel)
        {
            //Default
            levelName = "Garden of Eden";
            mainMap = ContentHelper.LoadTexture("Levels/1-2_main");
            wallMap = ContentHelper.LoadTexture("Levels/1-2_wall");
            song = ContentHelper.LoadSong("Music/Vivacity");
            wantClouds = true;

            switch (CurrentLevel)
            {
                case Level.Level0:
                    break;
                case Level.Level1and1:
                    levelName = "Garden of Eden";
                    mainMap = ContentHelper.LoadTexture("Levels/1-2_main");
                    wallMap = ContentHelper.LoadTexture("Levels/1-2_wall");
                    song = ContentHelper.LoadSong("Music/Vivacity");
                    wantClouds = true;
                    break;
                case Level.Level1and2:
                    break;
                case Level.Level1and3:
                    break;
                case Level.Level2and1:
                    levelName = "Desolate Desert";
                    mainMap = ContentHelper.LoadTexture("Levels/2-1_main");
                    wallMap = ContentHelper.LoadTexture("Levels/2-1_wall");
                    song = ContentHelper.LoadSong("Music/Desert City");
                    wantClouds = true;
                    break;
                case Level.Level3and1:
                    break;
                case Level.Level4and1:
                    break;
                case Level.Level8and1:
                    levelName = "Entrance to Hell";
                    mainMap = ContentHelper.LoadTexture("Levels/debug_main");
                    wallMap = ContentHelper.LoadTexture("Levels/debug_wall");
                    song = ContentHelper.LoadSong("Music/Heart of Nowhere");
                    wantClouds = true;
                    break;
                default:
                    break;
            }

        }
    }
}
