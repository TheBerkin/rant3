using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Rant.Core.Compiler.Syntax;
using Rant.Vocabulary.Querying;
using Rant.Core.Utilities;
using Rant.Core.Framework;
using Rant.Localization;

namespace Rant.Core.Compiler.Parsing
{
	internal class TagParser : Parser
	{
		public override IEnumerator<Parser> Parse(RantCompiler compiler, CompileContext context, TokenReader reader, Action<RantAction> actionCallback)
		{
			var nextType = reader.PeekType();

			// replacer
			if (nextType == R.Regex)
			{
				var regex = reader.Read(R.Regex, "replacer regex");
				reader.Read(R.Colon);

				var arguments = new List<RantAction>();

				var iterator = ReadArguments(compiler, reader, arguments);
				while (iterator.MoveNext())
				{
					yield return iterator.Current;
				}

				compiler.SetNextActionCallback(actionCallback);
				if (arguments.Count < 2)
				{
					compiler.SyntaxError(regex, "replacer requires source text and replacement pattern.", false);
					reader.Read(R.RightSquare, "replacer end");
					yield break;
				}
				else if (arguments.Count > 2)
				{
					compiler.SyntaxError(arguments[2].Range, "replacer only takes two arguments.", false);
					reader.Read(R.RightSquare, "replacer end");
					yield break;
				}

				actionCallback(new RAReplacer(regex, Util.ParseRegex(regex.Value), arguments[0], arguments[1]));
				yield break;
			}
			// subroutine
			else if (nextType == R.Dollar)
			{
				reader.ReadToken();
				var e = ParseSubroutine(compiler, context, reader, actionCallback);
				while (e.MoveNext())
				{
					yield return e.Current;
				}
				yield break;
			}
			// function
			else
			{
				var e = ParseFunction(compiler, context, reader, actionCallback);
				while (e.MoveNext())
				{
					yield return e.Current;
				}

				yield break;
			}
		}

		private IEnumerator<Parser> ParseFunction(RantCompiler compiler, CompileContext context, TokenReader reader, Action<RantAction> actionCallback)
		{
			var functionName = reader.Read(R.Text, "function name");

			var arguments = new List<RantAction>();

			if (reader.PeekType() == R.Colon)
			{
				reader.ReadToken();

				var iterator = ReadArguments(compiler, reader, arguments);
				while (iterator.MoveNext())
				{
					yield return iterator.Current;
				}

				compiler.SetNextActionCallback(actionCallback);
			}
			else
			{
				reader.Read(R.RightSquare, "function tag end");
			}

			if (!RantFunctionRegistry.FunctionExists(functionName.Value))
			{
				compiler.SyntaxError(functionName, Txtres.GetString("err-compiler-nonexistent-function", functionName.Value), false);
				yield break;
			}

			var sig = RantFunctionRegistry.GetFunction(functionName.Value, arguments.Count);
			if (sig == null)
			{
				compiler.SyntaxError(functionName, "function " + functionName.Value + " has no overload with " + arguments.Count + " arguments.", false);
				yield break;
			}

			actionCallback(new RAFunction(functionName, sig, arguments));
			yield break;
		}

		private IEnumerator<Parser> ParseSubroutine(RantCompiler compiler, CompileContext context, TokenReader reader, Action<RantAction> actionCallback)
		{
			// subroutine definition
			if (reader.TakeLoose(R.LeftSquare, false))
			{
				var inModule = false;

				if (reader.TakeLoose(R.Subtype))
				{
					inModule = true;
					compiler.HasModule = true;
				}
				var subroutineName = reader.ReadLoose(R.Text, "subroutine name");
				var subroutine = new RADefineSubroutine(subroutineName);
				subroutine.Parameters = new Dictionary<string, SubroutineParameterType>();

				if (reader.PeekLooseToken().ID == R.Colon)
				{
					reader.ReadLooseToken();

					do
					{
						var type = SubroutineParameterType.Greedy;
						if (reader.TakeLoose(R.At))
						{
							type = SubroutineParameterType.Loose;
						}

						subroutine.Parameters[reader.ReadLoose(R.Text, "argument name").Value] = type;
					} while (reader.TakeLoose(R.Semicolon, false));
				}

				reader.ReadLoose(R.RightSquare, "end of subroutine definition arguments");
				var bodyStart = reader.ReadLoose(R.Colon);

				var actions = new List<RantAction>();
				Action<RantAction> bodyActionCallback = (action) => actions.Add(action);

				compiler.AddContext(CompileContext.SubroutineBody);
				compiler.SetNextActionCallback(bodyActionCallback);
				yield return Get<SequenceParser>();
				compiler.SetNextActionCallback(actionCallback);

				subroutine.Body = new RASequence(actions, bodyStart);
				if (inModule)
				{
					compiler.Module.AddActionFunction(subroutineName.Value, subroutine);
				}
				actionCallback(subroutine);
				yield break;
			}
			else
			{
				// subroutine call
				var subroutineName = reader.Read(R.Text, "subroutine name");
				string moduleFunctionName = null;

				if (reader.TakeLoose(R.Subtype, false))
				{
					moduleFunctionName = reader.Read(R.Text, "module function name").Value;
				}

				var arguments = new List<RantAction>();

				if (reader.PeekType() == R.Colon)
				{
					reader.ReadToken();

					var iterator = ReadArguments(compiler, reader, arguments);
					while (iterator.MoveNext())
					{
						yield return iterator.Current;
					}

					compiler.SetNextActionCallback(actionCallback);
				}
				else
				{
					reader.Read(R.RightSquare, "function tag end");
				}

				var subroutine = new RACallSubroutine(subroutineName, moduleFunctionName);
				subroutine.Arguments = arguments;

				actionCallback(subroutine);
				yield break;
			}
		}

		private IEnumerator<Parser> ReadArguments(RantCompiler compiler, TokenReader reader, List<RantAction> arguments)
		{
			var actions = new List<RantAction>();

			Action<RantAction> argActionCallback = (action) => actions.Add(action);
			compiler.SetNextActionCallback(argActionCallback);
			compiler.AddContext(CompileContext.FunctionEndContext);
			compiler.AddContext(CompileContext.ArgumentSequence);

			while (compiler.NextContext == CompileContext.ArgumentSequence)
			{
				var startToken = reader.PeekToken();
				yield return Get<SequenceParser>();
				arguments.Add(new RASequence(actions, startToken));
				actions.Clear();
			}

			compiler.LeaveContext();
		}
	}
}
