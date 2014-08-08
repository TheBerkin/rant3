using System;
using System.Linq;

namespace Manhood
{
    internal partial class Interpreter
    {
        private readonly State _state;
        private readonly WordBank _wordBank;
        private readonly ChannelStack _channels;
        private readonly StringRequestCallback _stringRequested;

        private BlockAttribs _currentAttribs = new BlockAttribs();

        private string _lastText = "";
        
        private const string IgnoreChars = "\r\n\t";

        public Interpreter(WordBank wordBank, SubStore subStore, int sizeLimit = 0, StringRequestCallback _stringRequestCallback = null)
        {
            InitTagFuncs();
            _wordBank = wordBank;
            _channels = new ChannelStack(sizeLimit);
            _state = new State(subStore, DateTime.UtcNow.Ticks);
            _stringRequested = _stringRequestCallback;
        }

        public Interpreter(WordBank wordBank, SubStore subStore, long seed, int sizeLimit = 0, StringRequestCallback _stringRequestCallback = null)
        {
            InitTagFuncs();
            _wordBank = wordBank;
            _channels = new ChannelStack(sizeLimit);
            _state = new State(subStore, seed);
            _stringRequested = _stringRequestCallback;
        }

        private Interpreter(State state, WordBank wordBank, int sizeLimit = 0, StringRequestCallback _stringRequestCallback = null)
        {
            _state = state;
            _wordBank = wordBank;
            _channels = new ChannelStack(sizeLimit);
            _stringRequested = _stringRequestCallback;
        }

        public void DoSub(Subroutine sub, string[] args)
        {
            _state.PushArgs(new SubArgs(this, sub, args));
            Do(sub.Body);
            _state.PopArgs();
        }

        public string InterpretToString(Pattern input, bool stripIllegalChars = true)
        {
            Do(input.Code, stripIllegalChars);
            return _channels.GetChannels()["main"].Output;
        }

        private string InterpretToString(string input, bool stripIllegalChars = true)
        {
            Do(input, stripIllegalChars);
            return _channels.GetChannels()["main"].Output;
        }

        public ChannelSet InterpretToChannels(Pattern input, bool stripIllegalChars = true)
        {
            Do(input.Code, stripIllegalChars);
            return _channels.GetChannels();
        }

        internal void Do(string input, bool stripIllegalChars = true)
        {
            if (String.IsNullOrEmpty(input)) return;
            var scanner = new Scanner(input);
            while (!scanner.EndOfString)
            {
                BlockInfo block;
                if (scanner.ReadBlock(out block))
                {
                    var sync = _state.PopSynchronizer();

                    var attribs = _currentAttribs;
                    _currentAttribs = new BlockAttribs(); // Switch out interpreter attrib set with fresh instance in case of tags inside block
                    
                    if (block.IsEmpty) continue;
                    
                    if (attribs.Chance < 100)
                    {
                        if (_state.RNG.Next(1, 101) > attribs.Chance)
                        {
                            continue;
                        }
                    }

                    int reps = attribs.Repetitions < 0 ? block.ElementRanges.Length : attribs.Repetitions;

                    // Apply repeater
                    var rep = new Repeater(reps);
                    _state.PushRepeater(rep);

                    do
                    {
                        var element =
                            input.Substring(
                                block.ElementRanges[_state.SelectBlockItem(block.ElementRanges.Length, sync)]);

                        Do(attribs.BeforeString, stripIllegalChars);
                        Do(element, stripIllegalChars);
                        Do(attribs.AfterString, stripIllegalChars);

                        // Apply separator
                        if (!attribs.Separator.Any() || rep.CurrentRepIndex >= reps - 1) continue;

                        _state.PopRepeater(); // Pop repeater to prevent conflicting usage of [first], [last], etc.
                        Do(attribs.Separator, stripIllegalChars);
                        _state.PushRepeater(rep);

                    } while (rep.Step());

                    _state.PopRepeater();

                    continue;
                }

                TagInfo tag;
                if (scanner.ReadTag(out tag))
                {
                    if (!DoTag(tag))
                    {
                        _state.ReleaseContext();
                        return;
                    }
                    _state.ReleaseContext();
                    continue;
                }

                Query wordCall;
                if (scanner.ReadWordCall(out wordCall))
                {
                    Write(_wordBank.GetWord(this, wordCall));
                    continue;
                }
                
                if (!IgnoreChars.Contains((char)scanner.Next) || !stripIllegalChars)
                {
                    Write(scanner.ReadChar());
                }
                else
                {
                    scanner.ReadChar();
                }
            }
        }

        public State State
        {
            get { return _state; }
        }

        public string Evaluate(string input, bool stripIllegalChars = true)
        {
            return new Interpreter(_state, _wordBank, _channels.SizeLimit, _stringRequested).InterpretToString(input, stripIllegalChars);
        }

        public void Write(object input)
        {
            var inputString = input.ToString();

            // Caps modifier
            switch (_state.CurrentFormat)
            {
                case Capitalization.First:
                    if (inputString.Any(Char.IsLetterOrDigit))
                    {
                        inputString = inputString.Substring(0, 1).ToUpper() + inputString.Substring(1);
                        _state.CurrentFormat = Capitalization.None;
                    }
                    break;
                case Capitalization.Lower:
                    inputString = inputString.ToLower();
                    break;
                case Capitalization.Upper:
                    inputString = inputString.ToUpper();
                    break;
                case Capitalization.Proper:
                    if (_lastText.EndsWith(" ") || _lastText.EndsWith("\n") || String.IsNullOrEmpty(_lastText))
                    {
                        inputString = inputString.Substring(0, 1).ToUpper() + inputString.Substring(1);
                    }
                    break;
            }

            _channels.Write(_lastText = inputString);
        }

        public bool DoTag(TagInfo tag)
        {
            if (tag.Name.StartsWith("$"))
            {
                return CallImpl(tag);
            }

            Func<Interpreter, string[], bool> func;
            return TagFuncs.TryGetValue(tag.Name.ToLower(), out func) && func(this, tag.Arguments);
        }

        internal class BlockAttribs
        {
            public int Repetitions { get; set; }            
            public int Chance { get; set; }
            public string Separator { get; set; }
            public string BeforeString { get; set; }
            public string AfterString { get; set; }

            public BlockAttribs()
            {
                Repetitions = 1;
                Chance = 100;
                Separator = "";
                BeforeString = "";
                AfterString = "";
            }
        }
    }
}