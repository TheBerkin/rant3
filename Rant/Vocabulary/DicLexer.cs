using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using Rant.Stringes;

namespace Rant.Vocabulary
{
	internal static class DicLexer
	{
		public static IEnumerable<Token<DicTokenType>> Tokenize(string data)
		{
			var reader = new StringReader(data);

			while (true)
			{
				int nextChar = reader.Read();
				if (nextChar == -1)
				{
					yield return new Token<DicTokenType>(DicTokenType.EOF, "");
					yield break;
				}

				char currentChar = (char)nextChar;
				if (char.IsWhiteSpace(currentChar))
					continue;

				switch (currentChar)
				{
					// directive
					case '#':
						{
							string value = ReadRestOfLine(reader);
							yield return new Token<DicTokenType>(DicTokenType.Directive, value.Trim());
						}
						break;
					// entry or diffentry
					case '>':
						{
							int peekedChar = reader.Peek();
							if (peekedChar == -1)
								yield break;

							bool diffmark = false;
							// it's a diffentry
							if ((char)peekedChar == '>')
							{
								diffmark = true;
								reader.Read();
							}

							string value = ReadRestOfLine(reader, diffmark);
							yield return new Token<DicTokenType>(diffmark ? DicTokenType.DiffEntry : DicTokenType.Entry, value.Trim());
						}
						break;
					// property
					case '|':
						{
							string value = ReadRestOfLine(reader);
							yield return new Token<DicTokenType>(DicTokenType.Property, value.Trim());
						}
						break;
					default:
						throw new System.Exception("Unexpected character in dictionary.");
				}
			}
		}

		private static string ReadRestOfLine(StringReader reader, bool isDiffmark = false)
		{
			int nextChar;
			char currentChar;
			string value = "";
			bool pipeAllowed = !isDiffmark;

			// read to the end of the line
			while (true)
			{
				nextChar = reader.Peek();
				if (nextChar == -1)
					break;
				currentChar = (char)nextChar;
				if (isDiffmark && char.IsWhiteSpace(currentChar))
					pipeAllowed = true;

				// we've made it to the end
				if ((pipeAllowed && currentChar == '|') || currentChar == '>' || currentChar == '\n' || currentChar == '#')
					break;
				reader.Read();
				value = string.Concat(value, currentChar);
			}

			return value;
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
}