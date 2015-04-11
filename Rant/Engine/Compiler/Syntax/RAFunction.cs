using Rant.Stringes;
using System;
using System.Collections.Generic;

namespace Rant.Engine.Compiler.Syntax
{
	internal class RAFunction : RantAction
	{
		private readonly RantFunctionInfo _funcInfo;
		private readonly List<RantAction> _argActions;
		private readonly int _argc;

		public RAFunction(RantFunctionInfo funcInfo, List<RantAction> argActions)
		{
			_funcInfo = funcInfo;
			_argActions = argActions;
			_argc = argActions.Count;
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			// Convert arguments to their native types
			var args = new object[_argc];
			double d;
			for (int i = 0; i < _argc; i++)
			{
				switch (_funcInfo.Parameters[i].RantType)
				{
					// Patterns are passed right to the method
					case RantParameterType.Pattern:
						args[i] = _argActions[i];
						break;

					// Strings are evaluated
					case RantParameterType.String:
						sb.AddOutputWriter();
						yield return _argActions[i];
						args[i] = sb.PopOutput().MainValue;
						break;

					// Numbers are evaluated, verified, and converted
					case RantParameterType.Number:
						sb.AddOutputWriter();
						yield return _argActions[i];
						if (!Double.TryParse(sb.PopOutput().MainValue, out d))
						{
							d = 0;
						}
						args[i] = Convert.ChangeType(d, _funcInfo.Parameters[i].NativeType);
						break;
				}
			}

			// Invoke the function
			var requester = _funcInfo.Invoke(sb, args);
			while (requester.MoveNext())
			{
				yield return requester.Current;
			}
		}
	}
}