using System.Collections.Generic;

using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard
{
	internal class REAFunctionCall : RantExpressionAction
	{
		private RantExpressionAction _function;
		private RantExpressionAction[] _argValues;

		public REAFunctionCall(Stringe token, RantExpressionAction function, REAGroup args, string _sourceName)
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
                    {
                        if (lastArg == null)
                            throw new RantCompilerException(_sourceName, Range, "Blank argument in function call.");
                        argValues.Add(lastArg);
                    }
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
                var count = sb.ScriptObjectStack.Count;
				yield return (func as RantExpressionAction);
                if (count >= sb.ScriptObjectStack.Count)
                    throw new RantRuntimeException(sb.Pattern, Range, "Invalid function call target.");
				func = sb.ScriptObjectStack.Pop();
			}
            if (func is REAFunction)
            {
                var expectedCount = (func as REAFunction).ArgCount;
                if (expectedCount != _argValues.Length)
                    throw new RantRuntimeException(sb.Pattern, Range, $"Function expected {expectedCount} arguments, got {_argValues.Length}.");
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
                var expectedCount = (func as REANativeFunction).ArgCount;
                if (expectedCount != _argValues.Length)
                    throw new RantRuntimeException(sb.Pattern, Range, $"Native function expected {expectedCount} arguments, got {_argValues.Length}.");
                foreach (RantExpressionAction argAction in _argValues)
                    yield return argAction;
                var iterator = (func as REANativeFunction).Execute(sb);
                while (iterator.MoveNext())
                    yield return iterator.Current;
                yield break;
            }
			throw new RantRuntimeException(sb.Pattern, base.Range, "Cannot execute " + Util.ScriptingObjectType(func) + " as function.");
		}
	}
}
