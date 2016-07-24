using System;

namespace Rant.Internals.Engine.Compiler
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal sealed class DefaultParserAttribute : Attribute
    {
        public DefaultParserAttribute()
        {

        }
    }
}
