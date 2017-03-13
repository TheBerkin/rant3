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

using Rant.Metadata;

namespace Rant.Core.Constructs
{
    internal enum SyncType
    {
        [RantDescription("A random element is selected each time.")]
        None,

        [RantDescription("Executes from left to right.")]
        Forward,

        [RantDescription("Executes from right to left.")]
        Reverse,
        //[RantDescription("Executes from left to right, but randomly skips elements. This mode can reduce the iteration count when using [repeach].")]
        //ForwardSkip,
        //[RantDescription("Executes from right to left, but randomly skips elements. This mode can reduce the iteration count when using [repeach].")]
        //ReverseSkip,
        [RantDescription("Shuffles items and executes in order. Re-shuffled each time all items are used up.")]
        Deck,

        [RantDescription("Shuffles items and executes in order. The same order is reused for each traversal.")]
        Cdeck,

        [RantDescription("Chosen randomly, the same element is selected each time.")]
        Locked,

        [RantDescription("Starting at the first item, iterates through all elements in order and then reverses without repeating boundary elements.")]
        Ping,

        [RantDescription("Starting at the last item, iterates through all elements backwards and then reverses without repeating boundary elements.")]
        Pong
    }
}