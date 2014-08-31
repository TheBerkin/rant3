using System.Collections.Generic;
using System.Linq;

using Manhood.Compiler;

using Stringes.Tokens;

namespace Manhood
{
    internal partial class Interpreter
    {
        internal class State
        {
            private readonly ChannelStack _output;
            private readonly SourceReader _reader;
            private readonly Interpreter _interpreter;
            private readonly bool _sharesOutput;

            private Blueprint _blueprint;

            public bool SharesOutput
            {
                get { return _sharesOutput; }
            }

            private State(Interpreter ii, Source derivedSource, IEnumerable<Token<TokenType>> tokens,
                ChannelStack output)
            {
                _interpreter = ii;
                _output = output;
                _reader =
                    new SourceReader(new Source(derivedSource.Name, derivedSource.Type, tokens, derivedSource.Code));
                _sharesOutput = (output == _interpreter._output && _interpreter.PrevState != null) || (_interpreter._stateStack.Any() && output == _interpreter._stateStack.Peek().Output);
            }

            private State(Interpreter ii, Source source, ChannelStack output)
            {
                _interpreter = ii;
                _output = output;
                _reader = new SourceReader(source);
                _sharesOutput = (output == _interpreter._output && _interpreter.PrevState != null) || (_interpreter._stateStack.Any() && output == _interpreter._stateStack.Peek().Output);
            }

            public bool AddBlueprint(Blueprint bp)
            {
                if (_blueprint != null) return false;
                _blueprint = bp;
                return true;
            }

            public bool UseBlueprint()
            {
                if (_blueprint == null) return false;
                var bp = _blueprint;
                _blueprint = null;
                bp.Use();
                return _blueprint != null;
            }

            public static State CreateBase(Source source, Interpreter interpreter)
            {
                return new State(interpreter, source, interpreter._output);
            }

            public static State CreateShared(Source derivedSource, IEnumerable<Token<TokenType>> tokens,
                Interpreter interpreter)
            {
                return new State(interpreter, derivedSource, tokens, interpreter._output);
            }

            public static State CreateDistinct(Source derivedSource, IEnumerable<Token<TokenType>> tokens,
                Interpreter interpreter, ChannelStack output = null)
            {
                return new State(interpreter, derivedSource, tokens, output ?? new ChannelStack(0));
                    // TODO: Share size limit between sub-output and main output
            }

            public SourceReader Reader
            {
                get { return _reader; }
            }

            public ChannelStack Output
            {
                get { return _output; }
            }
        }
    }
}