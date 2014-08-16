using System.Collections.Generic;
using Manhood.Parselets;

namespace Manhood
{
    internal partial class Parser
    {
        private static readonly Dictionary<TokenType, IInfixParselet> infixParselets;
        private static readonly Dictionary<TokenType, IPrefixParselet> prefixParselets; 

        static Parser()
        {
            infixParselets = new Dictionary<TokenType, IInfixParselet>
            {
                {TokenType.Equals, new BinaryOperatorParselet((int)Precedence.Assignment, true)},
                {TokenType.Plus, new BinaryOperatorParselet((int)Precedence.Sum, false)},
                {TokenType.Minus, new BinaryOperatorParselet((int)Precedence.Sum, false)},
                {TokenType.Asterisk, new BinaryOperatorParselet((int)Precedence.Product, false)},
                {TokenType.Slash, new BinaryOperatorParselet((int)Precedence.Product, false)},
                {TokenType.Caret, new BinaryOperatorParselet((int)Precedence.Exponent, false)},
                {TokenType.Modulo, new BinaryOperatorParselet((int)Precedence.Product, false)},
                {TokenType.Increment, new PostfixOperatorParselet((int)Precedence.Postfix)},
                {TokenType.Decrement, new PostfixOperatorParselet((int)Precedence.Postfix)},
                {TokenType.Swap, new BinaryOperatorParselet((int)Precedence.Assignment, false)}
            };

            prefixParselets = new Dictionary<TokenType, IPrefixParselet>
            {
                {TokenType.Minus, new PrefixOperatorParselet((int)Precedence.Prefix)},
                {TokenType.Increment, new PrefixOperatorParselet((int)Precedence.Prefix)},
                {TokenType.Decrement, new PrefixOperatorParselet((int)Precedence.Prefix)},
                {TokenType.Number, new NumberParselet()},
                {TokenType.Name, new NameParselet()},
                {TokenType.LeftParen, new GroupParselet()}
            };
        }
    }
}