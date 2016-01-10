using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Misc;
using Adam.Misc.Helpers;

namespace Adam
{
    public class LoadingScreen
    {
        private Texture2D _background;
        SpriteFont _font64, _font32;
        Timer dotTimer = new Timer();

        public static string LoadingText = "Contemplating life...";
        private string _loadingDots;

        int _count = 0, _maxCount;

        public LoadingScreen(Vector2 monitorRes, ContentManager content)
        {
            _background = ContentHelper.LoadTexture("Tiles/black");
            _font64 = content.Load<SpriteFont>("Fonts/x64");
            _font32 = content.Load<SpriteFont>("Fonts/x32");
            dotTimer.ResetAndWaitFor(100);
            dotTimer.SetTimeReached += DotTimer_SetTimeReached;
        }

        private void DotTimer_SetTimeReached()
        {
            _count++;
            if (_count > 3)
                _count = 0;
            dotTimer.Reset();
        }

        public void Update()
        {
            switch (_count)
            {
                case 0:
                    _loadingDots = "Loading";
                    break;
                case 1:
                    _loadingDots = "Loading.";
                    break;
                case 2:
                    _loadingDots = "Loading..";
                    break;
                case 3:
                    _loadingDots = "Loading...";
                    break;
            }
        }

        //public void ChooseText()
        //{
        //    switch (_randGen.Next(0, 36))
        //    {
        //        case 0:
        //            _randomText = "Hello there.";
        //            break;
        //        case 1:
        //            _randomText = "Walnuts are yummy.";
        //            break;
        //        case 2:
        //            _randomText = "Stephen smells like roses.";
        //            break;
        //        case 3:
        //            _randomText = "Salmon is a good source of protein.";
        //            break;
        //        case 4:
        //            _randomText = "Indeeeeeeeeeed.";
        //            break;
        //        case 5:
        //            _randomText = "Loading the loading screen.";
        //            break;
        //        case 6:
        //            _randomText = "Reticulating spines.";
        //            break;
        //        case 7:
        //            _randomText = "Error 404: Level not found.";
        //            break;
        //        case 8:
        //            _randomText = "You are beautiful.";
        //            break;
        //        case 9:
        //            _randomText = "Trying to find lost keys.";
        //            break;
        //        case 10:
        //            _randomText = "Deleting system memory.";
        //            break;
        //        case 11:
        //            _randomText = "Windows update incoming.";
        //            break;
        //        case 12:
        //            _randomText = "You have lost connection to the internet.";
        //            break;
        //        case 13:
        //            _randomText = "Lighting the darkness.";
        //            break;
        //        case 14:
        //            _randomText = "Moving immovable objects.";
        //            break;
        //        case 15:
        //            _randomText = "Stopping unstoppable force.";
        //            break;
        //        case 16:
        //            _randomText = "Nerfing Irelia.";
        //            break;
        //        case 17:
        //            _randomText = "Meow.";
        //            break;
        //        case 18:
        //            _randomText = "Upgrading antiviruses.";
        //            break;
        //        case 19:
        //            _randomText = "Opening Internet Explorer.";
        //            break;
        //        case 20:
        //            _randomText = "Putting out the firewall.";
        //            break;
        //        case 21:
        //            _randomText = "Giving Satan a massage.";
        //            break;
        //        case 22:
        //            _randomText = "Doing Satan's pedicure.";
        //            break;
        //        case 23:
        //            _randomText = "Far Lands or Bust!";
        //            break;
        //        case 24:
        //            _randomText = "Shaving bears.";
        //            break;
        //        case 25:
        //            _randomText = "Drinking tea.";
        //            break;
        //        case 26:
        //            _randomText = "Starting pillow fight.";
        //            break;
        //        case 27:
        //            _randomText = "Reloading the unloadable.";
        //            break;
        //        case 28:
        //            _randomText = "Checking out pictures folder.";
        //            break;
        //        case 29:
        //            _randomText = "Taking a break.";
        //            break;
        //        case 30:
        //            _randomText = "Loading assets.";
        //            break;
        //        case 31:
        //            _randomText = "Googling for solution.";
        //            break;
        //        case 32:
        //            _randomText = "Oh oh, we might blue screen!";
        //            break;
        //        case 33:
        //            _randomText = "Deleting user files.";
        //            break;
        //        case 34:
        //            _randomText = "Drinking water.";
        //            break;
        //        case 35:
        //            _randomText = "Pretending to load.";
        //            break;
        //    }
        //    _textChosen = true;
        //}


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_background, new Rectangle(0,0,Main.UserResWidth,Main.UserResHeight),Color.White);
            FontHelper.DrawWithOutline(spriteBatch,_font64,_loadingDots,new Vector2(Main.UserResWidth - 500, Main.UserResHeight - 200),3,Color.White,Color.DarkGray);
            FontHelper.DrawWithOutline(spriteBatch, _font32, LoadingText, new Vector2(Main.UserResWidth -_font32.MeasureString(LoadingText).X, Main.UserResHeight - 100), 3, Color.White, Color.DarkGray);
        }

    }
}
