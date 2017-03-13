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

using Rant.Core.Compiler;
using Rant.Core.Compiler.Syntax;
using Rant.Core.ObjectModel;

namespace Rant.Resources
{
    /// <summary>
    /// Represents a collection of subroutines that can be loaded and used by other patterns.
    /// </summary>
    public sealed class RantModule
    {
        private readonly ObjectStack _objects = new ObjectStack(new ObjectTable());

        /// <summary>
        /// Creates a new RantModule with the specified name.
        /// </summary>
        /// <param name="name">The name to assign to the module.</param>
        public RantModule(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the module.
        /// </summary>
        public string Name { get; }

        internal RST this[string name] => (RST)_objects[name].Value;

        /// <summary>
        /// Adds a RantPattern containing a subroutine to this module.
        /// </summary>
        /// <param name="name">The name of the function to add.</param>
        /// <param name="pattern">The pattern that will make up the body of the function.</param>
        public void AddSubroutineFunction(string name, RantProgram pattern)
        {
            var action = pattern.SyntaxTree.GetType() == typeof(RstSequence)
                ? ((RstSequence)pattern.SyntaxTree).Actions[0]
                : pattern.SyntaxTree;
            if (action.GetType() != typeof(RstDefineSubroutine))
            {
                throw new RantRuntimeException(pattern, LineCol.Unknown,
                    "Attempted to add non-subroutine pattern to a module.");
            }
            _objects[name] = new RantObject(action);
        }

        internal void AddActionFunction(string name, RST body)
        {
            _objects[name] = new RantObject(body);
        }
    }
}