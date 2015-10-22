using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    internal class Whitespace : Parselet
    {
        public override R Identifier
        {
            get
            {
                return R.Whitespace;
            }
        }

        public Whitespace()
        {
        }

        public override IEnumerator<Parselet> Parse(NewRantCompiler compiler, TokenReader reader, ReadType readType, Token<R> token)
        {
            switch (reader.PeekType())
            {
                case R.EOF:
                case R.RightSquare:
                case R.RightAngle:
                case R.RightCurly:
                    yield break;
                case R.Pipe:
                    if (readType == ReadType.Block)
                        yield break;
                    break;
                case R.Semicolon:
                    switch (readType)
                    {
                        case ReadType.FuncArgs:
                        case ReadType.ReplacerArgs:
                        case ReadType.SubroutineArgs:
                            yield break;
                    }
                    break;
            }
            compiler.AddToOutput(new RAText(token));
        }
    }
}
