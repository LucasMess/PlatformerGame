using ThereMustBeAnotherWay.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using static ThereMustBeAnotherWay.TMBAW_Game;

namespace ThereMustBeAnotherWay.Levels
{
    public class WorldData
    {
        public Song Song;
        private SoundFx _ambience;
        public string LevelName = "";
        public bool HasClouds;
        public bool IsRaining;
        public bool IsSnowing;

        public bool Trigger0;
        public bool Trigger1;
        public bool Trigger2;
        public bool Trigger3;
        public bool Trigger4;
        public bool Trigger5;
        public bool Trigger6;

        public bool IsDealinGameWorldithData { get; set; }

        public TileType[] TileIDs { get; set; }
        public TileType[] WallIDs { get; set; }

        public int LevelWidth { get; set; }
        public int LevelHeight { get; set; }
        public byte BackgroundId { get; set; }
        public byte SoundtrackId { get; set; }
        public bool IsDarkOutline { get; set; }
        public Color SunLightColor { get; set; } = Color.White;

        public Vector2 SpawnPoint { get; set; }

        /// <summary>
        /// Used to retrieve special information about a tile in the index key.
        /// </summary>
        public Dictionary<int, string> MetaData { get; set; } = new Dictionary<int, string>();

        //public string[] MetaData { get; set; }

        public WorldData()
        {
        }

        public void Update()
        {
            _ambience?.PlayIfStopped();
        }
    }
}
