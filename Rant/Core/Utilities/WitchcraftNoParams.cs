using System;
using System.Reflection;

namespace Rant.Core.Utilities
{
	internal class WitchcraftNoParams : Witchcraft
	{
		private readonly XFunc<Sandbox, object> _func;

		public WitchcraftNoParams(MethodInfo methodInfo)
		{
			_func = (XFunc<Sandbox, object>)Delegate.CreateDelegate(
				typeof(XFunc<Sandbox, object>), methodInfo);
		}

		public override object Invoke(Sandbox sb, object[] args) => _func(sb);
	}
}