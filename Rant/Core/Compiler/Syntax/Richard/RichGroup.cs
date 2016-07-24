using System.Collections.Generic;
using System.Linq;

using Rant.Core.Compiler.Syntax.Richard.Operators;
using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax.Richard
{
	internal class RichGroup : RichActionBase
	{
		internal List<RichActionBase> Actions;

		private Stringe _origin;
        private string _sourceName;

        public int Lines = 0;

		public RichGroup(IEnumerable<RichActionBase> actions, Stringe token, string sourceName)
			: base(token)
		{
			Type = ActionValueType.Number;
			Actions = (List<RichActionBase>)actions;
			if (Actions.Any(x => x is RichConcatOperator))
				Type = ActionValueType.String;
			Condense();
			_origin = token;
            _sourceName = sourceName;
		}

        private void CheckOperator(RichInfixOperator op)
        {
            if (op.LeftSide == null || op.RightSide == null)
                throw new RantCompilerException(_sourceName, op.Range, "Operator missing left or right hand side.");
        }

		private void CondenseStep()
		{
			var op = Actions
				.Where(x => x is RichInfixOperator && (x as RichInfixOperator).LeftSide == null && (x as RichInfixOperator).RightSide == null)
				.OrderBy(x => (x as RichInfixOperator).Precedence)
				.First() as RichInfixOperator;
			var index = Actions.IndexOf(op);
            if (index - 1 >= 0)
            {
                op.LeftSide = Actions[index - 1];
                if (op.LeftSide is RichInfixOperator)
                    CheckOperator(op.LeftSide as RichInfixOperator);
            }
            if (index + 1 <= Actions.Count - 1)
            {
                op.RightSide = Actions[index + 1];
                if (op.RightSide is RichInfixOperator)
                    CheckOperator(op.RightSide as RichInfixOperator);
            }
            CheckOperator(op);
		    Actions.Remove(op.LeftSide);
		    Actions.Remove(op.RightSide);
		}

		private void Condense()
		{
			// put things under other things
			var operators = Actions.Where(x => x is RichInfixOperator).OrderBy(x => (x as RichInfixOperator).Precedence);
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
			foreach (RichActionBase action in Actions)
				yield return action;
			yield break;
		}
	}
}
