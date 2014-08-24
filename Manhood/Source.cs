using System;
using System.Text;

namespace Manhood
{
    /// <summary>
    /// Contains source code information that is useful when debugging patterns.
    /// </summary>
    public sealed class Source
    {
        private readonly string _code;
        private readonly SourceType _type;
        private readonly string _name;

        /// <summary>
        /// The name of the source code.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Describes the origin of the source.
        /// </summary>
        public SourceType Type
        {
            get { return _type; }
        }

        /// <summary>
        /// The code contained in the source.
        /// </summary>
        public string Code
        {
            get { return _code; }
        }

        internal Source(string name, SourceType type, string code)
        {
            _name = name;
            _type = type;
            _code = code;
        }

        /// <summary>
        /// Returns a string describing the source.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Concat(Type, ": \"", Name, "\" (", Encoding.UTF8.GetByteCount(_code), " B)");
        }
    }
}