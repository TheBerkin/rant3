using System.Collections.Generic;
using System.Linq;

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
            _mainState = State.CreateBase(input, this);
            _prevState = null;
        }

        public void Print(string input)
        {
            _stateStack.Peek().Output.Write(input);
        }

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

        public ChannelSet Run()
        {
            PushState(_mainState);

            State state = null; // The current state object being used
            SourceReader reader = null; // The current source reader being used

            next:
            while (Busy)
            {
                while (true)
                {
                    state = CurrentState;
                    reader = state.Reader;

                    if (!state.UseBlueprint()) break;
                }

                Token<TokenType> token;

                while (!reader.End)
                {
                    token = reader.PeekToken();

                    // Error on illegal closure
                    if (BracketPairs.All.ContainsClosing(token.Identifier))
                    {
                        throw new ManhoodException(_mainSource, token, "Unexpected token '" + Lexer.Rules.GetSymbolForId(token.Identifier) + "'");
                    }
                    
                    switch (token.Identifier)
                    {
                        case TokenType.LeftCurly:
                        {
                            var items = reader.ReadMultiItemScope(TokenType.LeftCurly, TokenType.RightCurly, TokenType.Pipe, BracketPairs.All).ToArray();
                            var attribs = PendingBlockAttribs;
                            _blockAttribs = new BlockAttribs();

                            if (!items.Any()) continue;

                            state.AddBlueprint(new RepeaterBlueprint(this, new Repeater(attribs.Repetitons), items));

                            goto next;
                        }
                        case TokenType.LeftSquare:
                        {
                            reader.ReadToken();
                            var name = reader.ReadToken();
                            if (!Util.ValidateName(name.Value))
                                throw new ManhoodException(reader.Source, name, "Invalid tag name '" + name.Value + "'");
                            bool none = false;
                            if (!reader.Take(TokenType.Colon))
                            {
                                if (!reader.Take(TokenType.RightSquare))
                                    throw new ManhoodException(reader.Source, name, "Expected ':' or ']' after tag name.");
                                none = true;
                            }

                            if (none)
                            {
                                state.AddBlueprint(new TagBlueprint(this, reader.Source, name, 0));
                            }
                            else
                            {
                                var items = reader.ReadItemsToScopeClose(TokenType.LeftSquare, TokenType.RightSquare,
                                    TokenType.Semicolon, BracketPairs.All).ToArray();

                                foreach(var item in items)
                                {
                                    PushState(State.CreateDistinct(reader.Source, item, this));
                                }

                                state.AddBlueprint(new TagBlueprint(this, reader.Source, name, items.Length));
                            }

                            goto next;
                        }
                        case TokenType.Text:
                        {
                            state.Output.Write(reader.ReadToken().Value);
                            break;
                        }
                        case TokenType.EscapeSequence:
                        {
                            state.Output.Write(Util.Unescape(reader.ReadToken().Value, _rng));
                            break;
                        }
                        case TokenType.ConstantLiteral:
                        {
                            state.Output.Write(Util.UnescapeConstantLiteral(reader.ReadToken().Value));
                            break;
                        }
                        default:
                            state.Output.Write(reader.ReadToken().Value);
                        break;
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