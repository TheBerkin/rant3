using System;
using System.Linq;

using Rant.Engine.Syntax;
using Rant.Vocabulary;
using Rant.Stringes;

using System.Collections.Generic;
using System.Text.RegularExpressions;

using Rant.Engine.Syntax.Richard;
using Rant.Engine.Syntax.Richard.Operators;

namespace Rant.Engine.Compiler
{
	internal class RantExpressionCompiler
	{
		internal enum ExpressionReadType
		{
			Expression,
			/// <summary>
			/// Read actions for groups (operations in parentheses)
			/// </summary>
			ExpressionGroup,
			/// <summary>
			/// Read actions for the value of a variable
			/// </summary>
			VariableValue,
			/// <summary>
			/// Read actions for a function body.
			/// </summary>
			FunctionBody,
            /// <summary>
            /// An expression group, except it won't allow REAArgumentSeperators
            /// (For if statements and things)
            /// </summary>
            ExpressionGroupNoList,
			/// <summary>
			/// Expression block
			/// </summary>
			ExpressionBlock,
            /// <summary>
            /// JSON-like object
            /// </summary>
			KeyValueObject,
            /// <summary>
            /// Read a value in an object
            /// </summary>
			KeyValueObjectValue,
            /// <summary>
            /// Read the value in a bracket value access
            /// </summary>
			BracketValue,
            /// <summary>
            /// A list is expected.
            /// </summary>
            List,
            /// <summary>
            /// A boolean value to invert is expected.
            /// </summary>
            InvertValue
		}

		private string _sourceName, _source;
		private TokenReader _reader;
		private List<string> _keywords;
        // a hack for VariableValue and FunctionBody going past end of expression
        private bool _hitEndOfExpr = false;
        private RantCompiler _rantCompiler;

		public RantExpressionCompiler(string sourceName, string source, TokenReader reader, RantCompiler parentCompiler)
		{
            _rantCompiler = parentCompiler;
			_sourceName = sourceName;
			_source = source;
			_reader = reader;
			_keywords = new List<string>()
			{
				"var",
				"function",
				"true",
                "false",
				"no",
				"maybe",
				"list",
				"if",
				"return",
                "while",
                "break",
                "for"
			};
		}

		public RantAction Read(ExpressionReadType type = ExpressionReadType.Expression)
		{
			List<RichActionBase> actions = new List<RichActionBase>();

			Token<R> token = null;
            if (type == ExpressionReadType.Expression)
                _hitEndOfExpr = false;

			while (!_reader.End)
			{
                if (_hitEndOfExpr) goto done;
                token = _reader.ReadToken();

				switch (token.ID)
				{
					case R.EOF:
						throw new RantCompilerException(_sourceName, token, "Unexpected end of file.");
                    case R.Whitespace:
                        if (type == ExpressionReadType.InvertValue)
                            goto done;
                        break;
                    case R.LeftSquare:
						{
                            if (!actions.Any())
                            {
                                // empty list initializer
                                if (!_reader.End && _reader.PeekLooseToken().ID == R.RightSquare)
                                {
                                    _reader.ReadLooseToken();
                                    actions.Add(new RichList(token, new List<RichActionBase>(), false));
                                    break;
                                }
                                actions.Add((Read(ExpressionReadType.List) as RAExpression).Group);
                                break;
                            }
							var val = Read(ExpressionReadType.BracketValue) as RAExpression;
                            if (val.Group.Actions.Count == 0)
                                SyntaxError(val.Range, "Expected value in bracket indexer.");
							var obj = actions.Last();
							actions.RemoveAt(actions.Count - 1);
							actions.Add(new RichObjectProperty(token, val.Group, obj));
						}
						break;
					case R.RightSquare:
                        if (type == ExpressionReadType.List)
                            goto done;
                        if (
                            type == ExpressionReadType.FunctionBody ||
                            type == ExpressionReadType.VariableValue)
                            _hitEndOfExpr = true;
						if (
							type != ExpressionReadType.Expression && 
							type != ExpressionReadType.VariableValue && 
							type != ExpressionReadType.BracketValue &&
							type != ExpressionReadType.FunctionBody)
							SyntaxError(token, "Unexpected end of expression.");
						goto done;
					case R.LeftParen:
                        {
                            var group = (Read(ExpressionReadType.ExpressionGroup) as RAExpression).Group;
                            // could be a function call
                            if (actions.Any() &&
                                (actions.Last() is RichPatternString ||
                                 actions.Last() is RichVariable ||
                                 actions.Last() is RichFunctionCall ||
                                 actions.Last() is RichGroup ||
                                 actions.Last() is RichObjectProperty))
                            {
                                var last = actions.Last();
                                actions.Remove(last);
                                actions.Add(new RichFunctionCall(token, last, group as RichGroup, _sourceName));
                                break;
                            }
                            // could be a function definition (lambda)
                            if (_reader.PeekLooseToken().ID == R.Equal)
                            {
                                _reader.ReadLooseToken();
                                _reader.ReadLoose(R.RightAngle, "fat arrow");
                                var body = Read(ExpressionReadType.FunctionBody);
                                actions.Add(new RichFunction(token, (body as RAExpression).Group, group as RichGroup));
                                if (type == ExpressionReadType.VariableValue)
                                    goto done;
                                if (_reader.PrevLooseToken.ID == R.RightParen && type == ExpressionReadType.ExpressionGroup)
                                    goto done;
                                break;
                            }
                            if (!group.Actions.Any())
                                Unexpected(token);
                            if (group.Actions.Any(x => x is RichArgumentSeperator))
                                actions.Add(CreateListFromActions(type, group.Actions));
                            else
                                actions.Add(group);
                        }
						break;
					case R.RightParen:
                        if (
                            type == ExpressionReadType.ExpressionGroup || 
                            type == ExpressionReadType.VariableValue || 
                            type == ExpressionReadType.ExpressionGroupNoList ||
                            type == ExpressionReadType.FunctionBody)
                            goto done;
						break;
					case R.LeftCurly:
						// is it an object? let's find out
						if (_reader.PeekLooseToken().ID == R.RightCurly)
						{
							// empty object
							_reader.Read(R.RightCurly);
							actions.Add(new RichObject(token, new RichObjectKeyValue[0]));
                            if (type == ExpressionReadType.VariableValue)
                            {
                                if (!_reader.End && _reader.PeekLooseToken().ID != R.Semicolon)
                                    Unexpected(_reader.PeekLooseToken());
                                goto done;
                            }
							break;
						}
						// maybe it's an object with key/values
						var tempPosition = _reader.Position;
						if (_reader.PeekLooseToken().ID == R.ConstantLiteral || _reader.PeekLooseToken().ID == R.Text)
						{
							var firstValue = _reader.ReadLooseToken();
							// ok, it's an object
							if (_reader.PeekLooseToken().ID == R.Colon)
							{
								_reader.Position = tempPosition;
								var obj = Read(ExpressionReadType.KeyValueObject);
								actions.Add(obj as RichActionBase);
								break;
							}
							else
								_reader.Position = tempPosition;
						}
						if (type == ExpressionReadType.FunctionBody || type == ExpressionReadType.Expression)
						{
							var body = Read(ExpressionReadType.ExpressionBlock);
							actions.Add(body as RichActionBase);
                            if (type == ExpressionReadType.FunctionBody)
                                goto done;
							break;
						}
						Unexpected(token);
						break;
					case R.RightCurly:
                        if (type == ExpressionReadType.ExpressionBlock)
                        {
                            var group = new RichGroup(actions, token, _sourceName);
                            return new RichBlock(token, new List<RichActionBase>() { group });
                        }
						if (type == ExpressionReadType.KeyValueObjectValue)
							return new RichGroup(actions, token, _sourceName);
						Unexpected(token);
						break;
					case R.Semicolon:
						if (type != ExpressionReadType.Expression && type != ExpressionReadType.ExpressionBlock)
							goto done;
						if (actions.Count == 0)
							break;
						var action = new RichGroup(actions.ToList(), token, _sourceName);
						actions.Clear();
						actions.Add(action);
						break;
					case R.ConstantLiteral:
						if (type == ExpressionReadType.KeyValueObject)
						{
							var name = token;
							_reader.ReadLoose(R.Colon, "key value seperator");
							var value = Read(ExpressionReadType.KeyValueObjectValue) as RichGroup;
                            if (value.Actions.Count == 0)
                                SyntaxError(value.Range, "Expected value for key.");
							actions.Add(new RichObjectKeyValue(name, value));
							if (_reader.PrevLooseToken.ID == R.RightCurly)
							{
								List<RichObjectKeyValue> keyValues = new List<RichObjectKeyValue>();
								foreach (RichActionBase kv in actions)
								{
									if (!(kv is RichObjectKeyValue))
										throw new RantCompilerException(_sourceName, token, "Unexpected token in key value object.");
									keyValues.Add(kv as RichObjectKeyValue);
								}
								return new RichObject(name, keyValues.ToArray());
							}
							break;
						}
						actions.Add(new RichString(Util.UnescapeConstantLiteral(token.Value), token));
						break;
					case R.Dollar:
						if (_reader.PeekType() == R.ConstantLiteral)
						{
							var value = _reader.Read(R.ConstantLiteral, "string");
							actions.Add(new RichPatternString(Util.UnescapeConstantLiteral(value.Value), token));
							break;
						}
						Unexpected(token);
						break;
					case R.Text:
						// numbers
						if (char.IsDigit(token.Value[0]))
						{
                            if (actions.Any() && actions.Last() is RichNumber)
                                Unexpected(token);
							double number;
							if (!ReadNumber(token, out number))
								SyntaxError(token, "Invalid number: " + token.Value);
							actions.Add(new RichNumber(number, token));
							break;
						}
						// keywords
						if (_keywords.IndexOf(token.Value) >= 0)
						{
							switch (token.Value)
							{
								case "var":
									var name = _reader.ReadLoose(R.Text, "variable name");
                                    if (_keywords.IndexOf(name.Value) > -1)
                                        SyntaxError(name, "Cannot use reserved variable name.");
									if (_reader.PeekType() == R.Equal)
									{
										_reader.ReadToken();
										var val = Read(ExpressionReadType.VariableValue) as RAExpression;
										actions.Add(new RichObjectPropertyAssignment(name, null, val.Group));
									}
									else if (_reader.PeekType() == R.Semicolon || _reader.End)
										actions.Add(new RichObjectPropertyAssignment(name, null, null));
									break;
								case "function":
									{
										_reader.ReadLoose(R.LeftParen, "function definition open paren");
										var argGroup = Read(ExpressionReadType.ExpressionGroup) as RAExpression;
										_reader.ReadLoose(R.LeftCurly, "function body open bracket");
										var body = Read(ExpressionReadType.ExpressionBlock);
										actions.Add(new RichFunction(token, body as RichActionBase, argGroup.Group));
										if (type == ExpressionReadType.VariableValue)
											goto done;
									}
									break;
								case "true":
									actions.Add(new RichBoolean(token, true));
									break;
								case "false":
									actions.Add(new RichBoolean(token, false));
									break;
                                case "no":
                                    actions.Add(new RichNull(token));
                                    break;
								case "maybe":
									actions.Add(new RichMaybe(token));
									break;
								case "list":
									{
										var num = _reader.ReadLoose(R.Text, "list length");
										double listLength = -1;
										ReadNumber(num, out listLength);
										if (listLength < 0)
											SyntaxError(num, "Invalid list length.");
										List<RichActionBase> fakeList = new List<RichActionBase>();
										for (var i = 0; i < listLength; i++)
											fakeList.Add(null);
										actions.Add(new RichList(token, fakeList, false));
									}
									break;
								case "if":
									{
										if (type != ExpressionReadType.Expression && type != ExpressionReadType.ExpressionBlock && type != ExpressionReadType.FunctionBody)
											Unexpected(token);
										_reader.ReadLoose(R.LeftParen, "if statement param");
										var val = Read(ExpressionReadType.ExpressionGroupNoList) as RAExpression;
										var body = Read(ExpressionReadType.FunctionBody) as RAExpression;
                                        RichActionBase elseBody = null;
                                        if (!_reader.End && _reader.PeekLooseToken().Value == "else")
                                        {
                                            _reader.ReadLooseToken();
                                            elseBody = (Read(ExpressionReadType.FunctionBody) as RAExpression).Group;
                                        }
										actions.Add(new RichIfStatement(token, val.Group, body.Group, elseBody));
									}
									break;
								case "return":
									{
										if (type != ExpressionReadType.ExpressionBlock && type != ExpressionReadType.FunctionBody && type != ExpressionReadType.Expression)
											Unexpected(token);
                                        RichActionBase expr = null;
                                        if (_reader.PeekLooseToken().ID != R.Semicolon)
                                            expr = (Read(ExpressionReadType.VariableValue) as RAExpression).Group;
                                        actions.Add(new RichReturn(token, expr));
                                        if (_reader.PrevLooseToken.ID == R.Semicolon && type == ExpressionReadType.FunctionBody)
                                            return new RAExpression(actions, token, _sourceName);
                                    }
									break;
                                case "while":
                                    {
                                        if (type != ExpressionReadType.Expression && type != ExpressionReadType.ExpressionBlock)
                                            Unexpected(token);
                                        _reader.ReadLoose(R.LeftParen, "while loop test");
                                        var test = (Read(ExpressionReadType.ExpressionGroupNoList) as RAExpression).Group;
                                        var body = (Read(ExpressionReadType.FunctionBody) as RAExpression).Group;
                                        actions.Add(new RichWhile(token, test, body));
                                    }
                                    break;
                                case "break":
                                    {
                                        if (type != ExpressionReadType.FunctionBody && type != ExpressionReadType.ExpressionBlock)
                                            Unexpected(token);
                                        actions.Add(new RichBreak(token));
                                    }
                                    break;
                                case "for":
                                    {
                                        if (type != ExpressionReadType.Expression && type != ExpressionReadType.ExpressionBlock)
                                            Unexpected(token);
                                        _reader.ReadLoose(R.LeftParen, "for loop open paren");
                                        var indexName = _reader.ReadLoose(R.Text, "for loop index name");
                                        if (indexName.Value == "var")
                                            indexName = _reader.ReadLoose(R.Text, "for loop index name");
                                        if (indexName.Value == "in")
                                            SyntaxError(indexName, "Expected index name in for loop.");
                                        var inStmt = _reader.ReadLoose(R.Text, "for loop in statement");
                                        if (inStmt.Value != "in")
                                            SyntaxError(inStmt, "Expected in statement in for loop.");
                                        var expr = (Read(ExpressionReadType.ExpressionGroupNoList) as RAExpression).Group;
                                        var body = (Read(ExpressionReadType.FunctionBody) as RAExpression).Group;
                                        actions.Add(new RichFor(token, indexName.Value, body, expr));
                                    }
                                    break;
							}
							break;
						}
                        // object key
                        if (type == ExpressionReadType.KeyValueObject)
                        {
                            var name = token;
                            _reader.ReadLoose(R.Colon, "key value seperator");
                            var value = Read(ExpressionReadType.KeyValueObjectValue) as RichGroup;
                            if (value.Actions.Count == 0)
                                SyntaxError(value.Range, "Expected value for key.");
                            actions.Add(new RichObjectKeyValue(name, value));
                            if (_reader.PrevLooseToken.ID == R.RightCurly)
                            {
                                List<RichObjectKeyValue> keyValues = new List<RichObjectKeyValue>();
                                foreach (RichActionBase kv in actions)
                                {
                                    if (!(kv is RichObjectKeyValue))
                                        throw new RantCompilerException(_sourceName, token, "Unexpected token in key value object.");
                                    keyValues.Add(kv as RichObjectKeyValue);
                                }
                                return new RichObject(name, keyValues.ToArray());
                            }
                            break;
                        }
                        if (actions.Any() && actions.Last() is RichVariable)
                            Unexpected(token);
                        // just a variable
                        actions.Add(new RichVariable(token));
                        if (type == ExpressionReadType.InvertValue)
                            return new RAExpression(actions, token, _sourceName);
						break;
					// binary number operators
					case R.Plus:
                        // increment operator
                        if (!_reader.End && _reader.PeekType() == R.Plus)
                        {
                            _reader.ReadToken();
                            // postfix
                            if (actions.Any() && actions.Last() is RichVariable)
                            {
                                var variable = actions.Last();
                                actions.RemoveAt(actions.Count - 1);
                                actions.Add(new RichPostfixIncDec(token) { LeftSide = variable });
                                break;
                            }
                            if (!_reader.End && _reader.PeekLooseToken().ID == R.Text)
                            {
                                var varName = _reader.ReadLooseToken();
                                double dummyNumber = -1;
                                if (ReadNumber(varName, out dummyNumber))
                                    SyntaxError(token, "Cannot increment constant.");
                                actions.Add(new RichPrefixIncDec(token) { RightSide = new RichVariable(varName) });
                                break;
                            }
                            Unexpected(token);
                        }
						actions.Add(new RichAdditionOperator(token));
						break;
					case R.Hyphen:
                        // decrement operator
                        if (!_reader.End && _reader.PeekType() == R.Hyphen)
                        {
                            _reader.ReadToken();
                            // postfix
                            if (actions.Any() && actions.Last() is RichVariable)
                            {
                                var variable = actions.Last();
                                actions.RemoveAt(actions.Count - 1);
                                actions.Add(new RichPostfixIncDec(token) { LeftSide = variable, Increment = false });
                                break;
                            }
                            if (!_reader.End && _reader.PeekLooseToken().ID == R.Text)
                            {
                                var varName = _reader.ReadLooseToken();
                                actions.Add(new RichPrefixIncDec(token) { Increment = false, RightSide = new RichVariable(varName) });
                                break;
                            }
                            Unexpected(token);
                        }

                        // could be negative sign on number - is there a digit directly following this?
                        if (!actions.Any() && char.IsDigit(_reader.PeekToken().Value[0]) && _reader.PrevToken.ID != R.Whitespace)
						{
							double number;
							if (!ReadNumber(token, out number))
								SyntaxError(token, "Invalid number: " + token.Value);
							actions.Add(new RichNumber(number, token));
							break;
						}
						actions.Add(new RichSubtractionOperator(token));
						break;
					case R.Comma:
						if (type == ExpressionReadType.KeyValueObjectValue)
							return new RichGroup(actions, token, _sourceName);
						if (type == ExpressionReadType.ExpressionGroup || type == ExpressionReadType.VariableValue || type == ExpressionReadType.List || type == ExpressionReadType.Expression)
						{
							actions.Add(new RichArgumentSeperator(token));
							break;
						}
                        Unexpected(token);
                        break;
					case R.ForwardSlash:
						actions.Add(new RichDivisionOperator(token));
						break;
					case R.Asterisk:
						actions.Add(new RichMultiplicationOperator(token));
						break;
					case R.Tilde:
						actions.Add(new RichConcatOperator(token));
						break;
					// accessing properties
					case R.Subtype:
						{
							var name = _reader.Read(R.Text, "object property access");
							var obj = actions.Last();
							actions.Remove(obj);
						    actions.Add(new RichObjectProperty(name, obj));
						}
						break;
					case R.LeftAngle:
						if (_reader.PeekType() == R.Equal)
						{
							_reader.ReadToken();
							actions.Add(new RichLessThanOperator(token, true));
						}
						else
							actions.Add(new RichLessThanOperator(token, false));
						break;
					case R.RightAngle:
						if (_reader.PeekType() == R.Equal)
						{
							_reader.ReadToken();
							actions.Add(new RichGreaterThanOperator(token, true));
						}
						else
							actions.Add(new RichGreaterThanOperator(token, false));
						break;
                    case R.Exclamation:
                        if (!_reader.End && _reader.PeekToken().ID == R.Equal)
                        {
                            _reader.ReadToken();
                            actions.Add(new RichInequalityOperator(token));
                            break;
                        }
                        // invert operator maybe
                        if (!_reader.End && (_reader.PeekLooseToken().ID == R.LeftParen || _reader.PeekLooseToken().ID == R.Text))
                        {
                            RichActionBase rightSide = null;
                            if (_reader.PeekLooseToken().ID == R.LeftParen)
                            {
                                _reader.ReadLooseToken();
                                rightSide = (Read(ExpressionReadType.ExpressionGroup) as RAExpression).Group;
                                actions.Add(new RichPrefixInvert(token) { RightSide = rightSide });
                                break;
                            }
                            rightSide = (Read(ExpressionReadType.InvertValue) as RAExpression).Group;
                            actions.Add(new RichPrefixInvert(token) { RightSide = rightSide });
                            break;
                        }
                        Unexpected(token);
                        break;
					case R.Equal:
						if (_reader.PeekType() == R.Equal)
						{
							_reader.ReadToken();
							actions.Add(new RichEqualityOperator(token));
							break;
						}
                        RichInfixOperator op = null;
                        if (!actions.Any())
                            SyntaxError(token, "Cannot assign value to nothing.");
                        if (actions.Last() is RichInfixOperator)
                        {
                            if (_reader.PrevToken.ID == R.Whitespace)
                                Unexpected(token);
                            op = actions.Last() as RichInfixOperator;
                            if (op.Type == RichActionBase.ActionValueType.Boolean)
                                Unexpected(token);
                            actions.RemoveAt(actions.Count - 1);
                            if (!actions.Any())
                                Unexpected(token);
                        }
                        // variable assignment
                        if (actions.Last() is RichVariable)
                        {
                            var lastAction = actions.Last();
                            var val = (Read(ExpressionReadType.VariableValue) as RAExpression).Group;
                            actions.RemoveAt(actions.Count - 1);

                            if (_keywords.IndexOf(lastAction.Range.Value) > -1)
                                SyntaxError(lastAction.Range, "Cannot use reserved variable name.");
                            var assignment = new RichObjectPropertyAssignment(lastAction.Range, null, val);
                            assignment.Operator = op;
                            actions.Add(assignment);
                            if (type == ExpressionReadType.FunctionBody)
                                goto done;
                            break;
                        }
                        // object property assignment
                        if (actions.Last() is RichObjectProperty)
                        {
                            var lastAction = actions.Last() as RichObjectProperty;
                            var value = Read(ExpressionReadType.VariableValue) as RAExpression;
                            RichObjectPropertyAssignment assignment = null;
                            if (lastAction.Name is RichActionBase)
                                assignment = new RichObjectPropertyAssignment(lastAction.Range, lastAction.Name as RichActionBase, lastAction.Object, value.Group);
                            else
                                assignment = new RichObjectPropertyAssignment(lastAction.Range, lastAction.Object, value.Group);
                            assignment.Operator = op;
                            actions.Add(assignment);
                            break;
                        }
                        Unexpected(token);
						break;
                    case R.Undefined:
                        if (!_reader.End && _reader.PeekLooseToken().ID != R.Semicolon && _reader.PeekLooseToken().ID != R.RightSquare)
                            Unexpected(_reader.PeekLooseToken());
                        actions.Add(new RichUndefined(token));
                        break;
                    case R.Percent:
                        actions.Add(new RichModuloOperator(token));
                        break;
                    case R.Ampersand:
                        // boolean AND
                        if (!_reader.End && _reader.PeekType() == R.Ampersand)
                        {
                            _reader.ReadToken();
                            actions.Add(new RichBooleanAndOperator(token));
                            break;
                        }
                        Unexpected(token);
                        break;
                    case R.Pipe:
                        // boolean OR
                        if (!_reader.End && _reader.PeekType() == R.Pipe)
                        {
                            _reader.ReadToken();
                            actions.Add(new RichBooleanOrOperator(token));
                            break;
                        }
                        Unexpected(token);
                        break;
					default:
						Unexpected(token);
						break;
				}
			}

            done:

            if ((actions.Any(x => x is RichArgumentSeperator) &&
                (type == ExpressionReadType.Expression ||
                type == ExpressionReadType.VariableValue) || type == ExpressionReadType.List))
            {
                // lists
                var list = CreateListFromActions(type, actions);
                actions.Clear();
                actions.Add(list);
            }
            else if(actions.Where(x => x is RichArgumentSeperator).Any() && type != ExpressionReadType.ExpressionGroup)
                Unexpected(actions.Where(x => x is RichArgumentSeperator).First().Range);

			switch (type)
			{
				case ExpressionReadType.ExpressionGroup:
                case ExpressionReadType.ExpressionGroupNoList:
                case ExpressionReadType.VariableValue:
				case ExpressionReadType.Expression:
				case ExpressionReadType.BracketValue:
                case ExpressionReadType.FunctionBody:
                case ExpressionReadType.List:
                case ExpressionReadType.InvertValue:
					return new RAExpression(actions, token, _sourceName);
				default:
					throw new RantCompilerException(_sourceName, token, "Unexpected end of expression.");
			}
		}

        private RichList CreateListFromActions(ExpressionReadType type, List<RichActionBase> actions)
        {
            List<RichActionBase> listActions = new List<RichActionBase>();
            RichActionBase lastItem = null;
            foreach (RichActionBase reAction in actions)
            {
                if (reAction is RichArgumentSeperator)
                {
                    if (lastItem == null)
                        Unexpected(reAction.Range);
                    listActions.Add(lastItem);
                    lastItem = null;
                }
                else
                    lastItem = reAction;
            }
            if (lastItem == null)
            {
                if (actions.Any() && actions.Where(x => x is RichArgumentSeperator).Any())
                    Unexpected(actions.Where(x => x is RichArgumentSeperator).Last().Range);
                else
                    SyntaxError(actions.First().Range, "Blank item in list.");
            }
            listActions.Add(lastItem);
            return new RichList(actions.First().Range, listActions, type != ExpressionReadType.List);
        }

		private bool ReadNumber(Token<R> token, out double number)
		{
			number = -1;
			string suffix = "";

			var initialString = token.Value;
			if (token.ID == R.Hyphen)
				initialString += _reader.Read(R.Text, "number").Value;
			if (_reader.PeekType() == R.Subtype)
			{
				initialString += _reader.ReadToken().Value;
				// read next text
				initialString += _reader.Read(R.Text, "number").Value;
			}
			if (char.IsLetter(initialString[initialString.Length - 1]))
			{
				suffix = initialString[initialString.Length - 1].ToString();
				if (suffix != "k" && suffix != "M" && suffix != "B")
					suffix = "";
				initialString = initialString.Substring(0, initialString.Length - 1);
            }
			if (!initialString.ToCharArray().All(x => char.IsDigit(x) || x == '.' || x == '-'))
				return false;
			return Util.ParseDouble(initialString + suffix, out number);
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
