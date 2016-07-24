using System;

namespace Rant.Internals.Engine.Compiler
{
    /// <summary>
    /// Marks a parselet as the default parselet.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class DefaultParseletAttribute : Attribute
    {
        public DefaultParseletAttribute()
        {
        }
    }
}
