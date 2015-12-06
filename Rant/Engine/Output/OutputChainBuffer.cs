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
			= new HashSet<char>(new[] { ' ', '\r', '\n', '\t', '\f', '\v', '\'', '"', '/', '-' });
		private static readonly HashSet<char> sentenceTerminators
			= new HashSet<char>(new[] { '.', '?', '!' });

		private readonly Sandbox sandbox;
		protected readonly StringBuilder _buffer;
		private readonly OutputChainBuffer _prevItem;
		private OutputChainBuffer _nextItem;

		private readonly bool _isTarget = false;
		private readonly NumberFormatter _numberFormatter = new NumberFormatter();
		private Capitalization _caps = Capitalization.None;
		// Determines if a print took place after capitalization was changed
		private bool _printedSinceCapsChange = false;
		// Size of the buffer before the last print
		private int oldSize;

		public StringBuilder Buffer => _buffer;

		public Capitalization Caps
		{
			get { return _caps; }
			set
			{
				_printedSinceCapsChange = false;
				_caps = value;
			}
		}

		public bool IsTarget => _isTarget;

		protected bool PrintedSinceCapsChange => _printedSinceCapsChange;

		public OutputChainBuffer(Sandbox sb, OutputChainBuffer prev)
		{
			_prevItem = prev;

			if (prev != null)
			{
				prev._nextItem = this;
				_caps = prev is OutputChainArticleBuffer && prev.Caps == Capitalization.First ? Capitalization.None : prev._caps;
				_numberFormatter.BinaryFormat = prev.NumberFormatter.BinaryFormat;
				_numberFormatter.BinaryFormatDigits = prev.NumberFormatter.BinaryFormatDigits;
				_numberFormatter.Endianness = prev.NumberFormatter.Endianness;
				_numberFormatter.NumberFormat = prev.NumberFormatter.NumberFormat;
			}

			_isTarget = true;
			_buffer = new StringBuilder(InitialCapacity);
			sandbox = sb;
		}

		public OutputChainBuffer(Sandbox sb, OutputChainBuffer prev, OutputChainBuffer targetOrigin)
		{
			_prevItem = prev;

			if (prev != null)
			{
				prev._nextItem = this;
				_caps = prev is OutputChainArticleBuffer && prev.Caps == Capitalization.First ? Capitalization.None : prev._caps;
			}

			_buffer = targetOrigin._buffer;
			sandbox = sb;
		}

		public NumberFormatter NumberFormatter => _numberFormatter;

		public OutputChainBuffer Next => _nextItem;

		public OutputChainBuffer Prev => _prevItem;

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
			if (String.IsNullOrEmpty(value)) return;
			Format(ref value);
			_buffer.Append(value);
			_printedSinceCapsChange = true;
			_prevItem?.OnNextBufferChange();
			_nextItem?.OnPrevBufferChange();
			UpdateSize();
		}

		public void Print(object value)
		{
			if (IOUtil.IsNumericType(value.GetType()))
			{
				_buffer.Append(_numberFormatter.FormatNumber(Convert.ToDouble(value)));
				return;
			}
			var str = value.ToString();
			Format(ref str);
			_buffer.Append(str);
			_printedSinceCapsChange = true;
			_prevItem?.OnNextBufferChange();
			_nextItem?.OnPrevBufferChange();
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
							: _prevItem?.LastChar ?? '\0';
						if (Char.IsWhiteSpace(lastChar) || wordSepChars.Contains(lastChar) || lastChar == '\0')
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
							if (_prevItem == null || (_prevItem.Prev == null && _prevItem.Length == 0))
							{
								CapitalizeFirstLetter(ref value);
								break;
							}

							// If there is a previous buffer, scan the end.
							b = _prevItem._buffer;
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
						char lastChar = _buffer.Length > 0
								? _buffer[_buffer.Length - 1]
								: _prevItem?.LastChar ?? '\0';
						bool boundary = Char.IsWhiteSpace(lastChar)
							|| Char.IsSeparator(lastChar)
							|| lastChar == '\0';

						// This ensures that the first title word is always capitalized
						if (!_printedSinceCapsChange)
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
				if (capitalize && Char.IsLetter(c))
				{
					sb.Append(Char.ToUpperInvariant(c));
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