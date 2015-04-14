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
		private readonly IEnumerable<Token<R>> _tokens;
		private readonly TokenReader _reader;
		private readonly Stack<RantFunctionInfo> _funcCalls = new Stack<RantFunctionInfo>();
		private readonly Stack<Regex> _regexes = new Stack<Regex>();
		private readonly Stack<RASubroutine> _subroutines = new Stack<RASubroutine>();
		private Query _query;

		private RantCompiler(string sourceName, string source)
		{
			_sourceName = sourceName;
			_source = source;
			_tokens = RantLexer.GenerateTokens(source.ToStringe());
			_reader = new TokenReader(sourceName, _tokens);
			_subroutines = new Stack<RASubroutine>();
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
			FuncCall,
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
			SubroutineBody
		}

		public static RantAction Compile(string sourceName, string source)
		{
			return new RantCompiler(sourceName, source).Read(ReadType.Sequence);
		}

		private RantAction Read(ReadType type, Token<R> fromToken = null)
		{
			// Stores actions for a single block item, argument, etc.
			var actions = new List<RantAction>();
			// Stores sequences of actions for blocks, function calls, etc.
			var sequences = new List<RantAction>();

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
										var func = RantFunctions.GetFunction(name);
										if (func == null)
											SyntaxError(tagToken, $"Unknown function: '{name}'");
										var argList = new List<RantAction>();
										if (_reader.TakeLoose(R.Colon))
										{
											_funcCalls.Push(func);
											actions.Add(Read(ReadType.FuncCall, token));
										}
										else
										{
											var end = _reader.Read(R.RightSquare);
											VerifyArgCount(func, 0, token, end);
											actions.Add(new RAFunction(Stringe.Range(token, end), func, argList));
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
								case R.Dollar:
									{
										bool call = false;
										var nextToken = _reader.ReadToken();
										if (nextToken.ID != R.LeftSquare)
											call = true;
										else
											nextToken = _reader.ReadToken();
										if (nextToken.ID != R.Text)
											SyntaxError(nextToken, "Expected subroutine name.");
										if (_reader.PeekToken().ID == R.Colon) _reader.ReadToken();
										_subroutines.Push(new RASubroutine(nextToken));
										_subroutines.Peek().IsCall = call;
										actions.Add(Read(ReadType.SubroutineArgs, token));
									}
									break;
								default:
									SyntaxError(tagToken, "Expected function name or regex.");
									break;
							}
							break;
						}

					case R.RightSquare:
						{
							if (!(type == ReadType.FuncCall || type == ReadType.ReplacerArgs || type == ReadType.SubroutineBody || type == ReadType.SubroutineArgs))
								Unexpected(token);
							// Add item to args
							sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions));
							switch (type)
							{
								case ReadType.SubroutineBody:
									return new RASequence(sequences);
								case ReadType.FuncCall:
									{
										var func = _funcCalls.Pop();
										VerifyArgCount(func, sequences.Count, fromToken, token);
										// TODO: Add support for function overloads
										return new RAFunction(Stringe.Range(fromToken, token), func, sequences);
									}
								case ReadType.ReplacerArgs:
									return new RAReplacer(Stringe.Range(fromToken, token),
										_regexes.Pop(), sequences[0], sequences[1]);
								case ReadType.SubroutineArgs:
									var subroutine = _subroutines.Peek();
									subroutine.Parameters = sequences
										// filter out empty sequences
										.Where(x => !(x is RASequence && (x as RASequence).Actions.Count == 0)).ToList();
									if (subroutine.IsCall)
										return _subroutines.Pop();
									// verify that every entry in sequences is a text action
									// i don't know how to do this better ok
									foreach (RantAction action in subroutine.Parameters)
										if (!(action is RAText))
											SyntaxError(token, "Subroutine argument names may only be text.");
									// seriously there needs to be a better way of converting the tokens to names
									var nextToken = _reader.ReadToken();
									if (nextToken.ID != R.Colon)
										SyntaxError(nextToken, "Expected subroutine body.");
									subroutine.Body = Read(ReadType.SubroutineBody, nextToken);
									return _subroutines.Pop();
							}
							break;
						}

					// Argument separator
					case R.Semicolon:
						if (!(type == ReadType.FuncCall || type == ReadType.ReplacerArgs || type == ReadType.SubroutineArgs)) goto default;
						// If it's a replacer, make sure there are no arguments already.
						if (type == ReadType.ReplacerArgs && sequences.Count == 1)
							SyntaxError(token, "Too many arguments in replacer.");
						// Add item to args
						sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions));
						actions.Clear();
						break;

					// Block
					case R.LeftCurly:
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
							// If it's a slash, just print it.
							goto default;
						}

						// Add item to block
						sequences.Add(actions.Count == 1 ? actions[0] : new RASequence(actions));
						if (token.ID == R.RightCurly)
							return new RABlock(Stringe.Range(fromToken, token), sequences);
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
								name = _reader.ReadLoose(R.Text);
							_query = new Query();
							_query.ClassFilters = new List<_<bool, string>[]>();
							_query.RegexFilters = new List<_<bool, Regex>>();
							_query.Carrier = new Carrier();
							_query.OriginStringe = name;
							_query.Name = (name == null ? null : name.Value);
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
							var subtype = _reader.ReadLoose(R.Text);
							_query.Subtype = subtype.Value;
						}
						break;
					// query filters
					case R.Hyphen:
						{
							if (type != ReadType.Query) goto default;
							var nextToken = _reader.PeekToken();
							Stringe className;
							bool negative = false;
							if (nextToken.ID == R.Exclamation)
							{
								negative = true;
								_reader.ReadToken();
								if (_query.Exclusive)
									throw new RantCompilerException(_sourceName, token, "You can't define a negative class filter in an exclusive query.");
							}
							className = _reader.ReadLoose(R.Text);
							// aaaaaaaaaaaaaaaaaaaaaaaaaaa
							((List<_<bool, string>[]>)_query.ClassFilters)
								.Add(new _<bool, string>[] { new _<bool, string>(!negative, className.Value) });
						}
						break;
					// query regex filters
					case R.Question:
						{
							if (type != ReadType.Query) goto default;
							var nextToken = _reader.ReadToken();
							bool negative = false;
							if (nextToken.ID == R.Exclamation)
							{
								negative = true;
								if (_query.Exclusive)
									throw new RantCompilerException(_sourceName, token, "You can't define a negative regex filter in an exclusive query.");
								nextToken = _reader.ReadToken();
							}
							if (nextToken.ID != R.Regex)
								throw new RantCompilerException(_sourceName, token, "Expected regex.");
							((List<_<bool, Regex>>)_query.RegexFilters)
								.Add(new _<bool, Regex>(!negative, Util.ParseRegex(nextToken.Value)));
						}
						break;
					// syllable range
					case R.LeftParen:
						{
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
						}
						break;
					// query carriers
					case R.DoubleColon:
						{
							if (type != ReadType.Query) goto default;
							return Read(ReadType.QueryCarrier, token);
						}
					// match carrier
					case R.Equal:
						{
							if (type != ReadType.QueryCarrier) goto default;
							var name = _reader.ReadLoose(R.Text);
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
							if (nextToken.ID == R.Exclamation)
							{
								componentType = CarrierComponent.Dissociative;
								_reader.ReadToken();
							}
							// divergent
							else if (nextToken.ID == R.Plus)
							{
								componentType = CarrierComponent.Divergent;
								_reader.ReadToken();
							}
							// relational
							else if (nextToken.ID == R.Question)
							{
								componentType = CarrierComponent.Relational;
								_reader.ReadToken();
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
							var name = _reader.ReadLoose(R.Text).Value;
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
							var name = _reader.ReadLoose(R.Text).Value;
							var componentType = (match ? CarrierComponent.MatchUnique : CarrierComponent.Unique);
							_query.Carrier.AddComponent(componentType, name);
						}
						break;
					// rhyme
					case R.Ampersand:
						{
							if (type != ReadType.QueryCarrier) goto default;
							var name = _reader.ReadLoose(R.Text).Value;
							_query.Carrier.AddComponent(CarrierComponent.Rhyme, name);
						}
						break;
					// end of queries
					case R.RightAngle:
						if (type != ReadType.Query && type != ReadType.QueryCarrier) goto default;
						return new RAQuery(_query);

					// Plain text
					default:
						actions.Add(new RAText(token));
						break;
				}
			}

			done:

			switch (type)
			{
				case ReadType.Sequence:
					return new RASequence(actions);
				case ReadType.Block:
					throw new RantCompilerException(_sourceName, fromToken, "Unterminated block found.");
				default:
					throw new RantCompilerException(_sourceName, token, "Unexpected end of file.");
			}
		}

		private void VerifyArgCount(RantFunctionInfo func, int argc, Stringe from, Stringe to)
		{
			if (argc != func.Parameters.Length)
				throw new RantCompilerException(_sourceName, Stringe.Range(from, to),
					$"The function '{func.Name}' requires '{func.Parameters.Length}' argument(s).");
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