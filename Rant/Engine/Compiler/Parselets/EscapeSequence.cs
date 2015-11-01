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

        public EscapeSequence()
        {
        }

        public override IEnumerator<Parselet> Parse(NewRantCompiler compiler, TokenReader reader, Token<R> token)
        {
            compiler.AddToOutput(new RAEscape(token));
            yield break;
        }
    }
}
