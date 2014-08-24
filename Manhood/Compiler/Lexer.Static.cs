using System;

namespace Manhood.Compiler
{
    internal partial class Lexer
    {
        public static readonly LexerBindings<TokenType> MainSymbols;
        public static readonly LexerBindings<TokenType> BlockSymbols;
        public static readonly LexerBindings<TokenType> MathBlockSymbols;
        public static readonly LexerBindings<TokenType> TagSymbols;
        public static readonly LexerBindings<TokenType> QuerySymbols;

        static Lexer()
        {
            MainSymbols = new[]
            {
                Tuple.Create("[", TokenType.LeftSquare),
                Tuple.Create("]", TokenType.RightSquare),
                Tuple.Create("{", TokenType.LeftCurly),
                Tuple.Create("}", TokenType.RightCurly),
                Tuple.Create("<", TokenType.LeftTriangle),
                Tuple.Create(">", TokenType.RightTriangle),
                Tuple.Create("(", TokenType.LeftParen),
                Tuple.Create(")", TokenType.RightParen)
            };

            BlockSymbols = new[]
            {
                Tuple.Create("|", TokenType.Pipe)
            };

            MathBlockSymbols = new[]
            {
                Tuple.Create("@", TokenType.At)
            };

            TagSymbols = new[]
            {
                Tuple.Create(";", TokenType.Semicolon),
                Tuple.Create(":", TokenType.Colon),
                Tuple.Create("@", TokenType.At),
                Tuple.Create("?", TokenType.Question)
            };

            QuerySymbols = new[]
            {
                Tuple.Create("?!", TokenType.Without),
                Tuple.Create("::", TokenType.DoubleColon),
                Tuple.Create("-", TokenType.Hyphen),
                Tuple.Create("!", TokenType.Exclamation),
                Tuple.Create(".", TokenType.Dot),
                Tuple.Create("/", TokenType.ForwardSlash),
                Tuple.Create("?", TokenType.Question),
                Tuple.Create("$", TokenType.Dollar)
            };
        }
    }
}