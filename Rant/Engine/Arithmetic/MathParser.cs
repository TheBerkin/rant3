using System;
using System.Linq;
using Rant.Arithmetic.Parselets;
using Rant.Stringes;
using Rant.Stringes.Tokens;

namespace Rant.Arithmetic
{
    internal partial class MathParser
    {
        private int _pos;
        private string _src;
        private readonly Token<RMathToken>[] _tokens;

        public MathParser(string expression)
        {
            _pos = 0;
            _src = expression;
            _tokens = new MathLexer(expression.ToStringe()).ToArray();
        }

        public string Source
        {
            get { return _src; }
        }

        public static double Calculate(VM ii, string expression)
        {
            double result = 0;
            foreach (var expr in expression.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var p = new MathParser(expr);
                result = p.ParseExpression().Evaluate(p, ii);
            }
            return result;
        }

        public Expression ParseExpression(int precedence = 0)
        {
            var token = Take();
            IPrefixParselet prefix;
            IInfixParselet infix;

            if (!PrefixParselets.TryGetValue(token.ID, out prefix))
            {
                throw new RantException(_src, token, "Invalid expression '\{token.Value}'.");
            }

            var left = prefix.Parse(this, token);
            // Break when the next token's precedence is less than or equal to the current precedence
            // This will assure that higher precedence operations like multiplication are parsed before lower operations.
            while (GetPrecedence() > precedence)
            {
                token = Take();                
                if (!InfixParselets.TryGetValue(token.ID, out infix))
                {
                    throw new RantException(_src, token, "Invalid operator '\{token.Value}'.");
                }

                // Replace the left expression with the next parsed expression.
                left = infix.Parse(this, left, token);
            }

            return left;
        }

        /// <summary>
        /// Returns the precedence of the next infix operator, or 0 if there is none.
        /// </summary>
        /// <returns></returns>
        private int GetPrecedence()
        {
            if (_pos == _tokens.Length) return 0;
            IInfixParselet infix;
            InfixParselets.TryGetValue(Peek().ID, out infix);
            return infix?.Precedence ?? 0;
        }

        public Token<RMathToken> Peek(int distance = 0)
        {
            if (_pos == _tokens.Length) return null;
            if (distance < 0) throw new ArgumentOutOfRangeException("distance");
            return _tokens[_pos + distance];
        }

        public Token<RMathToken> Take(RMathToken type)
        {
            var token = Take();
            if (token.ID != type)
            {
                throw new RantException(_src, token, "Expression expected \{type}, but found \{token.ID}.");
            }
            return token;
        }

        public Token<RMathToken> Take()
        {
            if (_pos < _tokens.Length) return _tokens[_pos++];
            return null;
        }
    }
}