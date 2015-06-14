using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    public class TextDB
    {
        static string[] db = new string[1];
        public static string GetText(int textID)
        {
            return db[textID];
        }
    }
}
