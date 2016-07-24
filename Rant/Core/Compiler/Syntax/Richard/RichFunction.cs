using System.Collections.Generic;

using Rant.Core.ObjectModel;
using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax.Richard
{
	internal class RichFunction : RichActionBase
	{
		private RichActionBase _body;
		private string[] _argNames;

        public int ArgCount => _argNames.Length;

		public RichFunction(Stringe origin, RichActionBase body, RichGroup args)
			: base(origin)
		{
			List<string> argNames = new List<string>();
			if (args.Actions.Count > 0)
			{
				string lastArg = "";
				for (var i = 0; i < args.Actions.Count; i++)
				{
					var action = args.Actions[i];
					if (action is RichArgumentSeperator)
						argNames.Add(lastArg);
					else
						lastArg = (action as RichVariable).Name;
				}
				argNames.Add(lastArg);
			}
            // reverse it, since things are popped from the object stack in reverse order
            argNames.Reverse();
            _argNames = argNames.ToArray();
			_body = body;
			Type = ActionValueType.Function;
            Returnable = true;
		}

		public override object GetValue(Sandbox sb)
		{
			return this;
		}

		public IEnumerator<RantAction> Execute(Sandbox sb)
		{
            // save old locals with the same name
            Dictionary<string, RantObject> oldLocals = new Dictionary<string, RantObject>();
            for (var i = 0; i < _argNames.Length; i++)
                oldLocals[_argNames[i]] = sb.Objects[_argNames[i]];
            sb.Objects.EnterScope();
            for (var i = 0; i < _argNames.Length; i++)
				sb.Objects[_argNames[i]] = new RantObject(sb.ScriptObjectStack.Pop());
			yield return _body;
			sb.Objects.ExitScope();
            for (var i = 0; i < _argNames.Length; i++)
                sb.Objects[_argNames[i]] = oldLocals[_argNames[i]];
            yield break;
		}

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			yield break;
		}

		public override string ToString()
		{
			return "function";
		}
	}
}
