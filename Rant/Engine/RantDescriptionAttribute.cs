using System;

namespace Rant.Engine
{
	internal class RantDescriptionAttribute : Attribute
	{
		public string Description { get; set; }

		public RantDescriptionAttribute(string desc)
		{
			Description = desc;
		}
	}
}