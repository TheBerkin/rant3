using System.Collections.Generic;

using Rant.Core.Compiler.Syntax;
using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Parselets
{
    internal class EscapeSequenceParselet : Parselet
    {
        [TokenParser(R.EscapeSequence)]
        private IEnumerable<Parselet> EscapeSequence(Token<R> token)
        {
            AddToOutput(new RAEscape(token));
            yield break;
        }
    }
}
