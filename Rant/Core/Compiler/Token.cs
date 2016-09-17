using System.Globalization;

namespace Rant.Core.Compiler
{
	internal struct Token
	{
		public static readonly Token None = new Token(R.Text, 0, -1, -1, null);
		public int Column;
		public int Index;
		public int Line;
		public R Type;
		public string Value;

		public Token(R type, int line, int lastLineStart, int index, string value)
		{
			Type = type;
			Line = line;
			Column = index - lastLineStart + 1;
			Index = index;
			Value = value;
		}

		public Token(R type, int line, int lastLineStart, int index, char value)
		{
			Type = type;
			Line = line;
			Column = index - lastLineStart + 1;
			Index = index;
			Value = value.ToString(CultureInfo.InvariantCulture);
		}

		public int Length => Value?.Length ?? 0;
		public LineCol ToLocation() => new LineCol(Line, Column, Index);
	}
}