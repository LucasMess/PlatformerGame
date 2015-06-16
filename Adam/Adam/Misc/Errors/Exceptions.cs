using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Misc.Errors
{
    class NotCollidableException : Exception
    {
        public NotCollidableException() { }
        public NotCollidableException(string message) { }
    }
}
