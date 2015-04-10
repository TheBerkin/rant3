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

		internal RantCompilerException(Stringe source)
		{
			
		}
	}
}