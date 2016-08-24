namespace Rant.Metadata
{
	internal class RantModeValue : IRantModeValue
	{
		public RantModeValue(string name, string desc)
		{
			Name = name;
			Description = desc;
		}

		public string Name { get; }
		public string Description { get; }
	}
}