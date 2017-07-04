#region License

// https://github.com/TheBerkin/Rant
// 
// Copyright (c) 2017 Nicholas Fleck
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Rant.Core.Compiler.Syntax;
using Rant.Core.Framework;

namespace Rant.Core.Compiler.Parsing
{
	internal class TagParser : Parser
	{
		public override IEnumerator<Parser> Parse(RantCompiler compiler, CompileContext context, TokenReader reader,
			Action<RST> actionCallback)
		{
			var nextType = reader.PeekType();

			var tagStart = reader.PrevToken;

			// replacer
			switch (nextType)
			{
				case R.Regex:
				{
					var regex = reader.Read(R.Regex, "acc-replacer-regex");
					var options = RegexOptions.Compiled | RegexOptions.ExplicitCapture;
					if (reader.IsNext(R.RegexFlags))
					{
						var flagsToken = reader.ReadToken();
						foreach (char flag in flagsToken.Value)
						{
							switch (flag)
							{
								case 'i':
								options |= RegexOptions.IgnoreCase;
								break;
								case 'm':
								options |= RegexOptions.Multiline;
								break;
							}
						}
					}

					reader.Read(R.Colon);

					var arguments = new List<RST>();

					var iterator = ReadArguments(compiler, reader, arguments);
					while (iterator.MoveNext())
						yield return iterator.Current;

					compiler.SetNextActionCallback(actionCallback);

					if (arguments.Count != 2)
					{
						compiler.SyntaxError(tagStart, reader.PrevToken, false, "err-compiler-replacer-argcount");
						yield break;
					}

					actionCallback(new RstReplacer(regex.ToLocation(), new Regex(regex.Value, options), arguments[0], arguments[1]));
				}
				break;
				case R.Dollar:
				{
					reader.ReadToken();
					var e = ParseSubroutine(compiler, context, reader, actionCallback);
					while (e.MoveNext())
						yield return e.Current;
				}
				break;
				default:
				{
					var e = ParseFunction(compiler, context, reader, actionCallback);
					while (e.MoveNext())
						yield return e.Current;
				}
				break;
			}
		}

		private IEnumerator<Parser> ParseFunction(RantCompiler compiler, CompileContext context, TokenReader reader,
			Action<RST> actionCallback)
		{
			var functionName = reader.Read(R.Text, "acc-function-name");

			var arguments = new List<RST>();

			if (reader.PeekType() == R.Colon)
			{
				reader.ReadToken();

				var iterator = ReadArguments(compiler, reader, arguments);
				while (iterator.MoveNext())
					yield return iterator.Current;

				compiler.SetNextActionCallback(actionCallback);
			}
			else
			{
				reader.Read(R.RightSquare);
			}

			RantFunctionSignature sig = null;

			if (functionName.Value != null)
			{
				if (!RantFunctionRegistry.FunctionExists(functionName.Value))
				{
					compiler.SyntaxError(functionName, false, "err-compiler-nonexistent-function", functionName.Value, RantUtils.GetClosestFunctionName(functionName.Value));
					yield break;
				}

				if ((sig = RantFunctionRegistry.GetFunction(functionName.Value, arguments.Count)) == null)
				{
					compiler.SyntaxError(functionName, false, "err-compiler-nonexistent-overload", functionName.Value, arguments.Count);
					yield break;
				}

				actionCallback(new RstFunction(functionName.ToLocation(), sig, arguments));
			}
		}

		private IEnumerator<Parser> ParseSubroutine(RantCompiler compiler, CompileContext context, TokenReader reader,
			Action<RST> actionCallback)
		{
			// subroutine definition
			if (reader.TakeLoose(R.LeftSquare, false))
			{
				if (reader.TakeLoose(R.Period))
				{
					compiler.HasModule = true;
				}
				var subroutineName = reader.ReadLoose(R.Text, "acc-subroutine-name");
				var subroutine = new RstDefineSubroutine(subroutineName.ToLocation())
				{
					Parameters = new Dictionary<string, SubroutineParameterType>(),
					Name = subroutineName.Value
				};

				if (reader.PeekLooseToken().Type == R.Colon)
				{
					reader.ReadLooseToken();

					do
					{
						var type = SubroutineParameterType.Greedy;
						if (reader.TakeLoose(R.At))
							type = SubroutineParameterType.Loose;

						subroutine.Parameters[reader.ReadLoose(R.Text, "acc-arg-name").Value] = type;
					} while (reader.TakeLoose(R.Semicolon, false));
				}

				reader.ReadLoose(R.RightSquare);

				var bodyStart = reader.ReadLoose(R.Colon);

				var actions = new List<RST>();
				Action<RST> bodyActionCallback = action => actions.Add(action);

				compiler.AddContext(CompileContext.SubroutineBody);
				compiler.SetNextActionCallback(bodyActionCallback);
				yield return Get<SequenceParser>();
				compiler.SetNextActionCallback(actionCallback);
				if (actions.Count == 1)
				{
					subroutine.Body = actions[0];
				}
				else
				{
					subroutine.Body = new RstSequence(actions, bodyStart.ToLocation());
				}				
				actionCallback(subroutine);
			}
			else
			{
				// subroutine call
				var subroutineName = reader.Read(R.Text, "acc-subroutine-name");
				string moduleFunctionName = null;

				if (reader.TakeLoose(R.Period, false))
					moduleFunctionName = reader.Read(R.Text, "module function name").Value;

				var arguments = new List<RST>();

				if (reader.PeekType() == R.Colon)
				{
					reader.ReadToken();

					var iterator = ReadArguments(compiler, reader, arguments);
					while (iterator.MoveNext())
						yield return iterator.Current;

					compiler.SetNextActionCallback(actionCallback);
				}
				else
				{
					reader.Read(R.RightSquare);
				}

				var subroutine = new RstCallSubroutine(subroutineName.Value, subroutineName.ToLocation(), moduleFunctionName) { Arguments = arguments };

				actionCallback(subroutine);
			}
		}

		private IEnumerator<Parser> ReadArguments(RantCompiler compiler, TokenReader reader, List<RST> arguments)
		{
			var actions = new List<RST>();

			Action<RST> argActionCallback = action => actions.Add(action);
			compiler.SetNextActionCallback(argActionCallback);
			compiler.AddContext(CompileContext.FunctionEndContext);
			compiler.AddContext(CompileContext.ArgumentSequence);

			while (compiler.NextContext == CompileContext.ArgumentSequence)
			{
				reader.SkipSpace();
				var startToken = reader.PeekToken();
				yield return Get<SequenceParser>();

				// Don't wrap single nodes in a sequence, it's unnecessary
				if (actions.Count == 1)
				{
					arguments.Add(actions[0]);
				}
				else
				{
					arguments.Add(new RstSequence(actions, startToken.ToLocation()));
				}

				actions.Clear();
			}

			compiler.LeaveContext();
		}
	}
}