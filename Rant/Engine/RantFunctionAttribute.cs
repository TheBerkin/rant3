using System;

namespace Rant.Engine
{
	/// <summary>
	/// Indicates to the Rant engine that a method should be registered as a Rant function.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	internal sealed class RantFunctionAttribute : Attribute
	{
		public string Name { get; set; } = String.Empty;
	}
}