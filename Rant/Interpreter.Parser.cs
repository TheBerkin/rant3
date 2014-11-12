using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using Rant.Arithmetic;
using Rant.Blueprints;
using Rant.Compiler;
using Rant.Vocabulary;
using Rant.Stringes.Tokens;

namespace Rant
{
    internal delegate bool TokenFunc(Interpreter interpreter, Token<TokenType> firstToken, PatternReader reader, Interpreter.State state);

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

        private static bool DoMath(Interpreter interpreter, Token<TokenType> firstToken, PatternReader reader, State state)
        {
            bool isStatement = reader.Take(TokenType.At);
            var tokens = reader.ReadToScopeClose(TokenType.LeftParen, TokenType.RightParen, BracketPairs.All);
            interpreter.PushState(State.CreateSub(reader.Source, tokens, interpreter));
            state.AddPreBlueprint(new DelegateBlueprint(interpreter, _ =>
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

        private static bool DoQuery(Interpreter interpreter, Token<TokenType> firstToken, PatternReader reader, State state)
        {
            reader.SkipSpace();

            bool storeMacro = false;
            bool macroIsGlobal = false;
            string macroName = null;

            // Check if this is a macro
            if (reader.Take(TokenType.At))
            {
                reader.SkipSpace();

                var macroNameToken = reader.Read(TokenType.Text, "query macro name");
                if (!Util.ValidateName(macroNameToken.Value))
                    throw new RantException(reader.Source, macroNameToken, "Invalid macro name.");

                macroName = macroNameToken.Value;

                reader.SkipSpace();

                // Check if the macro is a definition or a call.
                // A definition will start with a colon ':' or equals '=' after the name. A call will only consist of the name.
                switch (reader.ReadToken().Identifier)
                {
                    case TokenType.Colon: // Local definition
                    {
                        storeMacro = true;
                    }
                    break;
                    case TokenType.Equal: // Global definition
                    {
                        storeMacro = true;
                        macroIsGlobal = true;
                    }
                    break;
                    case TokenType.RightAngle: // Call
                    {
                        Query q;
                        if (!interpreter.LocalQueryMacros.TryGetValue(macroName, out q) && !interpreter.Engine.GlobalQueryMacros.TryGetValue(macroName, out q))
                        {
                            throw new RantException(reader.Source, macroNameToken, "Nonexistent query macro '" + macroName + "'");
                        }
                        interpreter.Print(interpreter.Engine.Vocabulary.Query(interpreter.RNG, q, interpreter.CarrierSyncState));
                        return false;
                    }
                }
            }

            reader.SkipSpace();
            var namesub = reader.Read(TokenType.Text, "dictionary name").Split(new[] { '.' }, 2).ToArray();
            reader.SkipSpace();

            bool exclusive = reader.Take(TokenType.Dollar);
            List<Tuple<bool, string>> cfList = null;
            List<Tuple<bool, string>[]> classFilterList = null;
            List<Tuple<bool, Regex>> regList = null;
            Carrier carrier = null;

            Token<TokenType> queryToken = null;

            // Read query arguments
            while (true)
            {
                reader.SkipSpace();
                if (reader.Take(TokenType.Hyphen))
                {
                    reader.SkipSpace();
                    // Initialize the filter list.
                    (cfList ?? (cfList = new List<Tuple<bool, string>>())).Clear();
                    do
                    {
                        bool notin = reader.Take(TokenType.Exclamation);
                        reader.SkipSpace();
                        if (notin && exclusive)
                            throw new RantException(reader.Source, reader.PrevToken, "Cannot use the '!' modifier on exclusive class filters.");
                        cfList.Add(Tuple.Create(!notin, reader.Read(TokenType.Text, "class identifier").Value.Trim()));
                        reader.SkipSpace();
                    } while (reader.Take(TokenType.Pipe));
                    (classFilterList ?? (classFilterList = new List<Tuple<bool, string>[]>())).Add(cfList.ToArray());
                }
                else if (reader.Take(TokenType.Question))
                {
                    reader.SkipSpace();
                    queryToken = reader.Read(TokenType.Regex, "regex");
                    (regList ?? (regList = new List<Tuple<bool, Regex>>())).Add(Tuple.Create(true, Util.ParseRegex(queryToken.Value)));
                }
                else if (reader.Take(TokenType.Without))
                {
                    reader.SkipSpace();
                    queryToken = reader.Read(TokenType.Regex, "regex");
                    (regList ?? (regList = new List<Tuple<bool, Regex>>())).Add(Tuple.Create(false, Util.ParseRegex(queryToken.Value)));
                }
                else if (reader.Take(TokenType.DoubleColon)) // Start of carrier
                {
                    reader.SkipSpace();

                    CarrierSyncType type;
                    Token<TokenType> typeToken;

                    switch ((typeToken = reader.ReadToken()).Identifier)
                    {
                        case TokenType.Exclamation:
                            type = CarrierSyncType.Unique;
                            break;
                        case TokenType.Equal:
                            type = CarrierSyncType.Match;
                            break;
                        case TokenType.Ampersand:
                            type = CarrierSyncType.Rhyme;
                            break;
                        default:
                            throw new RantException(reader.Source, typeToken, "Unrecognized token '" + typeToken.Value + "' in carrier.");
                    }

                    reader.SkipSpace();

                    carrier = new Carrier(type, reader.Read(TokenType.Text, "carrier sync ID").Value, 0, 0);

                    reader.SkipSpace();

                    if (!reader.Take(TokenType.RightAngle))
                    {
                        throw new RantException(reader.Source, queryToken, "Expected '>' after carrier. (The carrier should be your last query argument!)");
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

            var query = new Query(
                namesub[0].Value.Trim(),
                namesub.Length == 2 ? namesub[1].Value : "",
                carrier, exclusive, classFilterList, regList);

            if (storeMacro)
            {
                if (macroIsGlobal)
                {
                    interpreter.Engine.GlobalQueryMacros[macroName] = query;
                }
                else
                {
                    interpreter.LocalQueryMacros[macroName] = query;
                }
                return false;
            }

            // Query dictionary and print result
            interpreter.Print(interpreter.Engine.Vocabulary.Query(interpreter.RNG, query, interpreter.CarrierSyncState));

            return false;
        }

        private static bool DoText(Interpreter interpreter, Token<TokenType> firstToken, PatternReader reader, State state)
        {
            interpreter.Print(firstToken.Value);
            return false;
        }

        private static bool DoConstant(Interpreter interpreter, Token<TokenType> firstToken, PatternReader reader, State state)
        {
            interpreter.Print(Util.UnescapeConstantLiteral(firstToken.Value));
            return false;
        }

        private static bool DoEscape(Interpreter interpreter, Token<TokenType> firstToken, PatternReader reader, State state)
        {
            interpreter.Print(Util.Unescape(firstToken.Value, interpreter, interpreter.RNG));
            return false;
        }

        private static bool DoTag(Interpreter interpreter, Token<TokenType> firstToken, PatternReader reader, State state)
        {
            var name = reader.ReadToken();

            switch (name.Identifier)
            {
                case TokenType.Percent: // List
                    return DoListAction(interpreter, firstToken, reader, state);
                case TokenType.Question: // Metapattern
                    state.AddPreBlueprint(new MetapatternBlueprint(interpreter));
                    interpreter.PushState(State.CreateSub(reader.Source, reader.ReadToScopeClose(TokenType.LeftSquare, TokenType.RightSquare, BracketPairs.All), interpreter));
                    return true;
                case TokenType.Regex: // Replacer
                    return DoReplacer(name, interpreter, reader, state);
                case TokenType.Dollar: // Subroutine
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
                state.AddPreBlueprint(new FuncTagBlueprint(interpreter, reader.Source, name));
            }
            else
            {
                var items = reader.ReadMultiItemScope(TokenType.LeftSquare, TokenType.RightSquare,
                    TokenType.Semicolon, BracketPairs.All).ToArray();

                state.AddPreBlueprint(new FuncTagBlueprint(interpreter, reader.Source, name, items));
            }
            return true;
        }

        private static bool DoListAction(Interpreter interpreter, Token<TokenType> firstToken, PatternReader reader, State state)
        {
            bool create = false;
            bool createGlobal = false;
            bool clear = false;
            if (reader.TakeLoose(TokenType.Equal))
            {
                create = createGlobal = true;
            }
            else if (reader.TakeLoose(TokenType.Colon))
            {
                create = true;
            }
            else if (reader.TakeLoose(TokenType.Exclamation))
            {
                clear = true;
            }

            var nameToken = reader.ReadLoose(TokenType.Text, "list name");
            var name = nameToken.Value;

            if (!Util.ValidateName(name))
                throw new RantException(reader.Source, nameToken, "Invalid list name '" + name + "'");

            List<string> list;
            if (create)
            {
                list = new List<string>();
                if (createGlobal)
                {
                    interpreter.Engine.GlobalLists[name] = list;
                }
                else
                {
                    interpreter.LocalLists[name] = list;
                }
                reader.Read(TokenType.RightSquare);
                return false;
            }

            if (!interpreter.LocalLists.TryGetValue(name, out list) &&
                !interpreter.Engine.GlobalLists.TryGetValue(name, out list))
                throw new RantException(reader.Source, nameToken, "Tried to access nonexistent list '" + name + "'");

            if (clear)
            {
                list.Clear();
                reader.Read(TokenType.RightSquare);
                return false;
            }

            #region "+" Functions
            if (reader.TakeLoose(TokenType.Plus)) // add items
            {
                var atStart = reader.TakeLoose(TokenType.Caret);
                var fromList = reader.TakeLoose(TokenType.Percent);

                if (fromList) // add items from other list
                {
                    var nameTokens = reader.ReadToScopeClose(TokenType.LeftSquare, TokenType.RightSquare, BracketPairs.All);
                    interpreter.PushState(State.CreateSub(reader.Source, nameTokens, interpreter));
                    state.AddPreBlueprint(new DelegateBlueprint(interpreter, _ =>
                    {
                        var srcName = _.PopResultString();
                        List<string> src;
                        if (!_.GetList(srcName, out src))
                            throw new RantException(nameTokens, reader.Source, "Tried to access nonexistent list '" + srcName + "'");
                        if (atStart)
                        {
                            list.InsertRange(0, src);
                        }
                        else
                        {
                            list.AddRange(src);
                        }
                        return false;
                    }));
                    return true;
                }

                var items = reader.ReadMultiItemScope(TokenType.LeftSquare, TokenType.RightSquare,
                    TokenType.Semicolon, BracketPairs.All).ToArray();
                int count = items.Length;

                foreach (var item in items)
                {
                    interpreter.PushState(State.CreateSub(reader.Source, item, interpreter));
                }

                state.AddPreBlueprint(new DelegateBlueprint(interpreter, _ =>
                {
                    if (atStart)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            list.Insert(i, _.PopResultString());
                        }
                    }
                    else
                    {
                        for (int i = 0; i < count; i++)
                        {
                            list.Add(_.PopResultString());
                        }
                    }
                    return false;
                }));
                return true;
            }

            #endregion

            #region "^" functions
            if (reader.TakeLoose(TokenType.Caret)) // add items to start
            {
                
                if (reader.Take(TokenType.Exclamation)) // remove first item
                {
                    if (list.Any()) list.RemoveAt(0);
                    reader.ReadLoose(TokenType.RightSquare);
                    return true;
                }

                var items = reader.ReadMultiItemScope(TokenType.LeftSquare, TokenType.RightSquare,
                    TokenType.Semicolon, BracketPairs.All).ToArray();
                int count = items.Length;

                foreach (var item in items)
                {
                    interpreter.PushState(State.CreateSub(reader.Source, item, interpreter));
                }

                state.AddPreBlueprint(new DelegateBlueprint(interpreter, _ =>
                {
                    for (int i = 0; i < count; i++)
                    {
                        list.Insert(0, _.PopResultString());
                    }
                    return false;
                }));
                return true;
            }
            #endregion

            #region "=" functions
            if (reader.TakeLoose(TokenType.Equal))
            {
                if (reader.TakeLoose(TokenType.At)) // set item at index to value
                {
                    var args = reader.ReadMultiItemScope(TokenType.LeftSquare, TokenType.RightSquare, TokenType.Semicolon, BracketPairs.All).ToArray();
                    if (args.Length != 2) throw new RantException(args.SelectMany(a => a), reader.Source, "Two arguments are required for this operation.");
                    interpreter.PushState(State.CreateSub(reader.Source, args[0], interpreter)); // index
                    interpreter.PushState(State.CreateSub(reader.Source, args[1], interpreter)); // value
                    state.AddPreBlueprint(new DelegateBlueprint(interpreter, _ =>
                    {
                        var indexString = _.PopResultString();
                        var valueString = _.PopResultString();
                        int index;
                        if (!Int32.TryParse(indexString, out index))
                            throw new RantException(args[0], reader.Source, "'" + indexString + "' is not a valid index. Index must be a non-negative integer.");
                        if (index >= list.Count)
                            throw new RantException(args[0], reader.Source, "Index was out of range. (" + index + " > " + (list.Count - 1) + ")");

                        list[index] = valueString;
                        return false;
                    }));
                    return true;
                }

                var nameTokens = reader.ReadToScopeClose(TokenType.LeftSquare, TokenType.RightSquare, BracketPairs.All);
                interpreter.PushState(State.CreateSub(reader.Source, nameTokens, interpreter));
                state.AddPreBlueprint(new DelegateBlueprint(interpreter, _ =>
                {
                    List<string> srcList;
                    var srcName = _.PopResultString();
                    if (!_.GetList(srcName, out srcList))
                        throw new RantException(nameTokens, reader.Source, "Tried to access nonexistent list '" + srcName + "'");
                    list.Clear();
                    list.AddRange(srcList);
                    return false;
                }));

                return true;
            }
            #endregion

            #region "!" functions
            if (reader.TakeLoose(TokenType.Exclamation)) // remove item
            {
                TokenType special;
                bool isSpecial = reader.TakeAny(out special, TokenType.At, TokenType.Caret, TokenType.Dollar);

                if (!isSpecial || special == TokenType.At) // remove by value or index
                {
                    var valueTokens = reader.ReadToScopeClose(TokenType.LeftSquare, TokenType.RightSquare, BracketPairs.All);
                    interpreter.PushState(State.CreateSub(reader.Source, valueTokens, interpreter));

                    state.AddPreBlueprint(new DelegateBlueprint(interpreter, _ =>
                    {
                        var value = _.PopResultString();

                        if (isSpecial && special == TokenType.At)
                        {
                            int index;
                            if (!Int32.TryParse(value, out index) || index < 0)
                                throw new RantException(valueTokens, reader.Source, "'" + value + "' is not a valid index. Index must be a non-negative integer.");
                            if (index >= list.Count) return false;
                            list.RemoveAt(index);
                            return false;
                        }
                        list.Remove(value);
                        return false;
                    }));
                    return true;
                }

                switch (special)
                {
                    case TokenType.Caret: // remove first
                        if (list.Any()) list.RemoveAt(0);
                        break;
                    case TokenType.Dollar: // remove last
                        if (list.Any()) list.RemoveAt(list.Count - 1);
                        break;
                }

                reader.Read(TokenType.RightSquare);

                return false;
            }
            #endregion

            #region "$" functions
            if (reader.TakeLoose(TokenType.Dollar)) // list length
            {
                if (reader.Take(TokenType.Caret)) // reverse list
                {
                    list.Reverse();
                }
                else
                {
                    interpreter.Print(interpreter.FormatNumber(list.Count)); // count
                }

                reader.Read(TokenType.RightSquare);
                return false;
            }
            #endregion

            #region "@" functions
            if (reader.TakeLoose(TokenType.At)) // get item
            {
                TokenType special;
                bool isSpecial = reader.TakeAny(out special, TokenType.Question, TokenType.Caret, TokenType.Dollar);

                if (!isSpecial || special == TokenType.Question)
                {
                    var valueTokens = reader.ReadToScopeClose(TokenType.LeftSquare, TokenType.RightSquare, BracketPairs.All);
                    interpreter.PushState(State.CreateSub(reader.Source, valueTokens, interpreter));

                    state.AddPreBlueprint(new DelegateBlueprint(interpreter, _ =>
                    {
                        var arg = _.PopResultString();
                        if (isSpecial && special == TokenType.Question) // index of value
                        {
                            _.Print(_.FormatNumber(list.IndexOf(arg)));
                        }
                        else // value of index
                        {
                            int index;
                            if (!Int32.TryParse(arg, out index))
                                throw new RantException(valueTokens, reader.Source, "'" + arg + "' is not a valid index. Index must be a non-negative integer.");
                            if (index >= list.Count)
                                throw new RantException(valueTokens, reader.Source, "Index was out of range. (" + arg + " > " + (list.Count - 1) + ")");

                            _.Print(list[index]);
                        }
                        return false;
                    }));
                    return true;
                }

                switch (special)
                {
                    case TokenType.Caret: // get first
                        if (list.Any()) interpreter.Print(list.First());
                        break;
                    case TokenType.Dollar: // get last
                        if (list.Any()) interpreter.Print(list.Last());
                        break;
                }

                reader.Read(TokenType.RightSquare);

                return false;
            }
            #endregion


            throw new RantException(reader.Source, nameToken, "Expected operator after list name.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool DoSubCall(Token<TokenType> first, Interpreter interpreter, PatternReader reader, State state)
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
                args = reader.ReadMultiItemScope(TokenType.LeftSquare, TokenType.RightSquare, TokenType.Semicolon,
                    BracketPairs.All).ToArray();
                if((sub = interpreter.Engine.Subroutines.Get(name.Value, args.Length)) == null)
                    throw new RantException(reader.Source, name, "No subroutine was found with the name '" + name.Value + "' and " + args.Length + " parameter" + (args.Length != 1 ? "s" : "") + ".");
            }

            state.AddPreBlueprint(new SubCallBlueprint(interpreter, reader.Source, sub, args));

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool DoSubDefinition(Token<TokenType> first, Interpreter interpreter, PatternReader reader, State state)
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
                interpreter.PushState(State.CreateSub(reader.Source, body, interpreter));
                state.AddPreBlueprint(new DelegateBlueprint(interpreter, _ =>
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
        private static bool DoReplacer(Token<TokenType> name, Interpreter interpreter, PatternReader reader, State state)
        {
            reader.Read(TokenType.Colon);

            var args = reader.ReadMultiItemScope(TokenType.LeftSquare, TokenType.RightSquare, TokenType.Semicolon, BracketPairs.All).ToArray();
            if (args.Length != 2) throw new RantException(reader.Source, name, "Replacer expected 2 arguments, but got " + args.Length + ".");

            state.AddPreBlueprint(new ReplacerBlueprint(interpreter, Util.ParseRegex(name.Value), args[1]));

            interpreter.PushState(State.CreateSub(reader.Source, args[0], interpreter));
            return true;
        }

        private static bool DoBlock(Interpreter interpreter, Token<TokenType> firstToken, PatternReader reader, State state)
        {
            var attribs = interpreter.NextAttribs;
            interpreter._blockAttribs = new BlockAttribs();

            if (reader.Take(TokenType.Percent)) // List as block
            {
                reader.SkipSpace();
                var listNameToken = reader.Read(TokenType.Text, "list name");
                reader.SkipSpace();
                reader.Read(TokenType.RightCurly);

                var listName = listNameToken.Value;

                List<string> list;
                if (!interpreter.LocalLists.TryGetValue(listName, out list) && !interpreter.Engine.GlobalLists.TryGetValue(listName, out list))
                    throw new RantException(reader.Source, listNameToken, "Tried to access nonexistent list '" + listName + "'");

                var listItems = list.Any()
                    ? list.Select(str => new[] {new Token<TokenType>(TokenType.Text, str)}.AsEnumerable()).ToArray()
                    : new[] {Enumerable.Empty<Token<TokenType>>()};
                var listRep = new Repeater(listItems, attribs);
                interpreter.PushRepeater(listRep);
                interpreter.BaseStates.Add(state);
                state.AddPreBlueprint(new RepeaterBlueprint(interpreter, listRep));
                return true;
            }

            Tuple<IEnumerable<Token<TokenType>>[], int> items;

            // Check if the block is already cached
            if (!reader.Source.TryGetCachedBlock(firstToken, out items))
            {
                var block = reader.ReadMultiItemScope(TokenType.LeftCurly, TokenType.RightCurly, TokenType.Pipe, BracketPairs.All).ToArray();
                items = Tuple.Create(block, reader.Position);
                reader.Source.CacheBlock(firstToken, block, reader.Position);
            }
            else
            {
                // If the block is cached, seek to its end
                reader.Position = items.Item2;
            }

            if (!items.Item1.Any() || !interpreter.TakeChance()) return false;

            var rep = new Repeater(items.Item1, attribs);
            interpreter.PushRepeater(rep);
            interpreter.BaseStates.Add(state);
            state.AddPreBlueprint(new RepeaterBlueprint(interpreter, rep));
            return true;
        }
    }
}