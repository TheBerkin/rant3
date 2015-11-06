using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rant.Engine.Compiler
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class TokenParserAttribute : Attribute
    {
        public R TokenType => tokenType;

        readonly R tokenType;

        public TokenParserAttribute(R tokenType)
        {
            this.tokenType = tokenType;
        }
    }
}
