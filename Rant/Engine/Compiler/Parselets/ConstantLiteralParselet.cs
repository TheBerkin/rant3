using System.Collections.Generic;

using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    internal class ConstantLiteralParselet : Parselet
    {
        [TokenParser(R.ConstantLiteral)]
        IEnumerable<Parselet> ConstantLiteral(Token<R> token)
        {
            AddToOutput(new RAText(token, Util.UnescapeConstantLiteral(token.Value)));
            yield break;
        }
    }
}
