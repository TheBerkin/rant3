using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rant.Core.Stringes
{
	/// <summary>
	/// Represents a reader that can read data from a stringe.
	/// </summary>
	internal sealed class StringeReader
	{
		private readonly StringeErrorDelegate _errorCallback;
		private int _pos; 

		/// <summary>
		/// Creates a new StringeReader instance using the specified string as input.
		/// </summary>
		/// <param name="value">The string to use as input. This will be converted to a root-level stringe.</param>
		public StringeReader(string value)
		{
			Stringe = value.ToStringe();
			_pos = 0;
		}

		/// <summary>
		/// Creates a new StringeReader instance using the specified stringe as input.
		/// </summary>
		/// <param name="value">The stringe to use as input.</param>
		public StringeReader(Stringe value)
		{
			Stringe = value;
			_pos = 0;
		}

		public StringeReader(Stringe value, StringeErrorDelegate errorCallback)
		{
			Stringe = value;
			_errorCallback = errorCallback;
		}

		/// <summary>
		/// Gets or sets a string describing where the stringe originated from. Used for exception messages.
		/// </summary>
		public string Origin { get; set; } = string.Empty;

		/// <summary>
		/// The stringe being read by the current instance.
		/// </summary>
		public Stringe Stringe { get; }

		/// <summary>
		/// Indicates whether the reader position is at the end of the input string.
		/// </summary>
		public bool EndOfStringe => _pos >= Stringe.Length;

		/// <summary>
		/// The current zero-based position of the reader.
		/// </summary>
		public int Position
		{
			get { return _pos; }
			set
			{
				if (value < 0 || value > Stringe.Length)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}
				_pos = value;
			}
		}

		/// <summary>
		/// The total length, in characters, of the stringe being read.
		/// </summary>
		public int Length => Stringe.Length;

		public void Error(Stringe token, bool fatal, string messageType, params object[] args)
			=> _errorCallback?.Invoke(token, fatal, messageType, args);

		/// <summary>
		/// Reads a charactere from the input and advances the position by one.
		/// </summary>
		/// <returns></returns>
		public Chare ReadChare()
		{
			return Stringe[_pos++];
		}

		/// <summary>
		/// Returns the next charactere in the input, but does not consume it.
		/// </summary>
		/// <returns></returns>
		public Chare PeekChare()
		{
			return EndOfStringe ? null : Stringe[_pos];
		}

		/// <summary>
		/// Returns the next character in the input, but does not consume it. Returns -1 if no more characters can be read.
		/// </summary>
		/// <returns></returns>
		public int PeekChar()
		{
			return EndOfStringe ? -1 : Stringe[_pos].Character;
		}

		/// <summary>
		/// Reads a stringe from the input and advances the position by the number of characters read.
		/// </summary>
		/// <param name="length">The number of characters to read.</param>
		/// <returns></returns>
		public Stringe ReadStringe(int length)
		{
			int p = _pos;
			_pos += length;
			return Stringe.Substringe(p, length);
		}

		/// <summary>
		/// Reads a stringe from the current position to the next occurrence of the specified character. If no match is found, it
		/// reads to the end.
		/// </summary>
		/// <param name="value">The character to stop at.</param>
		/// <returns></returns>
		public Stringe ReadUntil(char value)
		{
			int start = _pos;
			while (_pos < Stringe.Length && Stringe[_pos] != value) _pos++;
			return Stringe.Substringe(start, _pos - start);
		}

		/// <summary>
		/// Reads a stringe from the current position to the next occurrence of any of the specified characters. If no match is
		/// found, it reads to the end.
		/// </summary>
		/// <param name="values">The characters to stop at.</param>
		/// <returns></returns>
		public Stringe ReadUntilAny(params char[] values)
		{
			int start = _pos;
			while (_pos < Stringe.Length && !values.Any(c => Stringe[_pos] == c)) _pos++;
			return Stringe.Substringe(start, _pos - start);
		}

		/// <summary>
		/// Reads to the end of the current line.
		/// </summary>
		/// <returns></returns>
		public Stringe ReadRestOfLine()
		{
			int start = _pos;
			while (_pos < Stringe.Length && !"\r\n".Contains(Stringe[_pos].Character)) _pos++;
			return Stringe.Substringe(start, _pos - start);
		}

		/// <summary>
		/// Indicates whether the specified character occurs at the reader's current position, and consumes it.
		/// </summary>
		/// <param name="value">The character to test for.</param>
		/// <returns></returns>
		public bool Eat(char value)
		{
			if (EndOfStringe || PeekChare() != value) return false;
			_pos++;
			return true;
		}

		public bool EatExactlyWhere(Func<char, bool> predicate, int count)
		{
			if (EndOfStringe || !predicate(PeekChare().Character)) return false;
			int oldPos = _pos;
			int n = 0;
			do
			{
				_pos++;
				n++;
			} while (!EndOfStringe && predicate(PeekChare().Character) && n < count);
			if (n < count)
			{
				_pos = oldPos;
				return false;
			}
			return true;
		}

		public bool EatAll(char value)
		{
			if (PeekChare() != value) return false;
			do
			{
				_pos++;
			} while (PeekChare() == value);
			return true;
		}

		public bool EatAny(params char[] values)
		{
			if (!EndOfStringe && values.Any(c => Stringe[_pos].Character == c))
			{
				_pos++;
				return true;
			}
			return false;
		}

		public bool EatAll(params char[] values)
		{
			if (EndOfStringe) return false;
			if (!values.Any(c => Stringe[_pos].Character == c)) return false;
			do
			{
				_pos++;
			} while (values.Any(c => Stringe[_pos].Character == c));
			return true;
		}

		public bool EatWhile(Func<char, bool> predicate)
		{
			if (EndOfStringe || !predicate(PeekChare().Character)) return false;
			do
			{
				_pos++;
			} while (!EndOfStringe && predicate(PeekChare().Character));
			return true;
		}

		public bool EatWhile(Func<Chare, bool> predicate)
		{
			if (EndOfStringe || !predicate(PeekChare())) return false;
			do
			{
				_pos++;
			} while (!EndOfStringe && predicate(PeekChare()));
			return true;
		}

		/// <summary>
		/// Indicates whether the specified string occurs at the reader's current position, and consumes it.
		/// </summary>
		/// <param name="value">The string to test for.</param>
		/// <returns></returns>
		public bool Eat(string value)
		{
			if (string.IsNullOrEmpty(value)) return false;
			if (Stringe.IndexOf(value, _pos) != _pos) return false;
			_pos += value.Length;
			return true;
		}

		public bool EatAll(string value)
		{
			if (string.IsNullOrEmpty(value)) return false;
			if (Stringe.IndexOf(value, _pos) != _pos) return false;
			do
			{
				_pos += value.Length;
			} while (Stringe.IndexOf(value, _pos) == _pos);
			return true;
		}

		/// <summary>
		/// Indicates whether the specified regular expression matches the input at the reader's current position. If a match is
		/// found, the reader consumes it.
		/// </summary>
		/// <param name="regex">The regular expression to test for.</param>
		/// <returns></returns>
		public bool Eat(Regex regex)
		{
			if (regex == null) throw new ArgumentNullException(nameof(regex));
			var match = regex.Match(Stringe.Value, _pos);
			if (!match.Success || match.Index != _pos) return false;
			_pos += match.Length;
			return true;
		}

		/// <summary>
		/// Indicates whether the specified regular expression matches the input at the reader's current position. If a match is
		/// found, the reader consumes it and outputs the result.
		/// </summary>
		/// <param name="regex">The regular expression to test for.</param>
		/// <param name="result">The stringe to output the result to.</param>
		/// <returns></returns>
		public bool Eat(Regex regex, out Stringe result)
		{
			if (regex == null) throw new ArgumentNullException(nameof(regex));
			result = null;
			var match = regex.Match(Stringe.Value, _pos);
			if (!match.Success || match.Index != _pos) return false;
			result = Stringe.Substringe(_pos, match.Length);
			_pos += match.Length;
			return true;
		}

		/// <summary>
		/// Indicates whether the specified character occurs at the reader's current position.
		/// </summary>
		/// <param name="value">The character to test for.</param>
		/// <returns></returns>
		public bool IsNext(char value)
		{
			return PeekChare() == value;
		}

		/// <summary>
		/// Indicates whether any of the specified characters occur at the reader's current position.
		/// </summary>
		/// <param name="chars"></param>
		/// <returns></returns>
		public bool IsNext(params char[] chars) => !EndOfStringe && chars.Any(c => Stringe[_pos].Character == c);

		/// <summary>
		/// Indicates whether the specified string occurs at the reader's current position.
		/// </summary>
		/// <param name="value">The string to test for.</param>
		/// <param name="strcmp">The string comparison type to use in the test.</param>
		/// <returns></returns>
		public bool IsNext(string value, StringComparison strcmp = StringComparison.InvariantCulture)
		{
			if (string.IsNullOrEmpty(value)) return false;
			return _pos + value.Length <= Stringe.Length
			       && string.Equals(Stringe.Substringe(_pos, value.Length).Value, value, strcmp);
		}

		/// <summary>
		/// Indicates whether the specified regular expression matches the input at the reader's current position.
		/// </summary>
		/// <param name="regex">The regular expression to test for.</param>
		/// <returns></returns>
		public bool IsNext(Regex regex)
		{
			if (regex == null) throw new ArgumentNullException(nameof(regex));
			var match = regex.Match(Stringe.Value, _pos);
			return match.Success && match.Index == _pos;
		}

		/// <summary>
		/// Indicates whether the specified regular expression matches the input at the reader's current position, and outputs the
		/// result.
		/// </summary>
		/// <param name="regex">The regular expression to test for.</param>
		/// <param name="result">The stringe to output the result to.</param>
		/// <returns></returns>
		public bool IsNext(Regex regex, out Stringe result)
		{
			if (regex == null) throw new ArgumentNullException(nameof(regex));
			result = null;
			var match = regex.Match(Stringe.Value, _pos);
			if (!match.Success || match.Index != _pos) return false;
			result = Stringe.Substringe(_pos, match.Length);
			return true;
		}

		/// <summary>
		/// Advances the reader position past any immediate white space characters.
		/// </summary>
		public bool SkipWhiteSpace()
		{
			int oldPos = _pos;
			while (!EndOfStringe && char.IsWhiteSpace(Stringe.Value[_pos]))
			{
				_pos++;
			}
			return _pos > oldPos;
		}

		/// <summary>
		/// Returns a boolean value indicating whether the previous character matches the specified character.
		/// </summary>
		/// <param name="c">The character to test for.</param>
		/// <returns></returns>
		public bool WasLast(char c) => _pos > 0 && Stringe[_pos - 1].Character == c;

		/// <summary>
		/// Returns a boolean value indicating whether the previous character matches any of the specified characters.
		/// </summary>
		/// <param name="chars">The characters to test for.</param>
		/// <returns></returns>
		public bool WasLast(params char[] chars) => _pos > 0 && chars.Any(c => Stringe[_pos - 1].Character == c);

		/// <summary>
		/// Reads the next token from the current position, then advances the position past it.
		/// </summary>
		/// <typeparam name="T">The token identifier type to use.</typeparam>
		/// <param name="rules">The lexer to use.</param>
		/// <returns></returns>
		public Token<T> ReadToken<T>(Lexer<T> rules) where T : struct
		{
			readStart:

			if (EndOfStringe)
			{
				if (rules.EndToken != null && !rules.IgnoreRules.Contains(rules.EndToken.Item2))
					return new Token<T>(rules.EndToken.Item2, rules.EndToken.Item1);

				throw new InvalidOperationException("Unexpected end of input.");
			}

			// Indicates if undefined tokens should be created
			bool captureUndef = rules.UndefinedCaptureRule != null;

			// Tracks the beginning of the undefined token content
			int u = _pos;

			do
			{
				// If we've reached the end, return undefined token, if present.
				if (EndOfStringe && captureUndef && u < _pos)
				{
					if (rules.IgnoreRules.Contains(rules.UndefinedCaptureRule.Item2)) goto readStart;
					return new Token<T>(rules.UndefinedCaptureRule.Item2, rules.UndefinedCaptureRule.Item1(Stringe.Slice(u, _pos)));
				}

				if (rules.HasPunctuation(PeekChar()))
				{
					// Check high priority symbol rules
					foreach (var t in rules.HighSymbols.Where(t => IsNext(t.Item1, t.Item3)))
					{
						// Return undefined token if present
						if (captureUndef && u < _pos)
						{
							if (rules.IgnoreRules.Contains(rules.UndefinedCaptureRule.Item2)) goto readStart;
							return new Token<T>(rules.UndefinedCaptureRule.Item2,
								rules.UndefinedCaptureRule.Item1(Stringe.Slice(u, _pos)));
						}

						// Return symbol token
						var c = Stringe.Substringe(_pos, t.Item1.Length);
						_pos += t.Item1.Length;
						if (rules.IgnoreRules.Contains(t.Item2)) goto readStart;
						return new Token<T>(t.Item2, c);
					}
				}

				if (rules.FunctionList.Any())
				{
					int origPos = _pos;

					foreach (var fn in rules.FunctionList)
					{
						if (fn.Item1(this))
						{
							// Return undefined token if present
							if (captureUndef && u < origPos)
							{
								_pos = origPos;
								if (rules.IgnoreRules.Contains(rules.UndefinedCaptureRule.Item2)) goto readStart;
								return new Token<T>(rules.UndefinedCaptureRule.Item2,
									rules.UndefinedCaptureRule.Item1(Stringe.Slice(u, origPos)));
							}

							if (rules.IgnoreRules.Contains(fn.Item2)) goto readStart;
							return new Token<T>(fn.Item2, Stringe.Slice(origPos, _pos));
						}

						// Reset for next function
						_pos = origPos;
					}
				}

				if (rules.HasPunctuation(PeekChar()))
				{
					// Check normal priority symbol rules
					foreach (var t in rules.NormalSymbols.Where(t => IsNext(t.Item1, t.Item3)))
					{
						// Return undefined token if present
						if (captureUndef && u < _pos)
						{
							if (rules.IgnoreRules.Contains(rules.UndefinedCaptureRule.Item2)) goto readStart;
							return new Token<T>(rules.UndefinedCaptureRule.Item2,
								rules.UndefinedCaptureRule.Item1(Stringe.Slice(u, _pos)));
						}

						// Return symbol token
						var c = Stringe.Substringe(_pos, t.Item1.Length);
						_pos += t.Item1.Length;
						if (rules.IgnoreRules.Contains(t.Item2)) goto readStart;
						return new Token<T>(t.Item2, c);
					}
				}

				_pos++;

				if (!captureUndef)
				{
					var bad = Stringe.Slice(u, _pos);
					throw new InvalidOperationException(string.Concat("(Ln ", bad.Line, ", Col ", bad.Column, ") Invalid token '", bad,
						"'"));
				}

				// ReSharper disable once LoopVariableIsNeverChangedInsideLoop
				// ReSharper disable once ConditionIsAlwaysTrueOrFalse
			} while (captureUndef);

			return null;
		}
	}
}