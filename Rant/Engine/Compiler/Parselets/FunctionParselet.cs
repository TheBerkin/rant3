using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    internal class FunctionParselet : Parselet
    {
        [TokenParser(R.LeftSquare)]
        IEnumerable<Parselet> LeftSquare(Token<R> token)
        {
            var tagToken = reader.ReadToken();
            switch (tagToken.ID)
            {
                default:
                    compiler.SyntaxError(tagToken, "Expected function name, subroutine, regex or a script.");
                    break;

                case R.Text:
                    foreach (var parselet in Text(tagToken, token))
                        yield return parselet;
                    break;

                case R.Regex:
                    foreach (var parselet in Regex(tagToken))
                        yield return parselet;
                    break;

                case R.At:
                    AddToOutput(compiler.ReadExpression());
                    break;

                case R.Dollar:
                    foreach (var parselet in Dollar(tagToken))
                        yield return parselet;
                    break;
            }
        }

        // TODO: all these "mini-parselets" are DRY. they're not too easy to extend. improve?
        IEnumerable<Parselet> Dollar(Token<R> token)
        {
            var call = false;
            var nextToken = reader.ReadToken();

            // if the first token isn't a square, it's the name of the subroutine call
            if (nextToken.ID != R.LeftSquare)
                call = true;
            else
                nextToken = reader.Read(R.Text, "subroutine name");

            // if there's a color here, there's args
            var hasArgs = reader.Take(R.Colon);

            if (call)
            {
                foreach (var parselet in SubroutineArgs(token, new RACallSubroutine(nextToken)))
                    yield return parselet;
                yield break;
            }

            var subroutine = new RADefineSubroutine(nextToken);
            subroutine.Parameters = new Dictionary<string, SubroutineParameterType>();

            // read ALL THE ARGS!!
            // right now we're at [$[name: or [$[name
            while (hasArgs && !reader.End)
            {
                var nameToken = reader.ReadLooseToken();

                if (nameToken.ID == R.Text)
                    subroutine.Parameters.Add(nameToken.Value, SubroutineParameterType.Greedy);
                else if (nameToken.ID == R.At)
                {
                    nameToken = reader.ReadLoose(R.Text);
                    subroutine.Parameters.Add(nameToken.Value, SubroutineParameterType.Loose);
                }
                else
                    compiler.SyntaxError(nameToken, "Expected subroutine parameter");

                nextToken = reader.ReadLooseToken();

                if (nextToken.ID == R.Semicolon)
                    continue;

                if (nextToken.ID == R.RightSquare)
                    break;

                compiler.SyntaxError(nextToken, "Unexpected token in subroutine parameter definition");
            }

            // if there's no args, we need to get rid of the right square before the body
            if (!hasArgs)
                reader.ReadLoose(R.RightSquare);

            // start reading the body
            reader.ReadLoose(R.Colon);

            RASequence body = null;

            foreach (var parselet in SubroutineBody(token, b => body = b))
                yield return parselet;

            subroutine.Body = body;
            AddToOutput(subroutine);
        }

        IEnumerable<Parselet> Regex(Token<R> token)
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

        IEnumerable<Parselet> Text(Token<R> token, Token<R> fromToken)
        {
            var name = token.Value;
            var group = RantFunctions.GetFunctionGroup(name);

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
                AddToOutput(new RAFunction(Stringe.Range(fromToken, end), compiler.GetFunctionInfo(group, 0, fromToken, end), new List<RantAction>()));
            }
        }

        // TODO: all these "mini-compilers" are DRY. they're not too easy to extend. improve?
        IEnumerable<Parselet> SubroutineBody(Token<R> fromToken, Action<RASequence> setSequence)
        {
            Token<R> funcToken = null;
            var actions = new List<RantAction>();
            var sequences = new List<RantAction>();

            while (!reader.End)
            {
                funcToken = reader.ReadToken();

                if (funcToken.ID == R.RightSquare)
                {
                    // add action to args and return
                    sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions, funcToken));
                    setSequence(new RASequence(sequences, funcToken));
                    yield break;
                }

                yield return Parselet.GetParselet(funcToken, actions.Add);
            }

            compiler.SyntaxError(fromToken, "Unterminated function: unexpected end of file");
        }

        IEnumerable<Parselet> SubroutineArgs(Token<R> fromToken, RASubroutine subroutine)
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
                    subroutine.Arguments = sequences.Where(x => !(x is RASequence && (x as RASequence).Actions.Count == 0)).ToList();
                    AddToOutput(subroutine);
                    yield break;
                }

                yield return Parselet.GetParselet(funcToken, actions.Add);
            }

            compiler.SyntaxError(fromToken, "Unterminated function: unexpected end of file");
        }

        IEnumerable<Parselet> ReplacerArgs(Token<R> fromToken, Regex regex)
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

                yield return Parselet.GetParselet(funcToken, actions.Add);
            }

            compiler.SyntaxError(fromToken, "Unterminated function: unexpected end of file");
        }

        IEnumerable<Parselet> FuncArgs(Token<R> fromToken, RantFunctionGroup group)
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

                yield return Parselet.GetParselet(funcToken, actions.Add);
            }

            compiler.SyntaxError(fromToken, "Unterminated function: unexpected end of file");
        }
    }
}
