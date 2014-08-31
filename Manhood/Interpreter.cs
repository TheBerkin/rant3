using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Manhood.Compiler;

using Stringes.Tokens;

namespace Manhood
{
    internal partial class Interpreter
    {
        public static int MaxStackSize = 64;

        private BlockAttribs _blockAttribs;

        private int _stateCount;

        private State _prevState;
        private readonly State _mainState;
        private readonly Stack<State> _stateStack;
        private readonly Stack<ChannelSet> _resultStack; 

        private readonly ChannelStack _output;
        private readonly RNG _rng;
        private readonly Source _mainSource;
        private readonly Engine _engine;

        public Engine Engine
        {
            get { return _engine; }
        }

        public BlockAttribs PendingBlockAttribs
        {
            get { return _blockAttribs; }
            set { _blockAttribs = value; }
        }

        public Interpreter(Engine engine, Source input, RNG rng)
        {
            _mainSource = input;
            _blockAttribs = new BlockAttribs();
            _rng = rng;
            _engine = engine;
            _output = new ChannelStack(0);
            _stateStack = new Stack<State>(1);
            _stateCount = 0;
            _resultStack = new Stack<ChannelSet>(1);
            _mainState = State.Create(input, this);
            _prevState = null;
        }

        public void Print(object input)
        {
            _stateStack.Peek().Output.Write(input.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string PopResult()
        {
            return !_resultStack.Any() ? "" : _resultStack.Pop().MainOutput;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushState(State state)
        {
            if (_stateCount >= MaxStackSize)
            {
                throw new ManhoodException(_mainSource, null, "Exceeded maximum stack size of " + MaxStackSize + ".");
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

        public bool Busy
        {
            get { return _stateCount > 0; }
        }

        public RNG RNG
        {
            get { return _rng; }
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
            while (Busy)
            {
                // Because blueprints can sometimes queue more blueprints, loop until the topmost state does not have one.
                do
                {
                    state = CurrentState;
                } while (state.UseBlueprint());
                
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
                    if (DoElement(token, reader, state))
                    {
                        goto next;
                    }
                }

                // This code will execute once the reader hits the end

                if (!state.SharesOutput)
                    _resultStack.Push(state.Output.GetChannels());

                PopState(); // Remove state from stack
            }

            return _resultStack.Pop();
        }
    }
}