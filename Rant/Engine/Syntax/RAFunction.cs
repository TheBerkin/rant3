using Rant.Stringes;

using System;
using System.Collections.Generic;
using System.Linq;

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

	    private RantParameter GetParameter(int index)
	    {
	        if (index >= _funcInfo.Parameters.Length - 1 && _funcInfo.HasParamArray)
	            return _funcInfo.Parameters[_funcInfo.Parameters.Length - 1];
	        return _funcInfo.Parameters[index];
	    }

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			// Convert arguments to their native types
		    int paramc = _funcInfo.Parameters.Length;
			var args = new object[_argc];
			double d;
		    RantParameter p;
			for (int i = 0; i < _argc; i++)
			{
			    p = GetParameter(i);
				switch (p.RantType)
				{
					// Patterns are passed right to the method
					case RantParameterType.Pattern:
						args[i] = _argActions[i];
						break;

					// Strings are evaluated
					case RantParameterType.String:
						sb.AddOutputWriter();
						yield return _argActions[i];
						args[i] = sb.Return().MainValue;
						break;

					// Numbers are evaluated, verified, and converted
					case RantParameterType.Number:
					{
						sb.AddOutputWriter();
						yield return _argActions[i];
						var strNum = sb.Return().MainValue;
						if (!Double.TryParse(strNum, out d))
						{
							d = 0;
							int n;
							if (Util.ParseInt(strNum, out n)) d = n;
						}
						args[i] = Convert.ChangeType(d, p.NativeType);
						break;
					}

					// Modes are parsed into enumeration members
					case RantParameterType.Mode:
					{
						sb.AddOutputWriter();
						yield return _argActions[i];
						var strMode = sb.Return().MainValue;
						object value;
						if (!Util.TryParseEnum(p.NativeType, strMode, out value))
						{
							throw new RantRuntimeException(sb.Pattern, _argActions[i].Range,
								$"Unknown mode value '{strMode}'.");
						}
						args[i] = value;
						break;
					}

                    // Flags are parsed from strings to enum members and combined with OR.
					case RantParameterType.Flags:
					{
						var enumType = p.NativeType;
						sb.AddOutputWriter();
						yield return _argActions[i];
						long flags = 0;
						var strFlags = sb.Return().MainValue;
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
		    IEnumerator<RantAction> requester;
		    if (_funcInfo.HasParamArray)
		    {
		        int required = paramc - 1;
		        int parrayCount = _argc - required;
		        var parray = Array.CreateInstance(_funcInfo.Parameters[required].NativeType, parrayCount);
                Array.Copy(args, required, parray, 0, parrayCount);
		        requester = _funcInfo.Invoke(sb, args.Take(required).Concat(new[] { parray }).ToArray());
		    }
		    else
		    {
                requester = _funcInfo.Invoke(sb, args);
            }

			while (requester.MoveNext())
			{
				yield return requester.Current;
			}
		}
	}
}