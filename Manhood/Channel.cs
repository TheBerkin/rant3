using System;
using System.Text;

namespace Manhood
{
    /// <summary>
    /// Stores output from a pattern channel.
    /// </summary>
    public class Channel
    {
        /// <summary>
        /// The name of the channel.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The visibility of the channel.
        /// </summary>
        public ChannelVisibility Visiblity { get; internal set; }

        private readonly StringBuilder _buffer = new StringBuilder(InitialBufferSize);

        private char _lastChar = ' ';
        private Capitalization _caps = Capitalization.None;

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

        internal void Write(string value)
        {
            _buffer.Append(Util.Capitalize(value, ref _caps, ref _lastChar));
        }

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
        /// Returns a string representation of the channel.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Concat(Name, " (", Visiblity, ")");
        }
    }
}