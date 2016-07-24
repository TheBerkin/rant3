using System.Collections.Generic;

using Rant.Internals.Engine.Compiler.Syntax;
using Rant.Internals.Stringes;

namespace Rant.Internals.Engine.Compiler.Parselets
{
    [DefaultParselet]
    internal class DefaultParselet : Parselet
    {
        [DefaultParser]
        private IEnumerable<Parselet> Default(Token<R> token)
        {
            AddToOutput(new RAText(token));
            yield break;
        }
    }
}
