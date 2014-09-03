using System.Collections.Generic;
using System.Linq;

using Manhood.Blueprints;
using Manhood.Compiler;

using Stringes.Tokens;

namespace Manhood
{
    internal partial class Interpreter
    {
        /// <summary>
        /// Maintains state information for a single stream of tokens.
        /// </summary>
        internal class State
        {
            private readonly ChannelStack _output;
            private readonly SourceReader _reader;
            private readonly Interpreter _interpreter;
            private readonly bool _sharesOutput;

            private Blueprint _preBlueprint, _postBlueprint;

            public bool SharesOutput
            {
                get { return _sharesOutput; }
            }

            private State(Interpreter ii, Source derivedSource, IEnumerable<Token<TokenType>> tokens,
                ChannelStack output)
            {
                _preBlueprint = null;
                _postBlueprint = null;
                _interpreter = ii;
                _output = output;
                _reader =
                    new SourceReader(new Source(derivedSource.Name, derivedSource.Type, tokens, derivedSource.Code));
                _sharesOutput = (output == _interpreter._output && _interpreter.PrevState != null) || (_interpreter._stateStack.Any() && output == _interpreter._stateStack.Peek().Output);
            }

            private State(Interpreter ii, Source source, ChannelStack output)
            {
                _preBlueprint = null;
                _postBlueprint = null;
                _interpreter = ii;
                _output = output;
                _reader = new SourceReader(source);
                _sharesOutput = (output == _interpreter._output && _interpreter.PrevState != null) || (_interpreter._stateStack.Any() && output == _interpreter._stateStack.Peek().Output);
            }

            /// <summary>
            /// Sets the current pre-blueprint for this state.
            /// </summary>
            /// <param name="bp">The blueprint to set.</param>
            /// <returns></returns>
            public bool AddPreBlueprint(Blueprint bp)
            {
                if (_preBlueprint != null) return false;
                _preBlueprint = bp;
                return true;
            }

            /// <summary>
            /// Consumes the available pre-blueprint, if any. Returns 'true' if either the blueprint hints at the interpreter to skip to the top of the stack, or another blueprint was set during execution.
            /// </summary>
            /// <returns></returns>
            public bool UsePreBlueprint()
            {
                if (_preBlueprint == null) return false;
                var bp = _preBlueprint;
                _preBlueprint = null;
                return bp.Use() || _preBlueprint != null;
            }

            /// <summary>
            /// Consumes the available post-blueprint, if any. Returns 'true' if another blueprint was set during execution.
            /// </summary>
            /// <returns></returns>
            public bool UsePostBlueprint()
            {
                if (_postBlueprint == null) return false;
                var bp = _postBlueprint;
                _postBlueprint = null;
                bp.Use();
                return _postBlueprint != null;
            }

            /// <summary>
            /// Sets the current post-blueprint for this state.
            /// </summary>
            /// <param name="bp">The blueprint to set.</param>
            /// <returns></returns>
            public bool AddPostBlueprint(Blueprint bp)
            {
                if (_postBlueprint != null) return false;
                _postBlueprint = bp;
                return true;
            }

            /// <summary>
            /// Creates a state object that reads tokens from the specified source.
            /// </summary>
            /// <param name="source">The source from which to read tokens.</param>
            /// <param name="interpreter">The interpreter that will read the tokens.</param>
            /// <returns></returns>
            public static State Create(Source source, Interpreter interpreter)
            {
                return new State(interpreter, source, interpreter._output);
            }

            /// <summary>
            /// Creates a state object that reads tokens from a custom collection that is associated with the specified source. Associates with the main output.
            /// </summary>
            /// <param name="derivedSource">The source with which to associate the tokens.</param>
            /// <param name="tokens">The tokens to read.</param>
            /// <param name="interpreter">The interpreter that will read the tokens.</param>
            /// <returns></returns>
            public static State CreateDerivedShared(Source derivedSource, IEnumerable<Token<TokenType>> tokens,
                Interpreter interpreter)
            {
                return new State(interpreter, derivedSource, tokens, interpreter._output);
            }

            /// <summary>
            /// Creates a state object that reads tokens from a custom collection that is associated with the specified source. Specifying an output that is distinct from the one below it in the stack will cause the output to be pushed to the result stack when finished.
            /// </summary>
            /// <param name="derivedSource">The source with which to associate the tokens.</param>
            /// <param name="tokens">The tokens to read.</param>
            /// <param name="interpreter">The interpreter that will read the tokens.</param>
            /// <param name="output">The output of the state. Excluding this will create a new output.</param>
            /// <returns></returns>
            public static State CreateDerivedDistinct(Source derivedSource, IEnumerable<Token<TokenType>> tokens,
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