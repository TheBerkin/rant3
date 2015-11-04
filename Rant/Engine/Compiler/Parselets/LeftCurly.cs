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

        protected override IEnumerable<Parselet> InternalParse(Token<R> token, Token<R> fromToken)
        {
            reader.SkipSpace();

            // LOOK AT ME. I'M THE COMPILER NOW
            Token<R> readToken = null;
            var actions = new List<RantAction>();
            var sequences = new List<RantAction>();

            while (!reader.End)
            {
                reader.SkipSpace();
                readToken = reader.ReadToken();

                if (readToken.ID == R.Pipe)
                {
                    // add action to block and continue
                    sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions, readToken));
                    reader.SkipSpace();
                    actions.Clear();
                    continue;
                }
                else if (readToken.ID == R.RightCurly)
                {
                    // add action to block and return
                    sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions, readToken));
                    AddToOutput(new RABlock(Stringe.Range(token, readToken), sequences));
                    yield break;
                }

                yield return Parselet.GetParselet(readToken, actions.Add);
            }

            compiler.SyntaxError(token, "Unterminated block: unexpected end of file.");
        }
    }
}
