using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rant.Vocabulary;
using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    internal class RightAngle : Parselet
    {
        public override R[] Identifiers
        {
            get
            {
                return new[] { R.RightAngle };
            }
        }

        public RightAngle()
        {
        }

        public override IEnumerator<Parselet> Parse(NewRantCompiler compiler, TokenReader reader, Token<R> token, Token<R> fromToken)
        {
            // TODO: figure out how to throw syntax errors if we're not coming from a query parselet

            var query = compiler.GetQuery();

            if (query.Name == null && query.Carrier.GetTotalCount() == 0)
                compiler.SyntaxError(token, "Carrier delete query specified without any carriers");

            compiler.AddToOutput(new RAQuery(query, Stringe.Range(fromToken, token)));
            yield break;
        }
    }
}
