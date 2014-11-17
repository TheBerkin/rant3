using System.Collections.Generic;
using Rant.Arithmetic.Parselets;

namespace Rant.Arithmetic
{
    internal partial class Parser
    {
        private static readonly Dictionary<RMathToken, IInfixParselet> InfixParselets;
        private static readonly Dictionary<RMathToken, IPrefixParselet> PrefixParselets; 

        static Parser()
        {
            InfixParselets = new Dictionary<RMathToken, IInfixParselet>
            {
                {RMathToken.Equals, new BinaryOperatorParselet((int)Precedence.Assignment, true)},
                {RMathToken.Plus, new BinaryOperatorParselet((int)Precedence.Sum, false)},
                {RMathToken.Minus, new BinaryOperatorParselet((int)Precedence.Sum, false)},
                {RMathToken.Asterisk, new BinaryOperatorParselet((int)Precedence.Product, false)},
                {RMathToken.Slash, new BinaryOperatorParselet((int)Precedence.Product, false)},
                {RMathToken.Caret, new BinaryOperatorParselet((int)Precedence.Exponent, false)},
                {RMathToken.Modulo, new BinaryOperatorParselet((int)Precedence.Product, false)},
                {RMathToken.Increment, new PostfixOperatorParselet((int)Precedence.Postfix)},
                {RMathToken.Decrement, new PostfixOperatorParselet((int)Precedence.Postfix)},
                {RMathToken.Swap, new BinaryOperatorParselet((int)Precedence.Assignment, false)},
                {RMathToken.AddAssign, new BinaryOperatorParselet((int)Precedence.Assignment, true)},
                {RMathToken.SubAssign, new BinaryOperatorParselet((int)Precedence.Assignment, true)},
                {RMathToken.MulAssign, new BinaryOperatorParselet((int)Precedence.Assignment, true)},
                {RMathToken.DivAssign, new BinaryOperatorParselet((int)Precedence.Assignment, true)},
                {RMathToken.ModAssign, new BinaryOperatorParselet((int)Precedence.Assignment, true)},
                {RMathToken.PowAssign, new BinaryOperatorParselet((int)Precedence.Assignment, true)},
            };

            PrefixParselets = new Dictionary<RMathToken, IPrefixParselet>
            {
                {RMathToken.Minus, new PrefixOperatorParselet((int)Precedence.Prefix)},
                {RMathToken.Increment, new PrefixOperatorParselet((int)Precedence.Prefix)},
                {RMathToken.Decrement, new PrefixOperatorParselet((int)Precedence.Prefix)},
                {RMathToken.Number, new NumberParselet()},
                {RMathToken.Name, new NameParselet()},
                {RMathToken.Pipe, new AbsParselet()}
            };
        }
    }
}