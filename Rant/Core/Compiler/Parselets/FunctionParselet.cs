using System.Collections.Generic;

using Rant.Core.Stringes;

namespace Rant.Core.Compiler.Parselets
{
    internal class FunctionParselet : Parselet
    {
        [TokenParser(R.RightSquare)]
        private IEnumerable<Parselet> RightSquare(Token<R> token)
        {
            // TODO: is it 'tag' or 'method' or 'function' or what?
            compiler.SyntaxError(token, "Unexpected function terminator");
            yield break;
        }

        [TokenParser(R.LeftSquare)]
        private IEnumerable<Parselet> LeftSquare(Token<R> token)
        {
            var tagToken = reader.ReadToken();
            switch (tagToken.ID)
            {
                default:
                    compiler.SyntaxError(tagToken, "Expected function name, subroutine, regex or a script.");
                    break;

				case R.Text:
                    yield return Parselet.GetParselet("FunctionText", tagToken, AddToOutput);
                    break;

                case R.Regex:
                    yield return Parselet.GetParselet("FunctionRegex", tagToken, AddToOutput);
                    break;

				case R.Dollar:
                    yield return Parselet.GetParselet("FunctionSubroutine", tagToken, AddToOutput);
                    break;
            }
        }
    }
}
