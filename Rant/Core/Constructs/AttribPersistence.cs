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

namespace Rant.Core.Constructs
{
    /// <summary>
    /// Defines persistence modes for block attributes.
    /// </summary>
    internal enum AttribPersistence
    {
        /// <summary>
        /// The next block consumes attributes immediately.
        /// </summary>
        Off,

        /// <summary>
        /// The next block consumes attributes and restores them when finished.
        /// </summary>
        Outer,

        /// <summary>
        /// The next block uses but does not consume attributes.
        /// They are inherited by and restored at the end of each child block.
        /// </summary>
        OuterShared,

        /// <summary>
        /// The current attributes are inherited by all blocks inside the current block.
        /// They are consumed at the end of the block.
        /// </summary>
        Inner,

        /// <summary>
        /// The current attributes are inherited by all blocks inside the current block, as well as their children.
        /// They are consumed at the end of the block.
        /// </summary>
        InnerShared,

        /// <summary>
        /// Disable changes to the current block attributes until the persistence mode is changed.
        /// </summary>
        ReadOnly,

        /// <summary>
        /// Attributes will persist over the next block, but any block after or inside it will consume them.
        /// </summary>
        Once,

        /// <summary>
        /// The next block uses but does not consume attributes. This also affects child blocks.
        /// </summary>
        On
    }
}