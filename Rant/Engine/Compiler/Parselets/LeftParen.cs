using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rant.Stringes;
using Rant.Vocabulary;

namespace Rant.Engine.Compiler.Parselets
{
    internal class LeftParen : Parselet
    {
        public override R[] Identifiers
        {
            get
            {
                return new[] { R.LeftParen };
            }
        }

        public LeftParen()
        {
        }

        public override IEnumerator<Parselet> Parse(NewRantCompiler compiler, TokenReader reader, Token<R> fromToken)
        {
            if (fromToken == null || fromToken.ID != R.LeftAngle)
            {
                yield return Parselet.GetDefaultParselet(Token);
                yield break;
            }

            var nextToken = reader.ReadToken();
            var firstNum = -1;
            var secondNum = -1;
            var range = new Range(null, null);

            if (nextToken.ID == R.Text) // NOTE: this was originally R.Number. the TokenReader returns a text token for numerals
            {
                Util.ParseInt(nextToken.Value, out firstNum);
                nextToken = reader.ReadToken();
            }

            if (nextToken.ID == R.Hyphen)
            {
                var numToken = reader.ReadToken();

                // (num - num)
                if (numToken.ID == R.Text)
                {
                    Util.ParseInt(numToken.Value, out secondNum);
                    range.Minimum = firstNum;
                    range.Maximum = secondNum;
                }
                else if (numToken.ID == R.RightParen && firstNum != -1) // (num -)
                {
                    range.Minimum = firstNum;
                }
            }
            else if (firstNum < 0) // (- num), interpreted as -num
            {
                range.Maximum = Math.Abs(firstNum);
            }
            else if (firstNum != -1) // (num)
            {
                range.Maximum = firstNum;
                range.Minimum = firstNum;
            }

            if (nextToken.ID != R.RightParen)
                reader.Read(R.RightParen, "right parenthesis");

            if (range.Minimum == null && range.Maximum == null)
                compiler.SyntaxError(Token, "Unkown syllable range syntax");

            compiler.GetQuery().SyllablePredicate = range;
        }
    }
}
