using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Manhood
{
    internal partial class Interpreter
    {
        private readonly State _state;
        private readonly WordBank _wordBank;
        private readonly ChannelStack _channels;
        private readonly StringRequestCallback _stringRequested;

        // The current block attributes that will be consumed by the next block.
        private BlockAttribs _currentAttribs = new BlockAttribs();

        // The last string written to the output. This will be a single character if the last input was text.
        private string _lastText = "";
        
        // These are to be ignored in plaintext found in patterns
        private const string IgnoreChars = "\r\n\t";

        public Interpreter(ManhoodContext manhood, int sizeLimit = 0, StringRequestCallback _stringRequestCallback = null)
        {
            InitTagFuncs();
            _wordBank = manhood.WordBank;
            _channels = new ChannelStack(sizeLimit);
            _state = new State(manhood.Subroutines, manhood.ListStore, manhood.Flags, DateTime.UtcNow.Ticks);
            _stringRequested = _stringRequestCallback;
        }

        public Interpreter(ManhoodContext manhood, long seed, int sizeLimit = 0, StringRequestCallback _stringRequestCallback = null)
        {
            InitTagFuncs();
            _wordBank = manhood.WordBank;
            _channels = new ChannelStack(sizeLimit);
            _state = new State(manhood.Subroutines, manhood.ListStore, manhood.Flags, seed);
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
            _state.Lists.DestroyLocals();
            return _channels.GetChannels()["main"].Output;
        }

        private string InterpretToString(string input, bool stripIllegalChars = true)
        {
            Do(input, stripIllegalChars);
            // Do not destroy local lists here -- this overload is never called by the external API.
            return _channels.GetChannels()["main"].Output;
        }

        public ChannelSet InterpretToChannels(Pattern input, bool stripIllegalChars = true)
        {
            Do(input.Code, stripIllegalChars);
            _state.Lists.DestroyLocals();
            return _channels.GetChannels();
        }

        /// <summary>
        /// Process a pattern into the current output for this instance.
        /// </summary>
        /// <param name="input">The input pattern.</param>
        /// <param name="stripIllegalChars">If true, ignore blacklisted characters in input.</param>
        internal void Do(string input, bool stripIllegalChars = true)
        {
            if (String.IsNullOrEmpty(input)) return;
            var scanner = new Scanner(input, _state.RNG);
            while (!scanner.EndOfString)
            {
                BlockInfo block;
                if (scanner.ReadBlock(out block))
                {
                    // Consume synchronizer. Will be null if none is set. This isn't a problem, as State.SelectBlockItem handles the null case.
                    var sync = _state.PopSynchronizer();

                    // Grab the current block attributes and replace them with defaults.
                    var attribs = _currentAttribs;
                    _currentAttribs = new BlockAttribs();
                    
                    if (block.IsEmpty) continue; // Skip empty blocks

                    // Skip according to [chance] probability
                    if (attribs.Chance < 100 && _state.RNG.Next(1, 101) > attribs.Chance) continue;

                    // Determine the number of repetitions
                    int reps = attribs.Repetitions < 0 ? block.ElementRanges.Length : attribs.Repetitions;

                    if (reps == 0) continue; // Skip blocks with zero reps

                    // Apply repeater
                    var rep = new Repeater(reps);

                    _state.PushRepeater(rep);
                    do
                    {
                        var element =
                            input.Substring(
                                block.ElementRanges[_state.SelectBlockItem(block.ElementRanges.Length, sync)]);

                        // Interpret the block item and any before/after patterns
                        Do(attribs.BeforeString, stripIllegalChars);
                        Do(element, stripIllegalChars);
                        Do(attribs.AfterString, stripIllegalChars);

                        // Apply separator
                        if (!attribs.Separator.Any() || rep.CurrentRepIndex >= reps - 1) continue;

                        _state.PopRepeater(); // Pop repeater to prevent conflicting usage of [first], [last], etc.
                        Do(attribs.Separator, stripIllegalChars);
                        _state.PushRepeater(rep); // Put the repeater back after separator is handled

                    } while (rep.Step());

                    // Remove repeater from stack
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

        /// <summary>
        /// This is used by tags that emulate block behavior, such as [lstblock]
        /// </summary>
        /// <param name="block">The block items to use.</param>
        public void DoEnumerableAsBlock(IEnumerable<string> block)
        {
            var sync = _state.PopSynchronizer();

            var attribs = _currentAttribs;
            _currentAttribs = new BlockAttribs();

            var blockArray = block as string[] ?? block.ToArray();

            if (!blockArray.Any()) return;

            if (attribs.Chance < 100 && _state.RNG.Next(1, 101) > attribs.Chance) return;

            int reps = attribs.Repetitions < 0 ? blockArray.Length : attribs.Repetitions;

            if (reps == 0) return;

            var rep = new Repeater(reps);

            _state.PushRepeater(rep);

            do
            {
                var element = blockArray[_state.SelectBlockItem(blockArray.Length, sync)];

                Do(attribs.BeforeString);
                Do(element);
                Do(attribs.AfterString);

                if (!attribs.Separator.Any() || rep.CurrentRepIndex >= reps - 1) continue;

                _state.PopRepeater();
                Do(attribs.Separator);
                _state.PushRepeater(rep);

            } while (rep.Step());

            _state.PopRepeater();
        }

        public State State
        {
            get { return _state; }
        }

        /// <summary>
        /// Creates a new interpreter that shares this instance's state object, evaluates the given pattern, and returns the main output
        /// </summary>
        /// <param name="input">The input pattern.</param>
        /// <param name="stripIllegalChars">If true, ignore blacklisted characters in input.</param>
        /// <returns></returns>
        public string Evaluate(string input, bool stripIllegalChars = true)
        {
            return new Interpreter(_state, _wordBank, _channels.SizeLimit, _stringRequested).InterpretToString(input, stripIllegalChars);
        }

        public void Write(object input)
        {
            var inputString = input.ToString();

            // Handle capitalization via the [caps] tag
            switch (_state.CurrentCapsFormat)
            {
                case Capitalization.First:
                    if (inputString.Any(Char.IsLetterOrDigit))
                    {
                        inputString = inputString.Substring(0, 1).ToUpper() + inputString.Substring(1);
                        _state.CurrentCapsFormat = Capitalization.None;
                    }
                    break;
                case Capitalization.Lower:
                    inputString = inputString.ToLower();
                    break;
                case Capitalization.Upper:
                    inputString = inputString.ToUpper();
                    break;
                case Capitalization.Proper:
                    if (String.IsNullOrEmpty(_lastText) || " \t\n".Contains(_lastText.Last()))
                    {
                        inputString = Regex.Replace(inputString, @"\b\w", m => m.Value.ToUpper());
                    }
                    break;
            }

            _channels.Write(_lastText = inputString);
        }

        public bool DoTag(TagInfo tag)
        {
            if (tag.Name.StartsWith("$")) // Syntactic sugar for the [call] tag.
            {
                return CallImpl(tag);
            }

            Func<Interpreter, string[], bool> func;
            if (!TagFuncs.TryGetValue(tag.Name.ToLower(), out func))
            {
                throw new ManhoodException("Invalid tag type '"+tag.Name+"'.");
            }
                
            return func(this, tag.Arguments);
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