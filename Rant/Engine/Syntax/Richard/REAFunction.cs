using System.Collections.Generic;

using Rant.Engine.ObjectModel;
using Rant.Stringes;

namespace Rant.Engine.Syntax.Richard
{
	internal class REAFunction : RantExpressionAction
	{
		private RantExpressionAction _body;
		private string[] _argNames;

        public int ArgCount => _argNames.Length;

		public REAFunction(Stringe origin, RantExpressionAction body, REAGroup args)
			: base(origin)
		{
			List<string> argNames = new List<string>();
			if (args.Actions.Count > 0)
			{
				string lastArg = "";
				for (var i = 0; i < args.Actions.Count; i++)
				{
					var action = args.Actions[i];
					if (action is REAArgumentSeperator)
						argNames.Add(lastArg);
					else
						lastArg = (action as REAVariable).Name;
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
			sb.Objects.EnterScope();
			for (var i = 0; i < _argNames.Length; i++)
				sb.Objects[_argNames[i]] = new RantObject(sb.ScriptObjectStack.Pop());
			yield return _body;
            for (var i = 0; i < _argNames.Length; i++)
                sb.Objects.RemoveLocal(_argNames[i]);
			sb.Objects.ExitScope();
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
