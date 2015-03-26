using System;

using Rant.Engine.Compiler;

namespace Rant.Debugger
{
#if EDITOR
    /// <summary>
    /// Used when the Rant VM encounters a new line.
    /// </summary>
    public sealed class LineEventArgs : EventArgs
    {
        /// <summary>
        /// The line number on which the token occurs.
        /// </summary>
        public int Line => Token.Line;

        /// <summary>
        /// The line index on which the token occurs.
        /// </summary>
        public int LineIndex => Token.Line - 1;

        /// <summary>
        /// The encountered token.
        /// </summary>
        public readonly Token<R> Token;

        /// <summary>
        /// The pattern the token is from.
        /// </summary>
        public readonly RantPattern Pattern;

        public LineEventArgs(RantPattern pattern, Token<R> token)
        {
            Token = token;
            Pattern = pattern;
        }
    }
#endif
}