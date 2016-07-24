using System.Collections.Generic;

using Rant.Internals.Engine.Compiler.Syntax;
using Rant.Internals.Engine.Utilities;
using Rant.Internals.Stringes;

namespace Rant.Internals.Engine.Compiler.Parselets
{
    internal class BlockParselet : Parselet
    {
        [TokenParser(R.RightCurly)]
        private IEnumerable<Parselet> RightCurly(Token<R> token)
        {
            compiler.SyntaxError(token, "Unexpected block terminator");
            yield break;
        }

        [TokenParser(R.LeftCurly)]
        private IEnumerable<Parselet> LeftCurly(Token<R> token)
        {
            reader.SkipSpace();

            // LOOK AT ME. I'M THE COMPILER NOW
            Token<R> readToken = null;
            var actions = new List<RantAction>();
            var sequences = new List<RantAction>();

	        List<_<int, double>> constantWeights = null;
			List<_<int, RantAction>> dynamicWeights = null;

            while (!reader.End)
            {
                readToken = reader.ReadToken();

                // TODO: kinda stupid having this handle it's own whitespace when we have a parselet for whitespace
                if (readToken.ID == R.Whitespace)
                {
                    switch (reader.PeekType())
                    {
                        case R.RightCurly:
                        case R.Pipe:
                            continue;
                    }
                }
                else if (readToken.ID == R.LeftParen) // weight
                {
                    RantAction weightAction = null;

                    // i like this AddToOutput thing because it's just a delegate that takes in a RantAction.
                    // it can do anything with the RantAction, in this case it sets it to our weightAction
                    // :>
                    yield return Parselet.GetParselet("BlockWeight", readToken, a => weightAction = a);

					constantWeights = constantWeights ?? new List<_<int, double>>();
					dynamicWeights = dynamicWeights ?? new List<_<int, RantAction>>();

                    if (weightAction is RAText) // constant
                    {
                        var strWeight = (weightAction as RAText).Text;
                        double d;

                        if (!Util.ParseDouble(strWeight, out d))
                        {
							compiler.SyntaxError(weightAction.Range, $"Invalid weight value '{strWeight}'.");
                        }
						
                        constantWeights.Add(_.Create(sequences.Count, d));
                    }
                    else // dynamic
                    {
                        // TODO: there's some weird issue going on with doubles being seen as dynamic weights
                        dynamicWeights.Add(_.Create(sequences.Count, weightAction));
                    }

                    continue;
                }
                else if (readToken.ID == R.Pipe)
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
                    AddToOutput(new RABlock(Stringe.Range(token, readToken), sequences, dynamicWeights, constantWeights));
                    yield break;
                }

                yield return Parselet.GetParselet(readToken, actions.Add);
            }

            compiler.SyntaxError(token, "Unterminated block: unexpected end of file.");
        }
    }
}
