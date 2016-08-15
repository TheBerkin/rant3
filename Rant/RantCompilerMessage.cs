using Rant.Localization;

namespace Rant
{
	public sealed class RantCompilerMessage
	{
		public RantCompilerMessageType Type { get; }
		public string Source { get; }
		public string Message { get; }
		public int Line { get; }
		public int Column { get; }
		public int Index { get; }

		public RantCompilerMessage(RantCompilerMessageType type, string source, string message, int line, int column, int index)
		{
			Type = type;
			Source = source;
			Message = message;
			Line = line;
			Column = column;
			Index = index;
		}

		public override string ToString()
		{
			return Line > 0 
				? $"{Txtres.GetString("src-line-col", Source, Line, Column)} {Message}"
				: $"({Source}) {Message}";
		}
	}
}