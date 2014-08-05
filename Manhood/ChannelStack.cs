using System;
using System.Collections.Generic;
using System.Linq;

namespace Manhood
{
    internal class ChannelStack
    {
        private readonly List<Channel> _activeSet;
        private readonly Dictionary<string, Channel> _channels;
        private readonly Channel _main;
        private readonly int _sizeLimit;

        public ChannelStack(int sizeLimit)
        {
            _sizeLimit = sizeLimit;
            _main = new Channel("main", ChannelVisibility.Public);

            _activeSet = new List<Channel>
            {
                _main
            };

            _channels = new Dictionary<string, Channel>
            {
                { "main", _main }
            };
        }

        public int SizeLimit
        {
            get { return _sizeLimit; }
        }

        public string GetOutput(string channelName)
        {
            Channel channel;
            return !_channels.TryGetValue(channelName, out channel) ? "" : channel.Output;
        }

        public void PushChannel(string channelName, ChannelVisibility visibility)
        {
            if (!Util.ValidateName(channelName))
            {
                throw new FormatException("Invalid channel name.");
            }

            Channel ch;
            if (!_channels.TryGetValue(channelName, out ch))
            {
                ch = new Channel(channelName, visibility);
                _channels[channelName] = ch;
            }
            
            if (channelName != "main")
            {
                ch.Visiblity = visibility;

                if (!_activeSet.Contains(ch))
                {
                    _activeSet.Add(ch);
                }
            }
        }

        public void PopChannel(string channelName)
        {
            if (channelName == "main") return;

            if (!Util.ValidateName(channelName))
            {
                throw new FormatException("Invalid channel name.");
            }

            Channel ch;
            if (_channels.TryGetValue(channelName, out ch))
            {
                _activeSet.Remove(ch);
            }
        }

        public void Write(string input)
        {
            int count = _activeSet.Count;
            var lastVisibility = ChannelVisibility.Public;
            if (_activeSet.Last().Visiblity == ChannelVisibility.Public) _main.Buffer.Append(input);

            for (int i = count - 1; i >= 0; i--)
            {
                if (_activeSet[i] == _main) break;

                switch (_activeSet[i].Visiblity)
                {
                    case ChannelVisibility.Public:
                        if (lastVisibility == ChannelVisibility.Internal) goto checkLimit;
                        break;
                    case ChannelVisibility.Private:
                        _activeSet[i].Buffer.Append(input);
                        goto checkLimit;
                    case ChannelVisibility.Internal:
                        break;
                }

                _activeSet[i].Buffer.Append(input);

                lastVisibility = _activeSet[i].Visiblity;
            }

            checkLimit:

            CheckSizeLimit();
        }

        private void CheckSizeLimit()
        {
            if (_sizeLimit <= 0) return;
            if (_channels.Sum(pair => pair.Value.Buffer.Length) > _sizeLimit)
            {
                throw new ManhoodException("Exceeded character limit (" + _sizeLimit + " chars)");
            }
        }

        public ChannelSet GetChannels()
        {
            return new ChannelSet(_channels);
        }
    }
}