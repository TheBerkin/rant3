using System;

using Rant.Localization;

namespace Rant.Vocabulary
{
	/// <summary>
	/// Thrown when Rant encounters an error while loading a dictionary table.
	/// </summary>
	public sealed class RantTableLoadException : Exception
	{
		internal RantTableLoadException(string origin, int line, int col, string messageType, params object[] messageArgs)
			: base(Txtres.GetString("src-line-col", Txtres.GetString(messageType, messageArgs), line, col))
		{
			Line = line;
			Column = col;
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
		/// Gets a string describing where the table was loaded from. For tables loaded from disk, this will be the file path.
		/// </summary>
		public string Origin { get; }
	}
}