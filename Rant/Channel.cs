using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rant
{
    /// <summary>
    /// Stores output from a pattern channel.
    /// </summary>
    public sealed class Channel
    {
        private StringBuilder _currentBuffer;
        private readonly List<StringBuilder> _buffers;
        private readonly Dictionary<string, StringBuilder> _backPrintPoints = new Dictionary<string, StringBuilder>();
        private readonly Dictionary<string, StringBuilder> _forePrintPoints = new Dictionary<string, StringBuilder>();

        private int _length = 0;

        private char _lastChar = ' ';
        private Capitalization _caps = Capitalization.None;

        /// <summary>
        /// The name of the channel.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The visibility of the channel.
        /// </summary>
        public ChannelVisibility Visiblity { get; internal set; }

        internal Capitalization Capitalization
        {
            get { return _caps; }
            set { _caps = value; }
        }

        internal const int InitialBufferSize = 512;

        internal Channel(string name, ChannelVisibility visibility)
        {
            Name = name;
            Visiblity = visibility;
            _currentBuffer = new StringBuilder(InitialBufferSize);
            _buffers = new List<StringBuilder>{_currentBuffer};
        }

        internal void Write(string value)
        {
            _length += value.Length;
            _currentBuffer.Append(Util.Capitalize(value, ref _caps, ref _lastChar));
        }

        internal void WriteToPoint(string name, string value)
        {
            StringBuilder sb;
            if (!_backPrintPoints.TryGetValue(name, out sb))
            {
                _forePrintPoints[name] = new StringBuilder(InitialBufferSize).Append(Util.Capitalize(value, ref _caps, ref _lastChar));
            }
            else
            {
                sb.Append(Util.Capitalize(value, ref _caps, ref _lastChar));
            }
        }

        internal void CreateNamedWritePoint(string name)
        {
            StringBuilder sp;
            if (_forePrintPoints.TryGetValue(name, out sp))
            {
                _buffers.Add(sp);
                _forePrintPoints.Remove(name);
            }
            else
            {
                StringBuilder sb;
                if (!_backPrintPoints.TryGetValue(name, out sb))
                {
                    sb = _backPrintPoints[name] = new StringBuilder(InitialBufferSize);
                }
                _buffers.Add(sb);
            }

            _buffers.Add(_currentBuffer = new StringBuilder(InitialBufferSize));
        }

        /// <summary>
        /// The number of characters in the output.
        /// </summary>
        public int Length
        {
            get { return _length; }
        }

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
        public override string ToString()
        {
            return String.Concat(Name, " (", Visiblity, ")");
        }
    }
}