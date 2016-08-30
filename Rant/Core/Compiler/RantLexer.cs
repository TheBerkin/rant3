using System;
using System.Collections.Generic;
using System.Text;

using Rant.Core.Utilities;

namespace Rant.Core.Compiler
{
	internal static class RantLexer
	{
		public static string GetSymbolForType(R type)
		{
			switch (type)
			{
				case R.Ampersand:
					return "&";
				case R.At:
					return "@";
				case R.Caret:
					return "^";
				case R.Colon:
					return ":";
				case R.Comma:
					return ",";
				case R.Dollar:
					return "$";
				case R.DoubleColon:
					return "::";
				case R.Equal:
					return "=";
				case R.Exclamation:
					return "!";
				case R.Hyphen:
					return "-";
				case R.LeftParen:
					return "(";
				case R.RightParen:
					return ")";
				case R.LeftSquare:
					return "[";
				case R.RightSquare:
					return "]";
				case R.LeftCurly:
					return "{";
				case R.RightCurly:
					return "}";
				case R.LeftAngle:
					return "<";
				case R.RightAngle:
					return ">";
				case R.Semicolon:
					return ";";
				case R.Question:
					return "?";
				case R.Period:
					return ".";
				case R.Without:
					return "?!";
				case R.Plus:
					return "+";
				case R.VerbatimString:
					return "\"...\"";
				default:
					return "<?>";
			}
		}

		public static IEnumerable<Token> Lex(RantCompiler compiler, string input)
		{
			if (Util.IsNullOrWhiteSpace(input)) yield break;

			int line = 1; // Pretty self-explanatory
			int i = 0; // The iterator index
			int len = input.Length; // The length of the input string
			int li = len - 1; // The last character index available in the input string, precalculated for your pleasure
			char c; // Currently scanned character
			bool lineStart = true; // Indicates if all characters so far on this line have been whitespace
			int lastLineStart = 0; // Index of last line start. Use to calculate columns lazily.
			var text = new StringBuilder(64); // Buffer for text token content
			var whitespace = new StringBuilder(16); // Buffer for whitespace
			bool cycleWroteText = false; // Determines if the last loop cycle wrote to the text buffer
			int lastTextIndex = 0; // The last initial character index recorded for the text buffer
			int lastTextLine = 1; // The last initial line number recorded for the text buffer
			int lastTextLineStart = 0; // The last initial line start character index recorded for the text buffer

			while (i < len)
			{
				// No text was written on this cycle yet, so set this to false
				cycleWroteText = false;

				// Set the current scanned character
				c = input[i];

				// If a newline is detected, update all line state variables
				if (c == '\n')
				{
					line++;
					lastLineStart = i + 1;
					lineStart = true;
					whitespace.Length = 0;
					goto iterate;
				}

				// Skip carriage returns too
				if (c == '\r') goto iterate;

				// If whitespace, capture it all before moving on
				if (char.IsWhiteSpace(c))
				{
					if (!lineStart) whitespace.Append(input[i]);
					goto iterate;
				}

				// At this point, a non-whitespace character has been read.

				// Emit whitespace if queued
				if (whitespace.Length > 0)
				{
					// If it's a line breaking character, a comment, or at the start of a line, skip it.
					if (!lineStart && c != '#')
						yield return new Token(R.Whitespace, line, lastLineStart, i, whitespace.ToString());
					whitespace.Length = 0;
				}

				lineStart = false;

				switch (c)
				{
					// Comment
					case '#':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						// Just eat the line and ignore it
						while (i < len && input[i] != '\n') i++;
						goto iterate;

						// Escape sequence
					case '\\':
					{
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						if (i == li)
						{
							// This is the last character! Blasphemy!
							compiler.SyntaxError(line, lastLineStart, i, 1, false, "err-compiler-incomplete-escape");
							yield break;
						}

						// At this point we know there's at least one character. Great!

						int escStart = i++; // Skip the slash

						// No escaping whitespace.
						if (char.IsWhiteSpace(input[i]))
						{
							compiler.SyntaxError(line, lastLineStart, i, 1, false, "err-compiler-incomplete-escape");
							break;
						}

						// There's a quantifier here.
						if (char.IsDigit(input[i]))
						{
							int qStart = i++; // Skip past the digit
							while (i < len && char.IsDigit(input[i])) i++;
							if (i >= li)
							{
								compiler.SyntaxError(line, lastLineStart, escStart, i - escStart, false, "err-compiler-incomplete-escape");
								break;
							}
							// We need a comma after a quantifier
							if (input[i] != ',')
							{
								compiler.SyntaxError(line, lastLineStart, i, 1, false, "err-compiler-missing-quantity-comma");
								break;
							}
							yield return new Token(R.EscapeSequenceQuantifier, line, lastLineStart, qStart, input.Substring(qStart, i - qStart));
							i++; // Skip past the comma
						}

						// At this point we need to make sure that there are more characters
						if (i >= len)
						{
							compiler.SyntaxError(line, lastLineStart, escStart, i - escStart, false, "err-compiler-incomplete-escape");
							break;
						}

						switch (input[i])
						{
							case 'u': // Unicode code point
								// This will require 4 characters
								if (len - ++i < 4)
								{
									compiler.SyntaxError(line, lastLineStart, escStart, len - escStart, false, "err-compiler-incomplete-escape");
									yield break;
								}
								yield return new Token(R.EscapeSequenceUnicode, line, lastLineStart, escStart, input.Substring(i, 4));
								i += 4;
								goto loop;
							case 'U': // Unicode surrogate pair
								// This will require 8 characters
								if (len - ++i < 8)
								{
									compiler.SyntaxError(line, lastLineStart, escStart, len - escStart, false, "err-compiler-incomplete-escape");
									yield break;
								}
								yield return new Token(R.EscapeSequenceSurrogatePair, line, lastLineStart, escStart, input.Substring(i, 8));
								i += 8;
								goto loop;
							default:
								// Spit out the escape char token
								yield return new Token(R.EscapeSequenceChar, line, lastLineStart, i, input[i]);
								goto iterate;
						}
					}

					// Verbatim string
					case '\"':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						// Make sure we have at least a pair of quotes
						if (len - i >= 2)
						{
							int vstrStart = ++i; // Skip past starting quote
							var buffer = new StringBuilder();
							while (i < len)
							{
								// Found a matching quote?
								if (input[i] == '\"')
								{
									// Oh, it's just an escaped quote. (two in a row)
									if (i < li && input[i + 1] == '\"')
									{
										buffer.Append('\"');
										i += 2; // Skip past current and second quote
										continue;
									}

									// Neato, looks like it's the end of the string literal.

									// We don't need a new token for this, so just add it to the text buffer.
									if (text.Length == 0)
									{
										lastTextLine = line;
										lastTextIndex = i;
										lastTextLineStart = lastLineStart;
									}
									text.Append(buffer);
									cycleWroteText = true;

									i++;
									goto loop;
								}
								buffer.Append(input[i]);
								i++;
							}
							compiler.SyntaxError(line, lastLineStart, vstrStart - 1, i - vstrStart, false, "err-compiler-incomplete-verbatim");
						}
						else
						{
							compiler.SyntaxError(line, lastLineStart, i, 1, false, "err-compiler-incomplete-verbatim");
						}
						break;

					// Regular expression
					case '`':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						// Make sure we have at least a pair of apostrophes
						if (li - i >= 2)
						{
							int regStart = ++i; // Skip past starting apostrophe
							var buffer = new StringBuilder();

							while (i < len)
							{
								// Found an escape sequence in the regex?
								if (input[i] == '\\')
								{
									buffer.Append('\\');

									// Make sure there's room for an escape code and the end of the literal
									if (li - i >= 2)
									{
										if (input[i + 1] == '`')
										{
											buffer.Append('`');
											i += 2; // Skip past current and second quote
											continue;
										}
										i++;
										continue;
									}

									// If we don't have enough room for an escape char and an ending apostrophe, error.
									compiler.SyntaxError(line, lastLineStart, regStart - 1, i - regStart + 1, false, "err-compiler-incomplete-regex");
									break;
								}

								// Found another apostrophe that isn't escaped?
								if (input[i] == '`')
								{
									yield return new Token(R.Regex, line, lastLineStart, regStart - 1, buffer.ToString());

									// Read flags
									if (++i < len && char.IsLetter(input[i]))
									{
										int flagsStart = i++;
										while (i < len && char.IsLetter(input[i])) i++;
										yield return new Token(R.RegexFlags, line, lastLineStart, flagsStart, input.Substring(flagsStart, i - flagsStart));
									}
									goto loop;
								}

								// Add character to buffer
								buffer.Append(input[i]);
								i++;
							}

							// Reached EOF
							compiler.SyntaxError(line, lastLineStart, regStart, i - regStart, false, "err-compiler-incomplete-regex");
						}
						else
						{
							// Impossible to create regex here because there are too few characters remaining
							compiler.SyntaxError(line, lastLineStart, i, len - i, false, "err-compiler-incomplete-regex");
						}
						break;

					case '[':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						yield return new Token(R.LeftSquare, line, lastLineStart, i, c);
						break;
					case ']':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						yield return new Token(R.RightSquare, line, lastLineStart, i, c);
						break;
					case '{':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						yield return new Token(R.LeftCurly, line, lastLineStart, i, c);
						break;
					case '}':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						yield return new Token(R.RightCurly, line, lastLineStart, i, c);
						break;
					case '(':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						yield return new Token(R.LeftParen, line, lastLineStart, i, c);
						break;
					case ')':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						yield return new Token(R.RightParen, line, lastLineStart, i, c);
						break;
					case '<':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						yield return new Token(R.LeftAngle, line, lastLineStart, i, c);
						break;
					case '>':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						yield return new Token(R.RightAngle, line, lastLineStart, i, c);
						break;
					case '|':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						yield return new Token(R.Pipe, line, lastLineStart, i, c);
						break;
					case '?':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						if (li - i > 0 && input[i + 1] == '!')
						{
							yield return new Token(R.Without, line, lastLineStart, i, "?!");
							i++;
							break;
						}
						yield return new Token(R.Question, line, lastLineStart, i, c);
						break;
					case '!':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						yield return new Token(R.Exclamation, line, lastLineStart, i, c);
						break;
					case '@':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						yield return new Token(R.At, line, lastLineStart, i, c);
						break;
					case '-':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						yield return new Token(R.Hyphen, line, lastLineStart, i, c);
						break;
					case '$':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						yield return new Token(R.Dollar, line, lastLineStart, i, c);
						break;
					case '=':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						yield return new Token(R.Equal, line, lastLineStart, i, c);
						break;
					case '&':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						yield return new Token(R.Ampersand, line, lastLineStart, i, c);
						break;
					case '+':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						yield return new Token(R.Plus, line, lastLineStart, i, c);
						break;
					case '.':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						yield return new Token(R.Period, line, lastLineStart, i, c);
						break;
					case ':':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						// Check for double-colon
						if (li - i > 0 && input[i + 1] == ':')
						{
							yield return new Token(R.DoubleColon, line, lastLineStart, i, "::");
							i += 2;
							goto loop;
						}
						yield return new Token(R.Colon, line, lastLineStart, i, c);
						break;
					case ';':
						if (text.Length > 0)
						{
							yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
							text.Length = 0;
						}
						yield return new Token(R.Semicolon, line, lastLineStart, i, c);
						break;
					default:
						if (text.Length == 0)
						{
							lastTextLine = line;
							lastTextIndex = i;
							lastTextLineStart = lastLineStart;
						}
						text.Append(c);
						cycleWroteText = true;
						break;
				}

				// Iterates the index
				iterate:
				i++;

				loop: // Skips iteration of the index

				// Emit text token if appropriate
				if (!cycleWroteText && text.Length > 0)
				{
					yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
					text.Length = 0;
				}
			}

			// Spit out any last text that remains
			if (text.Length > 0)
				yield return new Token(R.Text, lastTextLine, lastTextLineStart, lastTextIndex, text.ToString());
		}
	}
}