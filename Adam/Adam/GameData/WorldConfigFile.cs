using Adam.Misc.Databases;
using Adam.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using Adam.Levels;

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

        public byte BackgroundId { get; set; }
        public byte SoundtrackId { get; set; }
        public byte AmbienceId { get; set; }

        public bool HasClouds { get; set; }
        public bool IsRaining { get; set; }
        public bool IsSnowing { get; set; }

        public bool CanBeEdited { get; set; } = true;

        public string[] MetaData { get; set; }

        public WorldConfigFile() { }

        public WorldConfigFile(string levelName ,short width, short height)
        {
            if (levelName == null)
                levelName = "Name not Found";
            LevelWidth = width;
            LevelHeight = height;
            LevelName = levelName;
            BackgroundId = 1;
            SoundtrackId = 1;

            MetaData = new string[LevelWidth * LevelHeight];

            TileIDs = new byte[LevelWidth * LevelHeight];
            WallIDs = new byte[LevelWidth * LevelHeight];

            HasClouds = false;
            IsSnowing = false;
            IsRaining = false;
        }

        public void GetDataFromGameWorld()
        {
            //Creates arrays for the tiles.
            int size = GameWorld.TileArray.Length;
            TileIDs = new byte[size];
            WallIDs = new byte[size];

            //Sets the dimensions of the level.
            LevelWidth = (short)GameWorld.WorldData.LevelWidth;
            LevelHeight = (short)GameWorld.WorldData.LevelHeight;
            LevelName = GameWorld.WorldData.LevelName;

            //Sets soundtrack and background.
            BackgroundId = GameWorld.WorldData.BackgroundId;
            SoundtrackId = GameWorld.WorldData.SoundtrackId;

            MetaData = GameWorld.WorldData.MetaData;

            //Gets IDs of the arrays
            for (int i = 0; i < size; i++)
            {
                TileIDs[i] = GameWorld.TileArray[i].Id;
                WallIDs[i] = GameWorld.WallArray[i].Id;
            }

            //Level conditions
            IsRaining = GameWorld.WorldData.IsRaining;
            IsSnowing = GameWorld.WorldData.IsSnowing;
            HasClouds = GameWorld.WorldData.HasClouds;
        }

        public void LoadIntoEditor()
        {
            TransferDataToWorldData();
            //GameWorld.game1.LoadWorldFromFile(GameMode.Edit);
            Main.ChangeState(GameState.GameWorld, GameMode.Edit);
        }

        public void LoadIntoPlay()
        {
            TransferDataToWorldData();
            //GameWorld.game1.LoadWorldFromFile(GameMode.Play);
            Main.ChangeState(GameState.GameWorld, GameMode.Play);
        }

        public void TransferDataToWorldData()
        {
            GameWorld.WorldData.TileIDs = TileIDs;
            GameWorld.WorldData.WallIDs = WallIDs;

            GameWorld.WorldData.LevelWidth = LevelWidth;
            GameWorld.WorldData.LevelHeight = LevelHeight;

            GameWorld.WorldData.BackgroundId = BackgroundId;
            GameWorld.WorldData.SoundtrackId = SoundtrackId;

            GameWorld.WorldData.MetaData = MetaData;

            GameWorld.WorldData.LevelName = LevelName;
            GameWorld.WorldData.HasClouds = HasClouds;
            GameWorld.WorldData.IsRaining = IsRaining;
            GameWorld.WorldData.IsSnowing = IsSnowing;

            GameWorld.WorldData.Song = SoundtrackDb.GetSong(1);
        }

        /// <summary>
        /// Makes multiple checks to see if the world is playable.
        /// </summary>
        /// <returns>Returns true if level is valid.</returns>
        public bool IsValidLevel()
        {
            foreach (int id in  TileIDs)
            { 
                // Found player.
                if (id == 200)
                    return true;
            }

            return false;
        }
    }

    [Serializable]
    public class AdamDictionary
    {
        List<KeyValue> _keyValues = new List<KeyValue>();
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
            _keyValues.Add(newKey);
        }

        /// <summary>
        /// Try retrieving a value.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object TryGetValue(int key)
        {
            foreach (KeyValue kv in _keyValues)
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
            foreach (KeyValue kv in _keyValues)
            {
                if (kv.Key == key)
                {
                    _keyValues.Remove(kv);
                    return;
                }
            }

            throw new KeyNotFoundException();
        }

    }

    [Serializable]
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
