using System.Collections.Generic;

using Rant.Core.IO;
using Rant.Core.ObjectModel;
using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax
{
	[RST("dsub")]
	internal class RstDefineSubroutine : RstSubroutineBase
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

		protected override IEnumerator<RST> Serialize(EasyWriter output)
		{
			throw new System.NotImplementedException();
		}

		protected override IEnumerator<DeserializeRequest> Deserialize(EasyReader input)
		{
			throw new System.NotImplementedException();
		}
	}

	internal enum SubroutineParameterType
	{
		Loose,
		Greedy
	}
}