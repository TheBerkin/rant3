using System;
using System.Collections.Generic;
using System.Text;

using Rant.Engine.Formatters;
using Rant.Formats;
using Rant.IO;

namespace Rant.Engine
{
    /// <summary>
    /// Stores output from a pattern channel.
    /// </summary>
    internal sealed class Channel
    {
        internal const int InitialBufferSize = 512;

        private RantFormat _format;
        private StringBuilder _currentBuffer;
        private readonly List<StringBuilder> _buffers;
        private readonly Dictionary<string, StringBuilder> _backPrintPoints = new Dictionary<string, StringBuilder>();
        private readonly Dictionary<string, StringBuilder> _forePrintPoints = new Dictionary<string, StringBuilder>();
        private readonly Dictionary<StringBuilder, _<StringBuilder, OutputFormatter>> _articleConverters = new Dictionary<StringBuilder, _<StringBuilder, OutputFormatter>>();
	    private readonly Limit _limit;
        private readonly OutputFormatter _outputFormatter = new OutputFormatter();
	    private readonly NumberFormatter _numberFormatter = new NumberFormatter();

        private int _bufferCount;
        private int _length;
	    private bool _articles;

        /// <summary>
        /// The name of the channel.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The visibility of the channel.
        /// </summary>
        public ChannelVisibility Visiblity { get; internal set; }

        internal OutputFormatter OutputFormatter => _outputFormatter;

	    internal NumberFormatter NumberFormatter => _numberFormatter;

        internal RantFormat Format
        {
            get { return _format; }
            set { _format = value; }
        }

        internal Channel(string name, ChannelVisibility visibility, RantFormat format, Limit limit)
        {
            Name = name;
            Visiblity = visibility;
            _currentBuffer = new StringBuilder(InitialBufferSize);
            _buffers = new List<StringBuilder>{_currentBuffer};
            _format = format;
	        _articles = false;
	        _limit = limit;
        }

		/// <summary>
		/// Writes a value to the buffer.
		/// </summary>
		/// <param name="value">The value to print.</param>
        internal void Write(string value)
        {
			if (value == null) return;
			if (_limit.Accumulate(value.Length))
				throw new InvalidOperationException($"Exceeded character limit ({_limit.Maximum})");
            _length += value.Length;
            _currentBuffer.Append(_outputFormatter.Format(value, _format));
            if (_articles) UpdateArticle(_currentBuffer);
        }

		/// <summary>
		/// Writes a value to the buffer and applies the current number formatting if the value is a numeric type.
		/// </summary>
		/// <param name="value">The value to print.</param>
		internal void Write(object value)
		{
			if (value == null) return;
			var str = IOUtil.IsNumericType(value.GetType())
				? _numberFormatter.FormatNumber(Convert.ToDouble(value))
				: Convert.ToString(value, _format.Culture);
			if (_limit.Accumulate(str.Length))
				throw new InvalidOperationException($"Exceeded character limit ({_limit.Maximum})");
			_length += str.Length;
			_currentBuffer.Append(_outputFormatter.Format(str, _format));
			UpdateArticle(_currentBuffer);
		}

	    internal void WriteBuffer(StringBuilder buffer)
	    {
		    if (buffer == null) return;
			if (_limit.Accumulate(buffer.Length))
				throw new InvalidOperationException($"Exceeded character limit ({_limit.Maximum})");
		    _length += buffer.Length;
			_buffers.Add(buffer);
		    _buffers.Add(_currentBuffer = new StringBuilder());
	    }

		internal void WriteArticle()
		{
			_articles = true;
            var anBuilder = _.Create(new StringBuilder(_outputFormatter.Format(_format.IndefiniteArticles.ConsonantForm, _format, OutputFormatterOptions.NoUpdate | OutputFormatterOptions.IsArticle)), _outputFormatter.Clone());
            var afterBuilder = _currentBuffer = new StringBuilder();
            _articleConverters[afterBuilder] = anBuilder;
            _buffers.Add(anBuilder.Item1);
            _buffers.Add(afterBuilder);
            _bufferCount += 2;
            _length += anBuilder.Item1.Length;
        }

        private void UpdateArticle(StringBuilder target)
        {
            _<StringBuilder, OutputFormatter> aBuilder;
            if (!_articleConverters.TryGetValue(target, out aBuilder)) return;
            int l1 = aBuilder.Item1.Length;
            if (target.Length == 0) // Clear to "a" if the after-buffer is empty
            {
                aBuilder.Item1.Length = 0;
                aBuilder.Item1.Append(aBuilder.Item2.Format(_format.IndefiniteArticles.ConsonantForm, _format, OutputFormatterOptions.NoUpdate | OutputFormatterOptions.IsArticle));
                _length += -l1 + aBuilder.Item1.Length;
                return;
            }

            // Check for vowel
            if (!_format.IndefiniteArticles.PrecedesVowel(target)) return;
            aBuilder.Item1.Length = 0;
            aBuilder.Item1.Append(aBuilder.Item2.Format(_format.IndefiniteArticles.VowelForm, _format, OutputFormatterOptions.NoUpdate | OutputFormatterOptions.IsArticle));
            _length += -l1 + aBuilder.Item1.Length;
        }

        internal void ClearTarget(string name)
        {
            StringBuilder sb;
            if (_backPrintPoints.TryGetValue(name, out sb) || _forePrintPoints.TryGetValue(name, out sb))
            {
                sb.Length = 0;
            }
        }

        internal void WriteToTarget(string name, string value, bool overwrite = false)
        {
            StringBuilder sb;
            if (!_backPrintPoints.TryGetValue(name, out sb))
            {
                sb = _forePrintPoints[name] = new StringBuilder(InitialBufferSize);
                if (overwrite) sb.Length = 0;
                sb.Append(_outputFormatter.Format(value, _format));
            }
            else
            {
                if (overwrite) sb.Length = 0;
                sb.Append(_outputFormatter.Format(value, _format));
                UpdateArticle(sb);
            }
        }

        internal void CreateTarget(string name)
        {
            StringBuilder sb;
            if (_forePrintPoints.TryGetValue(name, out sb))
            {
                _buffers.Add(sb);
                _bufferCount++;
            }
            else
            {
                if (!_backPrintPoints.TryGetValue(name, out sb))
                {
                    sb = _backPrintPoints[name] = new StringBuilder(InitialBufferSize);
                }
                _buffers.Add(sb);
                _bufferCount++;
            }

            _buffers.Add(_currentBuffer = new StringBuilder(InitialBufferSize));
            _bufferCount++;
        }

        internal int MeasureDistance(int bufIndexA, int bufIndexB, int bufCharA, int bufCharB)
        {
            int ia = Math.Min(bufIndexA, bufIndexB);
            int ib = Math.Max(bufIndexA, bufIndexB);
            int len = bufCharB;
            for (int i = ia; i < ib; i++)
            {
                len += _buffers[i].Length;
            }
            return len - bufCharA;
        }

        internal string CopyRegion(int bufIndexA, int bufIndexB, int bufCharA, int bufCharB)
        {
            int ia = Math.Min(bufIndexA, bufIndexB);
            int ib = Math.Max(bufIndexA, bufIndexB);
            if (ia == ib) return _buffers[ia].ToString().Substring(bufCharA, bufCharB - bufCharA);
            var sb = new StringBuilder();
            for (int i = ia; i <= ib; i++)
            {
                if (i == ia)
                {
                    sb.Append(_buffers[i].ToString().Substring(bufCharA));
                }
                else if (i == ib)
                {
                    sb.Append(_buffers[i].ToString().Substring(0, bufCharB));
                }
                else
                {
                    sb.Append(_buffers[i]);
                }
            }
            return sb.ToString();
        }

        internal int CurrentBufferIndex => _bufferCount;

        internal int CurrentBufferLength => _currentBuffer.Length;

        /// <summary>
        /// The number of characters in the output.
        /// </summary>
        public int Length => _length;

        /// <summary>
        /// The output string stored in the channel.
        /// </summary>
        public string Value
        {
            get
            {
                var sb = new StringBuilder(InitialBufferSize);
                foreach (var b in _buffers)
                {
                    sb.Append(b);
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Returns a string that identifies the channel by name and visibility.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{Name} ({Visiblity})";
    }
}