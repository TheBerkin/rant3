using System;
using System.Reflection;

namespace Rant.Core.Utilities
{
	internal class WitchcraftNoParamsVoid : WitchcraftVoid
	{
		private readonly XAction<Sandbox> _func;

		public WitchcraftNoParamsVoid(MethodInfo methodInfo)
		{
			_func = (XAction<Sandbox>)Delegate.CreateDelegate(
				typeof(XAction<Sandbox>), methodInfo);
		}

		public override object Invoke(Sandbox sb, object[] args)
		{
			_func(sb);
			return null;
		}
	}
}