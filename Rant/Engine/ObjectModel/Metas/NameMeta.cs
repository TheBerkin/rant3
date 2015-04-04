using System;

namespace Rant.Engine.ObjectModel.Metas
{
	internal class NameMeta : Meta
	{
		private readonly string _name;

		public string Name => _name;

		public NameMeta(string name)
		{
			_name = name;
		}

		public override RantObject Resolve(Rave rave) => rave.Rant.Objects[_name];
	}
}