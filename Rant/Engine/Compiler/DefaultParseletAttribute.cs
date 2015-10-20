using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rant.Engine.Compiler
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class DefaultParseletAttribute : Attribute
    {
        public DefaultParseletAttribute()
        {
        }
    }
}
