using System;
using System.Collections.Generic;
using System.Globalization;

namespace Manhood
{
    internal sealed class Lexer
    {
        private readonly string _string;
        private int _pos;

        private static readonly Dictionary<char, TokenType> puncs = new Dictionary<char, TokenType>();

        static Lexer()
        {
            foreach (var value in (TokenType[])Enum.GetValues(typeof(TokenType)))
            {
                char c = value.Puntuator();
                if (c != '\0') puncs[c] = value;
            }
        }

        public Lexer(string input)
        {
            _pos = 0;
            _string = input;
        }

        public Token Next()
        {
            while (_pos < _string.Length)
            {
                char c = _string[_pos++];

                if (c == '[') // Manhood patterns
                {
                    int start = _pos;
                    bool escape = false;
                    int balance = 1;
                    while (balance > 0 && _pos < _string.Length)
                    {
                        switch (_string[_pos++])
                        {
                            case '\\':
                                escape = !escape;
                                break;
                            case '[':
                                if (!escape) balance++;
                                break;
                            case ']':
                                if (!escape) balance--;
                                break;
                            default:
                                escape = false;
                                break;
                        }
                    }
                    return new Token(_string.Substring(start, (_pos - 1) - start), TokenType.Manhood);
                }

                if (puncs.ContainsKey(c)) // Operator
                {
                    return new Token(c.ToString(CultureInfo.InvariantCulture), puncs[c]);
                }

                if (IsValueChar(c)) // Variable / number
                {
                    int start = _pos - 1;
                    while (_pos < _string.Length)
                    {
                        if (!IsValueChar(_string[_pos++])) break;
                    }
                    return new Token(_string.Substring(start, _pos - start), TokenType.Value);
                }

                if (!Char.IsWhiteSpace(c) && !Char.IsControl(c)) // No strange symbols allowed
                {
                    throw new ManhoodException("Invalid token '" + c + "' in expression \"" + _string + "\".");
                }

            }
            return new Token("", TokenType.End);
        }

        public static bool IsValueChar(char c)
        {
            return Char.IsLetterOrDigit(c) || c == '_';
        }
    }
}