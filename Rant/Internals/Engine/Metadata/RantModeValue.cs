namespace Rant.Internals.Engine.Metadata
{
    internal class RantModeValue : IRantModeValue
    {
        public string Name { get; }
        public string Description { get; }

        public RantModeValue(string name, string desc)
        {
            Name = name;
            Description = desc;
        }
    }
}