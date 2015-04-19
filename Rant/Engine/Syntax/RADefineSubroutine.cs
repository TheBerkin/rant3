using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			sb.Objects[Name] = new ObjectModel.RantObject(this);
			yield break;
		}
	}

	internal enum SubroutineParameterType
	{
		Loose,
		Greedy
	}
}
