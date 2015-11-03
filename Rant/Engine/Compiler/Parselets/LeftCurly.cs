using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    internal class LeftCurly : Parselet
    {
        public override R[] Identifiers
        {
            get
            {
                return new[] { R.LeftCurly };
            }
        }

        public LeftCurly()
        {
        }

        public override IEnumerator<Parselet> Parse(NewRantCompiler compiler, TokenReader reader, Token<R> fromToken)
        {
            reader.SkipSpace();

            // LOOK AT ME. I'M THE COMPILER NOW
            Token<R> token = null;
            var actions = new List<RantAction>();
            var sequences = new List<RantAction>();

            while (!reader.End)
            {
                reader.SkipSpace();
                token = reader.ReadToken();

                if (token.ID == R.Pipe)
                {
                    // add action to block and continue
                    sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions, Token));
                    reader.SkipSpace();
                    actions.Clear();
                    continue;
                }
                else if (token.ID == R.RightCurly)
                {
                    // add action to block and return
                    sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions, Token));
                    AddToOutput(new RABlock(Stringe.Range(Token, token), sequences));
                    yield break;
                }

                yield return Parselet.GetWithToken(compiler, token, actions.Add);
            }

            compiler.SyntaxError(Token, "Unterminated block: unexpected end of file.");
        }
    }
}
