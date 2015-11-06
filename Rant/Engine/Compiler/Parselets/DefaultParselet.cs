using System.Collections.Generic;

using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    [DefaultParselet]
    internal class DefaultParselet : Parselet
    {
        public override R Identifier
        {
            get
            {
                throw new RantInternalException("Cannot access DefaultParselet's identifier");
            }
        }

        protected override IEnumerable<Parselet> InternalParse(Token<R> token)
        {
            AddToOutput(new RAText(token));
            yield break;
        }
    }
}
