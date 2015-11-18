using System;
using System.Collections.Generic;
using System.Text;

using Rant.Engine.Formatters;
using Rant.IO;

namespace Rant.Engine.Output
{
	internal class OutputChainBuffer
	{
		private const int InitialCapacity = 256;
		private static readonly HashSet<char> wordSepChars
			= new HashSet<char>(new[] { ' ', '\r', '\n', '\t', '\f', '\v', '\'', '"', '/' });
		private static readonly HashSet<char> sentenceTerminators
			= new HashSet<char>(new[] { '.', '?', '!' });

		private readonly Sandbox sandbox;
		private readonly OutputChainBuffer _prevItem;
		private OutputChainBuffer _nextItem;
		private readonly NumberFormatter _numberFormatter = new NumberFormatter();
		private Capitalization _caps = Capitalization.None;
		private bool printedSinceCapsChange = false;
		private int oldSize;
		protected readonly StringBuilder buffer;

		public Capitalization Caps
		{
			get { return _caps; }
			set
			{
				printedSinceCapsChange = false;
				_caps = value;
			}
		}

		public OutputChainBuffer(Sandbox sb, OutputChainBuffer prev)
		{
			_prevItem = prev;

			if (prev != null)
			{
				prev._nextItem = this;
				_caps = prev._caps;
			}

			buffer = new StringBuilder(InitialCapacity);
			sandbox = sb;
		}

		public OutputChainBuffer(Sandbox sb, OutputChainBuffer prev, OutputChainBuffer targetOrigin)
		{
			_prevItem = prev;

			if (prev != null)
			{
				prev._nextItem = this;
				_caps = prev._caps;
			}

			buffer = targetOrigin.buffer;
			sandbox = sb;
		}

		public NumberFormatter NumberFormatter => _numberFormatter;

		public OutputChainBuffer Next => _nextItem;

		public OutputChainBuffer Prev => _prevItem;

		public char LastChar => buffer.Length > 0 ? buffer[buffer.Length - 1] : '\0';

		public char FirstChar => buffer.Length > 0 ? buffer[0] : '\0';

		public int Length => buffer.Length;

		protected virtual void OnPrevBufferChange()
		{
		}

		protected virtual void OnNextBufferChange()
		{
		}

		public void Print(string value)
		{
			if (String.IsNullOrEmpty(value)) return;
			Format(ref value);
			buffer.Append(value);
			printedSinceCapsChange = true;
			_prevItem?.OnNextBufferChange();
			_nextItem?.OnPrevBufferChange();
			UpdateSize();
		}

		public void Print(object value)
		{
			if (IOUtil.IsNumericType(value.GetType()))
			{
				buffer.Append(_numberFormatter.FormatNumber(Convert.ToDouble(value)));
				return;
			}
			var str = value.ToString();
			Format(ref str);
			buffer.Append(str);
			printedSinceCapsChange = true;
			_prevItem?.OnNextBufferChange();
			_nextItem?.OnPrevBufferChange();
			UpdateSize();
		}

		private void UpdateSize()
		{
			if (sandbox.SizeLimit.Accumulate(buffer.Length - oldSize))
				throw new InvalidOperationException($"Exceeded character limit ({sandbox.SizeLimit.Maximum})");
			oldSize = buffer.Length;
		}

		public void Clear()
		{
#if UNITY
			_buffer.Length = 0;
#else
			buffer.Clear();
#endif
		}

		public override string ToString() => buffer.ToString();

		protected void Format(ref string value)
		{
			if (Util.IsNullOrWhiteSpace(value)) return;

			switch (_caps)
			{
				case Capitalization.Upper:
					value = value.ToUpperInvariant();
					break;
				case Capitalization.Lower:
					value = value.ToLowerInvariant();
					break;
				case Capitalization.Word:
					{
						char lastChar = buffer.Length > 0
							? buffer[buffer.Length - 1]
							: _prevItem?.LastChar ?? '\0';
						if (Char.IsWhiteSpace(lastChar) || wordSepChars.Contains(lastChar) || lastChar == '\0')
						{
							CapitalizeFirstLetter(ref value);
						}
					}
					break;
				case Capitalization.Sentence:
					{
						var b = buffer;

						// Capitalize sentences in input value
						CapitalizeSentences(ref value);

						// If the buffer's empty, check previous buffer
						if (buffer.Length == 0)
						{
							// If the prev buffer is null, it's the very start.
							if (_prevItem == null)
							{
								CapitalizeFirstLetter(ref value);
								break;
							}

							// If there is a previous buffer, scan the end.
							b = _prevItem.buffer;
						}

						// Scan buffer end to determine if capitalization is needed
						for (int i = b.Length - 1; i >= 0; i--)
						{
							if (Char.IsLetterOrDigit(b[i])) break;
							if (!sentenceTerminators.Contains(b[i])) continue;
							CapitalizeFirstLetter(ref value);
							break;
						}
					}
					break;
				case Capitalization.Title:
					{
						char lastChar = buffer.Length > 0
								? buffer[buffer.Length - 1]
								: _prevItem?.LastChar ?? '\0';
						bool boundary = Char.IsWhiteSpace(lastChar)
							|| Char.IsSeparator(lastChar)
							|| lastChar == '\0';

						// Since the lexer splits text by words, this is easy.
						if (!printedSinceCapsChange)
						{
							CapitalizeFirstLetter(ref value);
							return;
						}
						// If it's not capitalizable in a title, skip it.
						if (sandbox.Format.Excludes(value) && boundary) return;

						CapitalizeFirstLetter(ref value);
					}
					break;
				case Capitalization.First:
					if (CapitalizeFirstLetter(ref value)) _caps = Capitalization.None;
					break;
			}
		}

		protected static void CapitalizeSentences(ref string value)
		{
			var sb = new StringBuilder();
			bool capitalize = false;
			foreach (char c in value)
			{
				if (capitalize && Char.IsLetter(c))
				{
					sb.Append(Char.ToUpperInvariant(c));
					capitalize = false;
				}
				else
				{
					if (sentenceTerminators.Contains(c))
					{
						capitalize = true;
					}
					sb.Append(c);
				}
			}
			value = sb.ToString();
		}

		protected static bool CapitalizeFirstLetter(ref string value)
		{
			for (int i = 0; i < value.Length; i++)
			{
				if (!Char.IsLetter(value[i])) continue;
				var sb = new StringBuilder();
				sb.Append(value.Substring(0, i));
				sb.Append(Char.ToUpperInvariant(value[i]));
				sb.Append(value.Substring(i + 1));
				value = sb.ToString();
				return true;
			}
			return false;
		}
	}
}