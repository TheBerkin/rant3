using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Rant.Arithmetic;
using Rant.Blueprints;
using Rant.Compiler;

using Stringes.Tokens;

namespace Rant
{
    // TODO: Replace the dreadful multiple PeekToken() calls with a single ReadToken() call

    internal delegate bool TokenFunc(Interpreter interpreter, SourceReader reader, Interpreter.State state);

    internal partial class Interpreter
    {
        private static readonly Dictionary<TokenType, TokenFunc> TokenFuncs = new Dictionary<TokenType, TokenFunc>
        {
            {TokenType.LeftCurly, DoBlock},
            {TokenType.LeftSquare, DoTag},
            {TokenType.LeftParen, DoMath},
            {TokenType.LeftAngle, DoQuery},
            {TokenType.EscapeSequence, DoEscape},
            {TokenType.ConstantLiteral, DoConstant},
            {TokenType.Text, DoText}
        };

        private static bool DoMath(Interpreter interpreter, SourceReader reader, State state)
        {
            reader.Read(TokenType.LeftParen);
            bool isStatement = reader.Take(TokenType.At);
            var tokens = reader.ReadToScopeClose(TokenType.LeftParen, TokenType.RightParen, BracketPairs.All);
            interpreter.PushState(State.CreateDerivedDistinct(reader.Source, tokens, interpreter));
            state.AddPreBlueprint(new FunctionBlueprint(interpreter, _ =>
            {
                var v = Parser.Calculate(_, _.PopResultString());
                if (!isStatement)
                {
                    _.Print(_.FormatNumber(v));
                }
                return false;
            }));
            return true;
        }

        private static bool DoQuery(Interpreter interpreter, SourceReader reader, State state)
        {
            var first = reader.ReadToken();
            reader.SkipSpace();

            var namesub = reader.Read(TokenType.Text, "list name").Split(new[] { '.' }, 2).ToArray();
            var q = new Query(namesub[0].Value.Trim(), namesub.Length == 2 ? namesub[1].Value : "", "", reader.Take(TokenType.Dollar), null, null);

            Token<TokenType> token = null;

            // Class filter list. Not initialized unless class filters actually exist.
            List<Tuple<bool, string>> cfList = null;

            while (true)
            {
                if (reader.Take(TokenType.Hyphen))
                {
                    // Initialize the filter list.
                    (cfList ?? (cfList = new List<Tuple<bool, string>>())).Clear();

                    do
                    {
                        bool notin = reader.Take(TokenType.Exclamation);
                        if (notin && q.Exclusive)
                            throw new RantException(reader.Source, reader.PrevToken, "Cannot use the '!' modifier on exclusive class filters.");
                        cfList.Add(Tuple.Create(!notin, reader.Read(TokenType.Text, "class identifier").Value.Trim()));
                    } while (reader.Take(TokenType.Pipe));
                    q.ClassFilters.Add(cfList.ToArray());
                }
                else if (reader.Take(TokenType.Question))
                {
                    token = reader.Read(TokenType.Regex, "regex");
                    q.RegexFilters.Add(Tuple.Create(true, Util.ParseRegex(token.Value)));
                }
                else if (reader.Take(TokenType.Without))
                {
                    token = reader.Read(TokenType.Regex, "regex");
                    q.RegexFilters.Add(Tuple.Create(false, Util.ParseRegex(token.Value)));
                }
                else if (reader.Take(TokenType.DoubleColon))
                {
                    token = reader.Read(TokenType.Text, "carrier name");
                    q.Carrier = token.Value.Trim();
                    if (!reader.Take(TokenType.RightAngle))
                    {
                        throw new RantException(reader.Source, token, "Expected '>' after carrier. (The carrier should be your last query argument!)");
                    }
                    break;
                }
                else if (reader.Take(TokenType.RightAngle))
                {
                    break;
                }
                else if (!reader.SkipSpace())
                {
                    var t = !reader.End ? reader.ReadToken() : null;
                    throw new RantException(reader.Source, t, t == null ? "Unexpected end-of-file in query." : "Unexpected token '" + t.Value + "' in query.");
                }
            }

            interpreter.Print(interpreter.Engine.Vocabulary.Query(interpreter.RNG, q));

            return false;
        }

        private static bool DoText(Interpreter interpreter, SourceReader reader, State state)
        {
            interpreter.Print(reader.ReadToken().Value);
            return false;
        }

        private static bool DoConstant(Interpreter interpreter, SourceReader reader, State state)
        {
            interpreter.Print(Util.UnescapeConstantLiteral(reader.ReadToken().Value));
            return false;
        }

        private static bool DoEscape(Interpreter interpreter, SourceReader reader, State state)
        {
            interpreter.Print(Util.Unescape(reader.ReadToken().Value, interpreter.RNG));
            return false;
        }

        private static bool DoTag(Interpreter interpreter, SourceReader reader, State state)
        {
            reader.Read(TokenType.LeftSquare);
            var name = reader.ReadToken();

            // Check if metapattern
            if (name.Identifier == TokenType.Question)
            {
                state.AddPreBlueprint(new MetapatternBlueprint(interpreter));
                interpreter.PushState(State.CreateDerivedDistinct(reader.Source, reader.ReadToScopeClose(TokenType.LeftSquare, TokenType.RightSquare, BracketPairs.All), interpreter));
                return true;
            }

            // Check if replacer
            if (name.Identifier == TokenType.Regex)
            {
                return DoReplacer(name, interpreter, reader, state);
            }

            if (name.Identifier == TokenType.Dollar)
            {
                return reader.IsNext(TokenType.Text) ? DoSubCall(name, interpreter, reader, state) : DoSubDefinition(name, interpreter, reader, state);
            }

            if (!Util.ValidateName(name.Value.Trim()))
                throw new RantException(reader.Source, name, "Invalid tag name '" + name.Value + "'");

            bool none = false;
            if (!reader.Take(TokenType.Colon))
            {
                if (!reader.Take(TokenType.RightSquare))
                    throw new RantException(reader.Source, name, "Expected ':' or ']' after tag name.");
                none = true;
            }

            if (none)
            {
                state.AddPreBlueprint(new TagBlueprint(interpreter, reader.Source, name));
            }
            else
            {
                var items = reader.ReadItemsToClosureTrimmed(TokenType.LeftSquare, TokenType.RightSquare,
                    TokenType.Semicolon, BracketPairs.All).ToArray();

                state.AddPreBlueprint(new TagBlueprint(interpreter, reader.Source, name, items));
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool DoSubCall(Token<TokenType> first, Interpreter interpreter, SourceReader reader, State state)
        {
            var name = reader.ReadToken();
            if (!Util.ValidateName(name.Value))
                throw new RantException(reader.Source, name, "Invalid subroutine name '" + name.Value + "'");
            
            bool none = false;

            if (!reader.Take(TokenType.Colon))
            {
                if (!reader.Take(TokenType.RightSquare))
                    throw new RantException(reader.Source, name, "Expected ':' or ']' after subroutine name.");
                
                none = true;
            }

            IEnumerable<Token<TokenType>>[] args = null;
            Subroutine sub = null;

            if (none)
            {
                if((sub = interpreter.Engine.Subroutines.Get(name.Value, 0)) == null)
                    throw new RantException(reader.Source, name, "No subroutine was found with the name '" + name.Value + "' and 0 parameters.");
            }
            else
            {
                args = reader.ReadItemsToClosureTrimmed(TokenType.LeftSquare, TokenType.RightSquare, TokenType.Semicolon,
                    BracketPairs.All).ToArray();
                if((sub = interpreter.Engine.Subroutines.Get(name.Value, args.Length)) == null)
                    throw new RantException(reader.Source, name, "No subroutine was found with the name '" + name.Value + "' and " + args.Length + " parameter" + (args.Length != 1 ? "s" : "") + ".");
            }

            state.AddPreBlueprint(new SubCallBlueprint(interpreter, reader.Source, sub, args));

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool DoSubDefinition(Token<TokenType> first, Interpreter interpreter, SourceReader reader, State state)
        {
            bool meta = reader.Take(TokenType.Question);
            reader.Read(TokenType.LeftSquare);

            var parameters = new List<Tuple<string, ParamFlags>>();
            var tName = reader.Read(TokenType.Text, "subroutine name");

            if (!Util.ValidateName(tName.Value))
                throw new RantException(reader.Source, tName, "Invalid subroutine name: '" + tName.Value + "'");
            
            if (!reader.Take(TokenType.Colon))
            {
                reader.Read(TokenType.RightSquare);
            }
            else
            {
                while (true)
                {
                    bool isTokens = reader.Take(TokenType.At);
                    parameters.Add(Tuple.Create(reader.Read(TokenType.Text, "parameter name").Value, isTokens ? ParamFlags.Code : ParamFlags.None));
                    if (reader.Take(TokenType.RightSquare, false)) break;
                    reader.Read(TokenType.Semicolon);
                }
            }

            reader.SkipSpace();
            reader.Read(TokenType.Colon);

            var body = reader.ReadToScopeClose(TokenType.LeftSquare, TokenType.RightSquare, BracketPairs.All).ToArray();

            if (meta)
            {
                interpreter.PushState(State.CreateDerivedDistinct(reader.Source, body, interpreter));
                state.AddPreBlueprint(new FunctionBlueprint(interpreter, _ =>
                {
                    _.Engine.Subroutines.Define(tName.Value, Subroutine.FromString(tName.Value, _.PopResultString(), parameters.ToArray()));
                    return false;
                }));
            }
            else
            {
                interpreter.Engine.Subroutines.Define(tName.Value, Subroutine.FromTokens(tName.Value, reader.Source, body, parameters.ToArray()));
            }

            return meta;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool DoReplacer(Token<TokenType> name, Interpreter interpreter, SourceReader reader, State state)
        {
            reader.Read(TokenType.Colon);

            var args = reader.ReadItemsToClosureTrimmed(TokenType.LeftSquare, TokenType.RightSquare, TokenType.Semicolon, BracketPairs.All).ToArray();
            if (args.Length != 2) throw new RantException(reader.Source, name, "Replacer expected 2 arguments, but got " + args.Length + ".");

            state.AddPreBlueprint(new ReplacerBlueprint(interpreter, Util.ParseRegex(name.Value), args[1]));

            interpreter.PushState(State.CreateDerivedDistinct(reader.Source, args[0], interpreter));
            return true;
        }

        private static bool DoBlock(Interpreter interpreter, SourceReader reader, State state)
        {
            var items = reader.ReadMultiItemScope(TokenType.LeftCurly, TokenType.RightCurly, TokenType.Pipe, BracketPairs.All).ToArray();
            var attribs = interpreter.NextAttribs;
            interpreter._blockAttribs = new BlockAttribs();

            if (!items.Any() || !interpreter.TakeChance()) return false;

            var rep = new Repeater(items, attribs);
            interpreter.PushRepeater(rep);
            state.AddPreBlueprint(new RepeaterBlueprint(interpreter, rep));
            return true;
        }
    }
}