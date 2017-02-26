using Steamworks;
using System;
using System.Windows.Forms;

namespace Adam
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
    static void Main(string[] args)
        {
            try
            {
                SteamAPI.Init();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
#if RELEASE

            if (SteamAPI.RestartAppIfNecessary(new AppId_t(595250)))
            {
                return;
            }
#endif
#if RELEASE
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

