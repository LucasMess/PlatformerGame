using Adam.GameData;
using Adam.Misc.Errors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Adam.UI
{
    public class DataFolder
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

        public static string CurrentLevelFilePath;


        /// <summary>
        /// Checks to see if the directory exists upon creation, and if it does not, it will create it.
        /// </summary>
        public DataFolder()
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
        /// Attempts to create a new world.
        /// </summary>
        /// <param name="levelName">The name of the level.</param>
        /// <param name="width">The width of the level.</param>
        /// <param name="height">The height of the level.</param>
        public static string CreateNewLevel(string levelName, short width, short height)
        {
            // Checks to see if levelName is valid.
            if (levelName == null)
            {
                throw new ArgumentNullException("The name of your level cannot be nothing.");
            }
            if (levelName.Length > 20 || levelName.Length < 3)
            {
                throw new ArgumentOutOfRangeException("The name of your level must be between 3 and 20 characters.");
            }
            if (levelName.IndexOfAny(Path.GetInvalidFileNameChars()) > 0)
            {
                throw new InvalidCharactersException("The name of your level contains invalid characters");
            }

            // Creates the new world and sets the file path for it.
            string filePath = Path.Combine(LevelDirectory, levelName);
            filePath += LevelFileExt;
            WorldConfigFile config = new WorldConfigFile(levelName, width, height);

            // Checks to see if name already exists.
            if (File.Exists(filePath))
            {
                throw new FileAlreadyExistsException("A level with this name already exists.");
            }

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
        /// Saves the current game world to the current level file.
        /// </summary>
        public static void SaveLevel()
        {
            XmlSerializer xs = new XmlSerializer(typeof(WorldConfigFile));
            using (FileStream fs = new FileStream(CurrentLevelFilePath, FileMode.OpenOrCreate))
            {
                xs.Serialize(fs, new WorldConfigFile(GameWorld.Instance));
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
        private static WorldConfigFile GetWorldConfigFile(string filePath)
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
                Main.Dialog.Show("Error: File not found.");
                throw new Exception();
            }

            return config;
        }

        /// <summary>
        /// Deletes the specified file.
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteFile(string filePath)
        {
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
            // Rename the file.
            Console.WriteLine(filePath);
            string newFilePath = filePath.Remove(filePath.Length - oldName.Length - LevelFileExt.Length, oldName.Length + LevelFileExt.Length);
            Console.WriteLine(newFilePath);
            newFilePath += newName;
            newFilePath += LevelFileExt;
            Console.WriteLine(newFilePath);
            File.Move(filePath, newFilePath);

            // Rename the level inside the config file.
            WorldConfigFile config = GetWorldConfigFile(newFilePath);
            config.LevelName = newName;
            CurrentLevelFilePath = newFilePath;
            SaveLevel(config);
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
