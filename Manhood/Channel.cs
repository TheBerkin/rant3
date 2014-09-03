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

        internal StringBuilder Buffer { get; set; }

        internal const int InitialBufferSize = 512;

        internal Channel(string name, ChannelVisibility visibility)
        {
            Name = name;
            Visiblity = visibility;
            Buffer = new StringBuilder(InitialBufferSize);
        }

        /// <summary>
        /// The output stored in the channel.
        /// </summary>
        public string Output
        {
            get { return Buffer.ToString(); }
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