using System;

namespace Adam.Misc.Errors
{
    /// <summary>
    /// Thrown when something is trying to detect collisions but is not collidable.
    /// </summary>
    public class NotCollidableException : Exception
    {
        string _message;
        public NotCollidableException() { }
        public NotCollidableException(string message)
        {
            _message = message;
        }
        public override string Message
        {
            get
            {
                return _message;
            }
        }
    }

    /// <summary>
    /// Thrown when the string contains invalid characters.
    /// </summary>
    public class InvalidCharactersException : Exception
    {
        string _message;
        public InvalidCharactersException() { }
        public InvalidCharactersException(string message) { _message = message; }
        public override string Message
        {
            get
            {
                return _message;
            }
        }
    }

    /// <summary>
    /// The exception that is thrown when the file attempting to be created already exists.
    /// </summary>
    public class FileAlreadyExistsException : Exception
    {
        string _message;
        public FileAlreadyExistsException() { }
        public FileAlreadyExistsException(string message) { _message = message; }
        public override string Message
        {
            get
            {
                return _message;
            }
        }
    }

}
