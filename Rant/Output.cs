using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rant
{
    /// <summary>
    /// Represents a collection of output channel values returned from Rant.
    /// </summary>
    public sealed class Output : IEnumerable<Channel>
    {
        private readonly Dictionary<string, Channel> _channels;
        private readonly List<Channel> _channelArray;

        internal Output(Dictionary<string, Channel> channels)
        {
            _channels = channels;
            _channelArray = channels.Values.ToList();
        }

        /// <summary>
        /// Retrieves the channel with the specified name.
        /// </summary>
        /// <param name="index">The name of the channel.</param>
        /// <returns></returns>
        public Channel this[string index]
        {
            get
            {
                Channel chan;
                return _channels.TryGetValue(index, out chan) ? chan : new Channel("", ChannelVisibility.Public);
            }
        }

        /// <summary>
        /// The main output channel.
        /// </summary>
        public string MainValue
        {
            get { return this["main"].Value; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the channels in the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Channel> GetEnumerator()
        {
            return _channelArray.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _channels.GetEnumerator();
        }

        /// <summary>
        /// Returns the output from the "main" channel.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return MainValue;
        }

        /// <summary>
        /// Returns the output from the "main" channel.
        /// </summary>
        /// <returns></returns>
        public static implicit operator string(Output output)
        {
            return output.MainValue;
        }
    }
}