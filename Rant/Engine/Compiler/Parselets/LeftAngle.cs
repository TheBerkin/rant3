using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Rant.Vocabulary;
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

        public override IEnumerator<Parselet> Parse(NewRantCompiler compiler, TokenReader reader, Token<R> fromToken)
        {
            Stringe name = null;

            if (reader.PeekToken().ID != R.DoubleColon)
                name = reader.ReadLoose(R.Text, "table name");

            if (name != null && (Util.IsNullOrWhiteSpace(name?.Value) || !Util.ValidateName(name?.Value)))
                compiler.SyntaxError(Token, "Invalid table name in query");

            compiler.SetNewQuery(new Query
            {
                ClassFilter = new ClassFilter(),
                RegexFilters = new List<_<bool, Regex>>(),
                Carrier = new Carrier(),
                Name = name?.Value
            });

            var exclusivity = reader.PeekToken();

            if (exclusivity == null)
                compiler.SyntaxError(Token, "Unexpected end of file");

            if (exclusivity.ID == R.Dollar)
            {
                compiler.GetQuery().Exclusive = true;
                reader.ReadToken();
            }

            // these are the tokens for queries
            var allowedTokens = new[] {
                R.Subtype, R.Hyphen,
                R.Without, R.Question,
                R.LeftParen, R.RightAngle
            };

            compiler.SetFromToken(Token);
            Token<R> queryReadToken = null;
            while ((queryReadToken = reader.ReadAny(allowedTokens)).ID != R.RightAngle)
            {
                yield return Parselet.GetWithToken(queryReadToken);
            }

            yield return Parselet.GetWithToken(queryReadToken);
        }
    }
}
