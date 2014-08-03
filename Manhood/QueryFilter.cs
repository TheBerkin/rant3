namespace Manhood
{
    internal class QueryFilter
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public QueryFilter(string name, string value)
        {
            Name = name.ToLower().Trim();
            Value = value;
        }
    }
}
