using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Misc.Errors
{
    public class NotCollidableException : Exception
    {
        public NotCollidableException() { }
        public NotCollidableException(string message) { }
    }
}
