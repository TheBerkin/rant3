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

		public RAFunction(Stringe range, RantFunctionInfo funcInfo, List<RantAction> argActions)
			: base(range)
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
						if (!Util.TryParseEnum(_funcInfo.Parameters[i].NativeType, strMode, out value))
						{
							throw new RantRuntimeException(sb.Pattern, _argActions[i].Range,
								$"Unknown mode value '{strMode}'.");
						}
						args[i] = value;
						break;
					}
					case RantParameterType.Flags:
					{
						var enumType = _funcInfo.Parameters[i].NativeType;
						sb.AddOutputWriter();
						yield return _argActions[i];
						long flags = 0;
						var strFlags = sb.PopOutput().MainValue;
						object value;
						foreach(var flag in strFlags.Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries))
						{
							if (!Util.TryParseEnum(enumType, flag, out value))
								throw new RantRuntimeException(sb.Pattern, _argActions[i].Range,
									$"Unknown flag value '{flag}'.");
							flags |= Convert.ToInt64(value);
						}
						args[i] = Enum.ToObject(enumType, flags);
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