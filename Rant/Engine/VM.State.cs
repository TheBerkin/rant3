using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

using Rant.Engine.Blueprints;
using Rant.Engine.Compiler;
using Rant.Stringes.Tokens;

namespace Rant.Engine
{
    internal partial class VM
    {
        /// <summary>
        /// Maintains state information for a single stream of tokens.
        /// </summary>
        internal class State
        {
            public readonly ChannelStack Output;
            public readonly PatternReader Reader;            
            public readonly bool SharesOutput;

            private bool _finished = false;

            private readonly Stack<Blueprint> _preBlueprints = new Stack<Blueprint>();
            private readonly Stack<Blueprint> _postBlueprints = new Stack<Blueprint>();

            public MatchCollection CurrentMatches;
            public int MatchPosition = 0;

            public bool Finish()
            {
                if (_finished) return false;
                _finished = true;
                return true;
            }

            private State(VM vm, RantPattern derivedSource, IEnumerable<Token<R>> tokens,
                ChannelStack output)
            {
                Output = output;
                Reader = new PatternReader(new RantPattern(derivedSource, tokens));
                SharesOutput = (output == vm._output && vm.PrevState != null) || (vm._stateStack.Any() && output == vm._stateStack.Peek().Output);
            }

            public State(VM vm, RantPattern source, ChannelStack output)
            {
                Output = output;
                Reader = new PatternReader(source);
                SharesOutput = (output == vm._output && vm.PrevState != null) || (vm._stateStack.Any() && output == vm._stateStack.Peek().Output);
            }

            /// <summary>
            /// Adds a pre-blueprint to this state.
            /// </summary>
            /// <param name="bp">The blueprint to set.</param>
            /// <returns></returns>
            public State Pre(Blueprint bp)
            {
                _preBlueprints.Push(bp);
                return this;
            }

            /// <summary>
            /// Consumes the available pre-blueprint, if any. Returns 'true' if either the blueprint hints at the interpreter to skip to the top of the stack, or another blueprint was set during execution.
            /// </summary>
            /// <returns></returns>
            public bool UsePreBlueprint()
            {
                return _preBlueprints.Any() && _preBlueprints.Pop().Use();
            }

            /// <summary>
            /// Consumes the available post-blueprint, if any. Returns 'true' if another blueprint was set during execution.
            /// </summary>
            /// <returns></returns>
            public bool UsePostBlueprint()
            {
                return _postBlueprints.Any() && _postBlueprints.Pop().Use();
            }

            /// <summary>
            /// Adds a post-blueprint to this state.
            /// </summary>
            /// <param name="bp">The blueprint to set.</param>
            /// <returns></returns>
            public State Post(Blueprint bp)
            {
                _postBlueprints.Push(bp);
                return this;
            }

            /// <summary>
            /// Creates a state object that reads tokens from the specified source.
            /// </summary>
            /// <param name="source">The source from which to read tokens.</param>
            /// <param name="interpreter">The interpreter that will read the tokens.</param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static State Create(RantPattern source, VM interpreter)
            {
                return new State(interpreter, source, interpreter.CurrentState.Output);
            }

            /// <summary>
            /// Creates a state object that reads tokens from a custom collection that is associated with the specified source. Specifying an output that is distinct from the one below it in the stack will cause the output to be pushed to the result stack when finished.
            /// </summary>
            /// <param name="derivedSource">The source with which to associate the tokens.</param>
            /// <param name="tokens">The tokens to read.</param>
            /// <param name="interpreter">The interpreter that will read the tokens.</param>
            /// <param name="output">The output of the state. Excluding this will create a new output.</param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static State CreateSub(RantPattern derivedSource, IEnumerable<Token<R>> tokens,
                VM interpreter, ChannelStack output = null)
            {
                return new State(interpreter, derivedSource, tokens, output ?? new ChannelStack(interpreter.Format, interpreter.CharLimit));
            }

            public void Print(string value)
            {
                Output.Write(value);
            }
        }
    }
}