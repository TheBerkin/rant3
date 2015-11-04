using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    internal class EscapeSequence : Parselet
    {
        public override R[] Identifiers
        {
            get
            {
                return new[] { R.EscapeSequence };
            }
        }

        protected override IEnumerable<Parselet> InternalParse(Token<R> token, Token<R> fromToken)
        {
            AddToOutput(new RAEscape(token));
            yield break;
        }
    }
}
