using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Rant.Vocabulary;
using Rant.Engine.Syntax;
using Rant.Stringes;

namespace Rant.Engine.Compiler.Parselets
{
    internal class QueryParselet : Parselet
    {
        public override R Identifier => R.LeftAngle;

        Query query;

        protected override IEnumerable<Parselet> InternalParse(Token<R> token)
        {
            Stringe name = null;

            if (reader.PeekToken().ID != R.DoubleColon)
                name = reader.ReadLoose(R.Text, "table name");

            if (name != null && (Util.IsNullOrWhiteSpace(name?.Value) || !Util.ValidateName(name?.Value)))
                compiler.SyntaxError(token, "Invalid table name in query");

            query = new Query
            {
                ClassFilter = new ClassFilter(),
                RegexFilters = new List<_<bool, Regex>>(),
                Carrier = new Carrier(),
                Name = name?.Value
            };

            var exclusivity = reader.PeekToken();

            if (exclusivity == null)
                compiler.SyntaxError(token, "Unexpected end of file");

            if (exclusivity.ID == R.Dollar)
            {
                query.Exclusive = true;
                reader.ReadToken();
            }

            while (true) // looks kinda dangerous
            {
                var queryReadToken = reader.ReadToken();
                // egh
                switch (queryReadToken.ID)
                {
                    default:
                        compiler.SyntaxError(queryReadToken, $"Invalid token in query: '{queryReadToken.Value}'");
                        break;

                    case R.RightAngle:
                        RightAngle(queryReadToken, queryReadToken);
                        yield break;

                    case R.Subtype:
                        Subtype();
                        break;

                    case R.Hyphen:
                        Hyphen(queryReadToken);
                        break;

                    case R.LeftParen:
                        LeftParen(queryReadToken);
                        break;

                    case R.Question:
                    case R.Without:
                        RegexFilter(queryReadToken);
                        break;

                    case R.DoubleColon:
                        DoubleColon(queryReadToken);
                        break;
                }
            }
        }

        void DoubleColon(Token<R> token)
        {
            var carrierToken = reader.ReadLooseToken();

            switch (carrierToken.ID)
            {
                default:
                    compiler.SyntaxError(carrierToken, $"Invalid token in query carrier: '{carrierToken.Value}'");
                    break;

                case R.Equal: // match carrier
                    var name = reader.ReadLoose(R.Text, "carrier match identifier");
                    query.Carrier.AddComponent(CarrierComponent.Match, name.Value);
                    break;

                case R.At: // associative, disassociative, divergent, relational or the match versions of those
                    {
                        // 0 = associative, 1 = disassociative, 2 = divergent, 3 = relational
                        var componentType = CarrierComponent.Associative;
                        var nextToken = reader.PeekToken();

                        // disassociative
                        switch (nextToken.ID)
                        {
                            case R.Exclamation:
                                componentType = CarrierComponent.Dissociative;
                                reader.ReadToken();
                                break;

                            case R.Plus:
                                componentType = CarrierComponent.Divergent;
                                reader.ReadToken();
                                break;

                            case R.Question:
                                componentType = CarrierComponent.Relational;
                                reader.ReadToken();
                                break;
                        }

                        // match
                        if (reader.PeekToken().ID == R.Equal)
                        {
                            reader.ReadToken();
                            // nice if clause
                            if (componentType == CarrierComponent.Associative)
                                componentType = CarrierComponent.MatchAssociative;
                            if (componentType == CarrierComponent.Dissociative)
                                componentType = CarrierComponent.MatchDissociative;
                            if (componentType == CarrierComponent.Divergent)
                                componentType = CarrierComponent.MatchDivergent;
                            if (componentType == CarrierComponent.Relational)
                                componentType = CarrierComponent.MatchRelational;
                        }

                        var nameToken = reader.ReadLoose(R.Text, "carrier association identifier");
                        query.Carrier.AddComponent(componentType, nameToken.Value);
                        break;
                    }

                case R.Exclamation: // unique or match-unique
                    {
                        var match = false;

                        if (reader.PeekToken().ID == R.Equal)
                        {
                            match = true;
                            reader.ReadToken();
                        }

                        var nameToken = reader.ReadLoose(R.Text, "carrier unique identifier");
                        var componentType = match ? CarrierComponent.MatchUnique : CarrierComponent.Unique;
                        query.Carrier.AddComponent(componentType, nameToken.Value);
                        break;
                    }

                case R.Ampersand: // rhymes
                    {
                        // NOTE: rhymes may be broken for some reason. i'm not getting proper results. i'm using the dictionary v2.0.21 -spanfile
                        var nameToken = reader.ReadLoose(R.Text, "carrier rhyme identifier");
                        query.Carrier.AddComponent(CarrierComponent.Rhyme, nameToken.Value);
                    }
                    break;
            }
        }

        void RightAngle(Token<R> token, Token<R> fromToken)
        {
            if (query.Name == null && query.Carrier.GetTotalCount() == 0)
                compiler.SyntaxError(token, "Carrier delete query specified without any carriers");

            AddToOutput(new RAQuery(query, Stringe.Range(fromToken, token)));
        }

        void RegexFilter(Token<R> token)
        {
            var negative = token.ID == R.Without;
            var regexToken = reader.ReadLoose(R.Regex, "regex");

            // NOTE: why is this so complicated?
            ((List<_<bool, Regex>>)query.RegexFilters).Add(new _<bool, Regex>(!negative, Util.ParseRegex(regexToken.Value)));
        }

        void Subtype()
        {
            var subtypeToken = reader.ReadLoose(R.Text, "subtype");
            query.Subtype = subtypeToken.Value;
        }

        void Hyphen(Token<R> fromToken)
        {
            var filterParts = new List<ClassFilterRule>();
            do
            {
                var negative = reader.Take(R.Exclamation);
                if (query.Exclusive && negative)
                    compiler.SyntaxError(fromToken, "You can't define a negative class filter in an exclusive query");

                var classNameToken = reader.ReadLoose(R.Text, "class name");
                filterParts.Add(new ClassFilterRule(classNameToken.Value, !negative));
            } while (reader.TakeLoose(R.Pipe));

            query.ClassFilter.AddRuleSwitch(filterParts.ToArray());
        }

        void LeftParen(Token<R> fromToken)
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
                nextToken = reader.ReadToken();

                // (num - num)
                if (nextToken.ID == R.Text) // see note above
                {
                    Util.ParseInt(nextToken.Value, out secondNum);
                    range.Minimum = firstNum;
                    range.Maximum = secondNum;
                }
                else if (nextToken.ID == R.RightParen && firstNum != -1) // (num -)
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
                compiler.SyntaxError(fromToken, "Unkown syllable range syntax");

            query.SyllablePredicate = range;
        }
    }
}
