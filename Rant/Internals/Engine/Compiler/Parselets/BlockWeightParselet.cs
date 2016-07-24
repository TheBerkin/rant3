using System.Collections.Generic;
using System.Linq;

using Rant.Internals.Engine.Compiler.Syntax;
using Rant.Internals.Stringes;

namespace Rant.Internals.Engine.Compiler.Parselets
{
    internal class BlockWeightParselet : Parselet
    {
        [TokenParser]
        private IEnumerable<Parselet> BlockWeight(Token<R> token)
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

					RantAction weightAction;
					// probably a number
					if (actions[0] is RAText)
					{
						if (actions.Count > 1)
						{
							if (!(actions[1] is RAText) || ((RAText)actions[1]).Text != "." || actions.Count != 3)
								compiler.SyntaxError(actions[1].Range, "Invalid block weight value.");
							weightAction = new RAText(actions[0].Range,
								(actions[0] as RAText).Text +
								(actions[1] as RAText).Text +
								(actions[2] as RAText).Text);
						}
						else
							weightAction = actions[0];
					}
					else
						weightAction = new RASequence(actions, funcToken);

                    AddToOutput(weightAction);
                    yield break;
                }

                yield return Parselet.GetParselet(funcToken, actions.Add);
            }

            // TODO: this token is "our" (the one that result is us being yielded) token. maybe have the 'fromToken' passed?
            compiler.SyntaxError(token, "Unterminated function: unexpected end of file");
        }
    }
}
