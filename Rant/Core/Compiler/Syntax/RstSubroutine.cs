using System.Collections.Generic;

using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Syntax
{
	internal abstract class RstSubroutine : RST
	{
		protected readonly Stringe _name;
		public List<RST> Arguments;
		public RST Body;
		public RantPattern Source;

		public RstSubroutine(Stringe name)
			: base(name)
		{
			_name = name;
		}

		public string Name => _name.Value;
	}
}