using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rant.Engine.Compiler
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
