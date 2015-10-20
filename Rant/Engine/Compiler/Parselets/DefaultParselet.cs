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
        public override R Identifier
        {
            get
            {
                throw new RantInternalException("Cannot access DefaultParselet's identifier");
            }
        }

        public DefaultParselet()
        {
        }

        public override IEnumerator<Parselet> Parse(NewRantCompiler compiler, TokenReader reader, ReadType readType, Token<R> token)
        {
            if (readType == ReadType.QueryCarrier)
                compiler.SyntaxError(token, "Expected query carrier");

            compiler.AddToOutput(new RAText(token));
            yield break;
        }
    }
}
