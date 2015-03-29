using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

using Rant.Debugger;
using Rant.Engine.Compiler;
using Rant.Engine.Constructs;
using Rant.Engine.Formatters;
using Rant.Engine.ObjectModel;
using Rant.Formats;
using Rant.Stringes;
using Rant.Vocabulary;

namespace Rant.Engine
{
    internal partial class VM
    {
#if EDITOR
        // Events
        public event EventHandler<LineEventArgs> ActiveLineChanged;

        // Fields
        private int _lastActiveLine = 0;
#endif

        // Main fields
        public readonly RNG RNG;
        public readonly RantEngine Engine;
        public readonly ObjectStack Objects;

        private readonly RantPattern _mainSource;
        private readonly long _startingGen;

        // Queries
        public readonly QueryState QueryState = new QueryState();
        public readonly Dictionary<string, Query> LocalQueryMacros = new Dictionary<string, Query>();

        // Next block attribs
        public BlockAttribs NextBlockAttribs = new BlockAttribs();

        // The chance value of the next block
        private int _chance = 100;

        // Formatting
        private readonly NumberFormatter _numFormatter = new NumberFormatter();
        private int _quoteLevel = 0;

        // State information
        private int _stateCount = 0;
        private RantState _prevState = null;
        private readonly RantState _mainState;

        // Flag conditional fields
        private bool _else = false;

        // Stacks
        private readonly Stack<RantState> _stateStack = new Stack<RantState>();
        private readonly Stack<RantOutput> _resultStack = new Stack<RantOutput>();
        private readonly Stack<Repeater> _repeaterStack = new Stack<Repeater>();
        private readonly Stack<Match> _matchStack = new Stack<Match>();
        private readonly Stack<Dictionary<string, Argument>> _subArgStack = new Stack<Dictionary<string, Argument>>();
        private readonly Stack<Comparison> _comparisons = new Stack<Comparison>();
        private readonly ChannelStack _output;

        private readonly HashSet<RantState> _baseStates = new HashSet<RantState>();

        private readonly Limit<int> _charLimit;

        public Limit<int> CharLimit => _charLimit;

        public ChannelStack Channels => _output;

        public HashSet<RantState> BaseStates => _baseStates;

        public Stack<Dictionary<string, Argument>> SubArgStack => _subArgStack;

        public Stack<Comparison> Comparisons => _comparisons;

        public NumberFormatter NumberFormatter => _numFormatter;

        public VM(RantEngine engine, RantPattern input, RNG rng, int limitChars = 0)
        {
            Engine = engine;
            RNG = rng;
            Objects = engine.Objects.CreateLocalStack();

            _mainSource = input;
            _startingGen = rng.Generation;
            _charLimit = new Limit<int>(0, limitChars, (a, b) => a + b, (a, b) => b == 0 || a <= b);
            _output = new ChannelStack(engine.Format, _charLimit);
            _mainState = new RantState(this, input, _output);
        }

        #region Formatting

        public void OpenQuote()
        {
            _quoteLevel++;
            Print(_quoteLevel == 1
                ? Engine.Format.OpeningPrimaryQuote
                : Engine.Format.OpeningSecondaryQuote);
        }

        public void CloseQuote()
        {
            Print(_quoteLevel == 1
                ? Engine.Format.ClosingPrimaryQuote
                : Engine.Format.ClosingSecondaryQuote);
            _quoteLevel--;
        }

        public RantFormat Format => Engine.Format;


        public string FormatNumber(double value) => _numFormatter.FormatNumber(value);

        #endregion

        #region Flag conditionals
        public void SetElse() => _else = true;

        public bool UseElse()
        {
            bool e = _else;
            _else = false;
            return e;
        }
        #endregion

        #region Repeaters


        public void PushRepeater(Repeater repeater) => _repeaterStack.Push(repeater);


        public Repeater PopRepeater() => _repeaterStack.Pop();

        public Repeater CurrentRepeater => _repeaterStack.Any() ? _repeaterStack.Peek() : null;

        #endregion

        #region Chance

        public void SetChance(int chance) => _chance = chance < 0 ? 0 : chance > 100 ? 100 : chance;

        public bool TakeChance()
        {
            if (_chance == 100) return true;
            bool pass = RNG.Next(100) < _chance;
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

        public void PushMatch(Match match) => _matchStack.Push(match);

        public Match PopMatch() => _matchStack.Pop();

        #endregion

        #region States


        public void PushState(RantState state)
        {
            if (_stateCount >= RantEngine.MaxStackSize)
                throw new RantException(_mainSource, null, "Exceeded maximum stack size of \{RantEngine.MaxStackSize}.");

            if (_stateStack.Any()) _prevState = _stateStack.Peek();
            _stateCount++;
            _stateStack.Push(state);
        }

        public RantState PrevState => _prevState;


        public RantState PopState()
        {
            _stateCount--;
            var s = _stateStack.Pop();
            _prevState = _stateCount > 0 ? _stateStack.Peek() : null;
            return s;
        }

        public RantState CurrentState => _stateStack.Any() ? _stateStack.Peek() : null;

        #endregion

        public void Print(object input) => _stateStack.Peek().Print(input?.ToString());

        public string PopResultString() => !_resultStack.Any() ? "" : _resultStack.Pop().MainValue;

        public RantOutput Run(double timeout = -1)
        {
            PushState(_mainState);

            // ReSharper disable TooWideLocalVariableScope

            RantState state;                // The current state object being used
            PatternReader reader;       // The current source reader being used
            Token<R> token;             // The next token in the stream
            TokenFunc currentFunc;      // Current parser function

            // ReSharper restore TooWideLocalVariableScope

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            next:
            while (_stateCount > 0)
            {
                if (timeout != -1 && timeout * 1000 - stopwatch.ElapsedMilliseconds < 0)
                    throw new RantException(_mainSource, null, "The pattern has timed out.");

                // Because blueprints can sometimes queue more blueprints, loop until the topmost state does not have one.
                do
                {
                    state = CurrentState;
                } while (state.UsePreBlueprint());

                reader = state.Reader;

                while (!reader.End)
                {
                    // Fetch the next token in the stream without consuming it
                    token = reader.ReadToken();

                    // Error on illegal delimiter
                    if (Delimiters.All.ContainsClosing(token.ID) && !Delimiters.All.Contains(token.ID, token.ID))
                        throw new RantException(reader.Source, token, "Unexpected delimiter '\{RantLexer.Rules.GetSymbolForId(token.ID)}'");

#if EDITOR
                    if (state.Reader.Source.SourceStringe == _mainSource.SourceStringe)
                    {
                        if (token.Line != _lastActiveLine)
                            ActiveLineChanged?.Invoke(this, new LineEventArgs(_mainSource, token));

                        _lastActiveLine = token.Line;
                    }

#endif

                    // Token function will return true if the interpreter should skip to the top of the stack
                    if (TokenFuncs.TryGetValue(token.ID, out currentFunc))
                    {
                        if (currentFunc(this, token, reader, state)) goto next;
                    }
                    else
                    {
                        Print(token.Value);
                    }
                }

                // Use post-blueprints
                while (state.UsePostBlueprint()) { }

                // Push a result string if the state's output differs from the one below it
                if (!state.SharesOutput && state.Finish())
                {
                    _resultStack.Push(new RantOutput(RNG.Seed, _startingGen, state.Output.Channels));
                }

                // Remove state from stack as long as nothing else was added beforehand
                if (state == CurrentState)
                {
                    PopState();
                    BaseStates.Remove(state);
                }
            }

            return _resultStack.Pop();
        }
    }
}