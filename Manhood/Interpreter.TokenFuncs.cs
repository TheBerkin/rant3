using System;
using System.Collections.Generic;
using System.Linq;

using Manhood.Blueprints;
using Manhood.Compiler;

using Stringes.Tokens;

namespace Manhood
{
    // TODO: Replace the dreadful multiple PeekToken() calls with a single ReadToken() call

    internal delegate bool TokenFunc(Interpreter interpreter, SourceReader reader, Interpreter.State state);

    internal partial class Interpreter
    {
        private static readonly Dictionary<TokenType, TokenFunc> TokenFuncs = new Dictionary<TokenType, TokenFunc>
        {
            {TokenType.LeftCurly, DoBlock},
            {TokenType.LeftSquare, DoTag},
            {TokenType.LeftAngle, DoQuery},
            {TokenType.EscapeSequence, DoEscape},
            {TokenType.ConstantLiteral, DoConstant},
            {TokenType.Text, DoText}
        };

        private static bool DoQuery(Interpreter interpreter, SourceReader reader, State state)
        {
            var first = reader.ReadToken();
            reader.SkipSpace();
            if (!reader.IsNext(TokenType.Text))
                throw new ManhoodException(reader.Source, first, "Expected dictionary identifier at beginning of query.");

            var namesub = reader.ReadToken().Split(new[] { '.' }, 2).ToArray();
            var q = new Query(namesub[0].Value.Trim(), namesub.Length == 2 ? namesub[1].Value : "", "", reader.Take(TokenType.Dollar), null, null);

            Token<TokenType> token = null;

            while (true)
            {
                if (reader.Take(TokenType.Hyphen))
                {
                    reader.SkipSpace();
                    bool notin = reader.Take(TokenType.Exclamation);
                    if (notin && q.Exclusive)
                        throw new ManhoodException(reader.Source, reader.PrevToken, "Cannot use the '!' modifier on exclusive class filters.");
                    reader.SkipSpace();
                    token = reader.ReadToken();
                    if (token.Identifier != TokenType.Text)
                        throw new ManhoodException(reader.Source, token, "Class identifier expected after '-' in query.");
                    q.ClassFilters.Add(Tuple.Create(!notin, token.Value.Trim()));
                }
                else if (reader.Take(TokenType.Question))
                {
                    reader.SkipSpace();
                    token = reader.ReadToken();
                    if (token.Identifier != TokenType.Regex)
                        throw new ManhoodException(reader.Source, token, "Regular expression expected after '?' in query.");
                    q.RegexFilters.Add(Tuple.Create(true, Util.ParseRegex(token.Value)));
                }
                else if (reader.Take(TokenType.Without))
                {
                    reader.SkipSpace();
                    token = reader.ReadToken();
                    if (token.Identifier != TokenType.Regex)
                        throw new ManhoodException(reader.Source, token, "Regular expression expected after '?!' in query.");
                    q.RegexFilters.Add(Tuple.Create(false, Util.ParseRegex(token.Value)));
                }
                else if (reader.Take(TokenType.DoubleColon))
                {
                    reader.SkipSpace();
                    token = reader.ReadToken();
                    if (token.Identifier != TokenType.Text)
                        throw new ManhoodException(reader.Source, token, "Carrier string expected after '::' in query.");
                    q.Carrier = token.Value.Trim();
                    reader.SkipSpace();
                    if (!reader.Take(TokenType.RightAngle))
                    {
                        throw new ManhoodException(reader.Source, token, "Expected '>' after carrier. (The carrier should be your last query argument!)");
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
                    throw new ManhoodException(reader.Source, t, t == null ? "Unexpected end-of-file in query." : "Unexpected token '" + t.Value + "' in query.");
                }
            }

            interpreter.Print(interpreter.Vocabulary.GetWord(interpreter, q));

            return false;
        }

        private static bool DoText(Interpreter interpreter, SourceReader reader, State state)
        {
            var token = reader.ReadToken();
            if (!token.Value.Trim().Any()) return false;
            interpreter.Print(token.Value);
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
            reader.Take(TokenType.LeftSquare);
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
                if (!reader.Take(TokenType.Colon))
                    throw new ManhoodException(reader.Source, name, "Expected ':' after replacer pattern.");

                var args = reader.ReadItemsToClosureTrimmed(TokenType.LeftSquare, TokenType.RightSquare, TokenType.Semicolon, BracketPairs.All).ToArray();
                if (args.Length != 2) throw new ManhoodException(reader.Source, name, "Replacer expected 2 arguments, but got " + args.Length + ".");

                state.AddPreBlueprint(new ReplacerBlueprint(interpreter, Util.ParseRegex(name.Value), args[1]));

                interpreter.PushState(State.CreateDerivedDistinct(reader.Source, args[0], interpreter));
                return true;
            }

            if (!Util.ValidateName(name.Value))
                throw new ManhoodException(reader.Source, name, "Invalid tag name '" + name.Value + "'");
            bool none = false;
            if (!reader.Take(TokenType.Colon))
            {
                if (!reader.Take(TokenType.RightSquare))
                    throw new ManhoodException(reader.Source, name, "Expected ':' or ']' after tag name.");
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