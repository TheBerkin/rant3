using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rant
{
    /// <summary>
    /// Represents an error caused by an internal flaw in Rant
    /// </summary>
    // TODO: better xml comment maybe?
    public class RantInternalException : Exception
    {
        internal RantInternalException()
            : base("An internal error has occurred. This is often caused by a bug in Rant.")
        {
        }

        internal RantInternalException(string message)
            : base($"An internal error has occurred. This is often caused by a bug in Rant. Message: {message}.")
        {
        }

        internal RantInternalException(string message, Exception inner)
            : base($"An internal error has occurred. This is often caused by a bug in Rant. Message: {message}.", inner)
        {
        }
    }
}
