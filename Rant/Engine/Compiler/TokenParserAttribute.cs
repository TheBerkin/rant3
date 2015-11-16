using System;

namespace Rant.Engine.Compiler
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal sealed class TokenParserAttribute : Attribute
    {
        public string Name { get; set; }
        public R? TokenType => tokenType;

        private readonly R? tokenType;

        public TokenParserAttribute()
        {
        }

        public TokenParserAttribute(R tokenType)
        {
            this.tokenType = tokenType;
        }
    }
}
