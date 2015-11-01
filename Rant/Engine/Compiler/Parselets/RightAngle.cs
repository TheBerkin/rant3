using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public override IEnumerator<Parselet> Parse(NewRantCompiler compiler, TokenReader reader, Token<R> fromToken)
        {
            if (fromToken == null || fromToken.ID != R.LeftAngle)
                compiler.SyntaxError(Token, "Unexpected query terminator");

            var query = compiler.GetQuery();

            if (query.Name == null && query.Carrier.GetTotalCount() == 0)
                compiler.SyntaxError(Token, "Carrier delete query specified without any carriers");

            compiler.AddToOutput(new RAQuery(query, Stringe.Range(fromToken, Token)));
            yield break;
        }
    }
}
