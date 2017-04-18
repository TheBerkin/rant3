#region License

// https://github.com/TheBerkin/Rant
// 
// Copyright (c) 2017 Nicholas Fleck
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

namespace Rant.Core.Framework
{
    /// <summary>
    /// Defines parameter types for Rant functions.
    /// </summary>
    public enum RantFunctionParameterType
    {
        /// <summary>
        /// Parameter is a static string.
        /// </summary>
        String,

        /// <summary>
        /// Parameter is a lazily evaluated pattern.
        /// </summary>
        Pattern,

        /// <summary>
        /// Parameter is numeric.
        /// </summary>
        Number,

        /// <summary>
        /// Parameter describes a mode, which is one of a specific set of allowed values.
        /// </summary>
        Mode,

        /// <summary>
        /// Parameter uses combinable flags.
        /// </summary>
        Flags,

        /// <summary>
        /// Parameter is a RantObject.
        /// </summary>
        RantObject,

        /// <summary>
        /// Parameter is a boolean.
        /// </summary>
        Boolean
    }
}