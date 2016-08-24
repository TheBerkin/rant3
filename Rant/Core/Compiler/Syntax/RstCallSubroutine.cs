using System.Collections.Generic;
using System.Linq;

using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax
{
	internal class RstCallSubroutine : RstSubroutine
	{
		private readonly bool _inModule = false;
		private readonly string _moduleFunctionName = null;

		public RstCallSubroutine(Stringe name, string moduleFunctionName = null)
			: base(name)
		{
			if (moduleFunctionName != null)
				_inModule = true;
			_moduleFunctionName = moduleFunctionName;
		}

		public override IEnumerator<RST> Run(Sandbox sb)
		{
			if (_inModule)
			{
				if (!sb.Modules.ContainsKey(Name))
					throw new RantRuntimeException(
						sb.Pattern,
						this,
						$"The module '{Name}' does not exist or has not been imported."
						);
				if (sb.Modules[Name][_moduleFunctionName] == null)
					throw new RantRuntimeException(
						sb.Pattern,
						this,
						$"The function '{_moduleFunctionName}' cannot be found in the module '{Name}'."
						);
			}
			else if (sb.Objects[Name] == null)
				throw new RantRuntimeException(sb.Pattern, this, $"The subroutine '{Name}' does not exist.");
			var sub = (RstDefineSubroutine)(_inModule ? sb.Modules[Name][_moduleFunctionName] : sb.Objects[Name].Value);
			if (sub.Parameters.Keys.Count != Arguments.Count)
				throw new RantRuntimeException(sb.Pattern, this, "Argument mismatch on subroutine call.");
			var action = sub.Body;
			var args = new Dictionary<string, RST>();
			var parameters = sub.Parameters.Keys.ToArray();
			for (int i = 0; i < Arguments.Count; i++)
			{
				if (sub.Parameters[parameters[i]] == SubroutineParameterType.Greedy)
				{
					sb.AddOutputWriter();
					yield return Arguments[i];
					var output = sb.Return();
					args[parameters[i]] = new RstText(_name, output.Main);
				}
				else
					args[parameters[i]] = Arguments[i];
			}
			sb.SubroutineArgs.Push(args);
			yield return action;
			sb.SubroutineArgs.Pop();
		}
	}
}