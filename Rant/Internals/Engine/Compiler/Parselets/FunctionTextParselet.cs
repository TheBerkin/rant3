using System.Collections.Generic;

using Rant.Internals.Engine.Compiler.Syntax;
using Rant.Internals.Engine.Framework;
using Rant.Internals.Stringes;

namespace Rant.Internals.Engine.Compiler.Parselets
{
    internal class FunctionTextParselet : Parselet
    {
        [TokenParser]
        private IEnumerable<Parselet> FunctionText(Token<R> token)
        {
            var name = token.Value;
            var group = RantFunctionRegistry.GetFunctionGroup(name);

            if (group == null)
                compiler.SyntaxError(token, $"Unknown function: '{name}'");

            if (reader.TakeLoose(R.Colon))
            {
                foreach (var parselet in FuncArgs(token, group))
                    yield return parselet;
            }
            else
            {
                var end = reader.Read(R.RightSquare);
                AddToOutput(new RAFunction(Stringe.Range(token, end), compiler.GetFunctionInfo(group, 0, token, end), new List<RantAction>()));
            }
        }

        private IEnumerable<Parselet> FuncArgs(Token<R> fromToken, RantFunction group)
        { 
            Token<R> funcToken = null;
            var actions = new List<RantAction>();
            var sequences = new List<RantAction>();

            while (!reader.End)
            {
                funcToken = reader.ReadToken();

                if (funcToken.ID == R.Semicolon)
                {
                    // add action to args and continue
                    sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions, funcToken));
                    actions.Clear();
                    reader.SkipSpace();
                    continue;
                }
                else if (funcToken.ID == R.RightSquare)
                {
                    // add action to args and return
                    sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions, funcToken));
                    AddToOutput(new RAFunction(Stringe.Range(fromToken, funcToken),
                        compiler.GetFunctionInfo(group, sequences.Count, fromToken, funcToken), sequences));
                    yield break;
                }

                yield return GetParselet(funcToken, actions.Add);
            }

            compiler.SyntaxError(fromToken, "Unterminated function: unexpected end of file");
        }
    }
}
