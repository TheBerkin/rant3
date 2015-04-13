using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rant.Stringes;

namespace Rant.Engine.Syntax
{
	internal class RASubroutine : RantAction
	{
		private Stringe _name;

		public string Name => _name.Value;
		public List<RantAction> Parameters;
		public RantAction Body;
		public bool IsCall = false;
		public RantPattern Source;

		public RASubroutine(Stringe name)
			: base(name)
		{
			_name = name;
		}
		
		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			if (!IsCall)
				sb.Objects[Name] = new ObjectModel.RantObject(this);
			else
			{
				if (sb.Objects[Name] == null)
					throw new RantRuntimeException(sb.Pattern, _name, $"The subroutine {Name} does not exist.");
				var sub = (RASubroutine)sb.Objects[Name].Value;
				if (sub.Parameters.Count != Parameters.Count)
					throw new RantRuntimeException(sb.Pattern, _name, "Argument mismatch on subroutine call.");
				var action = sub.Body;
				var args = new Dictionary<string, RantAction>();
				for (var i = 0; i < Parameters.Count; i++)
					args[((RAText)sub.Parameters[i]).Text] = Parameters[i];
				sb.SubroutineArgs.Push(args);
                yield return action;
				sb.SubroutineArgs.Pop();
            }
			yield break;
		}
	}
}
