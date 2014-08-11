using System;
using System.Linq;

namespace Manhood
{
    internal class Subroutine
    {
        public string Name { get; private set; }
        public string Body { get; private set; }
        public Parameter[] Parameters { get; private set; }

        public Subroutine(string name, string body, params string[] parameters)
        {
            Name = name;
            Body = new Pattern(body).Code;
            Parameters = parameters.Select(pname => new Parameter(pname)).ToArray();
        }

        public class Parameter
        {
            public string Name { get; set; }
            public bool Interpreted { get; set; }

            public Parameter(string name)
            {
                Interpreted = name.StartsWith("@");
                if (!Util.ValidateName(Name = name.TrimStart('@')))
                {
                    throw new FormatException("Invalid parameter name: " + name);
                }
            }
        }
    }
}
