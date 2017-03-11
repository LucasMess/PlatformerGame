using Steamworks;
using System;
using System.Windows.Forms;

namespace Adam
{
    static class Program
    {
        public static ulong GameLaunchLobbyId = 0;
        public static bool LaunchedFromInvite = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
    static void Main(string[] args)
        {
            Console.WriteLine("Arg count: " + args.Length);
            // Joining lobby from outside the game.
            if (args.Length > 1)
            {
                if (args[0] == "+connect_lobby")
                {
                    GameLaunchLobbyId = ulong.Parse(args[1]);
                    LaunchedFromInvite = true;
                }
            }

            try
            {
                SteamAPI.Init();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
#if !DEBUG

            if (SteamAPI.RestartAppIfNecessary(new AppId_t(595250)))
            {
                return;
            }
#endif
#if !DEBUG
            try
            {
                using (AdamGame game = new AdamGame())
                {
                    game.Run();
                }
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message + "::" + e.StackTrace + "::" + e.InnerException.Message);
            }
#else
            using (AdamGame game = new AdamGame())
            {
                game.Run();
            }
#endif

        }
    }
}

