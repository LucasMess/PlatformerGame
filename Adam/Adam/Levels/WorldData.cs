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
        public Song Song;
        private SoundFx _ambience;
        public string LevelName = "";
        public bool HasClouds;
        public bool IsRaining;
        public bool IsSnowing;
        double _gameTimer;

        bool _obj0;
        bool _obj1;
        bool _obj2;
        bool _obj3;
        bool _obj4;

        public bool Trigger0;
        public bool Trigger1;
        public bool Trigger2;
        public bool Trigger3;
        public bool Trigger4;
        public bool Trigger5;
        public bool Trigger6;

        bool _privTrig0;

        bool _editMode;
        public bool IsDealingWithData { get; set; }

        public byte[] TileIDs { get; set; }
        public byte[] WallIDs { get; set; }

        public int LevelWidth { get; set; }
        public int LevelHeight { get; set; }
        public byte BackgroundId { get; set; }
        public byte SoundtrackId { get; set; }

        public Vector2 SpawnPoint { get; set; }

        public string[] MetaData { get; set; }

        public WorldData()
        {
            LevelEditor.Brush.SizeChanged += Brush_SizeChanged;
        }

        private void Brush_SizeChanged()
        {
            _privTrig0 = true;
        }

        public void Update(GameTime gameTime)
        {
            _gameTimer += gameTime.ElapsedGameTime.TotalSeconds;

            _ambience?.PlayIfStopped();

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
                    if (InputHelper.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.E))
                    {
                        Main.ObjectiveTracker.CompleteObjective(0);
                    }

                    if (_privTrig0)
                    {
                        Main.ObjectiveTracker.CompleteObjective(1);
                    }

                    if (_gameTimer > 0)
                    {
                        if (!_obj0)
                        {
                            Objective obj = new Objective();
                            obj.Create("Press 'E' to open inventory.", 0);
                            Main.ObjectiveTracker.AddObjective(obj);
                            _obj0 = true;
                        }
                        if (!_obj1)
                        {
                            Objective obj = new Objective();
                            obj.Create("Use the scroll wheel to change brush size.", 1);
                            Main.ObjectiveTracker.AddObjective(obj);
                            _obj1 = true;
                        }
                    }
                    break;
            }
        }

        public void CreateNewWorld(string levelName)
        {
            WorldConfigFile config = new WorldConfigFile(levelName,256, 256);
            config.LoadIntoEditor();
        }

        public void WipeWorld()
        {

        }
    }
}
