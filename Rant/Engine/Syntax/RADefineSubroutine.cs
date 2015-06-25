using System.Collections.Generic;

using Rant.Engine.ObjectModel;
using Rant.Stringes;

namespace Rant.Engine.Syntax
{
	internal class RADefineSubroutine : RASubroutine
	{
		public Dictionary<string, SubroutineParameterType> Parameters;

		public RADefineSubroutine(Stringe name)
			: base(name)
		{ }

		public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			sb.Objects[Name] = new RantObject(this);
			yield break;
		}
	}

	internal enum SubroutineParameterType
	{
		Loose,
		Greedy
	}
}
