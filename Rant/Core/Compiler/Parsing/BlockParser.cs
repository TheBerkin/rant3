using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Rant.Core.Compiler.Syntax;
using Rant.Vocabulary.Querying;
using Rant.Core.Utilities;
using Rant.Core.Framework;

namespace Rant.Core.Compiler.Parsing
{
	internal class BlockParser : Parser
	{
		public override IEnumerator<Parser> Parse(RantCompiler compiler, CompileContext context, TokenReader reader, Action<RantAction> actionCallback)
		{
			var blockStartToken = reader.PrevLooseToken;
			var items = new List<RantAction>();
			var actions = new List<RantAction>();

			// "why are these not lists or arrays" i yell into the void, too lazy to find out why
			List<_<int, double>> constantWeights = null;
			List<_<int, RantAction>> dynamicWeights = null;
			var blockNumber = 0;
			Action<RantAction> itemCallback = (action) => actions.Add(action);

			compiler.SetNextActionCallback(itemCallback);
			compiler.AddContext(CompileContext.BlockEndSequence);
			compiler.AddContext(CompileContext.BlockSequence);

			//reader.SkipSpace();

			while (compiler.NextContext == CompileContext.BlockSequence)
			{
				// block weight
				if (reader.PeekLooseToken().ID == R.LeftParen)
				{
					constantWeights = constantWeights ?? (constantWeights = new List<_<int, double>>());
					dynamicWeights = dynamicWeights ?? (dynamicWeights = new List<_<int, RantAction>>());

					Stringes.Stringe firstToken = reader.ReadLooseToken();

					// constant weight
					if (reader.PeekLooseToken().ID == R.Text)
					{
						var value = reader.ReadLooseToken().Value;
						double doubleValue;
						if (!double.TryParse(value, out doubleValue))
						{
							compiler.SyntaxError(value, "invalid constant weight");
						}
						constantWeights.Add(new _<int, double>(blockNumber, doubleValue));
					}
					// dynamic weight
					else
					{
						var weightActions = new List<RantAction>();

						Action<RantAction> weightActionCallback = (action) => weightActions.Add(action);

						compiler.SetNextActionCallback(weightActionCallback);
						compiler.AddContext(CompileContext.BlockWeight);
						yield return Get<SequenceParser>();

						if (weightActions.Count > 0)
						{
							firstToken = weightActions[0].Range;
						}

						dynamicWeights.Add(new _<int, RantAction>(blockNumber, new RASequence(weightActions, firstToken)));
					}

					reader.Read(R.RightParen, "end of weight");
				}

				var startToken = reader.PeekToken();
				yield return Get<SequenceParser>();
				items.Add(new RASequence(actions, startToken));
				actions.Clear();
				blockNumber++;
			}

			compiler.LeaveContext();
			compiler.SetNextActionCallback(actionCallback);

			actionCallback(new RABlock(blockStartToken, items, dynamicWeights, constantWeights));
		}
	}
}
