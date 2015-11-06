using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    internal class BlockParselet : Parselet
    {
        [TokenParser(R.RightCurly)]
        IEnumerable<Parselet> RightCurly(Token<R> token)
        {
            compiler.SyntaxError(token, "Unexpected block terminator");
            yield break;
        }

        [TokenParser(R.LeftCurly)]
        IEnumerable<Parselet> LeftCurly(Token<R> token)
        {
            reader.SkipSpace();

            // LOOK AT ME. I'M THE COMPILER NOW
            Token<R> readToken = null;
            var actions = new List<RantAction>();
            var sequences = new List<RantAction>();

            var constantWeights = new List<_<int, double>>();
            var dynamicWeights = new List<_<int, RantAction>>();

            while (!reader.End)
            {
                reader.SkipSpace();
                readToken = reader.ReadToken();

                if (readToken.ID == R.LeftParen) // weight
                {
                    RantAction weightAction = null;

                    foreach (var parselet in BlockWeight(token, a => weightAction = a))
                        yield return parselet;

                    if (weightAction is RAText) // constant
                    {
                        var strWeight = (weightAction as RAText).Text;
                        double d;
                        int i;

                        if (!Double.TryParse(strWeight, out d))
                        {
                            if (Util.ParseInt(strWeight, out i))
                                d = 1;
                            else
                                compiler.SyntaxError(weightAction.Range, $"Invalid weight value '{strWeight}' - constant must be a number.");
                        }

                        if (d < 0)
                            compiler.SyntaxError(weightAction.Range, $"Invalid weight value '{strWeight}' - constant cannot be a negative.");

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

        IEnumerable<Parselet> BlockWeight(Token<R> fromToken, Action<RantAction> setAction)
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

                    setAction(actions.Count == 1 && actions[0] is RAText ? actions[0] : new RASequence(actions, funcToken));
                    yield break;
                }

                yield return Parselet.GetParselet(funcToken, actions.Add);
            }

            compiler.SyntaxError(fromToken, "Unterminated function: unexpected end of file");
        }
    }
}
