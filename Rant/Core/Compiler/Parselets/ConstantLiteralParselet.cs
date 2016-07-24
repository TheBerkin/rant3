using System.Collections.Generic;

using Rant.Core.Compiler.Syntax;
using Rant.Core.Stringes;
using Rant.Core.Utilities;

namespace Rant.Core.Compiler.Parselets
{
    internal class ConstantLiteralParselet : Parselet
    {
        [TokenParser(R.ConstantLiteral)]
        private IEnumerable<Parselet> ConstantLiteral(Token<R> token)
        {
            AddToOutput(new RAText(token, Util.UnescapeConstantLiteral(token.Value)));
            yield break;
        }
    }
}
