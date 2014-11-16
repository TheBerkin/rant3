using System;

namespace Rant
{
    [Flags]
    internal enum ParamFlags
    {
        /// <summary>
        /// Parameter requires a string generated from the provided tokens.
        /// </summary>
        None = 0x00,
        /// <summary>
        /// Parameter requires a series of tokens.
        /// </summary>
        Code = 0x01,
        /// <summary>
        /// Any number of arguments may be specified beyond this point. Valid only on last parameter.
        /// </summary>
        Multi = 0x02
    }
}