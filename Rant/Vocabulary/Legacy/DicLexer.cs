using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

using Rant.Core.Compiler;

namespace Rant.Vocabulary
{
	internal static class DicLexer
	{
		public static IEnumerable<DicToken> Tokenize(string source, string data)
		{
			var reader = new StringReader(data);
			string currentLine;
			char firstChar;
			int lineNumber = 1;

			while(reader.Peek() >= 0)
			{
				lineNumber++;

				currentLine = reader.ReadLine().Trim();
				if(string.IsNullOrWhiteSpace(currentLine)) continue;
				firstChar = currentLine[0];

				switch(firstChar)
				{
					case '#':
						yield return new DicToken(DicTokenType.Directive, lineNumber, currentLine.Substring(1).Trim());
						break;
					case '@':
						break;
					case '>':
						{
							currentLine = currentLine.Substring(1);

							bool diffmark = false;
							if(currentLine[0] == '>')
							{
								diffmark = true;
								currentLine = currentLine.Substring(1);
							}

							yield return new DicToken(diffmark ? DicTokenType.DiffEntry : DicTokenType.Entry, lineNumber, currentLine.Trim());
							break;
						}
					case '|':
						yield return new DicToken(DicTokenType.Property, lineNumber, currentLine.Substring(1).Trim());
						break;
					default:
						throw new InvalidDataException($"{source}: (Line {lineNumber}, Col 1) Unexpected token: '{firstChar}'.");
				}
			}

			yield return new DicToken(DicTokenType.EOF, lineNumber, null);
		}
	}

	internal enum DicTokenType
	{
		Directive,
		Entry,
		DiffEntry,
		Property,
		Ignore,
		EOF
	}

	internal struct DicToken
	{
		public static readonly DicToken None = new DicToken(DicTokenType.Ignore, 1, null);
		public DicTokenType Type;
		public string Value;
		public int Line;

		public DicToken(DicTokenType type, int line, string value)
		{
			Type = type;
			Value = value;
			Line = line;
		}

		public DicToken(DicTokenType type, int line, char value)
		{
			Type = type;
			Value = value.ToString(CultureInfo.InvariantCulture);
			Line = line;
		}

		public static implicit operator string(DicToken token)
		{
			return token.Value;
		}

		public static implicit operator DicToken(string data)
		{
			return new DicToken(DicTokenType.Ignore, 1, data);
		}

		public int Length => Value?.Length ?? 0;
	}
}