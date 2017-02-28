#region License

// https://github.com/TheBerkin/Rant
// 
// Copyright (c) 2017 Nicholas Fleck
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Rant.Core.Compiler.Syntax;
using Rant.Core.Utilities;
using Rant.Vocabulary.Querying;

namespace Rant.Core.Compiler.Parsing
{
	internal class QueryParser : Parser
	{
		public override IEnumerator<Parser> Parse(RantCompiler compiler, CompileContext context, TokenReader reader,
			Action<RST> actionCallback)
		{
			var tableName = reader.ReadLoose(R.Text, "acc-table-name");
			var query = new Query();
			query.Name = tableName.Value;
			query.Carrier = new Carrier();
			query.Exclusive = reader.TakeLoose(R.Dollar);
			bool subtypeRead = false;
			bool pluralSubtypeRead = false;
			bool complementRead = false;
			bool endOfQueryReached = false;

			while (!reader.End && !endOfQueryReached)
			{
				var token = reader.ReadLooseToken();

				switch (token.Type)
				{
					// read subtype
					case R.Period:
						if (reader.Take(R.Period)) // Plural subtype
						{
							if (pluralSubtypeRead)
							{
								compiler.SyntaxError(token, false, "err-compiler-multiple-pl-subtypes");
								reader.Read(R.Text, "acc-pl-subtype-name");
								break;
							}

							query.PluralSubtype = reader.Read(R.Text, "acc-pl-subtype-name").Value;
							pluralSubtypeRead = true;
							break;
						}
						// if there's already a subtype, throw an error and ignore it
						if (subtypeRead)
						{
							compiler.SyntaxError(token, false, "err-compiler-multiple-subtypes");
							reader.Read(R.Text, "acc-subtype-name");
							break;
						}
						query.Subtype = reader.Read(R.Text, "acc-subtype-name").Value;
						subtypeRead = true;
						break;
					// complement
					case R.LeftSquare:
					{
						if (complementRead) compiler.SyntaxError(token, false, "err-compiler-multiple-complements");
						var seq = new List<RST>();
						compiler.AddContext(CompileContext.QueryComplement);
						compiler.SetNextActionCallback(seq.Add);
						yield return Get<SequenceParser>();
						compiler.SetNextActionCallback(actionCallback);
						query.Complement = new RstSequence(seq, token.ToLocation());
						complementRead = true;
					}
						break;
					// read class filter
					case R.Hyphen:
					{
						var classFilter = new ClassFilter();
						do
						{
							reader.SkipSpace();
							bool blacklist = false;
							// check if it's a blacklist filter
							if (reader.PeekType() == R.Exclamation)
							{
								blacklist = true;
								reader.ReadToken();
							}
							var classFilterName = reader.Read(R.Text, "acc-class-filter-rule");
							if (classFilterName.Value == null) continue;
							var rule = new ClassFilterRule(classFilterName.Value, !blacklist);
							classFilter.AddRule(rule);
						} while (reader.TakeLoose(R.Pipe)); //fyi: this feature is undocumented

						query.AddFilter(classFilter);
						break;
					}
					// read regex filter
					case R.Without:
					case R.Question:
					{
						reader.SkipSpace();
						bool blacklist = token.Type == R.Without;

						var regexFilter = reader.Read(R.Regex, "regex filter rule");
						var options = RegexOptions.Compiled | RegexOptions.ExplicitCapture;
						if (reader.IsNext(R.RegexFlags))
						{
							var flagsToken = reader.ReadToken();
							foreach (char flag in flagsToken.Value)
								switch (flag)
								{
									case 'i':
										options |= RegexOptions.IgnoreCase;
										break;
									case 'm':
										options |= RegexOptions.Multiline;
										break;
								}
						}
						if (regexFilter.Value == null) break;
						query.AddFilter(new RegexFilter(new Regex(regexFilter.Value, options), !blacklist));
					}
						break;

					// read syllable range
					case R.LeftParen:
						// There are four possible types of values in a syllable range:
						// (a), (a-), (-b), (a-b)

						// either (a), (a-), or (a-b)
						if (reader.PeekLooseToken().Type == R.Text)
						{
							var firstNumberToken = reader.ReadLooseToken();
							int firstNumber;
							if (!Util.ParseInt(firstNumberToken.Value, out firstNumber))
								compiler.SyntaxError(firstNumberToken, false, "err-compiler-bad-sylrange-value");

							// (a-) or (a-b)
							if (reader.PeekLooseToken().Type == R.Hyphen)
							{
								reader.ReadLooseToken();
								// (a-b)
								if (reader.PeekLooseToken().Type == R.Text)
								{
									var secondNumberToken = reader.ReadLooseToken();
									int secondNumber;
									if (!Util.ParseInt(secondNumberToken.Value, out secondNumber))
										compiler.SyntaxError(secondNumberToken, false, "err-compiler-bad-sylrange-value");

									query.AddFilter(new RangeFilter(firstNumber, secondNumber));
								}
								// (a-)
								else
								{
									query.AddFilter(new RangeFilter(firstNumber, null));
								}
							}
							// (a)
							else
							{
								query.AddFilter(new RangeFilter(firstNumber, firstNumber));
							}
						}
						// (-b)
						else if (reader.PeekLooseToken().Type == R.Hyphen)
						{
							reader.ReadLooseToken();
							var secondNumberToken = reader.ReadLoose(R.Text, "acc-syllable-range-value");
							int secondNumber;
							if (!Util.ParseInt(secondNumberToken.Value, out secondNumber))
								compiler.SyntaxError(secondNumberToken, false, "err-compiler-bad-sylrange-value");
							query.AddFilter(new RangeFilter(null, secondNumber));
						}
						// ()
						else if (reader.PeekLooseToken().Type == R.RightParen)
						{
							compiler.SyntaxError(token, false, "err-compiler-empty-sylrange");
						}
						// (something else)
						else
						{
							var errorToken = reader.ReadLooseToken();
							compiler.SyntaxError(errorToken, false, "err-compiler-unknown-sylrange-token", errorToken.Value);
							reader.TakeAllWhile(t => !reader.IsNext(R.RightParen));
						}

						reader.ReadLoose(R.RightParen);
						break;

					// read carriers
					case R.DoubleColon:
						ReadCarriers(reader, query.Carrier, compiler);
						// this should be the last part of the query, so go to the end
						endOfQueryReached = true;
						break;

					// end of query
					case R.RightAngle:
						endOfQueryReached = true;
						break;

					case R.Whitespace:
						break;

					default:
						compiler.SyntaxError(token, false, "err-compiler-unexpected-token");
						break;
				}
			}

			if (!endOfQueryReached)
				compiler.SyntaxError(reader.PrevToken, true, "err-compiler-eof");

			if (tableName.Value != null)
				actionCallback(new RstQuery(query, tableName.ToLocation()));
		}

		private void ReadCarriers(TokenReader reader, Carrier carrier, RantCompiler compiler)
		{
			while (!reader.End)
			{
				var token = reader.ReadLooseToken();

				switch (token.Type)
				{
					// match carrier
					case R.Equal:
					{
						var name = reader.Read(R.Text, "acc-carrier-name");
						if (name.Value != null)
							carrier.AddComponent(CarrierComponentType.Match, name.Value);
					}
						break;

					// associative and match associative,
					// disassociative and match disassociative
					// divergent and match-divergent
					// relational and match-relational
					case R.At:
					{
						var carrierType = CarrierComponentType.Associative;
						// disassociative
						if (reader.PeekToken().Type == R.Exclamation)
						{
							carrierType = CarrierComponentType.Dissociative;
							reader.ReadToken();
						}
						// divergent
						else if (reader.PeekToken().Type == R.Plus)
						{
							carrierType = CarrierComponentType.Divergent;
							reader.ReadToken();
						}
						else if (reader.PeekToken().Type == R.Question)
						{
							carrierType = CarrierComponentType.Relational;
							reader.ReadToken();
						}

						// match
						if (reader.PeekToken().Type == R.Equal)
						{
							if (carrierType == CarrierComponentType.Associative)
								carrierType = CarrierComponentType.MatchAssociative;
							else if (carrierType == CarrierComponentType.Dissociative)
								carrierType = CarrierComponentType.MatchDissociative;
							else if (carrierType == CarrierComponentType.Divergent)
								carrierType = CarrierComponentType.MatchDivergent;
							else if (carrierType == CarrierComponentType.Relational)
								carrierType = CarrierComponentType.MatchRelational;
							reader.ReadToken();
						}

						var name = reader.Read(R.Text, "acc-carrier-name");
						if (name.Value != null)
							carrier.AddComponent(carrierType, name.Value);
					}
						break;

					// unique and match unique
					case R.Exclamation:
					{
						var carrierType = CarrierComponentType.Unique;
						// match unique
						if (reader.PeekToken().Type == R.Equal)
						{
							carrierType = CarrierComponentType.MatchUnique;
							reader.ReadToken();
						}

						var name = reader.Read(R.Text, "acc-carrier-name");
						if (name.Value != null)
							carrier.AddComponent(carrierType, name.Value);
					}
						break;

					// rhyming
					case R.Ampersand:
					{
						var name = reader.Read(R.Text, "acc-carrier-name");
						if (name.Value != null)
							carrier.AddComponent(CarrierComponentType.Rhyme, name.Value);
					}
						break;

					// we're done, go away
					case R.RightAngle:
						return;

					default:
						compiler.SyntaxError(token, false, "err-compiler-unexpected-token");
						break;
				}
			}
		}
	}
}