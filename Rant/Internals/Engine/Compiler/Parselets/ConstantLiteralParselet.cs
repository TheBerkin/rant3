using System.Collections.Generic;

using Rant.Internals.Engine.Compiler.Syntax;
using Rant.Internals.Engine.Utilities;
using Rant.Internals.Stringes;

namespace Rant.Internals.Engine.Compiler.Parselets
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
