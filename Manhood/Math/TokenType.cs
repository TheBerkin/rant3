using System;

namespace Manhood
{
    internal enum TokenType : int
    {
        LeftParen, // (
        RightParen, // )
        Equals, // =
        Plus, // +
        Minus, // -
        Asterisk, // *
        Slash, // /
        Caret, // ^
        Value, // abc 999
        Manhood, // [abc]
        End
    }

    internal static class TokenTypeExtensions
    {
        public static Char Puntuator(this TokenType type)
        {
            switch (type)
            {
                case TokenType.Asterisk:
                    return '*';
                case TokenType.Caret:
                    return '^';
                case TokenType.Equals:
                    return '=';
                case TokenType.LeftParen:
                    return '(';
                case TokenType.RightParen:
                    return ')';
                case TokenType.Minus:
                    return '-';
                case TokenType.Plus:
                    return '+';
                case TokenType.Slash:
                    return '/';
                default:
                    return '\0';
            }
        }
    }
}