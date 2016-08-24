using System;

using Rant.Core.Stringes;

namespace Rant.Vocabulary
{
	/// <summary>
	/// Thrown when Rant encounters an error while loading a dictionary table.
	/// </summary>
	public sealed class RantTableLoadException : Exception
	{
		internal RantTableLoadException(string origin, Stringe token, string message)
			: base($"{origin}: (Ln {token.Line}, Col {token.Column}) {message}")
		{
			Line = token.Line;
			Column = token.Column;
			Offset = token.Offset;
			Origin = origin;
		}

		/// <summary>
		/// Gets the line number on which the error occurred.
		/// </summary>
		public int Line { get; }

		/// <summary>
		/// Gets the column on which the error occurred.
		/// </summary>
		public int Column { get; }

		/// <summary>
		/// Gets the character offset on which the error occurred.
		/// </summary>
		public int Offset { get; }

		/// <summary>
		/// Gets a string describing where the table was loaded from. For tables loaded from disk, this will be the file path.
		/// </summary>
		public string Origin { get; }
	}
}