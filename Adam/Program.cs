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
                using (AdamGame game = new AdamGame())
                {
                    game.Run();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "::" + e.StackTrace + "::" + e.InnerException.Message);
            }
        }
    }
}

