using System.Collections.Generic;

using Rant.Core.Compiler.Syntax;
using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Parselets
{
    internal class WhitespaceParselet : Parselet
    {
        [TokenParser(R.Whitespace)]
        private IEnumerable<Parselet> Whitespace(Token<R> token)
        {
            // TODO: figure that out
            //switch (reader.PeekType())
            //{
            //    case R.EOF:
            //    case R.RightSquare:
            //    case R.RightAngle:
            //    case R.RightCurly:
            //        yield break;
            //    case R.Pipe:
            //        if (readType == ReadType.Block)
            //            yield break;
            //        break;
            //    case R.Semicolon:
            //        switch (readType)
            //        {
            //            case ReadType.FuncArgs:
            //            case ReadType.ReplacerArgs:
            //            case ReadType.SubroutineArgs:
            //                yield break;
            //        }
            //        break;
            //}
            AddToOutput(new RAText(token));
            yield break;
        }
    }
}
