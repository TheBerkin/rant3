#region License

// https://github.com/TheBerkin/Rant
// 
// Copyright (c) 2017 Nicholas Fleck
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;

using Rant.Core.Compiler;
using Rant.Core.Compiler.Syntax;

using static Rant.Localization.Txtres;
using Rant.Core;

namespace Rant
{
    /// <summary>
    /// Represents a runtime error raised by the Rant engine.
    /// </summary>
    public sealed class RantRuntimeException : Exception
    {
        internal RantRuntimeException(Sandbox sb, LineCol token, string errorMessageType = "err-generic-runtime",
            params object[] errorArgs)
            : base((token.Index != -1
                       ? $"{GetString("src-line-col", sb.Pattern.Name, token.Line, token.Column)} "
                       : $"({sb.Pattern.Name}) ") + GetString(errorMessageType, errorArgs))
        {
            Code = sb.Pattern.Code;
            Line = token.Line;
            Column = token.Column;
            Index = token.Index;
			RantStackTrace = sb.GetStackTrace();
        }

        internal RantRuntimeException(Sandbox sb, RST rst, string errorMessageType = "err-generic-runtime",
            params object[] errorArgs)
            : base(rst == null
                ? $"({sb.Pattern.Name}) {GetString(errorMessageType, errorArgs)}"
                : $"{GetString("src-line-col", sb.Pattern.Name, rst.Location.Line, rst.Location.Column)} {GetString(errorMessageType, errorArgs)}"
            )
        {
            Code = sb.Pattern.Code;
            Line = rst?.Location.Line ?? 0;
            Column = rst?.Location.Column ?? 0;
            Index = rst?.Location.Index ?? -1;
			RantStackTrace = sb.GetStackTrace();
        }

        /// <summary>
        /// The line on which the error occurred.
        /// </summary>
        public int Line { get; private set; }

        /// <summary>
        /// The column on which the error occurred.
        /// </summary>
        public int Column { get; private set; }

        /// <summary>
        /// The character index on which the error occurred.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// The length of the substring in which the error occurred.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// The source of the error.
        /// </summary>
        public string Code { get; }

		/// <summary>
		/// The stack trace from the pattern.
		/// </summary>
		public string RantStackTrace { get; }

		/// <summary>
		/// Returns a string representation of the runtime error, including the message and stack trace.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Message}\n\nStack Trace:\n{RantStackTrace}";
		}
	}
}