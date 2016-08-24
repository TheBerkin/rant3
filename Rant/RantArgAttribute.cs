using System;

namespace Rant
{
	/// <summary>
	/// Attribute used to change the name of an argument pulled from a field or property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class RantArgAttribute : Attribute
	{
		/// <summary>
		/// Creates a new RantArgAttribute with the specified name.
		/// </summary>
		/// <param name="name">The new name to assign to the argument.</param>
		public RantArgAttribute(string name)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			Name = name;
		}

		/// <summary>
		/// The new name to assign to the argument.
		/// </summary>
		public string Name { get; }
	}
}