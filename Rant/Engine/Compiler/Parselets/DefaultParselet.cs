using System.Collections.Generic;

using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    [DefaultParselet]
    internal class DefaultParselet : Parselet
    {
        [DefaultParser]
        IEnumerable<Parselet> Default(Token<R> token)
        {
            AddToOutput(new RAText(token));
            yield break;
        }
    }
}
