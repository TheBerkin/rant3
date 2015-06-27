using System.Collections.Generic;
using System.Linq;

using Rant.Engine.Syntax.Richard.Operators;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard
{
	internal class REAGroup : RantExpressionAction
	{
		internal List<RantExpressionAction> Actions;

		private Stringe _origin;
        private string _sourceName;

        public int Lines = 0;

		public REAGroup(IEnumerable<RantExpressionAction> actions, Stringe token, string sourceName)
			: base(token)
		{
			Type = ActionValueType.Number;
			Actions = (List<RantExpressionAction>)actions;
			if (Actions.Any(x => x is REAConcatOperator))
				Type = ActionValueType.String;
			Condense();
			_origin = token;
            _sourceName = sourceName;
		}

        private void CheckOperator(REAInfixOperator op)
        {
            if (op.LeftSide == null || op.RightSide == null)
                throw new RantCompilerException(_sourceName, op.Range, "Operator missing left or right hand side.");
        }

		private void CondenseStep()
		{
			var op = Actions
				.Where(x => x is REAInfixOperator && (x as REAInfixOperator).LeftSide == null && (x as REAInfixOperator).RightSide == null)
				.OrderBy(x => (x as REAInfixOperator).Precedence)
				.First() as REAInfixOperator;
			var index = Actions.IndexOf(op);
            if (index - 1 >= 0)
            {
                op.LeftSide = Actions[index - 1];
                if (op.LeftSide is REAInfixOperator)
                    CheckOperator(op.LeftSide as REAInfixOperator);
            }
            if (index + 1 <= Actions.Count - 1)
            {
                op.RightSide = Actions[index + 1];
                if (op.RightSide is REAInfixOperator)
                    CheckOperator(op.RightSide as REAInfixOperator);
            }
            CheckOperator(op);
		    Actions.Remove(op.LeftSide);
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
