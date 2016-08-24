using System;

namespace Rant.Metadata
{
	/// <summary>
	/// Indicates to the Rant engine that a method should be registered as a Rant function.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	internal sealed class RantFunctionAttribute : Attribute
	{
		public RantFunctionAttribute(params string[] aliases)
		{
			Aliases = aliases;
		}

		public RantFunctionAttribute(string name)
		{
			Aliases = new[] { name };
		}

		public RantFunctionAttribute()
		{
			Aliases = new[] { string.Empty };
		}

		public string Name => Aliases[0];
		public string[] Aliases { get; }
	}
}