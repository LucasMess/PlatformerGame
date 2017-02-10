using Adam.Levels;
using Adam.Misc.Databases;
using System;
using System.Collections.Generic;
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

        public byte BackgroundId { get; set; }
        public byte SoundtrackId { get; set; }
        public byte AmbienceId { get; set; }

        public bool HasClouds { get; set; }
        public bool IsRaining { get; set; }
        public bool IsSnowing { get; set; }

        public bool CanBeEdited { get; set; } = true;

        public string[] MetaData { get; set; }

        public WorldConfigFile() { }

        public WorldConfigFile(string levelName, short width, short height)
        {
            if (levelName == null)
                levelName = "Name not Found";
            LevelWidth = width;
            LevelHeight = height;
            LevelName = levelName;
            BackgroundId = 1;
            SoundtrackId = 1;

            TileIDs = new byte[LevelWidth * LevelHeight];
            WallIDs = new byte[LevelWidth * LevelHeight];

            HasClouds = true;
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

            // For the metadata, you need to convert the Dictionary into a string array.
            List<string> values = new List<string>();
            StringBuilder builder = new StringBuilder();
            foreach (var key in GameWorld.WorldData.MetaData.Keys)
            {
                builder.Clear();
                builder.Append(key + " ");
                string value;
                if (GameWorld.WorldData.MetaData.TryGetValue(key, out value))
                {
                    builder.Append(value);
                    values.Add(builder.ToString());
                }
            }

            MetaData = values.ToArray();



            //Gets IDs of the arrays
            for (int i = 0; i < size; i++)
            {
                TileIDs[i] = GameWorld.WorldData.TileIDs[i];
                WallIDs[i] = GameWorld.WorldData.WallIDs[i];
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
            AdamGame.ChangeState(GameState.GameWorld, GameMode.Edit, true);
        }

        public void LoadIntoPlay()
        {
            TransferDataToWorldData();
            //GameWorld.game1.LoadWorldFromFile(GameMode.Play);
            AdamGame.ChangeState(GameState.GameWorld, GameMode.Play, true);
        }

        public void LoadIntoView()
        {
            TransferDataToWorldData();
            AdamGame.ChangeState(AdamGame.CurrentGameState, AdamGame.CurrentGameMode, true);
        }

        public void TransferDataToWorldData()
        {
            GameWorld.WorldData.TileIDs = TileIDs;
            GameWorld.WorldData.WallIDs = WallIDs;

            GameWorld.WorldData.LevelWidth = LevelWidth;
            GameWorld.WorldData.LevelHeight = LevelHeight;

            GameWorld.WorldData.BackgroundId = BackgroundId;
            GameWorld.WorldData.SoundtrackId = SoundtrackId;

            // Need to convert metadata xml string array to dict.
            GameWorld.WorldData.MetaData.Clear();
            foreach (var keyVal in MetaData)
            {
                if (keyVal == null) continue;
                string[] keyValSeparated = keyVal.Split(' ');
                int key;
                int.TryParse(keyValSeparated[0], out key);
                GameWorld.WorldData.MetaData.Add(key, keyValSeparated[1]);
            }

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
            foreach (int id in TileIDs)
            {
                // Found player.
                if (id == 200)
                    return true;
            }

            return false;
        }
    }
}
