using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Misc.Errors
{
    /// <summary>
    /// Thrown when something is trying to detect collisions but is not collidable.
    /// </summary>
    public class NotCollidableException : Exception
    {
        public NotCollidableException() { }
        public NotCollidableException(string message) { }
    }

    /// <summary>
    /// Thrown when the string contains invalid characters.
    /// </summary>
    public class InvalidCharactersException : Exception
    {
        public InvalidCharactersException() { }
        public InvalidCharactersException(string message) { }
    }

    /// <summary>
    /// The exception that is thrown when the file attempting to be created already exists.
    /// </summary>
    public class FileAlreadyExistsException : Exception
    {
        public FileAlreadyExistsException() { }
        public FileAlreadyExistsException(string message) { }
    }
}
