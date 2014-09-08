using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using Manhood.Compiler;

using Stringes.Tokens;

namespace Manhood
{
    internal partial class Interpreter
    {
        // Main fields
        private readonly RNG _rng;
        private readonly Source _mainSource;
        private readonly Engine _engine;
        private readonly Vocabulary _vocab;

        // Next block attribs
        private BlockAttribs _blockAttribs = new BlockAttribs();

        // The chance value of the next block
        private int _chance = 100;

        // Number format
        private NumberFormat _numfmt = NumberFormat.Normal;

        // Capitalization
        private Capitalization _caps = Capitalization.None;
        private char _lastChar = ' ';
        private static readonly Regex RegCapsProper = new Regex(@"\b[a-z]", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        private static readonly Regex RegCapsFirst = new Regex(@"(?<![a-z].*?)[a-z]", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        // State information
        private int _stateCount = 0;
        private State _prevState = null;
        private readonly State _mainState;

        // Stacks
        private readonly Stack<State> _stateStack = new Stack<State>();
        private readonly Stack<ChannelSet> _resultStack = new Stack<ChannelSet>();
        private readonly Stack<Repeater> _repeaterStack = new Stack<Repeater>();
        private readonly Stack<Match> _matchStack = new Stack<Match>();
        private readonly Stack<Dictionary<string, TagArg>> _subArgStack = new Stack<Dictionary<string, TagArg>>(); 
        private readonly ChannelStack _output = new ChannelStack(0);

        public Engine Engine
        {
            get { return _engine; }
        }

        public BlockAttribs NextAttribs
        {
            get { return _blockAttribs; }
            set { _blockAttribs = value; }
        }

        public Vocabulary Vocabulary
        {
            get { return _vocab; }
        }

        public RNG RNG
        {
            get { return _rng; }
        }

        public Stack<Dictionary<string, TagArg>> SubArgStack
        {
            get { return _subArgStack; }
        }

        public NumberFormat NumberFormat
        {
            get { return _numfmt; }
            set { _numfmt = value; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string FormatNumber(double value)
        {
            return Numerals.FormatNumber(value, _numfmt);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Capitalize(string input)
        {
            if (String.IsNullOrEmpty(input) || _caps == Capitalization.None) return input;
            switch (_caps)
            {
                case Capitalization.Lower:
                    input = input.ToLower();
                    break;
                case Capitalization.Upper:
                    input = input.ToUpper();
                    break;
                case Capitalization.First:
                    input = RegCapsFirst.Replace(input, m => m.Value.ToUpper());
                    _caps = Capitalization.None;
                    break;
                case Capitalization.Proper:
                    input = RegCapsProper.Replace(input, m => m.Value.ToUpper());
                    break;
            }
            _lastChar = input[input.Length - 1];
            return input;
        }

        public Capitalization Capitalization
        {
            get { return _caps; }
            set { _caps = value; }
        }

        #region Repeaters

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushRepeater(Repeater repeater)
        {
            _repeaterStack.Push(repeater);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Repeater PopRepeater()
        {
            return _repeaterStack.Pop();
        }

        public Repeater CurrentRepeater
        {
            get { return _repeaterStack.Any() ? _repeaterStack.Peek() : null; }
        }

        #endregion

        #region Chance

        public void SetChance(int chance)
        {
            _chance = chance < 0 ? 0 : chance > 100 ? 100 : chance;
        }

        public bool TakeChance()
        {
            if (_chance == 100) return true;
            bool pass = _rng.Next(100) < _chance;
            _chance = 100;
            return pass;
        }

        #endregion

        #region Replacers

        public string GetMatchString(string group = null)
        {
            if (!_matchStack.Any()) return "";
            return !String.IsNullOrEmpty(@group) ? _matchStack.Peek().Groups[@group].Value : _matchStack.Peek().Value;
        }

        public void PushMatch(Match match)
        {
            _matchStack.Push(match);
        }

        public Match PopMatch()
        {
            return _matchStack.Pop();
        }

        #endregion

        #region States

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushState(State state)
        {
            if (_stateCount >= Engine.MaxStackSize)
            {
                throw new ManhoodException(_mainSource, null, "Exceeded maximum stack size of " + Engine.MaxStackSize + ".");
            }
            if (_stateStack.Any()) _prevState = _stateStack.Peek();
            _stateCount++;
            _stateStack.Push(state);
        }

        public State PrevState
        {
            get { return _prevState; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public State PopState()
        {
            _stateCount--;
            var s = _stateStack.Pop();
            _prevState = _stateCount > 0 ? _stateStack.Peek() : null;
            return s;
        }

        public State CurrentState
        {
            get { return _stateStack.Peek(); }
        }

        #endregion
        
        public Interpreter(Engine engine, Source input, RNG rng, Vocabulary vocab)
        {
            _mainSource = input;
            _rng = rng;
            _vocab = vocab;
            _engine = engine;
            _mainState = State.Create(input, this);
        }

        public void Print(object input)
        {
            _stateStack.Peek().Print(input.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string PopResultString()
        {
            return !_resultStack.Any() ? "" : _resultStack.Pop().MainOutput;
        }

        public ChannelSet Run()
        {
            PushState(_mainState);

            // ReSharper disable TooWideLocalVariableScope

            State state;                // The current state object being used
            SourceReader reader;        // The current source reader being used
            Token<TokenType> token;     // The next token in the stream

            // ReSharper restore TooWideLocalVariableScope

            next:
            while (_stateCount > 0)
            {
                // Because blueprints can sometimes queue more blueprints, loop until the topmost state does not have one.
                do
                {
                    state = CurrentState;
                } while (state.UsePreBlueprint());
                
                reader = state.Reader;

                while (!reader.End)
                {
                    // Fetch the next token in the stream without consuming it
                    token = reader.PeekToken();
                    
                    // Error on illegal closure
                    if (BracketPairs.All.ContainsClosing(token.Identifier))
                    {
                        throw new ManhoodException(reader.Source, token, "Unexpected token '" + Lexer.Rules.GetSymbolForId(token.Identifier) + "'");
                    }

                    // DoElement will return true if the interpreter should skip to the top of the stack
                    if (DoElement(reader, state))
                    {
                        goto next;
                    }
                }

                // Use post-blueprints
                while (state.UsePostBlueprint()) { }

                // Push a result string if the state's output differs from the one below it
                if (!state.SharesOutput && state.Finish())
                {
                    _resultStack.Push(state.Output.GetChannels());
                }

                // Remove state from stack as long as nothing else was added beforehand
                if (state == CurrentState) PopState();
            }

            return _resultStack.Pop();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool DoElement(SourceReader reader, State state)
        {
            TokenFunc func;
            if (TokenFuncs.TryGetValue(reader.PeekToken().Identifier, out func)) return func(this, reader, state);
            Print(reader.ReadToken().Value);
            return false;
        }
    }
}