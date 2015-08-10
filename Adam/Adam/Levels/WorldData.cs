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

namespace Adam.Levels
{
    class WorldData
    {
        public Texture2D mainMap;
        public Texture2D wallMap;
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
        public int[] IDs;
        public int width = 200;
        public int height = 200;

        public WorldData(Level CurrentLevel)
        {
            levelName = "Garden of Eden";
            mainMap = ContentHelper.LoadTexture("Levels/1-2_main");
            wallMap = ContentHelper.LoadTexture("Levels/1-2_wall");
            song = ContentHelper.LoadSong("Music/Adventure Awaits (Adam 1)");
            ambience = new SoundFx("Ambience/eden");
            wantClouds = true;


            switch (CurrentLevel)
            {
                case Level.Level0:
                    break;
                case Level.Level1and1:
                    levelName = "Garden of Eden";
                    mainMap = ContentHelper.LoadTexture("Levels/1-2_main");
                    wallMap = ContentHelper.LoadTexture("Levels/1-2_wall");
                    song = ContentHelper.LoadSong("Music/Adventure Awaits (Adam 1)");
                    ambience = new SoundFx("Ambience/eden");
                    wantClouds = true;

                    sign1 = "Press space to jump.";
                    sign2 = "Hold space to jump higher. You can also hold SHIFT to run faster.";
                    sign3 = "Yes, this is a floating island. God can do whatever he wants.";
                    sign4 = "Caution: You are exiting God's backyard!";
                    sign5 = "Take a leap of faith. The worst is over. == Love God <3";
                    sign6 = "Oh, you lived? Well, we couldn't go around my pet mountain so I made this path for my followers. It might be a little fro... hop... I don't know. Hope you like it.";
                    sign7 = "The Work of God ahead! Or maybe, just lazy developers. What is the difference? Take this gift. Use the left mouse button to activate it.";
                    break;
                case Level.Level1and2:
                    break;
                case Level.Level1and3:
                    break;
                case Level.Level2and1:
                    levelName = "Desolate Desert";
                    mainMap = ContentHelper.LoadTexture("Levels/2-1_main");
                    wallMap = ContentHelper.LoadTexture("Levels/2-1_wall");
                    song = ContentHelper.LoadSong("Music/Desert City");
                    wantClouds = true;
                    break;
                case Level.Level3and1:
                    break;
                case Level.Level4and1:
                    break;
                case Level.Level8and1:
                    levelName = "Entrance to Hell";
                    mainMap = ContentHelper.LoadTexture("Levels/debug_main");
                    wallMap = ContentHelper.LoadTexture("Levels/debug_wall");
                    song = ContentHelper.LoadSong("Music/Heart of Nowhere");
                    wantClouds = true;
                    break;
                case Level.Editor:
                    levelName = "Unnamed Creation";
                    mainMap = ContentHelper.LoadTexture("Levels/200x200");
                    wallMap = ContentHelper.LoadTexture("Levels/200x200");
                    //song = ContentHelper.LoadSong("Music/Heart of Nowhere");
                    wantClouds = true;
                    break;
            }

        }

        public void Update(GameTime gameTime)
        {
            gameTimer += gameTime.ElapsedGameTime.TotalSeconds;

            ambience?.PlayIfStopped();

            switch (GameWorld.Instance.CurrentLevel)
            {
                case Level.Level1and1:
                    if (InputHelper.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A) || InputHelper.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
                    {
                        privTrig0 = true;
                        Game1.ObjectiveTracker.CompleteObjective(0);
                    }
                    if (gameTimer > 5 && !privTrig0)
                    {
                        if (!obj0)
                        {
                            Objective obj = new Objective();
                            obj.Create("Press 'A' and 'D' to move.", 0);
                            Game1.ObjectiveTracker.AddObjective(obj);
                            obj0 = true;
                        }
                    }
                    break;
                case Level.Editor:
                    if (InputHelper.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Tab))
                    {
                        Game1.ObjectiveTracker.CompleteObjective(0);
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
            GameWorldData data;
            OpenFileDialog op = new OpenFileDialog();
            DialogResult dr = op.ShowDialog();
            if (dr == DialogResult.OK)
            {
                BinaryFormatter bf = new BinaryFormatter();
                    using (FileStream fs = new FileStream(op.FileName,FileMode.Open))
                    {
                        data = (GameWorldData)bf.Deserialize(fs);
                    }  data.Load();              
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
            DialogResult dr = sv.ShowDialog();
            if (dr == DialogResult.OK)
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, new GameWorldData(GameWorld.Instance));
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
