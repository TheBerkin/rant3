using System.Collections.Generic;

using Rant.Internals.Engine.Utilities;
using Rant.Internals.Stringes;

namespace Rant.Internals.Engine.Compiler.Syntax.Richard
{
	internal class RichFunctionCall : RichActionBase
	{
		private RichActionBase _function;
		private RichActionBase[] _argValues;

		public RichFunctionCall(Stringe token, RichActionBase function, RichGroup args, string _sourceName)
			: base(token)
		{
			_function = function;
			List<RichActionBase> argValues = new List<RichActionBase>();
			if (args.Actions.Count > 0)
			{
				RichActionBase lastArg = null;
				for (var i = 0; i < args.Actions.Count; i++)
				{
					var action = args.Actions[i];
                    if (action is RichArgumentSeperator)
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
                func is RichVariable || 
                func is RichFunctionCall || 
                func is RichGroup ||
                func is RichObjectProperty)
			{
                var count = sb.ScriptObjectStack.Count;
				yield return (func as RichActionBase);
                if (count >= sb.ScriptObjectStack.Count)
                    throw new RantRuntimeException(sb.Pattern, Range, "Invalid function call target.");
				func = sb.ScriptObjectStack.Pop();
			}
            if (func is RichFunction)
            {
                var expectedCount = (func as RichFunction).ArgCount;
                if (expectedCount != _argValues.Length)
                    throw new RantRuntimeException(sb.Pattern, Range, $"Function expected {expectedCount} arguments, got {_argValues.Length}.");
                foreach (RichActionBase argAction in _argValues)
                    yield return argAction;
                var iterator = (func as RichFunction).Execute(sb);
                while (iterator.MoveNext())
                    yield return iterator.Current;
                yield break;
            }
            else if (func is RichPatternString)
            {
                if (_argValues.Length > 0)
                    throw new RantRuntimeException(sb.Pattern, base.Range, "Unexpected arguments for pattern string.");
                yield return (func as RichPatternString).Execute(sb);
                yield break;
            }
            else if (func is RichNativeFunction)
            {
                var expectedCount = (func as RichNativeFunction).ArgCount;
                if (expectedCount != _argValues.Length)
                    throw new RantRuntimeException(sb.Pattern, Range, $"Native function expected {expectedCount} arguments, got {_argValues.Length}.");
                foreach (RichActionBase argAction in _argValues)
                    yield return argAction;
                // we have to set this for reasons
                (func as RichNativeFunction).Range = this.Range;
                var iterator = (func as RichNativeFunction).Execute(sb);
                while (iterator.MoveNext())
                    yield return iterator.Current;
                yield break;
            }
			throw new RantRuntimeException(sb.Pattern, base.Range, "Cannot execute " + Util.ScriptingObjectType(func) + " as function.");
		}
	}
}
