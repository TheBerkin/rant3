using System;

namespace Rant.Engine
{
	/// <summary>
	/// Indicates to the Rant engine that a method should be registered as a Rant function.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	internal sealed class RantFunctionAttribute : Attribute
	{
		private readonly string[] _aliases;

		public RantFunctionAttribute(params string[] aliases)
		{
			_aliases = aliases;
		}

		public RantFunctionAttribute(string name)
		{
			_aliases = new[] { name };
		}

		public RantFunctionAttribute()
		{
			_aliases = new[] { String.Empty };
		}

		public string Name => Aliases[0];

		public string[] Aliases => _aliases;
	}
}