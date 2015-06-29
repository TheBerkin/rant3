using Rant.Stringes;
using System;

namespace Rant
{
	/// <summary>
	/// Represents an error raised by Rant during pattern compilation.
	/// </summary>
	public sealed class RantCompilerException : Exception
	{
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
		/// The length of the token(s) on which the error occurred.
		/// </summary>
		public int Length { get; private set; }

		internal RantCompilerException(string name, Stringe source, string message) 
			: base(source != null 
				  ? $"{name} @ Line {source.Line}, Col {source.Column}: {message}"
				  : $"{name}: {message}")
		{
			if (source == null) return;
			Line = source.Line;
			Column = source.Column;
			Index = source.Offset;
			Length = source.Length;
		}

		internal RantCompilerException(string name, Stringe source, Exception innerException)
			: base(source != null 
				  ? $"{name} @ Line {source.Line}, Col {source.Column}: {innerException.Message}"
				  : $"{name}: {innerException.Message}",
				  innerException)
		{
			if (source == null) return;
			Line = source.Line;
			Column = source.Column;
			Index = source.Offset;
			Length = source.Length;
		}
	}
}