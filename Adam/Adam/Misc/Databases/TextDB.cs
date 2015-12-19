using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    public class TextDb
    {
        static string[] _db = new string[1];
        public static string GetText(int textId)
        {
            return _db[textId];
        }
    }
}
