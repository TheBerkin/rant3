using System;

namespace Rant.Engine.ObjectModel.Metas
{
	internal class ValueMeta : Meta
	{
		private readonly RantObject _obj;

		public ValueMeta(RantObject obj)
		{
			_obj = obj;
		}

		public ValueMeta(object obj)
		{
			_obj = new RantObject(obj);
		}

		public override RantObject Resolve(Rave rave) => _obj;
	}
}