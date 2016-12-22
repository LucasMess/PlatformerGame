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
            using (AdamGame game = new AdamGame())
            {
                game.Run();
            }
        }
    }
}

