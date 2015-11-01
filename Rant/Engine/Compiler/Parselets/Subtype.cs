using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    internal class Subtype : Parselet
    {
        public override R[] Identifiers
        {
            get
            {
                return new[] { R.Subtype };
            }
        }

        public Subtype()
        {
        }

        public override IEnumerator<Parselet> Parse(NewRantCompiler compiler, TokenReader reader, Token<R> fromToken)
        {
            if (fromToken == null || fromToken.ID != R.LeftAngle)
            {
                yield return Parselet.GetDefaultParselet(Token);
                yield break; 
            }

            var subtypeToken = reader.ReadLoose(R.Text, "subtype");
            compiler.GetQuery().Subtype = subtypeToken.Value;
            // for some reason, we don't need a yield break here because there's already one over there in the if block
        }
    }
}
