using System;
using System.Collections.Generic;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Expressions
{
	internal class REAFunctionCall : RantExpressionAction
	{
		private RantExpressionAction _function;
		private RantExpressionAction[] _argValues;

		public REAFunctionCall(Stringe token, RantExpressionAction function, REAGroup args)
			: base(token)
		{
			_function = function;
			List<RantExpressionAction> argValues = new List<RantExpressionAction>();
			if (args.Actions.Count > 0)
			{
				RantExpressionAction lastArg = null;
				for (var i = 0; i < args.Actions.Count; i++)
				{
					var action = args.Actions[i];
					if (action is REAArgumentSeperator)
						argValues.Add(lastArg);
					else
						lastArg = action;
				}
				argValues.Add(lastArg);
			}
			_argValues = argValues.ToArray();

            Returnable = true;
		}

		public override object GetValue(Sandbox sb)
		{
			return null;
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			object func = _function;
			if (
                func is REAVariable || 
                func is REAFunctionCall || 
                func is REAGroup ||
                func is REAObjectProperty)
			{
				yield return (func as RantExpressionAction);
				func = sb.ScriptObjectStack.Pop();
			}
            if (func is REAFunction)
            {
                foreach (RantExpressionAction argAction in _argValues)
                    yield return argAction;
                var iterator = (func as REAFunction).Execute(sb);
                while (iterator.MoveNext())
                    yield return iterator.Current;
                yield break;
            }
            else if (func is REAPatternString)
            {
                if (_argValues.Length > 0)
                    throw new RantRuntimeException(sb.Pattern, base.Range, "Unexpected arguments for pattern string.");
                yield return (func as REAPatternString).Execute(sb);
                yield break;
            }
            else if (func is REANativeFunction)
            {
                foreach (RantExpressionAction argAction in _argValues)
                    yield return argAction;
                var iterator = (func as REANativeFunction).Execute(sb);
                while (iterator.MoveNext())
                    yield return iterator.Current;
                yield break;
            }
			throw new RantRuntimeException(sb.Pattern, base.Range, "Cannot execute " + func.GetType() + " as function.");
		}
	}
}
