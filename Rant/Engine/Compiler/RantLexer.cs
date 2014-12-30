using System.Collections.Generic;
using System.Text.RegularExpressions;

using Rant.Engine.Stringes;
using Rant.Engine.Stringes.Tokens;

namespace Rant.Engine.Compiler
{
    internal static class RantLexer
    {
        private const RegexOptions DefaultOptions = RegexOptions.Compiled | RegexOptions.ExplicitCapture;

        public static readonly Regex EscapeRegex = new Regex(@"\\((?<count>\d+((\.\d+)?[kMB])?),)?((?<code>[^u\s\r\n])|u(?<unicode>[0-9a-f]{4}))", DefaultOptions);
        public static readonly Regex RegexRegex = new Regex(@"//(.*?[^\\])?//i?", DefaultOptions);

        private static readonly Regex WeightRegex = new Regex(@"\*[ ]*(?<value>\d+(\.\d+)?)[ ]*\*", DefaultOptions);
        private static readonly Regex WhitespaceRegex = new Regex(@"\s+", DefaultOptions);
        private static readonly Regex BlackspaceRegex = new Regex(@"(^\s+|\s*[\r\n]+\s*|\s+$)", DefaultOptions | RegexOptions.Multiline);
        private static readonly Regex CommentRegex = new Regex(@"\s*#.*?(?=[\r\n]|$)", DefaultOptions | RegexOptions.Multiline);
        private static readonly Regex ConstantLiteralRegex = new Regex(@"""([^""]|"""")*""", DefaultOptions);
        private static readonly Regex SyllableRangeRegex = new Regex(@"\(\s*(-?\d+|\d+\s*-(\s*\d+)?)\s*\)", DefaultOptions);

        internal static readonly LexerRules<R> Rules;

        private static Stringe TruncatePadding(Stringe input)
        {
            var ls = input.LeftPadded ? input.TrimStart() : input;
            return ls.RightPadded ? ls.TrimEnd() : ls;
        }

        static RantLexer()
        {
            Rules = new LexerRules<R>
            {
                {EscapeRegex, R.EscapeSequence},
                {RegexRegex, R.Regex},
                {ConstantLiteralRegex, R.ConstantLiteral},
                {"[", R.LeftSquare}, {"]", R.RightSquare},
                {"{", R.LeftCurly}, {"}", R.RightCurly},
                {"<", R.LeftAngle}, {">", R.RightAngle},
                {"|", R.Pipe},
                {";", R.Semicolon},
                {":", R.Colon},
                {"@", R.At},
                {"?", R.Question},
                {"::", R.DoubleColon},
                {"?!", R.Without},
                {"-", R.Hyphen},
                {"!", R.Exclamation},
                {"$", R.Dollar},
                {"=", R.Equal},
                {"&", R.Ampersand},
                {"%", R.Percent},
                {"+", R.Plus},
                {"^", R.Caret},
                {"`", R.Backtick},
                {SyllableRangeRegex, R.RangeLiteral},
                {WeightRegex, R.Weight},
                {CommentRegex, R.Ignore, 3},
                {BlackspaceRegex, R.Ignore, 2},
                {WhitespaceRegex, R.Whitespace}
            };
            Rules.AddUndefinedCaptureRule(R.Text, TruncatePadding);
            Rules.AddEndToken(R.EOF);
            Rules.IgnoreRules.Add(R.Ignore);
        }

        public static IEnumerable<Token<R>> GenerateTokens(string input)
        {
            var reader = new StringeReader(input);
            while (!reader.EndOfStringe)
            {
                yield return reader.ReadToken(Rules);
            }
        }
    }
}