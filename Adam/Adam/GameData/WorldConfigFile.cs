using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.GameData
{
    [Serializable]
    public class WorldConfigFile
    {
        public byte[] TileIDs { get; set; }
        public byte[] WallIDs { get; set; }

        public short LevelWidth { get; set; }
        public short LevelHeight { get; set; }
        public string LevelName { get; set; }
        public string FileName { get; set; }

        public byte BackgroundID { get; set; }
        public byte SoundtrackID { get; set; }
        public byte AmbienceID { get; set; }

        public bool HasClouds { get; set; }
        public bool IsRaining { get; set; }
        public bool IsSnowing { get; set; }

        public Dictionary<int, string> SignMessages { get; set; }
        public Dictionary<int, int> PortalLinks { get; set; }

        public WorldConfigFile() { }

        public WorldConfigFile(short width, short height)
        {
            LevelWidth = width;
            LevelHeight = height;
            LevelName = "Unnamed Level";
            BackgroundID = 1;
            SoundtrackID = 1;

            SignMessages = new Dictionary<int, string>();
            PortalLinks = new Dictionary<int, int>();

            TileIDs = new byte[LevelWidth * LevelHeight];
            WallIDs = new byte[LevelWidth * LevelHeight];

            HasClouds = false;
            IsSnowing = false;
            IsRaining = false;
        }

        public WorldConfigFile(GameWorld gw)
        {
            //Creates arrays for the tiles.
            int size = gw.tileArray.Length;
            TileIDs = new byte[size];
            WallIDs = new byte[size];

            //Sets the dimensions of the level.
            LevelWidth = (short)gw.worldData.LevelWidth;
            LevelHeight = (short)gw.worldData.LevelHeight;

            //Sets soundtrack and background.
            BackgroundID = gw.worldData.BackgroundID;
            SoundtrackID = gw.worldData.SoundtrackID;

            //Gets IDs of the arrays
            for (int i = 0; i < size; i++)
            {
                TileIDs[i] = gw.tileArray[i].ID;
                WallIDs[i] = gw.wallArray[i].ID;
            }

            //Gets sign texts
            SignMessages = gw.worldData.SignMessages;
        }

        public void LoadIntoEditor()
        {
            TransferDataToWorldData();
            GameWorld.Instance.game1.LoadWorldFromFile(GameMode.Edit);
        }

        public void LoadIntoPlay()
        {
            TransferDataToWorldData();
            GameWorld.Instance.game1.LoadWorldFromFile(GameMode.Play);
        }

        public void TransferDataToWorldData()
        {
            GameWorld gw = GameWorld.Instance;

            gw.worldData.TileIDs = TileIDs;
            gw.worldData.WallIDs = WallIDs;

            gw.worldData.LevelWidth = LevelWidth;
            gw.worldData.LevelHeight = LevelHeight;

            gw.worldData.BackgroundID = BackgroundID;
            gw.worldData.SoundtrackID = SoundtrackID;

            gw.worldData.SignMessages = SignMessages;
            gw.worldData.PortalLinks = PortalLinks;

            gw.worldData.levelName = LevelName;
            gw.worldData.HasClouds = HasClouds;
            gw.worldData.IsRaining = IsRaining;
            gw.worldData.IsSnowing = IsSnowing;
        }
    }
}
