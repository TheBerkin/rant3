using System;
using System.Collections.Generic;
using System.Linq;

namespace Rant
{
    internal class ChannelStack
    {
        private readonly List<Channel> _stack;
        private readonly Dictionary<string, Channel> _channels;
        private readonly Channel _main;
        private readonly Limit<int> _sizeLimit;

        private int _stackSize;
        private int _lastWriteSize, _size;

        public ChannelStack(RantFormatStyle formatStyle, Limit<int> sizeLimit)
        {
            _sizeLimit = sizeLimit;
            _main = new Channel("main", ChannelVisibility.Public, formatStyle);

            _stack = new List<Channel> { _main };
            _stackSize = 1;

            _channels = new Dictionary<string, Channel>
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

        public long LastWriteSize
        {
            get { return _lastWriteSize; }
        }

        public long Size
        {
            get { return _size; }
        }

        public string GetOutputFor(string channelName)
        {
            Channel channel;
            return !_channels.TryGetValue(channelName, out channel) ? "" : channel.Value;
        }

        public void PushChannel(string channelName, ChannelVisibility visibility, RantFormatStyle formatStyle)
        {   
            Channel ch;
            if (!_channels.TryGetValue(channelName, out ch))
            {
                ch = new Channel(channelName, visibility, formatStyle);
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

        public void SetCaps(Case caps)
        {
            foreach (var ch in GetActive())
            {
                ch.Capitalization = caps;
            }
        }

        public void Write(string input)
        {
            foreach (var ch in GetActive())
            {
                if (!_sizeLimit.Accumulate(input.Length))
                    throw new InvalidOperationException("Exceeded character limit (" + _sizeLimit.LimitValue + ")");
                ch.Write(input);
            }

            CheckSizeLimit();
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

        private void CheckSizeLimit()
        {
            int _lastSize = _size;
            _size = _channels.Sum(pair => pair.Value.Length);
            _lastWriteSize = _size - _lastSize;
        }

        public Dictionary<string, Channel> Channels => _channels;
    }
}