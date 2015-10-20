using System;
using System.Linq;

using Rant.Engine.Syntax;
using Rant.Vocabulary;
using Rant.Stringes;

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Rant.Engine.Compiler
{
	internal class RantCompiler
	{
		private readonly string _source;
		private readonly string _sourceName;
		//private readonly IEnumerable<Token<R>> _tokens;
		private readonly TokenReader _reader;
		private readonly Stack<RantFunctionGroup> _funcCalls = new Stack<RantFunctionGroup>();
		private readonly Stack<Regex> _regexes = new Stack<Regex>();
		private readonly Stack<RASubroutine> _subroutines = new Stack<RASubroutine>();
		private Query _query;
		private readonly RantExpressionCompiler _expressionCompiler;

		private RantCompiler(string sourceName, string source)
		{
			_sourceName = sourceName;
			_source = source;
			_reader = new TokenReader(sourceName, RantLexer.GenerateTokens(sourceName, source.ToStringe()));

            // commented so this builds
			//_expressionCompiler = new RantExpressionCompiler(sourceName, source, _reader, this);
		}

		private enum ReadType
		{
			/// <summary>
			/// Reads a list of items and returns an RASequence.
			/// </summary>
			Sequence,

			/// <summary>
			/// Reads a list of items and returns an RABlock.
			/// </summary>
			Block,

			/// <summary>
			/// Reads a list of arguments and returns an RAFunction.
			/// </summary>
			FuncArgs,

			/// <summary>
			/// Reads a query and returns a RAQuery
			/// </summary>
			Query,

			/// <summary>
			/// Reads a query carrier
			/// </summary>
			QueryCarrier,

			/// <summary>
			/// Reads the arguments needed by a replacer and return an RAReplacer.
			/// </summary>
			ReplacerArgs,

			/// <summary>
			/// Reads the arguments needed by a subroutine.
			/// </summary>
			SubroutineArgs,

			/// <summary>
			/// Reads the body of a subroutine.
			/// </summary>
			SubroutineBody,

			/// <summary>
			/// Reads a block weight.
			/// </summary>
			BlockWeight
		}

		public static RantAction Compile(string sourceName, string source)
		{
			return new NewRantCompiler(sourceName, source).Read(Compiler.ReadType.Sequence);
		}

		private RantAction Read(ReadType type, Token<R> fromToken = null)
		{
			// Stores actions for a single block item, argument, etc.
			var actions = new List<RantAction>();
			// Stores sequences of actions for blocks, function calls, etc.
			var sequences = new List<RantAction>();

			List<_<int, RantAction>> dynamicWeights = null;
			List<_<int, double>> constantWeights = null;

			Token<R> token = null;

			// ok let's do this
			while (!_reader.End)
			{
				token = _reader.ReadToken();

				switch (token.ID)
				{
					case R.EOF:
						goto done;

					// Escape sequence
					case R.EscapeSequence:
						actions.Add(new RAEscape(token));
						break;

					case R.LeftSquare:
						{
							var tagToken = _reader.ReadToken();
							switch (tagToken.ID)
							{
								case R.Text:
									{
										string name = tagToken.Value;
										var group = RantFunctions.GetFunctionGroup(name);
										if (group == null)
											SyntaxError(tagToken, $"Unknown function: '{name}'");
										var argList = new List<RantAction>();
										if (_reader.TakeLoose(R.Colon))
										{
											_funcCalls.Push(group);
											actions.Add(Read(ReadType.FuncArgs, token));
										}
										else
										{
											var end = _reader.Read(R.RightSquare);
											actions.Add(new RAFunction(Stringe.Range(token, end),
                                                GetFunctionInfo(group, 0, token, end), argList));
										}
										break;
									}
								case R.Regex:
									{
										try
										{
											_regexes.Push(Util.ParseRegex(tagToken.Value));
										}
										catch (Exception e)
										{
											SyntaxError(tagToken, e);
										}
										_reader.ReadLoose(R.Colon);
										actions.Add(Read(ReadType.ReplacerArgs, token));
										break;
									}
								case R.At:
									{
										actions.Add(_expressionCompiler.Read());
										break;
									}
								case R.Dollar:
									{
										bool call = false;
										var nextToken = _reader.ReadToken();
										// if the first token isn't a square, it's the name of the subroutine call
										if (nextToken.ID != R.LeftSquare)
											call = true;
										// read the name for the subroutine definition
										else
											nextToken = _reader.Read(R.Text, "subroutine name");
										// if there's a token here, there's args
										var hasArgs = _reader.Take(R.Colon);
										if (call)
										{
											_subroutines.Push(new RACallSubroutine(nextToken));
											actions.Add(Read(ReadType.SubroutineArgs, token));
											break;
										}
										// this is a subroutine definition
										var subroutine = new RADefineSubroutine(nextToken);
										subroutine.Parameters = new Dictionary<string, SubroutineParameterType>();
										// read all the args
										// right now we're at [$[name: or [$[name
										while (hasArgs && !_reader.End)
										{
											var nameToken = _reader.ReadLooseToken();
											if (nameToken.ID == R.Text)
												subroutine.Parameters.Add(nameToken.Value, SubroutineParameterType.Greedy);
											else if (nameToken.ID == R.At)
											{
												nameToken = _reader.ReadLoose(R.Text);
												subroutine.Parameters.Add(nameToken.Value, SubroutineParameterType.Loose);
											}
											else
												SyntaxError(nameToken, "Expected subroutine parameter.");
											nextToken = _reader.ReadLooseToken();
											if (nextToken.ID == R.Semicolon)
												continue;
											if (nextToken.ID == R.RightSquare)
												break;
											SyntaxError(nextToken, "Unexpected token in subroutine parameter definition.");
										}
										// if there's no args, we need to get rid of the right square before the body
										if(!hasArgs)
											_reader.ReadLoose(R.RightSquare);
										// start reading the body
										_reader.ReadLoose(R.Colon);
										subroutine.Body = Read(ReadType.SubroutineBody, token);
										actions.Add(subroutine);
										break;
									}
								default:
									SyntaxError(tagToken, "Expected function name, subroutine, or regex.");
									break;
							}
							break;
						}

					case R.RightSquare:
						{
							if (
								!(type == ReadType.FuncArgs || type == ReadType.ReplacerArgs || type == ReadType.SubroutineBody ||
								  type == ReadType.SubroutineArgs))
								Unexpected(token);
							// Add item to args
							sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions, token));
							switch (type)
							{
								case ReadType.SubroutineBody:
									return new RASequence(sequences, token);
								case ReadType.FuncArgs:
									{
										return new RAFunction(Stringe.Range(fromToken, token), 
                                            GetFunctionInfo(_funcCalls.Pop(), sequences.Count, fromToken, token), sequences);
									}
								case ReadType.ReplacerArgs:
									if (sequences.Count != 2)
										SyntaxError(Stringe.Between(token, fromToken), "Replacer must have two arguments.");
									return new RAReplacer(Stringe.Range(fromToken, token),
										_regexes.Pop(), sequences[0], sequences[1]);
								case ReadType.SubroutineArgs:
									var subroutine = _subroutines.Peek();
									subroutine.Arguments = sequences
										// filter out empty sequences
										.Where(x => !(x is RASequence && (x as RASequence).Actions.Count == 0)).ToList();
									if (subroutine is RACallSubroutine)
										return _subroutines.Pop();
									break;
							}
							break;
						}

					// Argument separator
					case R.Semicolon:
						if (!(type == ReadType.FuncArgs || type == ReadType.ReplacerArgs || type == ReadType.SubroutineArgs))
							goto default;
						// If it's a replacer, make sure there are no arguments already.
						if (type == ReadType.ReplacerArgs && sequences.Count == 1)
							SyntaxError(token, "Too many arguments in replacer.");
						// Add item to args
						sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions, token));
						actions.Clear();
						_reader.SkipSpace();
						break;

					// Block
					case R.LeftCurly:
						_reader.SkipSpace();
						actions.Add(Read(ReadType.Block, token));
						break;

					// Block item boundary
					case R.Pipe:
					case R.RightCurly:
						// Wrong mode?
						if (type != ReadType.Block)
						{
							// Throw an error if it's '}'
							if (token.ID == R.RightCurly)
								Unexpected(token);
							// If it's a separator, just print it.
							goto default;
						}

						// Add item to block
						sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions, token));
						if (token.ID == R.RightCurly)
							return new RABlock(Stringe.Range(fromToken, token), sequences,
								dynamicWeights, constantWeights);
						_reader.SkipSpace();
						actions.Clear();
						break;

					// Constant literals
					case R.ConstantLiteral:
						actions.Add(new RAText(token, Util.UnescapeConstantLiteral(token.Value)));
						break;
					//queries
					case R.LeftAngle:
						{
							Stringe name = null;
							if (_reader.PeekToken().ID != R.DoubleColon)
								name = _reader.ReadLoose(R.Text, "table name");
							if (name != null && (Util.IsNullOrWhiteSpace(name?.Value) || !Util.ValidateName(name?.Value)))
								SyntaxError(token, "Invalid table name in query.");
							_query = new Query
							{
								ClassFilter = new ClassFilter(),
								RegexFilters = new List<_<bool, Regex>>(),
								Carrier = new Carrier(),
								Name = name?.Value
							};
							var exclusivity = _reader.PeekToken();
							if (exclusivity.ID == R.Dollar)
							{
								_query.Exclusive = true;
								_reader.ReadToken();
							}
							actions.Add(Read(ReadType.Query, token));
						}
						break;
					// query subtypes
					case R.Subtype:
						{
							if (type != ReadType.Query) goto default;
							var subtype = _reader.ReadLoose(R.Text, "subtype");
							_query.Subtype = subtype.Value;
						}
						break;
					// query filters
					case R.Hyphen:
						{
							if (type != ReadType.Query) goto default;
						    var filterParts = new List<ClassFilterRule>();
						    do
						    {
                                bool negative = _reader.Take(R.Exclamation);
                                if (_query.Exclusive && negative)
                                    throw new RantCompilerException(_sourceName, token,
                                        "You can't define a negative class filter in an exclusive query.");
                                var className = _reader.ReadLoose(R.Text, "class name");
						        filterParts.Add(new ClassFilterRule(className.Value, !negative));
						    } while (_reader.TakeLoose(R.Pipe));
							_query.ClassFilter.AddRuleSwitch(filterParts.ToArray());
						}
						break;
					// query regex filters
					case R.Without:
					case R.Question:
						{
							if (type != ReadType.Query) goto default;
							bool negative = token.ID == R.Without;
							var regexToken = _reader.ReadLoose(R.Regex, "regex");
							((List<_<bool, Regex>>)_query.RegexFilters)
								.Add(new _<bool, Regex>(!negative, Util.ParseRegex(regexToken.Value)));
							break;
						}

					// syllable range, block weight
					case R.LeftParen:
						{
							// Block weight
							if (type == ReadType.Block && !actions.Any())
							{
								if (constantWeights == null) constantWeights = new List<_<int, double>>();
								if (dynamicWeights == null) dynamicWeights = new List<_<int, RantAction>>();
								var weightAction = Read(ReadType.BlockWeight, token);
								if (weightAction is RAText)	// Constant
								{
									var strWeight = (weightAction as RAText).Text;
									double d;
									int i;
									if (!Double.TryParse(strWeight, out d))
									{
										if (Util.ParseInt(strWeight, out i))
										{
											d = i;
										}
										else
										{
											SyntaxError(weightAction.Range,
												$"Invalid weight value '{strWeight}' - constant must be a number.");
										}
									}
									if (d < 0)
										SyntaxError(weightAction.Range,
											$"Invalid weight value '{strWeight}' - constant cannot be negative.");

									constantWeights.Add(_.Create(sequences.Count, d));
								}
								else // Dynamic
								{
									dynamicWeights.Add(_.Create(sequences.Count, weightAction));
								}
								break;
							}

							if (type != ReadType.Query) goto default;
							var nextToken = _reader.ReadToken();
							int firstNumber = -1;
							int secondNumber = -1;
							var range = new Range(null, null);
							if (nextToken.ID == R.Number)
							{
								Util.ParseInt(nextToken.Value, out firstNumber);
								nextToken = _reader.ReadToken();
							}
							if (nextToken.ID == R.Hyphen)
							{
								var number = _reader.ReadToken();
								// (num - num)
								if (number.ID == R.Number)
								{
									Util.ParseInt(number.Value, out secondNumber);
									range.Minimum = firstNumber;
									range.Maximum = secondNumber;
								}
								// (num -)
								else if (number.ID == R.RightParen && firstNumber != -1)
									range.Minimum = firstNumber;
							}
							// (- num), interpreted as -num
							else if (firstNumber < 0)
								range.Maximum = Math.Abs(firstNumber);
							// (num)
							else if (firstNumber != -1)
								range.Maximum = range.Minimum = firstNumber;
							if (range.Minimum == null && range.Maximum == null)
								throw new RantCompilerException(_sourceName, token, "Unknown syllable range syntax.");
							_query.SyllablePredicate = range;
							break;
						}

					case R.RightParen:
						{
							if (type != ReadType.BlockWeight) goto default;
							_reader.SkipSpace();
							if (!actions.Any()) SyntaxError(token, "Expected weight value.");
							return actions.Count == 1 && actions[0] is RAText ? actions[0] : new RASequence(actions, token);
						}

					// query carriers
					case R.DoubleColon:
						{
							if (type == ReadType.QueryCarrier)
								SyntaxError(token, "Unexpected carrier operator.");
							if (type != ReadType.Query) goto default;
							return Read(ReadType.QueryCarrier, token);
						}
					// match carrier
					case R.Equal:
						{
							if (type != ReadType.QueryCarrier) goto default;
							var name = _reader.ReadLoose(R.Text, "carrier match identifier");
							_query.Carrier.AddComponent(CarrierComponent.Match, name.Value);
						}
						break;
					// associative, disassociative, divergent, relational, or the match versions of those
					case R.At:
						{
							if (type != ReadType.QueryCarrier) goto default;
							// 0 = associative, 1 = disassociative, 2 = divergent, 3 = relational
							var componentType = CarrierComponent.Associative;
							var nextToken = _reader.PeekToken();
							// disassociative
							switch (nextToken.ID)
							{
								case R.Exclamation:
									componentType = CarrierComponent.Dissociative;
									_reader.ReadToken();
									break;
								case R.Plus:
									componentType = CarrierComponent.Divergent;
									_reader.ReadToken();
									break;
								case R.Question:
									componentType = CarrierComponent.Relational;
									_reader.ReadToken();
									break;
							}
							// match
							if (_reader.PeekToken().ID == R.Equal)
							{
								_reader.ReadToken();
								if (componentType == CarrierComponent.Associative)
									componentType = CarrierComponent.MatchAssociative;
								if (componentType == CarrierComponent.Dissociative)
									componentType = CarrierComponent.MatchDissociative;
								if (componentType == CarrierComponent.Divergent)
									componentType = CarrierComponent.MatchDivergent;
								if (componentType == CarrierComponent.Relational)
									componentType = CarrierComponent.MatchRelational;
							}
							var name = _reader.ReadLoose(R.Text, "carrier association identifier").Value;
							_query.Carrier.AddComponent(componentType, name);
						}
						break;
					// unique or match-unique
					case R.Exclamation:
						{
							if (type != ReadType.QueryCarrier) goto default;
							bool match = false;
							if (_reader.PeekToken().ID == R.Equal)
							{
								match = true;
								_reader.ReadToken();
							}
							var name = _reader.ReadLoose(R.Text, "carrier unique identifier").Value;
							var componentType = (match ? CarrierComponent.MatchUnique : CarrierComponent.Unique);
							_query.Carrier.AddComponent(componentType, name);
						}
						break;
					// rhyme
					case R.Ampersand:
						{
							if (type != ReadType.QueryCarrier) goto default;
							var name = _reader.ReadLoose(R.Text, "carrier rhyme identifier").Value;
							_query.Carrier.AddComponent(CarrierComponent.Rhyme, name);
						}
						break;
					// end of queries
					case R.RightAngle:
						if (type != ReadType.Query && type != ReadType.QueryCarrier)
							SyntaxError(token, "Unexpected query terminator.");
						if (_query.Name == null && _query.Carrier.GetTotalCount() == 0)
							SyntaxError(token, "Carrier delete query specified without any carriers.");

						return new RAQuery(_query, Stringe.Between(fromToken, token));

					// Plain text
					case R.Whitespace:
						{
							switch (_reader.PeekType())
							{
								case R.EOF:
								case R.RightSquare:
								case R.RightAngle:
								case R.RightCurly:
									continue;
								case R.Pipe:
									if (type == ReadType.Block) continue;
									break;
								case R.Semicolon:
									switch (type)
									{
										case ReadType.FuncArgs:
										case ReadType.ReplacerArgs:
										case ReadType.SubroutineArgs:
											continue;
									}
									break;
							}
							actions.Add(new RAText(token));
							break;
						}
					default:
						if (type == ReadType.QueryCarrier)
							SyntaxError(token, "Expected query carrier.");
						actions.Add(new RAText(token));
						break;
				}
			}

			done:

			switch (type)
			{
				case ReadType.Sequence:
					return new RASequence(actions, token);
				case ReadType.Block:
					throw new RantCompilerException(_sourceName, fromToken, "Unterminated block found.");
				default:
					throw new RantCompilerException(_sourceName, token, "Unexpected end of file.");
			}
		}

		private RantFunctionInfo GetFunctionInfo(RantFunctionGroup group, int argc, Stringe from, Stringe to)
		{
		    RantFunctionInfo func = group.GetFunction(argc);
            if (func == null)
                SyntaxError(Stringe.Between(from, to), $"No overload of function '{group.Name}' can take {argc} argument(s).");
		    return func;
		}

		private void Unexpected(Stringe token)
		{
			throw new RantCompilerException(_sourceName, token, $"Unexpected token: '{token.Value}'");
		}

		private void SyntaxError(Stringe token, string message)
		{
			throw new RantCompilerException(_sourceName, token, message);
		}

		private void SyntaxError(Stringe token, Exception innerException)
		{
			throw new RantCompilerException(_sourceName, token, innerException);
		}
	}
}