using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Core.Compiler;
using Rant.Core.Compiler.Syntax;
using Rant.Core.Stringes;

using static Rant.Localization.Txtres;

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

        internal RantRuntimeException(RantPattern source, TokenLocation token, string errorMessageType = "err-generic-runtime", params object[] errorArgs) 
            : base((token.Index != -1 
				  ? ($"{GetString("src-line-col", source.Name, token.Line, token.Column)} ") 
				  : $"({source.Name}) ") + GetString(errorMessageType, errorArgs))
        {
            _source = source.Code;
			_line = token.Line;
			_col = token.Column;
			_index = token.Index;
		}

	    internal RantRuntimeException(RantPattern source, RST rst, string errorMessageType = "err-generic-runtime",
		    params object[] errorArgs)
		    : base(rst == null
			    ? $"({source.Name}) {GetString(errorMessageType, errorArgs)}"
			    : $"{GetString("src-line-col", source.Name, rst.Location.Line, rst.Location.Column)} {GetString(errorMessageType, errorArgs)}"
			    )
	    {
		    _source = source.Code;
		    _line = rst?.Location.Line ?? 0;
		    _col = rst?.Location.Column ?? 0;
		    _index = rst?.Location.Index ?? -1;
	    }
    }
}