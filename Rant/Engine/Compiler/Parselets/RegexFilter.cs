using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    internal class RegexFilter : Parselet
    {
        public override R[] Identifiers
        {
            get
            {
                return new[] { R.Without, R.Question };
            }
        }

        public RegexFilter()
        {
        }

        public override IEnumerator<Parselet> Parse(NewRantCompiler compiler, TokenReader reader, Token<R> fromToken)
        {
            if (fromToken != null && fromToken.ID != R.LeftAngle)
            {
                yield return Parselet.DefaultParselet;
                yield break;
            }

            var negative = Token.ID == R.Without;
            var regexToken = reader.ReadLoose(R.Regex, "regex");

            ((List<_<bool, Regex>>)compiler.GetQuery().RegexFilters).Add(new _<bool, Regex>(!negative, Util.ParseRegex(regexToken.Value)));
        }
    }
}
