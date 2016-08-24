using System.Collections.Generic;

using Rant.Core.ObjectModel;
using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax
{
	internal class RstDefineSubroutine : RstSubroutine
	{
		public Dictionary<string, SubroutineParameterType> Parameters;

		public RstDefineSubroutine(Stringe name)
			: base(name)
		{
		}

		public override IEnumerator<RST> Run(Sandbox sb)
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