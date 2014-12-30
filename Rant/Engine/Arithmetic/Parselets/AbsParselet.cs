using Rant.Engine.Arithmetic.Expressions;
using Rant.Engine.Stringes.Tokens;

namespace Rant.Engine.Arithmetic.Parselets
{
    internal class AbsParselet : IPrefixParselet
    {
        public Expression Parse(MathParser parser, Token<RMathToken> token)
        {
            var e = new AbsExpression(parser.ParseExpression());
            parser.Take(RMathToken.Pipe);
            return e;
        }
    }
}