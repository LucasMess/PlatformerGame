using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc.Databases;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using static ThereMustBeAnotherWay.TMBAW_Game;

namespace ThereMustBeAnotherWay.GameData
{
    [Serializable]
    public class WorldConfigFile
    {
        public int[] TileIDs { get; set; }
        public int[] WallIDs { get; set; }

        public short LevelWidth { get; set; }
        public short LevelHeight { get; set; }
        public string LevelName { get; set; }

        public byte BackgroundId { get; set; }
        public byte SoundtrackId { get; set; }
        public byte AmbienceId { get; set; }

        public bool HasClouds { get; set; }
        public bool IsRaining { get; set; }
        public bool IsSnowing { get; set; }
        public bool IsDarkOutline { get; set; }
        public Color SunLightColor { get; set; } = Color.White;

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

            TileIDs = new int[LevelWidth * LevelHeight];
            WallIDs = new int[LevelWidth * LevelHeight];

            HasClouds = true;
            IsSnowing = false;
            IsRaining = false;
            IsDarkOutline = false;
            SunLightColor = Color.White;
        }

        public void GetDataFromGameWorld()
        {
            //Creates arrays for the tiles.
            int size = GameWorld.TileArray.Length;
            TileIDs = new int[size];
            WallIDs = new int[size];

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
                if (GameWorld.WorldData.MetaData.TryGetValue(key, out string value))
                {
                    builder.Append(value);
                    values.Add(builder.ToString());
                }
            }

            MetaData = values.ToArray();



            //Gets IDs of the arrays
            for (int i = 0; i < size; i++)
            {
                TileIDs[i] = (byte)GameWorld.WorldData.TileIDs[i];
                WallIDs[i] = (byte)GameWorld.WorldData.WallIDs[i];
            }

            //Level conditions
            IsRaining = GameWorld.WorldData.IsRaining;
            IsSnowing = GameWorld.WorldData.IsSnowing;
            HasClouds = GameWorld.WorldData.HasClouds;
            IsDarkOutline = GameWorld.WorldData.IsDarkOutline;
            SunLightColor = GameWorld.WorldData.SunLightColor;
        }

        public void LoadIntoEditor()
        {
            TransferDataToWorldData();
            //GameWorld.game1.LoadWorldFromFile(GameMode.Edit);
            TMBAW_Game.ChangeState(GameState.GameWorld, GameMode.Edit, true);
        }

        public void LoadIntoPlay()
        {
            TransferDataToWorldData();
            //GameWorld.game1.LoadWorldFromFile(GameMode.Play);
            TMBAW_Game.ChangeState(GameState.GameWorld, GameMode.Play, true);
        }

        public void LoadIntoView()
        {
            TransferDataToWorldData();
            TMBAW_Game.ChangeState(TMBAW_Game.CurrentGameState, GameMode.None, true);
        }

        public void TransferDataToWorldData()
        {
            GameWorld.WorldData.TileIDs = (TileType[])(object)TileIDs;
            GameWorld.WorldData.WallIDs = (TileType[])(object)WallIDs;

            GameWorld.WorldData.LevelWidth = LevelWidth;
            GameWorld.WorldData.LevelHeight = LevelHeight;

            GameWorld.WorldData.BackgroundId = BackgroundId;
            GameWorld.WorldData.SoundtrackId = SoundtrackId;

            // Need to convert metadata xml string array to dict.
            GameWorld.WorldData.MetaData.Clear();
            if (MetaData != null)
            {
                foreach (var keyVal in MetaData)
                {
                    if (keyVal == null) continue;
                    string[] keyValSeparated = keyVal.Split(' ');
                    int.TryParse(keyValSeparated[0], out int key);
                    GameWorld.WorldData.MetaData.Add(key, keyValSeparated[1]);
                }
            }

            GameWorld.WorldData.LevelName = LevelName;
            GameWorld.WorldData.HasClouds = HasClouds;
            GameWorld.WorldData.IsRaining = IsRaining;
            GameWorld.WorldData.IsSnowing = IsSnowing;
            GameWorld.WorldData.IsDarkOutline = IsDarkOutline;
            GameWorld.WorldData.SunLightColor = SunLightColor;

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
