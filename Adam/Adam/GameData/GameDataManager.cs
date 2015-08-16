﻿using Adam.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace Adam.GameData
{
    public class GameDataManager
    {
        public Save[] saves;
        Settings settings;

        public int SelectedSave { get; set; }

        public GameDataManager()
        {
            saves = new Save[3];
            for (int i = 0; i < saves.Length; i++)
            {
                saves[i] = new Save();
            }

            settings = new Settings();

            LoadSaves();
            Thread.Sleep(10);
            LoadSettings();
        }


        private void LoadSaves()
        {
            for (int s = 0; s < saves.Length; s++)
            {
                //int saveNumber = s + 1;
                //FileStream fs = new FileStream("save" + saveNumber + ".xml", FileMode.OpenOrCreate);
                //XmlSerializer xs = new XmlSerializer(typeof(Save));
                //saves[s] = (Save)xs.Deserialize(fs);
                //fs.Flush();
                //fs.Close();
            }
        }

        public void SaveGame()
        {
            int saveNumber = SelectedSave + 1;
            FileStream fs = new FileStream("save" + saveNumber + ".xml", FileMode.OpenOrCreate);
            XmlSerializer xs = new XmlSerializer(typeof(Save));
            xs.Serialize(fs, CurrentSave);
            fs.Close();
        }

        private void LoadSettings()
        {
            try
            {
                FileStream fs = new FileStream("settings.xml", FileMode.Open, FileAccess.Read);
                XmlSerializer xs = new XmlSerializer(typeof(Settings));
                settings = (Settings)xs.Deserialize(fs);
                fs.Close();
            }

            catch
            {
                FileStream fs = new FileStream("settings.xml", FileMode.Create);
                XmlSerializer xs = new XmlSerializer(typeof(Settings));
                xs.Serialize(fs, settings);
                fs.Close();
            }
        }

        public void SaveSettings()
        {
            FileStream fs = new FileStream("settings.xml", FileMode.Create);
            XmlSerializer xs = new XmlSerializer(typeof(Settings));
            xs.Serialize(fs, settings);
            fs.Close();
        }

        public Settings Settings
        {
            get { return settings; }
        }

        public Save CurrentSave
        {
            get { return saves[SelectedSave]; }
        }

        public Save GetSave(int number)
        {
            return saves[number];
        }

    }
}
