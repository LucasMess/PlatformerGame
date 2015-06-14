using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Adam.GameData
{
    class GameDataManager
    {
        Save[] saves;
        Settings settings;

        public int SelectedSave { get; set; }

        public GameDataManager()
        {
            saves = new Save[2];
            settings = new Settings();

            LoadSaves();
            LoadSettings();
        }


        private void LoadSaves()
        {
            for (int s = 0; s < saves.Length; s++)
            {
                int saveNumber = s + 1;
                try
                {
                    FileStream fs = new FileStream("save" + saveNumber + ".xml", FileMode.Open, FileAccess.Read);
                    XmlSerializer xs = new XmlSerializer(typeof(Save));
                    saves[s] = (Save)xs.Deserialize(fs);
                    fs.Flush();
                    fs.Close();
                }
                catch
                {
                    FileStream fs = new FileStream("save" + saveNumber + ".xml", FileMode.Create);
                    XmlSerializer xs = new XmlSerializer(typeof(Save));
                    xs.Serialize(fs, saves[s]);
                    fs.Flush();
                    fs.Close();
                }
            }
        }

        public void SaveGame()
        {
            int saveNumber = SelectedSave + 1;
            FileStream fs = new FileStream("save" + saveNumber + ".xml", FileMode.Create);
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
    }
}
