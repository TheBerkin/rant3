using System.Collections.Generic;

using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    internal class ConstantLiteralParselet : Parselet
    {
        public override R Identifier => R.ConstantLiteral;

        protected override IEnumerable<Parselet> InternalParse(Token<R> token)
        {
            AddToOutput(new RAText(token, Util.UnescapeConstantLiteral(token.Value)));
            yield break;
        }
    }
}
