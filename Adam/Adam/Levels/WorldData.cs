using Adam.UI.Information;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Levels
{
    class WorldData
    {
        public Texture2D mainMap;
        public Texture2D wallMap;
        public Song song;
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

        bool privTrig0;


        public WorldData(Level CurrentLevel)
        {
            //Default
            levelName = "Garden of Eden";
            mainMap = ContentHelper.LoadTexture("Levels/1-2_main");
            wallMap = ContentHelper.LoadTexture("Levels/1-2_wall");
            song = ContentHelper.LoadSong("Music/Vivacity");
            wantClouds = true;

            switch (CurrentLevel)
            {
                case Level.Level0:
                    break;
                case Level.Level1and1:
                    levelName = "Garden of Eden";
                    mainMap = ContentHelper.LoadTexture("Levels/1-2_main");
                    wallMap = ContentHelper.LoadTexture("Levels/1-2_wall");
                    song = ContentHelper.LoadSong("Music/Vivacity");
                    wantClouds = true;

                    sign1 = "Press space to jump.";
                    sign2 = "Hold space to jump higher. You can also hold SHIFT to run faster.";
                    sign3 = "Yes, this is a floating island. God can do whatever he wants.";
                    sign4 = "Caution: You are exiting God's backyard!";
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
                default:
                    break;
            }

        }

        public void Update(GameTime gameTime)
        {
            gameTimer += gameTime.ElapsedGameTime.TotalSeconds;
            switch (GameWorld.Instance.CurrentLevel)
            {
                case Level.Level1and1:
                    if (InputHelper.IsKeyDown(Keys.A) || InputHelper.IsKeyDown(Keys.D))
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
            }
            return "ERROR: Text not found.";
        }
    }
}
