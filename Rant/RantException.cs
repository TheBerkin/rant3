using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Engine.Compiler;
using Rant.Stringes;

namespace Rant
{
    /// <summary>
    /// Represents a runtime error raised by the Rant engine.
    /// </summary>
    public sealed class RantException : Exception
    {
        private readonly int _line;
        private readonly int _col;
        private readonly int _index;
        private readonly string _source;
        private readonly int _length;

        /// <summary>
        /// The line on which the error occurred.
        /// </summary>
        public int Line => _line;

        /// <summary>
        /// The column on which the error occurred.
        /// </summary>
        public int Column => _col;

        /// <summary>
        /// The character index on which the error occurred.
        /// </summary>
        public int Index => _index;

        /// <summary>
        /// The length of the substring in which the error occurred.
        /// </summary>
        public int Length => _length;

        /// <summary>
        /// The source of the error.
        /// </summary>
        public string Code => _source;

        internal RantException(RantPattern source, Stringe token, string message = "A generic syntax error was encountered.") 
            : base((token != null ? ("(\{source.Name} @ Ln \{token.Line}, Col \{token.Column}): ") : "") + message)
        {
            _source = source.Code;
            if (token != null)
            {
                _line = token.Line;
                _col = token.Column;
                _index = token.Offset;
                _length = token.Length;
            }
            else
            {
                _line = _col = 1;
                _index = 0;
                _length = 0;
            }
        }

        internal RantException(IEnumerable<Token<R>> tokens, RantPattern source, string message = "A generic syntax error was encountered.")
            : base((tokens != null ? ("(\{source.Name} @ Ln \{tokens.First().Line}, Col \{tokens.First().Column}): ") : "") + message)
        {
            _source = source.Code;
            
            if (tokens != null)
            {
                var first = tokens.First();
                var last = tokens.Last();
                _line = first.Line;
                _col = first.Column;
                _index = first.Offset;
                _length = (last.Offset + last.Length) - first.Offset;
            }
            else
            {

                _line = _col = 1;
                _index = 0;
                _length = 0;
            }
        }

        internal RantException(string source, Stringe token, string message = "A generic syntax error was encountered.")
            : base((token != null ? ("(Ln \{token.Line}, Col \{token.Column}) - ") : "") + message)
        {
            _source = source;
            if (token != null)
            {
                _line = token.Line;
                _col = token.Column;
                _index = token.Offset;
                _length = token.Length;
            }
            else
            {
                _line = _col = 1;
                _index = 0;
                _length = 0;
            }
        }
    }
}