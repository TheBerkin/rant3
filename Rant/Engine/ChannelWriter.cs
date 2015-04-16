using System.Collections.Generic;
using System.Text;

using Rant.Engine.Formatters;
using Rant.Formats;

namespace Rant.Engine
{
    internal class ChannelWriter
    {
        private readonly List<RantChannel> _stack;
        private readonly Dictionary<string, RantChannel> _channels;
        private readonly RantChannel _main;
        private readonly Limit _sizeLimit;

        private int _stackSize;

        public ChannelWriter(RantFormat formatStyle, Limit sizeLimit)
        {
            _sizeLimit = sizeLimit;
            _main = new RantChannel("main", RantChannelVisibility.Public, formatStyle, _sizeLimit);

            _stack = new List<RantChannel> { _main };
            _stackSize = 1;

            _channels = new Dictionary<string, RantChannel>
            {
                { "main", _main }
            };
        }

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
            RantChannel channel;
            return !_channels.TryGetValue(channelName, out channel) ? "" : channel.Value;
        }

        public void PushChannel(string channelName, RantChannelVisibility visibility, RantFormat formatStyle)
        {   
            RantChannel ch;
            if (!_channels.TryGetValue(channelName, out ch))
            {
                ch = new RantChannel(channelName, visibility, formatStyle, _sizeLimit);
                _channels[channelName] = ch;
            }

            if (channelName == "main") return;

            ch.Visiblity = visibility;

            if (_stack.Contains(ch)) return;
            _stack.Add(ch);
            _stackSize++;
        }

        public void PopChannel(string channelName)
        {
            if (channelName == "main") return;

            RantChannel ch;
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

        public Dictionary<RantChannel, Case> GetCurrentCases()
        {
            var table = new Dictionary<RantChannel, Case>();
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

	    public void Write(StringBuilder buffer)
	    {
		    if (buffer == null) return;
		    foreach (var ch in GetActive())
		    {
			    ch.WriteBuffer(buffer);
		    }
	    }

		public IEnumerable<RantChannel> GetActive()
        {
            var lastVisibility = RantChannelVisibility.Public;
            bool p = false;

            for (int i = _stackSize - 1; i >= 0; i--)
            {
                switch (_stack[i].Visiblity)
                {
                    case RantChannelVisibility.Public:
                        if (lastVisibility == RantChannelVisibility.Internal)
                        {
                            if (p) yield return _main;
                            yield break;
                        }
                        p = true;
                        break;
                    case RantChannelVisibility.Private:
                        yield return _stack[i];
                        if (p) yield return _main;
                        yield break;
                    case RantChannelVisibility.Internal:
                        break;
                }

                lastVisibility = _stack[i].Visiblity;
                yield return _stack[i];
            }
        }

        public Dictionary<string, RantChannel> Channels => _channels;
    }
}