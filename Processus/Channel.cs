using System;
using System.Collections.Generic;
using System.Text;

namespace Processus
{
    /// <summary>
    /// Stores output from a pattern channel.
    /// </summary>
    public sealed class Channel
    {
        private readonly StringBuilder _buffer = new StringBuilder(InitialBufferSize);

        private char _lastChar = ' ';
        private Capitalization _caps = Capitalization.None;

        private readonly Dictionary<string, int> _markers = new Dictionary<string, int>();

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
        }

        internal void SetMarker(string name)
        {
            _markers[name] = _buffer.Length;
        }

        internal int GetMarkerPos(string name)
        {
            int i;
            return _markers.TryGetValue(name, out i) ? i : 0;
        }

        internal void Write(string value)
        {
            _buffer.Append(Util.Capitalize(value, ref _caps, ref _lastChar));
        }

        /// <summary>
        /// The number of characters in the output.
        /// </summary>
        public int Length
        {
            get { return _buffer.Length; }
        }

        /// <summary>
        /// The output string stored in the channel.
        /// </summary>
        public string Value
        {
            get { return _buffer.ToString(); }
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