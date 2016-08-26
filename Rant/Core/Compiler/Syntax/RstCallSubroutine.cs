using System.Collections.Generic;
using System.Linq;

using Rant.Core.IO;
using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax
{
	[RST("csub")]
	internal class RstCallSubroutine : RstSubroutineBase
	{
		private bool _inModule = false;
		private string _moduleFunctionName = null;
		public List<RST> Arguments;

		public RstCallSubroutine(Stringe name, string moduleFunctionName = null)
			: base(name)
		{
			if (moduleFunctionName != null)
				_inModule = true;
			_moduleFunctionName = moduleFunctionName;
		}

		public RstCallSubroutine(TokenLocation location) : base(location)
		{
			// Used by serializer
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
					args[parameters[i]] = new RstText(Location, output.Main);
				}
				else
					args[parameters[i]] = Arguments[i];
			}
			sb.SubroutineArgs.Push(args);
			yield return action;
			sb.SubroutineArgs.Pop();
		}

		protected override IEnumerator<RST> Serialize(EasyWriter output)
		{
			var iterMain = base.Serialize(output);
			while (iterMain.MoveNext()) yield return iterMain.Current;
			output.Write(_inModule);
			output.Write(_moduleFunctionName);
			output.Write(Arguments.Count);
			foreach (var arg in Arguments) yield return arg;
		}

		protected override IEnumerator<DeserializeRequest> Deserialize(EasyReader input)
		{
			var iterMain = base.Deserialize(input);
			while (iterMain.MoveNext()) yield return iterMain.Current;
			input.ReadBoolean(out _inModule);
			input.ReadString(out _moduleFunctionName);
			int argc = input.ReadInt32();
			if (Arguments == null) Arguments = new List<RST>(argc);
			for (int i = 0; i < argc; i++)
			{
				var request = new DeserializeRequest();
				yield return request;
				Arguments.Add(request.Result);
			}
		}
	}
}