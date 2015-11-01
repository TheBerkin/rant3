using System.Collections.Generic;
using System.Linq;

using Rant.Engine.Formatters;
using Rant.Formats;

namespace Rant.Engine
{
    internal class ChannelWriter
    {
        private readonly List<Channel> _stack;
        private readonly Dictionary<string, Channel> _channels;
        private readonly Channel _main;
        private readonly Limit _sizeLimit;

        private int _stackSize;

        public ChannelWriter(RantFormat formatStyle, Limit sizeLimit)
        {
            _sizeLimit = sizeLimit;
            _main = new Channel("main", ChannelVisibility.Public, formatStyle, _sizeLimit);

            _stack = new List<Channel> { _main };
            _stackSize = 1;

            _channels = new Dictionary<string, Channel>
            {
                { "main", _main }
            };
        }

        public Channel GetActiveChannel() => _stack.Last();

        public void CreateTarget(string name)
        {
            foreach (var ch in GetActive())
            {
                ch.CreateTarget(name);
            }
        }

        public void WriteToTarget(string name, string value, bool overwrite = false)
        {
            foreach (var ch in GetActive())
            {
                ch.WriteToTarget(name, value, overwrite);
            }
        }

        public void ClearTarget(string name)
        {
            foreach (var ch in GetActive())
            {
                ch.ClearTarget(name);
            }
        }

        public string GetOutputFor(string channelName)
        {
            Channel channel;
            return !_channels.TryGetValue(channelName, out channel) ? "" : channel.Value;
        }

        public void OpenChannel(string channelName, ChannelVisibility visibility, RantFormat formatStyle)
        {   
            Channel ch;
            if (!_channels.TryGetValue(channelName, out ch))
            {
                ch = new Channel(channelName, visibility, formatStyle, _sizeLimit);
                _channels[channelName] = ch;
            }

            if (channelName == "main") return;

            ch.Visiblity = visibility;

            if (_stack.Contains(ch)) return;
            _stack.Add(ch);
            _stackSize++;
        }

        public void CloseChannel(string channelName)
        {
            if (channelName == "main") return;

            Channel ch;
            if (!_channels.TryGetValue(channelName, out ch)) return;
            _stack.Remove(ch);
            _stackSize--;
        }

        public void SetCase(Case caps)
        {
            foreach (var ch in GetActive())
            {
                ch.OutputFormatter.Case = caps;
            }
        }

        public Dictionary<Channel, Case> GetCurrentCases()
        {
            var table = new Dictionary<Channel, Case>();
            foreach (var ch in GetActive())
            {
                table[ch] = ch.OutputFormatter.Case;
            }
            return table;
        } 

        public void Write(string input)
        {
	        if (input == null) return;
            foreach (var ch in GetActive())
            {
                ch.Write(input);
            }
        }

		public void Write(object input)
		{
			if (input == null) return;
			foreach (var ch in GetActive())
			{
				ch.Write(input);
			}
		}

		public IEnumerable<Channel> GetActive()
        {
            var lastVisibility = ChannelVisibility.Public;
            bool p = false;

            for (int i = _stackSize - 1; i >= 0; i--)
            {
                switch (_stack[i].Visiblity)
                {
                    case ChannelVisibility.Public:
                        if (lastVisibility == ChannelVisibility.Internal)
                        {
                            if (p) yield return _main;
                            yield break;
                        }
                        p = true;
                        break;
                    case ChannelVisibility.Private:
                        yield return _stack[i];
                        if (p) yield return _main;
                        yield break;
                    case ChannelVisibility.Internal:
                        break;
                }

                lastVisibility = _stack[i].Visiblity;
                yield return _stack[i];
            }
        }

        public Dictionary<string, Channel> Channels => _channels;
    }
}