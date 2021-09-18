using System;

namespace FlagPFP.Core.Exceptions
{
    /// <summary>
    /// Thrown when no flag JSONs are found.
    /// </summary>
    public class NoFlagsFoundException : Exception
    {
        public NoFlagsFoundException()
        {
        }

        public NoFlagsFoundException(string message)
            : base(message)
        {
        }

        public NoFlagsFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    /// <summary>
    /// Thrown when the flag name provided has no definition.
    /// </summary>
    public class InvalidFlagException : Exception
    {
        public InvalidFlagException()
        {
        }

        public InvalidFlagException(string message)
            : base(message)
        {
        }

        public InvalidFlagException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
