using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Core.Compiler;
using Rant.Core.Stringes;

namespace Rant
{
    /// <summary>
    /// Represents a runtime error raised by the Rant engine.
    /// </summary>
    public sealed class RantRuntimeException : Exception
    {
        private int _line;
        private int _col;
        private int _index;
        private string _source;
        private int _length;

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

        internal void SetToken(Stringe token)
        {
            _line = token.Line;
            _col = token.Column;
            _index = token.Offset;
            _length = token.Length;
        }

        internal RantRuntimeException(RantPattern source, Stringe token, string message = "A generic syntax error was encountered.") 
            : base((token != null ? ($"({source.Name} @ Ln {token.Line}, Col {token.Column}): ") : "") + message)
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

        internal RantRuntimeException(IEnumerable<Token<R>> tokens, RantPattern source, string message = "A generic syntax error was encountered.")
            : base((tokens != null ? ($"({source.Name} @ Ln {tokens.First().Line}, Col {tokens.First().Column}): ") : "") + message)
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

        internal RantRuntimeException(string source, Stringe token, string message = "A generic syntax error was encountered.")
            : base((token != null ? ($"(Ln {token.Line}, Col {token.Column}) - ") : "") + message)
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