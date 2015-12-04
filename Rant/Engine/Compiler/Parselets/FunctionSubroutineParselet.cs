using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    internal class FunctionSubroutineParselet : Parselet
    {
        [TokenParser]
        private IEnumerable<Parselet> FunctionSubroutine(Token<R> token)
        {
            var call = false;
            var nextToken = reader.ReadToken();
			var inModule = false;
			string functionName = null;

			// if the first token isn't a square, it's the name of the subroutine call
			if (nextToken.ID != R.LeftSquare)
			{
				call = true;

				// probably an in-module subroutine call
				if (reader.PeekType() == R.Subtype)
				{
					reader.ReadToken();
					var name = reader.Read(R.Text, "subroutine name");
					functionName = name.Value;
					inModule = true;
				}
			}
			else
			{
				if (reader.PeekType() == R.Subtype)
				{
					compiler.HasModule = true;
					inModule = true;
					reader.ReadToken();
				}
				nextToken = reader.Read(R.Text, "subroutine name");
			}

            // if there's a colon here, there's args
            var hasArgs = reader.Take(R.Colon);

            if (call)
            {
                foreach (var parselet in SubroutineArgs(token, nextToken, functionName))
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

            foreach (var parselet in SubroutineBody(token, a => body = a))
                yield return parselet;

            subroutine.Body = body;
			if(inModule)
				compiler.Module.AddActionFunction(subroutine.Name, subroutine);
            AddToOutput(subroutine);
        }

        private IEnumerable<Parselet> SubroutineBody(Token<R> fromToken, Action<RASequence> setSequence)
        {
            Token<R> funcToken = null;
            var actions = new List<RantAction>();
            var sequences = new List<RantAction>();

            while (!reader.End)
            {
                funcToken = reader.ReadToken();

                // TODO: this has to be made not-so-DRY
                if (funcToken.ID == R.Whitespace)
                {
                    switch (reader.PeekType())
                    {
                        case R.RightSquare:
                            continue;
                    }
                }
                else if (funcToken.ID == R.RightSquare)
                {
                    // add action to args and return
                    sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions, funcToken));
                    setSequence(new RASequence(sequences, funcToken));
                    yield break;
                }

                yield return GetParselet(funcToken, actions.Add);
            }

            compiler.SyntaxError(fromToken, "Unterminated function: unexpected end of file");
        }

        private IEnumerable<Parselet> SubroutineArgs(Token<R> fromToken, Token<R> token, string functionName)
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
                    sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions, funcToken));
                    actions.Clear();
                    reader.SkipSpace();
                    continue;
                }
                else if (funcToken.ID == R.RightSquare)
                {
                    // add action to args and return
                    sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions, funcToken));

                    var subroutine = new RACallSubroutine(token, functionName);
                    subroutine.Arguments = sequences.Where(x => !(x is RASequence && (x as RASequence).Actions.Count == 0)).ToList();
                    AddToOutput(subroutine);
                    yield break;
                }

                yield return GetParselet(funcToken, actions.Add);
            }

            compiler.SyntaxError(fromToken, "Unterminated function: unexpected end of file");
        }
    }
}
