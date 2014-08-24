using System;

using Manhood.Compiler;

namespace Manhood
{
    internal class Token
    {
        public int Line { get; private set; }
        public int Column { get; private set; }
        public int Index { get; private set; }

        public TokenType Type { get; private set; }
        public string Content { get; private set; }

        public override string ToString()
        {
            return String.Concat("<", Type, ", '", Content, "'>");
        }
    }
}