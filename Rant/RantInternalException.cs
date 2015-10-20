using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rant
{
    public class RantInternalException : Exception
    {
        public RantInternalException()
            : base("An internal error has occurred. This is often caused by a bug in Rant.")
        {
        }

        public RantInternalException(string message)
            : base($"An internal error has occurred. This is often caused by a bug in Rant. Message: {message})")
        {
        }

        public RantInternalException(string message, Exception inner)
            : base($"An internal error has occurred. This is often caused by a bug in Rant. Message: {message})", inner)
        {
        }
    }
}
