using System.Collections.Generic;

using Rant.Core.Stringes;

namespace Rant.Core.Compiler
{
	internal static class RantLexer
	{
		internal static readonly Lexer<R> Rules;

		static RantLexer()
		{
			Rules = new Lexer<R>
			{
				{ reader => reader.EatWhile(char.IsWhiteSpace), R.Whitespace },
				// Comments
				{
					reader =>
					{
						reader.SkipWhiteSpace();
						if (!reader.Eat('#')) return false;
						reader.ReadUntilAny('\r', '\n');
						return true;
					},
					R.Ignore, 3
				},
				// Blackspace (^\s*)
				{
					reader =>
					{
						if (reader.Position == 0) return reader.EatWhile(char.IsWhiteSpace);
						return reader.IsNext('\r', '\n') && reader.EatWhile(char.IsWhiteSpace);
					},
					R.Ignore, 2
				},
				// Blackspace (\s*[\r\n]+\s*)
				{
					reader =>
					{
						reader.EatWhile(
							c => char.IsWhiteSpace(c.Character) && c.Character != '\r' && c.Character != '\n');
						if (!reader.EatAll('\r', '\n')) return false;
						reader.EatWhile(char.IsWhiteSpace);
						return true;
					},
					R.Ignore, 2
				},
				// Blackspace (\s*$)
				{
					reader =>
					{
						while (!reader.EndOfStringe)
						{
							if (reader.EatAll('\r', '\n')) return true;
							if (!reader.SkipWhiteSpace()) return false;
						}
						return true;
					},
					R.Ignore, 2
				},
				// Escape sequence
				{
					reader =>
					{
						if (!reader.Eat('\\')) return false;
						if (reader.EndOfStringe)
						{
							reader.Error(reader.Stringe.Substringe(reader.Position - 1, 1), true, "err-compiler-incomplete-escape");
							return false;
						}
						if (reader.EatWhile(char.IsDigit))
						{
							if (reader.Eat('.')) reader.EatWhile(char.IsDigit);
							reader.EatAny('k', 'M', 'B');
							if (!reader.Eat(','))
							{
								reader.Error(reader.Stringe.Substringe(reader.Position - 1, 1), true, "err-compiler-missing-quantity-comma");
								return true;
							}
						}
						if (reader.Eat('u'))
						{
							for (int i = 0; i < 4; i++)
							{
								char c = reader.ReadChare().Character;
								if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
									return false;
							}
						}
						else if (reader.Eat('U'))
						{
							for (int i = 0; i < 8; i++)
							{
								char c = reader.ReadChare().Character;
								if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
									return false;
							}
						}
						else
						{
							reader.ReadChare();
						}
						return true;
					},
					R.EscapeSequenceChar
				},
				// Regex
				{
					reader =>
					{
						if (!reader.Eat('`')) return false;
						var token = reader.Stringe.Substringe(reader.Position - 1, 1);
						while (!reader.EndOfStringe)
						{
							reader.EatAll("\\`");
							if (reader.ReadChare() == '`')
							{
								reader.Eat('i');
								return true;
							}
						}
						reader.Error(token, true, "err-compiler-incomplete-regex");
						return false;
					},
					R.Regex
				},
				// Verbatim string
				{
					reader =>
					{
						if (!reader.Eat('"')) return false;
						var token = reader.Stringe.Substringe(reader.Position - 1, 1);
						while (!reader.EndOfStringe)
						{
							reader.EatAll("\"\"");
							if (reader.ReadChare() == '"') return true;
						}
						reader.Error(token, true, "err-compiler-incomplete-verbatim");
						return false;
					},
					R.VerbatimString
				},
				{ "[", R.LeftSquare },
				{ "]", R.RightSquare },
				{ "{", R.LeftCurly },
				{ "}", R.RightCurly },
				{ "<", R.LeftAngle },
				{ ">", R.RightAngle },
				{ "(", R.LeftParen },
				{ ")", R.RightParen },
				{ "|", R.Pipe },
				{ ";", R.Semicolon },
				{ ":", R.Colon },
				{ "@", R.At },
				{ "?", R.Question },
				{ "::", R.DoubleColon },
				{ "?!", R.Without },
				{ "-", R.Hyphen },
				{ "!", R.Exclamation },
				{ "$", R.Dollar },
				{ "=", R.Equal },
				{ "&", R.Ampersand },
				{ "+", R.Plus },
				{ ".", R.Period }
			};
			Rules.AddUndefinedCaptureRule(R.Text, TruncatePadding);
			Rules.AddEndToken(R.EOF);
			Rules.Ignore(R.Ignore);
		}

		private static Stringe TruncatePadding(Stringe input)
		{
			var ls = input.LeftPadded ? input.TrimStart() : input;
			return ls.RightPadded ? ls.TrimEnd() : ls;
		}

		public static IEnumerable<Token<R>> GenerateTokens(string name, Stringe input, StringeErrorDelegate errorCallback)
		{
			var reader = new StringeReader(input, errorCallback)
			{
				Origin = name
			};
			while (!reader.EndOfStringe) yield return reader.ReadToken(Rules);
		}
	}
}