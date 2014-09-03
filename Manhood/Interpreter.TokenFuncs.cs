using System.Collections.Generic;
using System.Linq;

using Manhood.Blueprints;
using Manhood.Compiler;

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

            return false;
        }

        private static bool DoText(Interpreter interpreter, SourceReader reader, State state)
        {
            var token = reader.ReadToken();
            if (!token.Value.Trim().Any()) return false;
            state.Output.Write(token.Value);
            return false;
        }

        private static bool DoConstant(Interpreter interpreter, SourceReader reader, State state)
        {
            state.Output.Write(Util.UnescapeConstantLiteral(reader.ReadToken().Value));
            return false;
        }

        private static bool DoEscape(Interpreter interpreter, SourceReader reader, State state)
        {
            state.Output.Write(Util.Unescape(reader.ReadToken().Value, interpreter.RNG));
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
                var items = reader.ReadItemsToScopeClose(TokenType.LeftSquare, TokenType.RightSquare,
                    TokenType.Semicolon, BracketPairs.All).ToArray();

                foreach (var item in items)
                {
                    interpreter.PushState(State.CreateDerivedDistinct(reader.Source, item, interpreter));
                }

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