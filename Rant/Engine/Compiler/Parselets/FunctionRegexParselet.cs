using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    internal class FunctionRegexParselet : Parselet
    {
        [TokenParser]
        private IEnumerable<Parselet> FunctionRegex(Token<R> token)
        {
            Regex regex = null;
            try
            {
                regex = Util.ParseRegex(token.Value);
            }
            catch (Exception e)
            {
                compiler.SyntaxError(token, e);
            }

            reader.ReadLoose(R.Colon);

            foreach (var parselet in ReplacerArgs(token, regex))
                yield return parselet;
        }

        private IEnumerable<Parselet> ReplacerArgs(Token<R> fromToken, Regex regex)
        {
            Token<R> funcToken = null;
            var actions = new List<RantAction>();
            var sequences = new List<RantAction>();

            while (!reader.End)
            {
                funcToken = reader.ReadToken();

                if (funcToken.ID == R.Whitespace)
                {
                    switch (reader.PeekType())
                    {
                        case R.RightSquare:
                            continue;
                    }
                }
                else if (funcToken.ID == R.Semicolon)
                {
                    // add action to args and continue
                    if (sequences.Count == 1)
                        compiler.SyntaxError(funcToken, "Too many arguments in replacer");

                    sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions, funcToken));
                    actions.Clear();
                    reader.SkipSpace();
                    continue;
                }
                else if (funcToken.ID == R.RightSquare)
                {
                    // add action to args and return
                    sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions, funcToken));

                    if (sequences.Count != 2)
                        compiler.SyntaxError(Stringe.Between(funcToken, fromToken), "Replacer must have two arguments");

                    AddToOutput(new RAReplacer(Stringe.Range(fromToken, funcToken), regex, sequences[0], sequences[1]));
                    yield break;
                }

                yield return GetParselet(funcToken, actions.Add);
            }

            compiler.SyntaxError(fromToken, "Unterminated function: unexpected end of file");
        }
    }
}
