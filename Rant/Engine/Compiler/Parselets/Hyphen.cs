using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rant.Stringes;
using Rant.Vocabulary;

namespace Rant.Engine.Compiler.Parselets
{
    internal class Hyphen : Parselet
    {
        public override R[] Identifiers
        {
            get
            {
                return new[] { R.Hyphen };
            }
        }

        public Hyphen()
        {
        }

        public override IEnumerator<Parselet> Parse(NewRantCompiler compiler, TokenReader reader, Token<R> fromToken)
        {
            if (fromToken != null && fromToken.ID != R.LeftAngle)
            {
                yield return Parselet.DefaultParselet;
                yield break;
            }

            var filterParts = new List<ClassFilterRule>();
            do
            {
                var negative = reader.Take(R.Exclamation);
                if (compiler.GetQuery().Exclusive && negative)
                    compiler.SyntaxError(Token, "You can't define a negative class filter in an exclusive query");

                var classNameToken = reader.ReadLoose(R.Text, "class name");
                filterParts.Add(new ClassFilterRule(classNameToken.Value, !negative));
            } while (reader.TakeLoose(R.Pipe));

            compiler.GetQuery().ClassFilter.AddRuleSwitch(filterParts.ToArray());
        }
    }
}
