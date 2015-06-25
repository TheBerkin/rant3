using System;
using System.Linq;
using System.Collections.Generic;
using Rant.Stringes;
using Rant.Engine.Syntax.Expressions.Operators;

namespace Rant.Engine.Syntax.Expressions
{
	internal class REAGroup : RantExpressionAction
	{
		internal List<RantExpressionAction> Actions;

		private Stringe origin;

		public REAGroup(IEnumerable<RantExpressionAction> actions, Stringe token)
			: base(token)
		{
			Type = ActionValueType.Number;
			Actions = (List<RantExpressionAction>)actions;
			if (Actions.Any(x => x is REAConcatOperator))
				Type = ActionValueType.String;
			Condense();
			origin = token;
		}

		private void CondenseStep()
		{
			var op = Actions
				.Where(x => x is REAInfixOperator && (x as REAInfixOperator).LeftSide == null && (x as REAInfixOperator).RightSide == null)
				.OrderBy(x => (x as REAInfixOperator).Precedence)
				.First() as REAInfixOperator;
			var index = Actions.IndexOf(op);
			if (index - 1 >= 0)
				op.LeftSide = Actions[index - 1];
			if (index + 1 <= Actions.Count - 1)
				op.RightSide = Actions[index + 1];
			if (op.LeftSide == null && op.RightSide == null)
				throw new RantCompilerException(null, origin, "Operator missing left and right sides.");
			if (op.LeftSide != null)
				Actions.Remove(op.LeftSide);
			if (op.RightSide != null)
				Actions.Remove(op.RightSide);
		}

		private void Condense()
		{
			// put things under other things
			var operators = Actions.Where(x => x is REAInfixOperator).OrderBy(x => (x as REAInfixOperator).Precedence);
			for (int i = 0; i < operators.Count(); i++)
				CondenseStep();
		}

		public override object GetValue(Sandbox sb)
		{
			return null;
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			if (!Actions.Any())
				yield break;
			foreach (RantExpressionAction action in Actions)
				yield return action;
			yield break;
		}
	}
}
