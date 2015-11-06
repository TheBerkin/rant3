using System.Collections.Generic;

using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    internal class EscapeSequenceParselet : Parselet
    {
        public override R Identifier => R.EscapeSequence;

        protected override IEnumerable<Parselet> InternalParse(Token<R> token)
        {
            AddToOutput(new RAEscape(token));
            yield break;
        }
    }
}
