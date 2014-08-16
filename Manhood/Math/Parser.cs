using System;
using System.Collections.Generic;
using System.Linq;
using Manhood.Parselets;

namespace Manhood
{
    internal partial class Parser
    {
        private int _pos;
        private readonly Token[] _tokens;

        public Parser(IEnumerable<Token> tokens)
        {
            _pos = 0;
            _tokens = tokens.ToArray();
        }

        public static double Calculate(Interpreter ii, string expression)
        {
            return new Parser(new Lexer(expression)).ParseExpression().Evaluate(ii);
        }

        public Expression ParseExpression(int precedence = 0)
        {
            var token = Take();

            IPrefixParselet prefixParselet;
            if (!prefixParselets.TryGetValue(token.Type, out prefixParselet))
            {
                throw new ManhoodException("Invalid expression '" + token.Text + "'.");
            }

            var left = prefixParselet.Parse(this, token);
            while (GetPrecedence() > precedence)
            {
                token = Take();
                IInfixParselet infix;
                if (!infixParselets.TryGetValue(token.Type, out infix))
                {
                    throw new ManhoodException("Invalid operator '" + token.Text + "'.");
                }

                left = infix.Parse(this, left, token);
            }

            return left;
        }

        private int GetPrecedence()
        {
            IInfixParselet infix;
            infixParselets.TryGetValue(Peek().Type, out infix);
            return infix != null ? infix.Precedence : 0;
        }

        public Token Peek(int distance = 0)
        {
            if (distance < 0) throw new ArgumentOutOfRangeException("Distance cannot be negative.");
            return _tokens[_pos + distance];
        }

        public Token Take(TokenType type)
        {
            var token = Take();
            if (token.Type != type)
            {
                throw new ManhoodException(String.Concat("Expression expected ", type, ", but found ", token.Type, "."));
            }
            return token;
        }

        public Token Take()
        {
            if (_pos < _tokens.Length) return _tokens[_pos++];
            return null;
        }
    }
}