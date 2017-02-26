using Adam.GameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Adam.UI
{
    public static class DataFolder
    {

        /// <summary>
        /// The file path of the main directory.
        /// </summary>
        public static string MainDirectory;

        /// <summary>
        /// The file path of the levels directory.
        /// </summary>
        public static string LevelDirectory;

        public const string LevelFileExt = ".lvl";
        private const string SettingsFileName = "settings.xml";

        public static string CurrentLevelFilePath;


        /// <summary>
        /// Checks to see if the directory exists upon creation, and if it does not, it will create it.
        /// </summary>
        public static void Initialize()
        {
            // The folder for the roaming current user.
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "Adam");

            // Check if main Adam directory folder exists and if not, create it.
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);
            MainDirectory = specificFolder;

            // Check if Levels folder exists or creates it.
            specificFolder = Path.Combine(MainDirectory, "Levels");
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);
            LevelDirectory = specificFolder;

            // Check if the Settings folder exists or creates it.
            specificFolder = MainDirectory;
            specificFolder = Path.Combine(MainDirectory, "Settings");

        }


        /// <summary>
        /// Checks if the name already exists, if it is too short, etc, and return the file path if it is ok.
        /// </summary>
        /// <param name="levelName"></param>
        /// <returns></returns>
        private static string GetFilePath(string levelName)
        {
            // Checks to see if levelName is valid.
            if (levelName == null)
            {
                throw new Exception("The name of your level cannot be nothing.");
            }
            if (levelName.Length > 40 || levelName.Length < 1)
            {
                throw new Exception("The name of your level must be between 1 and 40 characters.");
            }
            if (levelName.IndexOfAny(Path.GetInvalidFileNameChars()) > 0)
            {
                throw new Exception("The name of your level contains invalid characters");
            }

            // Creates the new world and sets the file path for it.
            string filePath = Path.Combine(LevelDirectory, levelName);
            filePath += LevelFileExt;

            // Checks to see if name already exists.
            if (File.Exists(filePath))
            {
                throw new Exception("A level with this name: " + filePath + " already exists.");
            }

            return filePath;

        }


        /// <summary>
        /// Searches the game directory for a settings file. If none is found, it creates a new one with the default parameters.
        /// </summary>
        /// <returns></returns>
        public static SettingsFile GetSettingsFile()
        {
            string filePath = MainDirectory + "/" + SettingsFileName;

            SettingsFile settings = new SettingsFile();

            if (File.Exists(filePath))
            {
                XmlSerializer xs = new XmlSerializer(typeof(SettingsFile));
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open))
                    {
                        settings = (SettingsFile)xs.Deserialize(fs);
                    }
                }
                catch (InvalidOperationException)
                {
                    AdamGame.MessageBox.Show("Settings corrupt.");
                    throw;
                }
            }
            else
            {
                // Creates the file for the settings.
                XmlSerializer xs = new XmlSerializer(typeof(SettingsFile));
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    xs.Serialize(fs, settings);
                }
            }

            return settings;
        }

        /// <summary>
        /// Saves the given settings file to the current settings file location.
        /// </summary>
        /// <param name=""></param>
        public static void SaveSettingsFile(SettingsFile newFile)
        {
            XmlSerializer xs = new XmlSerializer(typeof(SettingsFile));

            File.Delete(MainDirectory + "/" + SettingsFileName);

            using (FileStream fs = new FileStream(MainDirectory + "/" + SettingsFileName, FileMode.OpenOrCreate))
            {
                xs.Serialize(fs, newFile);
            }
        }

        /// <summary>
        /// Attempts to create a new world.
        /// </summary>
        /// <param name="levelName">The name of the level.</param>
        /// <param name="width">The width of the level.</param>
        /// <param name="height">The height of the level.</param>
        public static string CreateNewLevel(string levelName, short width, short height)
        {
            string filePath;

            try
            {
                filePath = GetFilePath(levelName);
            }
            catch
            {
                throw;
            }

            WorldConfigFile config = new WorldConfigFile(levelName, width, height);

            // Creates the file for the world.
            XmlSerializer xs = new XmlSerializer(typeof(WorldConfigFile));
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                xs.Serialize(fs, config);
            }

            return filePath;
        }

        /// <summary>
        /// Used to load a level into gameworld and edit it.
        /// </summary>
        /// <param name="filePath"></param>
        public static void EditLevel(string filePath)
        {

            WorldConfigFile config = GetWorldConfigFile(filePath);

            if (config.TileIDs.Length == 0)
            {
                AdamGame.MessageBox.Show("There is something wrong with this level and it cannot be loaded.");
                return;
            }

            if (!config.CanBeEdited)
            {
                AdamGame.MessageBox.Show("This level cannot be edited.");
                return;
            }

            CurrentLevelFilePath = filePath;
            config.LoadIntoEditor();

        }

        /// <summary>
        /// Used to load a level into gameworld and play it.
        /// </summary>
        /// <param name="filePath"></param>
        public static void PlayLevel(string filePath)
        {
            WorldConfigFile config = GetWorldConfigFile(filePath);
            CurrentLevelFilePath = filePath;

            if (!config.IsValidLevel())
            {
                throw new Exception("This level cannot be played because there is no player spawn point. Edit the level first.");
            }

            config.LoadIntoPlay();
        }

        /// <summary>
        /// Used to load a level from the content folder of the game. These will be levels created by the developer.
        /// </summary>
        /// <param name="levelName"></param>
        public static void PlayStoryLevel(string levelName)
        {
            string basePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            PlayLevel(basePath + "/Content/Levels/" + levelName);
        }

        /// <summary>
        /// Loads the level but does not change game state.
        /// </summary>
        /// <param name="filePath"></param>
        public static void LoadLevelForBackground(string filePath)
        {
            // Do not check if level is valid or the game will not launch if there are errors in the file.
            WorldConfigFile config = GetWorldConfigFile(filePath);
            CurrentLevelFilePath = filePath;
            config.LoadIntoView();
        }

        /// <summary>
        /// Saves the current game world to the current level file.
        /// </summary>
        public static void SaveLevel()
        {
            XmlSerializer xs = new XmlSerializer(typeof(WorldConfigFile));

            DeleteLevel(CurrentLevelFilePath);

            using (FileStream fs = new FileStream(CurrentLevelFilePath, FileMode.OpenOrCreate))
            {
                WorldConfigFile config = new WorldConfigFile();
                config.GetDataFromGameWorld();
                xs.Serialize(fs, config);
            }
        }

        private static void SaveLevel(WorldConfigFile config)
        {
            XmlSerializer xs = new XmlSerializer(typeof(WorldConfigFile));
            using (FileStream fs = new FileStream(CurrentLevelFilePath, FileMode.OpenOrCreate))
            {
                xs.Serialize(fs, config);
            }
        }

        /// <summary>
        /// Used to retrieve level information from the file in the form of a WorldConfigFile.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static WorldConfigFile GetWorldConfigFile(string filePath)
        {
            // Deserialize file into WorldConfigFile.
            WorldConfigFile config;

            XmlSerializer xs = new XmlSerializer(typeof(WorldConfigFile));
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    config = (WorldConfigFile)xs.Deserialize(fs);
                }
            }
            catch (FileNotFoundException)
            {
                AdamGame.MessageBox.Show("Error: File not found.");
                throw;
            }
            catch (InvalidOperationException)
            {
                AdamGame.MessageBox.Show("Error: Level data is corrupt. Cannot load level.");
                throw;
            }

            return config;
        }

        /// <summary>
        /// Deletes the specified file.
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteLevel(string filePath)
        {
            try
            {
                WorldConfigFile config = GetWorldConfigFile(filePath);
                if (!config.CanBeEdited)
                {
                    AdamGame.MessageBox.Show("This level cannot be deleted.");
                    return;
                }
            }
            catch
            {
                // Do nothing. If it can't read the file, let the user delete it.
            }

            File.Delete(filePath);
        }

        /// <summary>
        /// Renames the specified file.
        /// </summary>
        /// <param name="filePath">The file path of the file.</param>
        /// <param name="oldName">The current name of the file.</param>
        /// <param name="newName">The new name for the file.</param>
        public static void RenameFile(string filePath, string oldName, string newName)
        {
            // Try to see if level can be edited. If there is a problem with the lvl file, abort.
            WorldConfigFile config;
            try
            {
                config = GetWorldConfigFile(filePath);
                if (!config.CanBeEdited)
                {
                    AdamGame.MessageBox.Show("This level cannot be renamed.");
                    return;
                }
            }
            catch
            {
                return;
            }

            // Exceptions are thrown when level name is invalid and are handled by the caller.
            string newFilePath;
            try
            {
                newFilePath = GetFilePath(newName);
            }
            catch
            {
                throw;
            }

            // Proceed if eveything worked.
            File.Move(filePath, newFilePath);

            // Rename the level inside the config file.
            WorldConfigFile config2 = GetWorldConfigFile(newFilePath);
            config.LevelName = newName;
            CurrentLevelFilePath = newFilePath;
            SaveLevel(config2);


        }

        /// <summary>
        /// Returns the names of all level files inside the Levels directory.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetLevelNames()
        {
            // Get path for levels and iterate through them to get their names.
            List<string> levelPaths = GetLevelPaths();
            List<string> levelNames = new List<string>();

            foreach (string s in levelPaths)
            {
                string name = Path.GetFileNameWithoutExtension(s);
                levelNames.Add(name);
            }

            return levelNames;
        }

        /// <summary>
        /// Returns true if there is a level with this name.
        /// </summary>
        /// <param name="levelName"></param>
        /// <returns></returns>
        public static bool LevelExists(string levelName)
        {
            return (GetLevelNames().Contains(levelName));
        }

        /// <summary>
        /// Returns the date in which the levels were last written to.
        /// </summary>
        /// <returns></returns>
        public static List<DateTime> GetLevelLastModifiedDates()
        {
            // Get path for levels and iterate through them to get their last modified dates.
            List<string> levelPaths = GetLevelPaths();
            List<DateTime> levelLastModifiedDates = new List<DateTime>();

            foreach (string s in levelPaths)
            {
                DateTime date = File.GetLastWriteTime(s);
                levelLastModifiedDates.Add(date);
            }

            return levelLastModifiedDates;
        }

        /// <summary>
        /// Returns the file path of all levels inside the Levels directory.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetLevelPaths()
        {
            // Gets all the files inside of the level folder.
            string[] files = Directory.GetFiles(LevelDirectory);
            List<string> filesInLevelFolder = files.ToList();

            // Check the extension of the files and remove any that are not .lvl
            for (int i = filesInLevelFolder.Count - 1; i >= 0; i--)
            {
                if (Path.GetExtension(filesInLevelFolder[i]) != LevelFileExt)
                {
                    filesInLevelFolder.Remove(filesInLevelFolder[i]);
                }
            }

            return filesInLevelFolder;
        }


    }
}
