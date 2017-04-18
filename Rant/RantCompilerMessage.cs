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

using static Rant.Localization.Txtres;

namespace Rant
{
    /// <summary>
    /// Represents a message emitted by the Rant compiler while performing a job.
    /// </summary>
    public sealed class RantCompilerMessage
    {
        internal RantCompilerMessage(RantCompilerMessageType type, string source, string message, int line, int column,
            int index, int length)
        {
            Type = type;
            Source = source;
            Message = message;
            Line = line;
            Column = column;
            Index = index;
            Length = length;
        }

        /// <summary>
        /// The type of message.
        /// </summary>
        public RantCompilerMessageType Type { get; }

        /// <summary>
        /// The source path of the pattern being compiled when the message was generated.
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// The message text.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The line on which the message was generated.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// The column on which the message was generated.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// The character index on which the message was generated.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// The length, in characters, of the code snippet to which the message pertains.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Generates a string representation of the message.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Line > 0
                ? $"{GetString("src-line-col", Source, Line, Column)} {Message}"
                : $"({Source}) {Message}";
        }
    }
}