using Rant.Arithmetic.Expressions;
using Rant.Stringes.Tokens;

namespace Rant.Arithmetic.Parselets
{
    internal class AbsParselet : IPrefixParselet
    {
        public Expression Parse(Parser parser, Token<RMathToken> token)
        {
            var e = new AbsExpression(parser.ParseExpression());
            parser.Take(RMathToken.Pipe);
            return e;
        }
    }
}