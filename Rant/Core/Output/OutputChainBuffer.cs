using System;
using System.Collections.Generic;
using System.Text;

using Rant.Core.Formatting;
using Rant.Core.IO;
using Rant.Core.Utilities;

namespace Rant.Core.Output
{
	internal class OutputChainBuffer
	{
		private const int InitialCapacity = 256;

		private static readonly HashSet<char> wordSepChars
			= new HashSet<char>(new[] { ' ', '\r', '\n', '\t', '\f', '\v', '\'', '\"', '/', '-' });

		private static readonly HashSet<char> sentenceTerminators
			= new HashSet<char>(new[] { '.', '?', '!' });

		protected readonly StringBuilder _buffer;
		private readonly Sandbox sandbox;
		private Capitalization _caps = Capitalization.None;
		// Determines if a print took place after capitalization was changed
		// Size of the buffer before the last print
		private int oldSize;

		public OutputChainBuffer(Sandbox sb, OutputChainBuffer prev)
		{
			Prev = prev;

			if (prev != null)
			{
				prev.Next = this;
				_caps = prev is OutputChainArticleBuffer && prev.Caps == Capitalization.First ? Capitalization.None : prev._caps;
				NumberFormatter.BinaryFormat = prev.NumberFormatter.BinaryFormat;
				NumberFormatter.BinaryFormatDigits = prev.NumberFormatter.BinaryFormatDigits;
				NumberFormatter.Endianness = prev.NumberFormatter.Endianness;
				NumberFormatter.NumberFormat = prev.NumberFormatter.NumberFormat;
			}

			IsTarget = true;
			_buffer = new StringBuilder(InitialCapacity);
			sandbox = sb;
		}

		public OutputChainBuffer(Sandbox sb, OutputChainBuffer prev, OutputChainBuffer targetOrigin)
		{
			Prev = prev;

			if (prev != null)
			{
				prev.Next = this;
				_caps = prev is OutputChainArticleBuffer && prev.Caps == Capitalization.First ? Capitalization.None : prev._caps;
			}

			_buffer = targetOrigin._buffer;
			sandbox = sb;
		}

		public StringBuilder Buffer => _buffer;

		public Capitalization Caps
		{
			get { return _caps; }
			set
			{
				PrintedSinceCapsChange = false;
				_caps = value;
			}
		}

		public bool IsTarget { get; } = false;
		protected bool PrintedSinceCapsChange { get; private set; } = false;
		public NumberFormatter NumberFormatter { get; } = new NumberFormatter();
		public OutputChainBuffer Next { get; private set; }
		public OutputChainBuffer Prev { get; }
		public char LastChar => _buffer.Length > 0 ? _buffer[_buffer.Length - 1] : '\0';
		public char FirstChar => _buffer.Length > 0 ? _buffer[0] : '\0';
		public int Length => _buffer.Length;

		protected virtual void OnPrevBufferChange()
		{
		}

		protected virtual void OnNextBufferChange()
		{
		}

		public void Print(string value)
		{
			if (string.IsNullOrEmpty(value)) return;
			Format(ref value);
			_buffer.Append(value);
			PrintedSinceCapsChange = true;
			Prev?.OnNextBufferChange();
			Next?.OnPrevBufferChange();
			UpdateSize();
		}

		public void Print(object value)
		{
			if (IOUtil.IsNumericType(value.GetType()))
			{
				_buffer.Append(NumberFormatter.FormatNumber(Convert.ToDouble(value)));
				return;
			}
			string str = value.ToString();
			Format(ref str);
			_buffer.Append(str);
			PrintedSinceCapsChange = true;
			Prev?.OnNextBufferChange();
			Next?.OnPrevBufferChange();
			UpdateSize();
		}

		private void UpdateSize()
		{
			if (sandbox.SizeLimit.Accumulate(_buffer.Length - oldSize))
				throw new InvalidOperationException($"Exceeded character limit ({sandbox.SizeLimit.Maximum})");
			oldSize = _buffer.Length;
		}

		public void Clear()
		{
#if UNITY
			_buffer.Length = 0;
#else
			_buffer.Clear();
#endif
			sandbox.SizeLimit.Accumulate(-oldSize);
			oldSize = 0;
		}

		public override string ToString() => _buffer.ToString();

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
					char lastChar = _buffer.Length > 0
						? _buffer[_buffer.Length - 1]
						: Prev?.LastChar ?? '\0';
					if (char.IsWhiteSpace(lastChar) || wordSepChars.Contains(lastChar) || lastChar == '\0')
					{
						CapitalizeFirstLetter(ref value);
					}
				}
					break;
				case Capitalization.Sentence:
				{
					var b = _buffer;

					// Capitalize sentences in input value
					CapitalizeSentences(ref value);

					// If the buffer's empty, check previous buffer
					if (_buffer.Length == 0)
					{
						// Check if we're at the start
						if (Prev == null || (Prev.Prev == null && Prev.Length == 0))
						{
							CapitalizeFirstLetter(ref value);
							break;
						}
						// If there is a previous buffer, scan the end.
						b = Prev._buffer;
					}
					else if (Prev == null || Prev.Length == 0)
					{
						for (int i = b.Length - 1; i >= 0; i--)
						{
							if (char.IsLetterOrDigit(b[i])) break;
							if (sentenceTerminators.Contains(b[i])) break;
							if (i == 0)
								CapitalizeFirstLetter(ref value);
						}
					}

					// Scan buffer end to determine if capitalization is needed
					for (int i = b.Length - 1; i >= 0; i--)
					{
						if (char.IsLetterOrDigit(b[i])) break;
						if (!sentenceTerminators.Contains(b[i])) continue;
						CapitalizeFirstLetter(ref value);
						break;
					}
				}
					break;
				case Capitalization.Title:
				{
					char lastChar = _buffer.Length > 0
						? _buffer[_buffer.Length - 1]
						: Prev?.LastChar ?? '\0';
					bool boundary = char.IsWhiteSpace(lastChar)
					                || char.IsSeparator(lastChar)
					                || lastChar == '\0';

					// This ensures that the first title word is always capitalized
					if (!PrintedSinceCapsChange)
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
					if (CapitalizeFirstLetter(ref value) && !(this is OutputChainArticleBuffer)) _caps = Capitalization.None;
					break;
			}
		}

		protected static void CapitalizeSentences(ref string value)
		{
			var sb = new StringBuilder();
			bool capitalize = false;
			foreach (char c in value)
			{
				if (capitalize && char.IsLetter(c))
				{
					sb.Append(char.ToUpperInvariant(c));
					capitalize = false;
				}
				else
				{
					if (sentenceTerminators.Contains(c)) capitalize = true;
					sb.Append(c);
				}
			}
			value = sb.ToString();
		}

		protected static bool CapitalizeFirstLetter(ref string value)
		{
			for (int i = 0; i < value.Length; i++)
			{
				if (!char.IsLetter(value[i])) continue;
				var sb = new StringBuilder();
				sb.Append(value.Substring(0, i));
				sb.Append(char.ToUpperInvariant(value[i]));
				sb.Append(value.Substring(i + 1));
				value = sb.ToString();
				return true;
			}
			return false;
		}
	}
}