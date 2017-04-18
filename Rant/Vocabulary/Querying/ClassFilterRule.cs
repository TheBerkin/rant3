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

namespace Rant.Vocabulary.Querying
{
    /// <summary>
    /// Defines a query filter for a single dictionary entry class.
    /// </summary>
    public sealed class ClassFilterRule
    {
        /// <summary>
        /// Initializes a new ClassFilterRule that checks for a positive match to the specified class.
        /// </summary>
        /// <param name="className">The name of the class to search for.</param>
        public ClassFilterRule(string className)
        {
            Class = className;
        }

        /// <summary>
        /// Initializes a new ClassFilterRule that checks for a positive or negative match to the specified class.
        /// </summary>
        /// <param name="className">The name of the class to search for.</param>
        /// <param name="shouldMatch">Determines whether the filter item expects a positive or negative match for the class.</param>
        public ClassFilterRule(string className, bool shouldMatch)
        {
            Class = className;
            ShouldMatch = shouldMatch;
        }

        /// <summary>
        /// Determines whether the filter item expects a positive or negative match for the class.
        /// </summary>
        public bool ShouldMatch { get; set; } = true;

        /// <summary>
        /// The name of the class to search for.
        /// </summary>
        public string Class { get; set; }
    }
}