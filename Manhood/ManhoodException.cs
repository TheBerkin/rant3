using System;

namespace Manhood
{
    /// <summary>
    /// Represents runtime errors raised by the Manhood engine.
    /// </summary>
    public sealed class ManhoodException : Exception
    {
        private readonly int _line;
        private readonly int _col;
        private readonly int _index;
        private readonly Source _source;
        
        /// <summary>
        /// The line on which the error occurred.
        /// </summary>
        public int Line { get { return _line; } }

        /// <summary>
        /// The column on which the error occurred.
        /// </summary>
        public int Column { get { return _col; } }

        /// <summary>
        /// The character index on which the error occurred.
        /// </summary>
        public int Index { get { return _index; } }

        /// <summary>
        /// The source of the error.
        /// </summary>
        public Source SourceCode { get { return _source; } }

        internal ManhoodException(Source source, Token token, string message = "A generic syntax error was encountered.") : base(message)
        {
            _source = source;
            _line = token.Line;
            _col = token.Column;
            _index = token.Index;
        }
    }
}