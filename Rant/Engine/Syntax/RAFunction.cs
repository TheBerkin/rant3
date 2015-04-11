using Rant.Stringes;
using System;
using System.Collections.Generic;

namespace Rant.Engine.Syntax
{
	internal class RAFunction : RantAction
	{
		private readonly RantFunctionInfo _funcInfo;
		private readonly List<RantAction> _argActions;
		private readonly int _argc;

		public RAFunction(Stringe stringe, RantFunctionInfo funcInfo, List<RantAction> argActions) 
			: base(stringe)
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
						{
							sb.AddOutputWriter();
							yield return _argActions[i];
							var strNum = sb.PopOutput().MainValue;
							if (!Double.TryParse(strNum, out d))
							{
								d = 0;
								int n;
								if (Util.ParseInt(strNum, out n)) d = n;
							}
							args[i] = Convert.ChangeType(d, _funcInfo.Parameters[i].NativeType);
							break;
						}

					// Modes are parsed into enumeration members
					case RantParameterType.Mode:
						{
							sb.AddOutputWriter();
							yield return _argActions[i];
							var strMode = sb.PopOutput().MainValue;
							object value;
							if (!Util.TryParseMode(_funcInfo.Parameters[i].NativeType, strMode, out value))
							{
								throw new RantRuntimeException(_argActions[i].Stringe.ParentString, _argActions[i].Stringe,
									$"Invalid mode value '{strMode}'.");
							}
							args[i] = value;
							break;
						}
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