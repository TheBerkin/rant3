using System.Collections.Generic;

using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    internal class EscapeSequenceParselet : Parselet
    {
        [TokenParser(R.EscapeSequence)]
        IEnumerable<Parselet> EscapeSequence(Token<R> token)
        {
            AddToOutput(new RAEscape(token));
            yield break;
        }
    }
}
