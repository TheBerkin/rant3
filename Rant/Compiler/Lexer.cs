using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

using Stringes;
using Stringes.Tokens;

namespace Rant.Compiler
{
    internal static class Lexer
    {
        private const RegexOptions MhRegexOptions = RegexOptions.Compiled | RegexOptions.ExplicitCapture;

        public static readonly Regex EscapeRegex = new Regex(@"\\((?<count>\d+),)?((?<code>[^u\s\r\n])|u(?<unicode>[0-9a-f]{4}))", MhRegexOptions);
        public static readonly Regex RegexRegex = new Regex(@"/(.*?[^\\])?/i?", MhRegexOptions);

        private static readonly Regex WhitespaceRegex = new Regex(@"\s+", MhRegexOptions);
        private static readonly Regex BlackspaceRegex = new Regex(@"(^\s+|\s*[\r\n]+\s*|\s+$)", MhRegexOptions | RegexOptions.Multiline);
        private static readonly Regex CommentRegex = new Regex(@"\s*#.*?(?=[\r\n]|$)", MhRegexOptions | RegexOptions.Multiline);
        private static readonly Regex ConstantLiteralRegex = new Regex(@"""([^""]|"""")*""", MhRegexOptions);

        internal static readonly LexerRules<TokenType> Rules;

        private static Stringe TruncatePadding(Stringe input)
        {
            var ls = input.LeftPadded ? input.TrimStart() : input;
            return ls.RightPadded ? ls.TrimEnd() : ls;
        }

        static Lexer()
        {
            Rules = new LexerRules<TokenType>
            {
                {EscapeRegex, TokenType.EscapeSequence},
                {RegexRegex, TokenType.Regex},
                {ConstantLiteralRegex, TokenType.ConstantLiteral},
                {"[", TokenType.LeftSquare}, {"]", TokenType.RightSquare},
                {"{", TokenType.LeftCurly}, {"}", TokenType.RightCurly},
                {"(", TokenType.LeftParen}, {")", TokenType.RightParen},
                {"<", TokenType.LeftAngle}, {">", TokenType.RightAngle},
                {"|", TokenType.Pipe},
                {";", TokenType.Semicolon},
                {":", TokenType.Colon},
                {"@", TokenType.At},
                {"?", TokenType.Question},
                {"::", TokenType.DoubleColon},
                {"?!", TokenType.Without},
                {"-", TokenType.Hyphen},
                {"!", TokenType.Exclamation},
                {"$", TokenType.Dollar},
                {CommentRegex, TokenType.Ignore, 3},
                {BlackspaceRegex, TokenType.Ignore, 2},
                {WhitespaceRegex, TokenType.Whitespace}
            };
            Rules.AddUndefinedCaptureRule(TokenType.Text, TruncatePadding);
            Rules.AddEndToken(TokenType.EOF);
            Rules.IgnoreRules.Add(TokenType.Ignore);
        }

        public static IEnumerable<Token<TokenType>> GenerateTokens(string input)
        {
            var reader = new StringeReader(input);
            while (!reader.EndOfStringe)
            {
                yield return reader.ReadToken(Rules);
            }
        }
    }
}