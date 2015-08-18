using Adam.GameData;
using Adam.Misc;
using Adam.Network;
using Adam.UI;
using Adam.UI.Information;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Adam.Levels
{
    public class WorldData
    {
        public Song song;
        private SoundFx ambience;
        public string LevelName = "";
        public bool HasClouds;
        public bool IsRaining;
        public bool IsSnowing;
        double gameTimer;

        bool obj0;
        bool obj1;
        bool obj2;
        bool obj3;
        bool obj4;

        public bool trigger0;
        public bool trigger1;
        public bool trigger2;
        public bool trigger3;
        public bool trigger4;
        public bool trigger5;
        public bool trigger6;

        bool privTrig0;

        bool editMode;
        public bool IsDealingWithData { get; set; }

        public byte[] TileIDs { get; set; }
        public byte[] WallIDs { get; set; }

        public int LevelWidth { get; set; }
        public int LevelHeight { get; set; }
        public byte BackgroundID { get; set; }
        public byte SoundtrackID { get; set; }

        public Vector2 SpawnPoint { get; set; }

        public AdamDictionary SignMessages { get; set; }
        public AdamDictionary PortalLinks { get; set; }

        public WorldData()
        {
            GameWorld.Instance.levelEditor.brush.SizeChanged += Brush_SizeChanged;
        }

        private void Brush_SizeChanged()
        {
            privTrig0 = true;
        }

        public void Update(GameTime gameTime)
        {
            gameTimer += gameTime.ElapsedGameTime.TotalSeconds;

            ambience?.PlayIfStopped();

            switch (GameWorld.Instance.CurrentGameMode)
            {
                //case GameMode.Level1and1:
                //    if (InputHelper.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A) || InputHelper.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
                //    {
                //        privTrig0 = true;
                //        Game1.ObjectiveTracker.CompleteObjective(0);
                //    }
                //    if (gameTimer > 5 && !privTrig0)
                //    {
                //        if (!obj0)
                //        {
                //            Objective obj = new Objective();
                //            obj.Create("Press 'A' and 'D' to move.", 0);
                //            Game1.ObjectiveTracker.AddObjective(obj);
                //            obj0 = true;
                //        }
                //    }
                //    break;
                case GameMode.Edit:
                    if (InputHelper.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Tab))
                    {
                        Main.ObjectiveTracker.CompleteObjective(0);
                    }

                    if (privTrig0)
                    {
                        Main.ObjectiveTracker.CompleteObjective(1);
                    }

                    if (gameTimer > 0)
                    {
                        if (!obj0)
                        {
                            Objective obj = new Objective();
                            obj.Create("Press 'TAB' to open inventory.", 0);
                            Main.ObjectiveTracker.AddObjective(obj);
                            obj0 = true;
                        }
                        if (!obj1)
                        {
                            Objective obj = new Objective();
                            obj.Create("Use the scroll wheel to change brush size.", 1);
                            Main.ObjectiveTracker.AddObjective(obj);
                            obj1 = true;
                        }
                    }
                    break;
            }
        }

        public void OpenLevelLocally(bool editMode)
        {
            if (!IsDealingWithData)
            {
                Thread thread = new Thread(new ThreadStart(BackgroundThread_Open));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                IsDealingWithData = true;
                this.editMode = editMode;
            }
        }

        private void BackgroundThread_Open()
        {
            WorldConfigFile data;
            OpenFileDialog op = new OpenFileDialog();
            op.DefaultExt = "lvl";
            op.Title = "Choose level to open:";
            DialogResult dr = op.ShowDialog();
            if (dr == DialogResult.OK)
            {
                XmlSerializer xs = new XmlSerializer(typeof(WorldConfigFile));
                using (FileStream fs = new FileStream(op.FileName, FileMode.Open))
                {
                    data = (WorldConfigFile)xs.Deserialize(fs);
                }

                if (editMode)
                {
                    data.LoadIntoEditor();
                }
                else
                {
                    data.LoadIntoPlay();
                }

            }

            IsDealingWithData = false;
        }

        public void SaveLevelLocally()
        {
            if (!IsDealingWithData)
            {
                Thread thread = new Thread(new ThreadStart(BackgroundThread_Save));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                IsDealingWithData = true;
            }
        }

        private void BackgroundThread_Save()
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.AddExtension = true;
            sv.DefaultExt = "lvl";
            sv.Title = "Save level as:";

            DialogResult dr = sv.ShowDialog();
            if (dr == DialogResult.OK)
            {
                XmlSerializer xs = new XmlSerializer(typeof(WorldConfigFile));
                using (FileStream fs = new FileStream(sv.FileName, FileMode.OpenOrCreate))
                {
                    xs.Serialize(fs, new WorldConfigFile(GameWorld.Instance));
                }

            }

            IsDealingWithData = false;
        }

        public void CreateNewWorld()
        {
            WorldConfigFile config = new WorldConfigFile(300, 300);
            config.LoadIntoEditor();
        }

        public void WipeWorld()
        {

        }
    }
}
