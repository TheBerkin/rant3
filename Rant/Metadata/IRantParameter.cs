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

using System.Collections.Generic;

using Rant.Core.Framework;

namespace Rant.Metadata
{
    /// <summary>
    /// Provides access to metadata for a Rant function parameter.
    /// </summary>
    public interface IRantParameter
    {
        /// <summary>
        /// Gets the data type accepted by the parameter.
        /// </summary>
        RantFunctionParameterType RantType { get; }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Indicates whether the parameter accepts multiple values.
        /// </summary>
        bool IsParams { get; }

        /// <summary>
        /// Gets the description for the parameter.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Enumerates all possible values for flag and mode parameters.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRantModeValue> GetEnumValues();
    }
}