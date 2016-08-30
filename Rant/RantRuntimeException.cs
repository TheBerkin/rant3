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
				? ($"{GetString("src-line-col", source.Name, token.Line, token.Column)} ")
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