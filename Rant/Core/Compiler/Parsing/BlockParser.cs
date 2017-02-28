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

using Rant.Core.Compiler.Syntax;

namespace Rant.Core.Compiler.Parsing
{
	internal class BlockParser : Parser
	{
		public override IEnumerator<Parser> Parse(RantCompiler compiler, CompileContext context, TokenReader reader,
			Action<RST> actionCallback)
		{
			var blockStartToken = reader.PrevLooseToken;
			var items = new List<RST>();
			var actions = new List<RST>();

			// "why are these not lists or arrays" i yell into the void, too lazy to find out why
			List<_<int, double>> constantWeights = null;
			List<_<int, RST>> dynamicWeights = null;
			int blockNumber = 0;
			Action<RST> itemCallback = action => actions.Add(action);

			compiler.AddContext(CompileContext.BlockEndSequence);
			compiler.AddContext(CompileContext.BlockSequence);

			while (compiler.NextContext == CompileContext.BlockSequence)
			{
				// block weight
				if (reader.PeekLooseToken().Type == R.LeftParen)
				{
					constantWeights = constantWeights ?? (constantWeights = new List<_<int, double>>());
					dynamicWeights = dynamicWeights ?? (dynamicWeights = new List<_<int, RST>>());

					var firstToken = reader.ReadLooseToken();

					// constant weight
					if (reader.PeekLooseToken().Type == R.Text)
					{
						string value = reader.ReadLooseToken().Value;
						double doubleValue;
						if (!double.TryParse(value, out doubleValue))
							compiler.SyntaxError(reader.PrevLooseToken, false, "err-compiler-invalid-constweight");
						else
							constantWeights.Add(new _<int, double>(blockNumber, doubleValue));
						reader.Read(R.RightParen);
					}
					// dynamic weight
					else
					{
						var weightActions = new List<RST>();

						Action<RST> weightActionCallback = rst => weightActions.Add(rst);

						compiler.SetNextActionCallback(weightActionCallback);
						compiler.AddContext(CompileContext.BlockWeight);
						yield return Get<SequenceParser>();

						if (weightActions.Count == 0)
							compiler.SyntaxError(firstToken, false, "err-compiler-empty-weight");
						else
							dynamicWeights.Add(new _<int, RST>(blockNumber, new RstSequence(weightActions, weightActions[0].Location)));
					}
				}

				compiler.SetNextActionCallback(itemCallback);
				var startToken = reader.PeekToken();
				yield return Get<SequenceParser>();
				items.Add(new RstSequence(actions, startToken.ToLocation()));
				actions.Clear();
				blockNumber++;
			}

			compiler.LeaveContext();
			compiler.SetNextActionCallback(actionCallback);

			actionCallback(new RstBlock(blockStartToken.ToLocation(), items, dynamicWeights, constantWeights));
		}
	}
}