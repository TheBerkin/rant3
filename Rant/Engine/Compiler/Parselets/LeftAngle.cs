using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    internal class LeftAngle : Parselet
    {
        public override R[] Identifiers
        {
            get
            {
                return new[] { R.LeftAngle };
            }
        }

        public LeftAngle()
        {
        }

        public override IEnumerator<Parselet> Parse(NewRantCompiler compiler, TokenReader reader, Token<R> token)
        {
            
        }
    }
}
