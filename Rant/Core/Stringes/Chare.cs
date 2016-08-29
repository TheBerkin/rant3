using System.Globalization;

namespace Rant.Core.Stringes
{
	/// <summary>
	/// Represents a charactere, which provides location information on a character taken from a stringe.
	/// </summary>
	internal sealed class Chare
	{
		private int _column;
		private int _line;

		internal Chare(Stringe source, char c, int offset)
		{
			Source = source;
			Character = c;
			Offset = offset;
			_line = _column = 0;
		}

		internal Chare(Stringe source, char c, int offset, int line, int col)
		{
			Source = source;
			Character = c;
			Offset = offset;
			_line = line;
			_column = col;
		}

		/// <summary>
		/// The stringe from which the charactere was taken.
		/// </summary>
		public Stringe Source { get; }

		/// <summary>
		/// The underlying character.
		/// </summary>
		public char Character { get; }

		/// <summary>
		/// The index of the character in the main string.
		/// </summary>
		public int Offset { get; }

		/// <summary>
		/// The line on which the charactere appears.
		/// </summary>
		public int Line
		{
			get
			{
				if (_line == 0) SetLineCol();
				return _line;
			}
		}

		/// <summary>
		/// The column on which the charactere appears.
		/// </summary>
		public int Column
		{
			get
			{
				if (_column == 0) SetLineCol();
				return _column;
			}
		}

		private bool Equals(Chare other)
		{
			return Character == other.Character;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj is Chare && Equals((Chare)obj);
		}

		public override int GetHashCode()
		{
			return Character.GetHashCode();
		}

		private void SetLineCol()
		{
			_line = Source.Line;
			_column = Source.Column;
			if (Offset <= 0) return; // We are at the first character, nothing to do here

			int start = Offset - 1; // We have to start at least at the previous character

			// Find the last character before this one that has line/col assigned
			for (; start >= 0; start--)
			{
				if (Source[start]._line > 0) break;
			}

			// Fill all lines/cols on previous chars and current one
			for (int i = start; i < Offset; i++)
			{
				if (Source.ParentString[Offset] == '\n')
				{
					Source[i]._line = _line++;
					Source[i]._column = _column = 1;
				}
				else
				{
					Source[i]._column = _column++;
				}
			}
		}

		/// <summary>
		/// Returns the string representation of the current charactere.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => Character.ToString(CultureInfo.InvariantCulture);

		public static bool operator ==(Chare chare, char c)
		{
			return chare?.Character == c;
		}

		public static bool operator !=(Chare chare, char c)
		{
			return !(chare == c);
		}
	}
}