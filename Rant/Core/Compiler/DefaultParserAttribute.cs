using System;

namespace Rant.Core.Compiler
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal sealed class DefaultParserAttribute : Attribute
    {
        public DefaultParserAttribute()
        {

        }
    }
}
