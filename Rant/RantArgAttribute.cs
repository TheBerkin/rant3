using System;

namespace Rant
{
	public sealed class RantArgAttribute : Attribute
	{
		public string Name { get; }

		public RantArgAttribute(string name)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			Name = name;
		}
	}
}