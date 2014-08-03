using System;
using System.Linq;

namespace Manhood
{
    internal class TagInfo
    {
        private readonly string _name;
        private readonly string[] _args;

        public TagInfo(string body)
        {
            if (String.IsNullOrEmpty(body))
            {
                throw new FormatException("Empty tag body.");
            }

            var split = Util.SplitArgs(body).ToArray();

            if (!split.Any()) throw new FormatException("Empty tag body.");

            _name = split[0] ?? "";
            _args = split.Skip(1).ToArray();
        }

        public string Name
        {
            get { return _name; }
        }

        public string[] Arguments
        {
            get { return _args; }
        }
    }
}