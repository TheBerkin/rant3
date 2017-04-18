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

using System;

using Rant.Localization;

namespace Rant.Vocabulary
{
    /// <summary>
    /// Thrown when Rant encounters an error while loading a dictionary table.
    /// </summary>
    public sealed class RantTableLoadException : Exception
    {
        internal RantTableLoadException(string origin, int line, int col, string messageType, params object[] messageArgs)
            : base($"{Txtres.GetString("src-line-col", origin, line, col)} {Txtres.GetString(messageType, messageArgs)}")
        {
            Line = line;
            Column = col;
            Origin = origin;
        }

        /// <summary>
        /// Gets the line number on which the error occurred.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Gets the column on which the error occurred.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Gets a string describing where the table was loaded from. For tables loaded from disk, this will be the file path.
        /// </summary>
        public string Origin { get; }
    }
}