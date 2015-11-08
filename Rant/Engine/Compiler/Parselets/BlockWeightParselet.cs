using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    internal class BlockWeightParselet : Parselet
    {
        [TokenParser]
        IEnumerable<Parselet> BlockWeight(Token<R> token)
        {
            Token<R> funcToken = null;
            var actions = new List<RantAction>();

            while (!reader.End)
            {
                funcToken = reader.ReadToken();

                if (funcToken.ID == R.RightParen)
                {
                    reader.SkipSpace();

                    if (!actions.Any())
                        compiler.SyntaxError(funcToken, "Expected weight value");

                    AddToOutput(actions.Count == 1 && actions[0] is RAText ? actions[0] : new RASequence(actions, funcToken));
                    yield break;
                }

                yield return Parselet.GetParselet(funcToken, actions.Add);
            }

            // TODO: this token is "our" (the one that result is us being yielded) token. maybe have the 'fromToken' passed?
            compiler.SyntaxError(token, "Unterminated function: unexpected end of file");
        }
    }
}
