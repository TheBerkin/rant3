using System.Collections.Generic;

using Rant.Internals.Engine.Compiler.Syntax;
using Rant.Internals.Stringes;

namespace Rant.Internals.Engine.Compiler.Parselets
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
