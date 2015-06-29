using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Stringes;

namespace Rant.Engine.Syntax
{
	internal class RACallSubroutine : RASubroutine
	{
		public RACallSubroutine(Stringe name)
			: base(name) { }

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			if (sb.Objects[Name] == null)
				throw new RantRuntimeException(sb.Pattern, _name, $"The subroutine '{Name}' does not exist.");
			var sub = (RADefineSubroutine)sb.Objects[Name].Value;
			if (sub.Parameters.Keys.Count != Arguments.Count)
				throw new RantRuntimeException(sb.Pattern, _name, "Argument mismatch on subroutine call.");
			var action = sub.Body;
			var args = new Dictionary<string, RantAction>();
			var parameters = sub.Parameters.Keys.ToArray();
			for (var i = 0; i < Arguments.Count; i++)
			{
				if (sub.Parameters[parameters[i]] == SubroutineParameterType.Greedy)
				{
					sb.AddOutputWriter();
					yield return Arguments[i];
					var output = sb.Return();
					args[parameters[i]] = new RAText(_name, output.Main);
				}
				else
					args[parameters[i]] = Arguments[i];
			}
			sb.SubroutineArgs.Push(args);
            yield return action;
			sb.SubroutineArgs.Pop();
			yield break;
		}
	}
}
