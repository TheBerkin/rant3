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

        Query query;

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

            query = new Query
            {
                ClassFilter = new ClassFilter(),
                RegexFilters = new List<_<bool, Regex>>(),
                Carrier = new Carrier(),
                Name = name?.Value
            };

            var exclusivity = reader.PeekToken();

            if (exclusivity == null)
                compiler.SyntaxError(Token, "Unexpected end of file");

            if (exclusivity.ID == R.Dollar)
            {
                query.Exclusive = true;
                reader.ReadToken();
            }

            // these are the tokens for queries
            var allowedTokens = new[] {
                R.Subtype, R.Hyphen,
                R.Without, R.Question,
                R.LeftParen, R.RightAngle
            };

            Token<R> queryReadToken = null;
            while ((queryReadToken = reader.ReadToken()).ID != R.RightAngle)
            {
                // egh
                switch (queryReadToken.ID)
                {
                    default:
                        compiler.SyntaxError(queryReadToken, $"Invalid token in query: {queryReadToken.Value}");
                        break;

                    case R.Subtype:
                        Subtype(compiler, reader);
                        break;

                    case R.Hyphen:
                        Hyphen(compiler, reader);
                        break;

                    case R.LeftParen:
                        LeftParen(compiler, reader);
                        break;

                    case R.Question:
                    case R.Without:
                        RegexFilter(compiler, reader);
                        break;
                }
            }

            RightAngle(compiler, reader, Token);
            yield break;
        }

        void RightAngle(NewRantCompiler compiler, TokenReader reader, Token<R> fromToken)
        {
            if (query.Name == null && query.Carrier.GetTotalCount() == 0)
                compiler.SyntaxError(Token, "Carrier delete query specified without any carriers");

            compiler.AddToOutput(new RAQuery(query, Stringe.Range(fromToken, Token)));
        }

        void RegexFilter(NewRantCompiler compiler, TokenReader reader)
        {
            var negative = Token.ID == R.Without;
            var regexToken = reader.ReadLoose(R.Regex, "regex");

            ((List<_<bool, Regex>>)query.RegexFilters).Add(new _<bool, Regex>(!negative, Util.ParseRegex(regexToken.Value)));
        }

        void Subtype(NewRantCompiler compiler, TokenReader reader)
        {
            var subtypeToken = reader.ReadLoose(R.Text, "subtype");
            query.Subtype = subtypeToken.Value;
        }

        void Hyphen(NewRantCompiler compiler, TokenReader reader)
        {
            var filterParts = new List<ClassFilterRule>();
            do
            {
                var negative = reader.Take(R.Exclamation);
                if (query.Exclusive && negative)
                    compiler.SyntaxError(Token, "You can't define a negative class filter in an exclusive query");

                var classNameToken = reader.ReadLoose(R.Text, "class name");
                filterParts.Add(new ClassFilterRule(classNameToken.Value, !negative));
            } while (reader.TakeLoose(R.Pipe));

            query.ClassFilter.AddRuleSwitch(filterParts.ToArray());
        }

        void LeftParen(NewRantCompiler compiler, TokenReader reader)
        {
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

            query.SyllablePredicate = range;
        }
    }
}
