using System;
using System.Collections.Generic;
using System.Linq;

namespace Manhood
{
    internal class ChannelStack
    {
        private readonly List<Channel> _stack;
        private readonly Dictionary<string, Channel> _channels;
        private readonly Channel _main;
        private readonly Limit<int> _sizeLimit;

        private int _stackSize;
        private int _lastWriteSize, _size;

        public ChannelStack(Limit<int> sizeLimit)
        {
            _sizeLimit = sizeLimit;
            _main = new Channel("main", ChannelVisibility.Public);

            _stack = new List<Channel> { _main };
            _stackSize = 1;

            _channels = new Dictionary<string, Channel>
            {
                { "main", _main }
            };
        }

        public int LastWriteSize
        {
            get { return _lastWriteSize; }
        }

        public int Size
        {
            get { return _size; }
        }

        public string GetOutput(string channelName)
        {
            Channel channel;
            return !_channels.TryGetValue(channelName, out channel) ? "" : channel.Value;
        }

        public void PushChannel(string channelName, ChannelVisibility visibility)
        {
            
            Channel ch;
            if (!_channels.TryGetValue(channelName, out ch))
            {
                ch = new Channel(channelName, visibility);
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

            Channel ch;
            if (!_channels.TryGetValue(channelName, out ch)) return;
            _stack.Remove(ch);
            _stackSize--;
        }

        public void SetCaps(Capitalization caps)
        {
            foreach (var ch in GetVisibleChannels())
            {
                ch.Capitalization = caps;
            }
        }

        public void Write(string input)
        {
            foreach (var ch in GetVisibleChannels())
            {
                if (!_sizeLimit.Accumulate(input.Length))
                    throw new InvalidOperationException("Exceeded character limit (" + _sizeLimit.LimitValue + ")");
                ch.Write(input);
            }

            CheckSizeLimit();
        }

        private IEnumerable<Channel> GetVisibleChannels()
        {
            var lastVisibility = ChannelVisibility.Public;
            if (_stack.Last().Visiblity == ChannelVisibility.Public) yield return _main;

            for (int i = _stackSize - 1; i >= 0; i--)
            {
                if (_stack[i] == _main) break;

                switch (_stack[i].Visiblity)
                {
                    case ChannelVisibility.Public:
                        if (lastVisibility == ChannelVisibility.Internal) yield break;
                        break;
                    case ChannelVisibility.Private:
                        yield return _stack[i];
                        yield break;
                    case ChannelVisibility.Internal:
                        break;
                }

                yield return _stack[i];

                lastVisibility = _stack[i].Visiblity;
            }
        }

        private void CheckSizeLimit()
        {
            int _lastSize = _size;
            _size = _channels.Sum(pair => pair.Value.Length);
            _lastWriteSize = _size - _lastSize;
        }

        public Output GetChannels()
        {
            return new Output(_channels);
        }
    }
}