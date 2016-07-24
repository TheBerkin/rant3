using System;

namespace Rant.Internals.Engine.Metadata
{
	/// <summary>
	/// Used for annotating Rant functions and their parameters with descriptions that can be used to generate documentation.
	/// </summary>
	internal class RantDescriptionAttribute : Attribute
	{
		public string Description { get; set; }

		public RantDescriptionAttribute(string desc)
		{
			Description = desc;
		}
	}
}