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
        public override R Identifier
        {
            get
            {
                return R.EscapeSequence;
            }
        }

        public EscapeSequence()
        {
        }

        public override IEnumerator<Parselet> Parse(NewRantCompiler compiler, TokenReader reader, ReadType readType, Token<R> token)
        {
            compiler.AddToOutput(new RAEscape(token));
            yield break;
        }
    }
}
