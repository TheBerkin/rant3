using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rant.Engine.Delegates
{
	internal abstract class Witchcraft
	{
		private static readonly Type[] _funcTypes;
		private static readonly Type[] _voidTypes;

		static Witchcraft()
		{
			var ass = Assembly.GetAssembly(typeof(Witchcraft));
			var lstFuncTypes = new List<Type>();
			var lstVoidTypes = new List<Type>();
			foreach (var type in ass.GetTypes().Where(t => t.IsSubclassOf(typeof(Witchcraft)) && t.IsGenericTypeDefinition))
			{
				if (type.IsSubclassOf(typeof(WitchcraftVoid)))
				{
					lstVoidTypes.Add(type);
				}
				else
				{
					lstFuncTypes.Add(type);
				}
			}

			_funcTypes = lstFuncTypes.OrderBy(t => t.GetGenericArguments().Length).ToArray();
			_voidTypes = lstVoidTypes.OrderBy(t => t.GetGenericArguments().Length).ToArray();
		}

		public static Witchcraft Create(MethodInfo methodInfo)
		{
			bool isVoid = methodInfo.ReturnType == typeof(void);
            var types = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
			if (types.Length == 0 || types[0] != typeof(Sandbox))
				throw new ArgumentException("Method must have a Sandbox parameter come first.", nameof(methodInfo));
			
			var argTypes = types.Skip(1).ToArray();
			if (argTypes.Length >= _funcTypes.Length) return null;

			if (argTypes.Length == 0)
			{
				if (isVoid)
				{
					return new WitchcraftNoParamsVoid(methodInfo);
				}
				else
				{
					return new WitchcraftNoParams(methodInfo);
				}
			}

			Type type = isVoid 
				? _voidTypes[argTypes.Length - 1].MakeGenericType(argTypes) 
				: _funcTypes[argTypes.Length - 1].MakeGenericType(argTypes);

			return (Witchcraft)Activator.CreateInstance(type, methodInfo);
		}

		public abstract object Invoke(Sandbox sb, object[] args);
	}

	internal abstract class WitchcraftVoid : Witchcraft
	{
	}

	internal class WitchcraftNoParams : Witchcraft
	{
		private readonly Func<Sandbox, object> _func;

		public WitchcraftNoParams(MethodInfo methodInfo)
		{
			_func = (Func<Sandbox, object>)Delegate.CreateDelegate(typeof(Func<Sandbox, object>), methodInfo);
		}

		public override object Invoke(Sandbox sb, object[] args) => _func(sb);
	}

	internal class WitchcraftNoParamsVoid : WitchcraftVoid
	{
		private readonly Action<Sandbox> _func;

		public WitchcraftNoParamsVoid(MethodInfo methodInfo)
		{
			_func = (Action<Sandbox>)Delegate.CreateDelegate(typeof(Action<Sandbox>), methodInfo);
		}

		public override object Invoke(Sandbox sb, object[] args)
		{
			_func(sb);
			return null;
		}
	}

	internal class Witchcraft<A> : Witchcraft
	{
		private readonly Func<Sandbox, A, object> _func;

		public Witchcraft(MethodInfo methodInfo)
		{
			_func = (Func<Sandbox, A, object>)Delegate.CreateDelegate(typeof(Func<Sandbox, A, object>), methodInfo);
		}

		public override object Invoke(Sandbox sb, object[] args) => _func(sb, (A)args[0]);
	}

	internal class WitchcraftVoid<A> : WitchcraftVoid
	{
		private readonly Action<Sandbox, A> _func;

		public WitchcraftVoid(MethodInfo methodInfo)
		{
			_func = (Action<Sandbox, A>)Delegate.CreateDelegate(typeof(Action<Sandbox, A>), methodInfo);
		}

		public override object Invoke(Sandbox sb, object[] args)
		{
			_func(sb, (A)args[0]);
			return null;
		}
	}

	internal class Witchcraft<A, B> : Witchcraft
	{
		private readonly Func<Sandbox, A, B, object> _func;

		public Witchcraft(MethodInfo methodInfo)
		{
			_func = (Func<Sandbox, A, B, object>)Delegate.CreateDelegate(typeof(Func<Sandbox, A, B, object>), methodInfo);
		}

		public override object Invoke(Sandbox sb, object[] args) =>
			_func(sb, (A)args[0], (B)args[1]);
	}

	internal class WitchcraftVoid<A, B> : WitchcraftVoid
	{
		private readonly Action<Sandbox, A, B> _func;

		public WitchcraftVoid(MethodInfo methodInfo)
		{
			_func = (Action<Sandbox, A, B>)Delegate.CreateDelegate(typeof(Action<Sandbox, A, B>), methodInfo);
		}

		public override object Invoke(Sandbox sb, object[] args)
		{
			_func(sb, (A)args[0], (B)args[1]);
			return null;
		}
	}

	internal class Witchcraft<A, B, C> : Witchcraft
	{
		private readonly Func<Sandbox, A, B, C, object> _func;

		public Witchcraft(MethodInfo methodInfo)
		{
			_func = (Func<Sandbox, A, B, C, object>)Delegate.CreateDelegate(typeof(Func<Sandbox, A, B, C, object>), methodInfo);
		}

		public override object Invoke(Sandbox sb, object[] args) =>
			_func(sb, (A)args[0], (B)args[1], (C)args[2]);
	}

	internal class WitchcraftVoid<A, B, C> : WitchcraftVoid
	{
		private readonly Action<Sandbox, A, B, C> _func;

		public WitchcraftVoid(MethodInfo methodInfo)
		{
			_func = (Action<Sandbox, A, B, C>)Delegate.CreateDelegate(typeof(Action<Sandbox, A, B, C>), methodInfo);
		}

		public override object Invoke(Sandbox sb, object[] args)
		{
			_func(sb, (A)args[0], (B)args[1], (C)args[2]);
			return null;
		}
	}

	internal class Witchcraft<A, B, C, D> : Witchcraft
	{
		private readonly Func<Sandbox, A, B, C, D, object> _func;

		public Witchcraft(MethodInfo methodInfo)
		{
			_func = (Func<Sandbox, A, B, C, D, object>)Delegate.CreateDelegate(typeof(Func<Sandbox, A, B, C, D, object>), methodInfo);
		}

		public override object Invoke(Sandbox sb, object[] args) =>
			_func(sb, (A)args[0], (B)args[1], (C)args[2], (D)args[3]);
	}

	internal class WitchcraftVoid<A, B, C, D> : WitchcraftVoid
	{
		private readonly Action<Sandbox, A, B, C, D> _func;

		public WitchcraftVoid(MethodInfo methodInfo)
		{
			_func = (Action<Sandbox, A, B, C, D>)Delegate.CreateDelegate(typeof(Action<Sandbox, A, B, C, D>), methodInfo);
		}

		public override object Invoke(Sandbox sb, object[] args)
		{
			_func(sb, (A)args[0], (B)args[1], (C)args[2], (D)args[3]);
			return null;
		}
	}

	internal class Witchcraft<A, B, C, D, E> : Witchcraft
	{
		private readonly Func<Sandbox, A, B, C, D, E, object> _func;

		public Witchcraft(MethodInfo methodInfo)
		{
			_func = (Func<Sandbox, A, B, C, D, E, object>)Delegate.CreateDelegate(typeof(Func<Sandbox, A, B, C, D, E, object>), methodInfo);
		}

		public override object Invoke(Sandbox sb, object[] args) =>
			_func(sb, (A)args[0], (B)args[1], (C)args[2], (D)args[3], (E)args[4]);
	}

	internal class WitchcraftVoid<A, B, C, D, E> : WitchcraftVoid
	{
		private readonly Action<Sandbox, A, B, C, D, E> _func;

		public WitchcraftVoid(MethodInfo methodInfo)
		{
			_func = (Action<Sandbox, A, B, C, D, E>)Delegate.CreateDelegate(typeof(Action<Sandbox, A, B, C, D, E>), methodInfo);
		}

		public override object Invoke(Sandbox sb, object[] args)
		{
			_func(sb, (A)args[0], (B)args[1], (C)args[2], (D)args[3], (E)args[4]);
			return null;
		}
	}
}