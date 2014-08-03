namespace Manhood
{
    internal class Entry
    {
        private readonly string _name;
        private readonly string _value;

        public string Name
        {
            get { return _name; }
        }

        public string Value
        {
            get { return _value; }
        }

        public Entry(string name, string value)
        {
            _name = name;
            _value = value;
        }
    }
}