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
        private readonly int _sizeLimit;

        private int _lastWriteSize, _size;

        public ChannelStack(int sizeLimit)
        {
            _sizeLimit = sizeLimit;
            _main = new Channel("main", ChannelVisibility.Public);

            _stack = new List<Channel> { _main };

            _channels = new Dictionary<string, Channel>
            {
                { "main", _main }
            };
        }

        public int SizeLimit
        {
            get { return _sizeLimit; }
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

            if (!_stack.Contains(ch))
            {
                _stack.Add(ch);
            }
        }

        public void PopChannel(string channelName)
        {
            if (channelName == "main") return;

            Channel ch;
            if (_channels.TryGetValue(channelName, out ch))
            {
                _stack.Remove(ch);
            }
        }

        public void SetCaps(Capitalization caps)
        {
            int count = _stack.Count;
            var lastVisibility = ChannelVisibility.Public;
            if (_stack.Last().Visiblity == ChannelVisibility.Public) _main.Capitalization = caps;

            for (int i = count - 1; i >= 0; i--)
            {
                if (_stack[i] == _main) break;

                switch (_stack[i].Visiblity)
                {
                    case ChannelVisibility.Public:
                        if (lastVisibility == ChannelVisibility.Internal) return;
                        break;
                    case ChannelVisibility.Private:
                        _stack[i].Capitalization = caps;
                        return;
                    case ChannelVisibility.Internal:
                        break;
                }

                _stack[i].Capitalization = caps;

                lastVisibility = _stack[i].Visiblity;
            }
        }
        
        public void Write(string input)
        {
            int count = _stack.Count;
            var lastVisibility = ChannelVisibility.Public;
            if (_stack.Last().Visiblity == ChannelVisibility.Public) _main.Write(input);

            for (int i = count - 1; i >= 0; i--)
            {
                if (_stack[i] == _main) break;

                switch (_stack[i].Visiblity)
                {
                    case ChannelVisibility.Public:
                        if (lastVisibility == ChannelVisibility.Internal) goto checkLimit;
                        break;
                    case ChannelVisibility.Private:
                        _stack[i].Write(input);
                        goto checkLimit;
                    case ChannelVisibility.Internal:
                        break;
                }

                _stack[i].Write(input);

                lastVisibility = _stack[i].Visiblity;
            }

            checkLimit:

            CheckSizeLimit();
        }

        private void CheckSizeLimit()
        {
            int _lastSize = _size;
            _size = _channels.Sum(pair => pair.Value.Length);
            _lastWriteSize = _size - _lastSize;
            if (_sizeLimit <= 0) return;
            if (_size > _sizeLimit)
            {
                throw new InvalidOperationException("Exceeded character limit (" + _sizeLimit + " chars)");
            }
        }

        public ChannelSet GetChannels()
        {
            return new ChannelSet(_channels);
        }
    }
}