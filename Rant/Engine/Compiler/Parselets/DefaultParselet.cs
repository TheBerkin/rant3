using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    [DefaultParselet]
    internal class DefaultParselet : Parselet
    {
        public override R[] Identifiers
        {
            get
            {
                throw new RantInternalException("Cannot access DefaultParselet's identifier");
            }
        }

        public DefaultParselet()
        {
        }

        public override IEnumerator<Parselet> Parse(NewRantCompiler compiler, TokenReader reader, Token<R> token, Token<R> fromToken)
        {
            compiler.AddToOutput(new RAText(token));
            yield break;
        }
    }
}
