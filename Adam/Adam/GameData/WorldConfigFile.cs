using Adam.Misc.Databases;
using Adam.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

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

        public AdamDictionary SignMessages { get; set; }
        public AdamDictionary PortalLinks { get; set; }

        public WorldConfigFile() { }

        public WorldConfigFile(string levelName ,short width, short height)
        {
            if (levelName == null)
                levelName = "Name not Found";
            LevelWidth = width;
            LevelHeight = height;
            LevelName = levelName;
            BackgroundID = 1;
            SoundtrackID = 1;

            SignMessages = new AdamDictionary();
            PortalLinks = new AdamDictionary();

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
            LevelName = gw.worldData.LevelName;

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
            PortalLinks = gw.worldData.PortalLinks;

            //Level conditions
            IsRaining = gw.worldData.IsRaining;
            IsSnowing = gw.worldData.IsSnowing;
            HasClouds = gw.worldData.HasClouds;
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

            gw.worldData.LevelName = LevelName;
            gw.worldData.HasClouds = HasClouds;
            gw.worldData.IsRaining = IsRaining;
            gw.worldData.IsSnowing = IsSnowing;

            gw.worldData.song = SoundtrackDB.GetSong(1);
        }
    }

    public class AdamDictionary
    {
        List<KeyValue> keyValues = new List<KeyValue>();
        public AdamDictionary()
        {

        }

        /// <summary>
        /// Add a new entry to the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyword"></param>
        public void Add(int key, object keyword)
        {
            KeyValue newKey = new KeyValue(key, keyword);
            keyValues.Add(newKey);
        }

        /// <summary>
        /// Try retrieving a value.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object TryGetValue(int key)
        {
            foreach (KeyValue kv in keyValues)
            {
                if (kv.Key == key)
                {
                    return kv.Value;
                }
            }

            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Remove specified key.
        /// </summary>
        /// <param name="key"></param>
        public void Remove(int key)
        {
            foreach (KeyValue kv in keyValues)
            {
                if (kv.Key == key)
                {
                    keyValues.Remove(kv);
                    return;
                }
            }

            throw new KeyNotFoundException();
        }
    }

    public struct KeyValue
    {
        public int Key { get; set; }
        public object Value { get; set; }

        public KeyValue(int key, object value)
        {
            Key = key;
            Value = value;
        }
    }
}
