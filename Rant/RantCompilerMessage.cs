using static Rant.Localization.Txtres;

namespace Rant
{
	/// <summary>
	/// Represents a message emitted by the Rant compiler while performing a job.
	/// </summary>
	public sealed class RantCompilerMessage
	{
		internal RantCompilerMessage(RantCompilerMessageType type, string source, string message, int line, int column,
			int index, int length)
		{
			Type = type;
			Source = source;
			Message = message;
			Line = line;
			Column = column;
			Index = index;
			Length = length;
		}

		/// <summary>
		/// The type of message.
		/// </summary>
		public RantCompilerMessageType Type { get; }

		/// <summary>
		/// The source path of the pattern being compiled when the message was generated.
		/// </summary>
		public string Source { get; }

		/// <summary>
		/// The message text.
		/// </summary>
		public string Message { get; }

		/// <summary>
		/// The line on which the message was generated.
		/// </summary>
		public int Line { get; }

		/// <summary>
		/// The column on which the message was generated.
		/// </summary>
		public int Column { get; }

		/// <summary>
		/// The character index on which the message was generated.
		/// </summary>
		public int Index { get; }

		/// <summary>
		/// The length, in characters, of the code snippet to which the message pertains.
		/// </summary>
		public int Length { get; }

		/// <summary>
		/// Generates a string representation of the message.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Line > 0
				? $"{GetString("src-line-col", Source, Line, Column)} {Message}"
				: $"({Source}) {Message}";
		}
	}
}