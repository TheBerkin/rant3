using System.Collections.Generic;

using Rant.Stringes;

namespace Rant.Engine.Syntax
{
	internal abstract class RASubroutine : RantAction
	{
		protected readonly Stringe _name;

		public string Name => _name.Value;
		public List<RantAction> Arguments;
		public RantAction Body;
		public RantPattern Source;

		public RASubroutine(Stringe name)
			: base(name)
		{
			_name = name;
		}
	}
}
