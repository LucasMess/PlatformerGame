using Adam.Misc;
using Adam.Network;
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
    class WorldData
    {
        public Song song;
        private SoundFx ambience;
        public string levelName = "";
        public bool wantClouds;
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

        string sign1 = "";
        string sign2 = "";
        string sign3 = "";
        string sign4 = "";
        string sign5 = "";
        string sign6 = "";
        string sign7 = "";
        string sign8 = "";

        bool privTrig0;

        bool dealingWithData;
        public int[] tileIDs = new int[200 * 200];
        public int[] wallIDs = new int[200 * 200];
        public int width = 200;
        public int height = 200;

        public Vector2 SpawnPoint { get; set; }

        public WorldData(GameMode CurrentLevel)
        {
            levelName = "Garden of Eden";
            song = ContentHelper.LoadSong("Music/Adventure Awaits (Adam 1)");
            ambience = new SoundFx("Ambience/eden");
            wantClouds = true;


            switch (CurrentLevel)
            {
                case GameMode.Editor:
                    levelName = "Unnamed Creation";
                    //song = ContentHelper.LoadSong("Music/Heart of Nowhere");
                    wantClouds = true;

                    GameWorld.Instance.levelEditor.brush.SizeChanged += Brush_SizeChanged;
                    break;
            }

        }

        private void Brush_SizeChanged()
        {
            privTrig0 = true;
        }

        public void Update(GameTime gameTime)
        {
            gameTimer += gameTime.ElapsedGameTime.TotalSeconds;

            ambience?.PlayIfStopped();

            switch (GameWorld.Instance.CurrentLevel)
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
                case GameMode.Editor:
                    if (InputHelper.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Tab))
                    {
                        Game1.ObjectiveTracker.CompleteObjective(0);
                    }

                    if (privTrig0)
                    {
                        Game1.ObjectiveTracker.CompleteObjective(1);
                    }

                    if (gameTimer > 0)
                    {
                        if (!obj0)
                        {
                            Objective obj = new Objective();
                            obj.Create("Press 'TAB' to open inventory.", 0);
                            Game1.ObjectiveTracker.AddObjective(obj);
                            obj0 = true;
                        }
                        if (!obj1)
                        {
                            Objective obj = new Objective();
                            obj.Create("Use the scroll wheel to change brush size.", 1);
                            Game1.ObjectiveTracker.AddObjective(obj);
                            obj1 = true;
                        }
                    }
                    break;
            }
        }

        public string GetSignMessage(int ID)
        {
            switch (ID)
            {
                case 1:
                    return sign1;
                case 2:
                    return sign2;
                case 3:
                    return sign3;
                case 4:
                    return sign4;
                case 5:
                    return sign5;
                case 6:
                    return sign6;
                case 7:
                    return sign7;
                case 8:
                    return sign8;
            }
            return "ERROR: Text not found.";
        }

        public void OpenLevelLocally()
        {
            if (!dealingWithData)
            {
                Thread thread = new Thread(new ThreadStart(ThreadOpen));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                dealingWithData = true;
            }
        }

        private void ThreadOpen()
        {
            WorldConfigFile data;
            OpenFileDialog op = new OpenFileDialog();
            DialogResult dr = op.ShowDialog();
            if (dr == DialogResult.OK)
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream fs = new FileStream(op.FileName, FileMode.Open))
                {
                    data = (WorldConfigFile)bf.Deserialize(fs);
                }
                data.LoadIntoEditor();
            }

            dealingWithData = false;
        }

        public void SaveLevelLocally()
        {
            if (!dealingWithData)
            {
                Thread thread = new Thread(new ThreadStart(ThreadSave));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                dealingWithData = true;
            }
        }
        private void ThreadSave()
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.AddExtension = true;
            sv.DefaultExt = "lvl";
            sv.Title = "Save level as:";

            DialogResult dr = sv.ShowDialog();
            if (dr == DialogResult.OK)
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, new WorldConfigFile(GameWorld.Instance));
                    byte[] data = ms.ToArray();

                    using (BinaryWriter b = new BinaryWriter(File.Open(sv.FileName, FileMode.Create)))
                    {
                        foreach (byte i in data)
                        {
                            b.Write(i);
                        }
                    }
                }


            }

            dealingWithData = false;
        }
    }
}
