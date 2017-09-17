using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace ThereMustBeAnotherWay
{
    public static class LoadingScreen
    {
        private static Texture2D _background;
        static SpriteFont _fontBig, _fontSmall;
        static Timer dotTimer = new Timer(true);

        public static string LoadingText = "Contemplating life...";
        private static string _loadingDots;

        static int _count = 0;

        public static void Initialize()
        {
            _background = ContentHelper.LoadTexture("Tiles/black");
            _fontBig = ContentHelper.LoadFont("Fonts/x32");
            _fontSmall = ContentHelper.LoadFont("Fonts/x16");
            dotTimer.ResetAndWaitFor(100);
            dotTimer.SetTimeReached += DotTimer_SetTimeReached;
        }

        private static void DotTimer_SetTimeReached()
        {
            _count++;
            if (_count > 3)
                _count = 0;
            dotTimer.Reset();
        }

        public static void Update()
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


        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_background, new Rectangle(0, 0, TMBAW_Game.UserResWidth, TMBAW_Game.UserResHeight), Color.White);
            FontHelper.DrawWithOutline(spriteBatch, _fontBig, _loadingDots, new Vector2(50, TMBAW_Game.UserResHeight - 100), 3, Color.White, Color.DarkGray);
            FontHelper.DrawWithOutline(spriteBatch, _fontSmall, LoadingText, new Vector2(50, TMBAW_Game.UserResHeight - 50), 3, Color.White, Color.DarkGray);
        }
    }
}
