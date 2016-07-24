using System.Collections.Generic;

using Rant.Core.ObjectModel;
using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax
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
