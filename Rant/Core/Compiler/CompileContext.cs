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

namespace Rant.Core.Compiler
{
    /// <summary>
    /// Distinguishes execution contexts to the compiler to make it aware of context-specific token behaviors.
    /// </summary>
    internal enum CompileContext
    {
        /// <summary>
        /// Point of execution is outside of any specialized constructs.
        /// </summary>
        DefaultSequence,

        /// <summary>
        /// Point of execution is inside a tag argument.
        /// </summary>
        ArgumentSequence,

        /// <summary>
        /// Point of execution is inside of a block.
        /// </summary>
        BlockSequence,

        /// <summary>
        /// We're reading a block weight.
        /// </summary>
        BlockWeight,

        /// <summary>
        /// We're reading a subroutine body.
        /// </summary>
        SubroutineBody,

        /// <summary>
        /// The end of a block.
        /// </summary>
        BlockEndSequence,

        /// <summary>
        /// The end of function arguments.
        /// </summary>
        FunctionEndContext,

        /// <summary>
        /// Complement pattern for queries.
        /// </summary>
        QueryComplement
    }
}