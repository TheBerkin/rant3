#region License

// https://github.com/TheBerkin/Rant
// 
// Copyright (c) 2017 Nicholas Fleck
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using Rant.Core.Formatting;
using Rant.Core.IO;
using Rant.Core.Utilities;
using Rant.Formats;

namespace Rant.Core.Output
{
    internal class OutputChainBuffer
    {
        private const int InitialCapacity = 256;

        private static readonly HashSet<char> _wordSepChars
            = new HashSet<char>(new[] { ' ', '\r', '\n', '\t', '\f', '\v', '\'', '\"', '/', '-' });

        private static readonly HashSet<char> _sentenceTerminators
            = new HashSet<char>(new[] { '.', '?', '!' });

        protected readonly StringBuilder _buffer;
        private readonly Sandbox _sandbox;
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
            _sandbox = sb;
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
            _sandbox = sb;
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
            string str;
            if (IOUtil.IsNumericType(value.GetType()))
            {
                double num = Convert.ToDouble(value);
                str = NumberFormatter.FormatNumber(num);
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                _sandbox.SetPlural(num != 1.0);
            }
            else
                str = value.ToString();

            Format(ref str);
            _buffer.Append(str);
            PrintedSinceCapsChange = true;
            Prev?.OnNextBufferChange();
            Next?.OnPrevBufferChange();
            UpdateSize();
        }

        private void UpdateSize()
        {
            if (_sandbox.SizeLimit.Accumulate(_buffer.Length - oldSize))
                throw new InvalidOperationException($"Exceeded character limit ({_sandbox.SizeLimit.Maximum})");
            oldSize = _buffer.Length;
        }

        public void Clear()
        {
#if UNITY
			_buffer.Length = 0;
#else
            _buffer.Clear();
#endif
            _sandbox.SizeLimit.Accumulate(-oldSize);
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
                    if (char.IsWhiteSpace(lastChar) || _wordSepChars.Contains(lastChar) || lastChar == '\0')
                        CapitalizeFirstLetter(ref value);
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
                        if (Prev == null || Prev.Prev == null && Prev.Length == 0)
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
                            if (_sentenceTerminators.Contains(b[i])) break;
                            if (i == 0)
                                CapitalizeFirstLetter(ref value);
                        }
                    }

                    // Scan buffer end to determine if capitalization is needed
                    for (int i = b.Length - 1; i >= 0; i--)
                    {
                        if (char.IsLetterOrDigit(b[i])) break;
                        if (!_sentenceTerminators.Contains(b[i])) continue;
                        CapitalizeFirstLetter(ref value);
                        break;
                    }
                }
                    break;
                case Capitalization.Title:
                {
                    CapitalizeTitleString(ref value, _sandbox.Format, !PrintedSinceCapsChange);
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
                    if (_sentenceTerminators.Contains(c)) capitalize = true;
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

        protected static bool CapitalizeTitleString(ref string value, RantFormat format, bool capitalizeFirstLetter)
        {
            if (Util.IsNullOrWhiteSpace(value)) return false;
            var wordBuffer = new StringBuilder(32);
            var titleBuffer = new StringBuilder(value.Length);
            bool first = true;
            foreach (char c in value)
            {
                if (char.IsWhiteSpace(c))
                {
                    if (wordBuffer.Length > 0)
                    {
                        if (first && capitalizeFirstLetter || !format.Excludes(wordBuffer.ToString()))
                            wordBuffer[0] = char.ToUpperInvariant(wordBuffer[0]);
                        first = false;
                        titleBuffer.Append(wordBuffer);
                        wordBuffer.Length = 0;
                    }
                    titleBuffer.Append(c);
                }
                else
                    wordBuffer.Append(c);
            }
            if (wordBuffer.Length > 0)
            {
                if (first && capitalizeFirstLetter || !format.Excludes(wordBuffer.ToString()))
                    wordBuffer[0] = char.ToUpperInvariant(wordBuffer[0]);
                titleBuffer.Append(wordBuffer);
            }
            value = titleBuffer.ToString();
            return true;
        }
    }
}