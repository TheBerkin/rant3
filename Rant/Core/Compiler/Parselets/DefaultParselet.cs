using System.Collections.Generic;

using Rant.Core.Compiler.Syntax;
using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Parselets
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
