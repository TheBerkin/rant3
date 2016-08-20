using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Rant.Core.Compiler.Syntax;
using Rant.Vocabulary.Querying;
using Rant.Core.Utilities;
using Rant.Core.Framework;
using Rant.Core.Stringes;
using Rant.Localization;

namespace Rant.Core.Compiler.Parsing
{
	internal class TagParser : Parser
	{
		public override IEnumerator<Parser> Parse(RantCompiler compiler, CompileContext context, TokenReader reader, Action<RST> actionCallback)
		{
			var nextType = reader.PeekType();

			var tagStart = reader.PrevToken;

			// replacer
			if (nextType == R.Regex)
			{
				var regex = reader.Read(R.Regex, "replacer regex");
				reader.Read(R.Colon);

				var arguments = new List<RST>();

				var iterator = ReadArguments(compiler, reader, arguments);
				while (iterator.MoveNext())
				{
					yield return iterator.Current;
				}

				compiler.SetNextActionCallback(actionCallback);
				if (arguments.Count < 2)
				{
					compiler.SyntaxError(regex, false, "replacer requires source text and replacement pattern.");
					reader.Read(R.RightSquare, "replacer end");
					yield break;
				}

				if (arguments.Count > 2)
				{
					compiler.SyntaxError(Stringe.Range(tagStart, reader.PrevToken), false, "replacer only takes two arguments.");
					reader.Read(R.RightSquare, "replacer end");
					yield break;
				}

				actionCallback(new RstReplacer(regex, Util.ParseRegex(regex.Value), arguments[0], arguments[1]));
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
			}
			// function
			else
			{
				var e = ParseFunction(compiler, context, reader, actionCallback);
				while (e.MoveNext())
				{
					yield return e.Current;
				}
			}
		}

		private IEnumerator<Parser> ParseFunction(RantCompiler compiler, CompileContext context, TokenReader reader, Action<RST> actionCallback)
		{
			var functionName = reader.Read(R.Text, "function name");

			var arguments = new List<RST>();

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
				compiler.SyntaxError(functionName, false, "err-compiler-nonexistent-function", functionName.Value);
				yield break;
			}

			var sig = RantFunctionRegistry.GetFunction(functionName.Value, arguments.Count);
			if (sig == null)
			{
				compiler.SyntaxError(functionName, false, "err-compiler-nonexistent-overload", functionName.Value, arguments.Count);
				yield break;
			}

			actionCallback(new RstFunction(functionName, sig, arguments));
			yield break;
		}

		private IEnumerator<Parser> ParseSubroutine(RantCompiler compiler, CompileContext context, TokenReader reader, Action<RST> actionCallback)
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
				var subroutine = new RstDefineSubroutine(subroutineName);
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

				var actions = new List<RST>();
				Action<RST> bodyActionCallback = (action) => actions.Add(action);

				compiler.AddContext(CompileContext.SubroutineBody);
				compiler.SetNextActionCallback(bodyActionCallback);
				yield return Get<SequenceParser>();
				compiler.SetNextActionCallback(actionCallback);

				subroutine.Body = new RstSequence(actions, bodyStart);
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

				var arguments = new List<RST>();

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

				var subroutine = new RstCallSubroutine(subroutineName, moduleFunctionName);
				subroutine.Arguments = arguments;

				actionCallback(subroutine);
				yield break;
			}
		}

		private IEnumerator<Parser> ReadArguments(RantCompiler compiler, TokenReader reader, List<RST> arguments)
		{
			var actions = new List<RST>();

			Action<RST> argActionCallback = (action) => actions.Add(action);
			compiler.SetNextActionCallback(argActionCallback);
			compiler.AddContext(CompileContext.FunctionEndContext);
			compiler.AddContext(CompileContext.ArgumentSequence);

			while (compiler.NextContext == CompileContext.ArgumentSequence)
			{
				var startToken = reader.PeekToken();
				yield return Get<SequenceParser>();
				arguments.Add(new RstSequence(actions, startToken));
				actions.Clear();
			}

			compiler.LeaveContext();
		}
	}
}
