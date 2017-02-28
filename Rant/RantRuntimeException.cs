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

namespace Rant
{
	/// <summary>
	/// Represents a runtime error raised by the Rant engine.
	/// </summary>
	public sealed class RantRuntimeException : Exception
	{
		internal RantRuntimeException(RantProgram source, LineCol token, string errorMessageType = "err-generic-runtime",
			params object[] errorArgs)
			: base((token.Index != -1
				       ? $"{GetString("src-line-col", source.Name, token.Line, token.Column)} "
				       : $"({source.Name}) ") + GetString(errorMessageType, errorArgs))
		{
			Code = source.Code;
			Line = token.Line;
			Column = token.Column;
			Index = token.Index;
		}

		internal RantRuntimeException(RantProgram source, RST rst, string errorMessageType = "err-generic-runtime",
			params object[] errorArgs)
			: base(rst == null
				? $"({source.Name}) {GetString(errorMessageType, errorArgs)}"
				: $"{GetString("src-line-col", source.Name, rst.Location.Line, rst.Location.Column)} {GetString(errorMessageType, errorArgs)}"
			)
		{
			Code = source.Code;
			Line = rst?.Location.Line ?? 0;
			Column = rst?.Location.Column ?? 0;
			Index = rst?.Location.Index ?? -1;
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
	}
}