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

using Rant.Util;

namespace Rant
{
    internal delegate bool TokenFunc(Interpreter interpreter, Token<R> firstToken, PatternReader reader, Interpreter.State state);

    internal partial class Interpreter
    {
        private static readonly Dictionary<R, TokenFunc> TokenFuncs = new Dictionary<R, TokenFunc>
        {
            {R.LeftCurly, DoBlock},
            {R.LeftSquare, DoTag},
            {R.LeftParen, DoMath},
            {R.LeftAngle, DoQuery},
            {R.EscapeSequence, DoEscape},
            {R.ConstantLiteral, DoConstant},
            {R.Text, DoText}
        };

        private static bool DoMath(Interpreter interpreter, Token<R> firstToken, PatternReader reader, State state)
        {
            bool isStatement = reader.Take(R.At);
            var tokens = reader.ReadToScopeClose(R.LeftParen, R.RightParen, Brackets.All);
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

        private static bool DoQuery(Interpreter interpreter, Token<R> firstToken, PatternReader reader, State state)
        {
            reader.SkipSpace();

            bool storeMacro = false;
            bool macroIsGlobal = false;
            string macroName = null;

            // Check if this is a macro
            if (reader.Take(R.At))
            {
                reader.SkipSpace();

                var macroNameToken = reader.Read(R.Text, "query macro name");
                if (!ValidateName(macroNameToken.Value))
                    throw new RantException(reader.Source, macroNameToken, "Invalid macro name.");

                macroName = macroNameToken.Value;

                reader.SkipSpace();

                // Check if the macro is a definition or a call.
                // A definition will start with a colon ':' or equals '=' after the name. A call will only consist of the name.
                switch (reader.ReadToken().ID)
                {
                    case R.Colon: // Local definition
                    {
                        storeMacro = true;
                    }
                    break;
                    case R.Equal: // Global definition
                    {
                        storeMacro = true;
                        macroIsGlobal = true;
                    }
                    break;
                    case R.RightAngle: // Call
                    {
                        Query q;
                        if (!interpreter.LocalQueryMacros.TryGetValue(macroName, out q) && !interpreter.Engine.GlobalQueryMacros.TryGetValue(macroName, out q))
                        {
                            throw new RantException(reader.Source, macroNameToken, "Nonexistent query macro '\{macroName}'");
                        }
                        interpreter.Print(interpreter.Engine.Vocabulary?.Query(interpreter.RNG, q, interpreter.CarrierSyncState));
                        return false;
                    }
                }
            }

            reader.SkipSpace();
            var namesub = reader.Read(R.Text, "dictionary name").Split(new[] { '.' }, 2).ToArray();
            reader.SkipSpace();

            bool exclusive = reader.Take(R.Dollar);
            List<Tuple<bool, string>> cfList = null;
            List<Tuple<bool, string>[]> classFilterList = null;
            List<Tuple<bool, Regex>> regList = null;
            Carrier carrier = null;

            Token<R> queryToken = null;

            // Read query arguments
            while (true)
            {
                reader.SkipSpace();
                if (reader.Take(R.Hyphen))
                {
                    reader.SkipSpace();
                    // Initialize the filter list.
                    (cfList ?? (cfList = new List<Tuple<bool, string>>())).Clear();
                    do
                    {
                        bool notin = reader.Take(R.Exclamation);
                        reader.SkipSpace();
                        if (notin && exclusive)
                            throw new RantException(reader.Source, reader.PrevToken, "Cannot use the '!' modifier on exclusive class filters.");
                        cfList.Add(Tuple.Create(!notin, reader.Read(R.Text, "class identifier").Value.Trim()));
                        reader.SkipSpace();
                    } while (reader.Take(R.Pipe));
                    (classFilterList ?? (classFilterList = new List<Tuple<bool, string>[]>())).Add(cfList.ToArray());
                }
                else if (reader.Take(R.Question))
                {
                    reader.SkipSpace();
                    queryToken = reader.Read(R.Regex, "regex");
                    (regList ?? (regList = new List<Tuple<bool, Regex>>())).Add(Tuple.Create(true, ParseRegex(queryToken.Value)));
                }
                else if (reader.Take(R.Without))
                {
                    reader.SkipSpace();
                    queryToken = reader.Read(R.Regex, "regex");
                    (regList ?? (regList = new List<Tuple<bool, Regex>>())).Add(Tuple.Create(false, ParseRegex(queryToken.Value)));
                }
                else if (reader.Take(R.DoubleColon)) // Start of carrier
                {
                    reader.SkipSpace();

                    CarrierSyncType type;
                    Token<R> typeToken;

                    switch ((typeToken = reader.ReadToken()).ID)
                    {
                        case R.Exclamation:
                            type = CarrierSyncType.Unique;
                            break;
                        case R.Equal:
                            type = CarrierSyncType.Match;
                            break;
                        case R.Ampersand:
                            type = CarrierSyncType.Rhyme;
                            break;
                        default:
                            throw new RantException(reader.Source, typeToken, "Unrecognized token '\{typeToken.Value}' in carrier.");
                    }

                    reader.SkipSpace();

                    carrier = new Carrier(type, reader.Read(R.Text, "carrier sync ID").Value, 0, 0);

                    reader.SkipSpace();

                    if (!reader.Take(R.RightAngle))
                    {
                        throw new RantException(reader.Source, queryToken, "Expected '>' after carrier. (The carrier should be your last query argument!)");
                    }
                    break;
                }
                else if (reader.Take(R.RightAngle))
                {
                    break;
                }
                else if (!reader.SkipSpace())
                {
                    var t = !reader.End ? reader.ReadToken() : null;
                    throw new RantException(reader.Source, t, t == null ? "Unexpected end-of-file in query." : "Unexpected token '\{t.Value}' in query.");
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
            interpreter.Print(interpreter.Engine.Vocabulary?.Query(interpreter.RNG, query, interpreter.CarrierSyncState));

            return false;
        }

        private static bool DoText(Interpreter interpreter, Token<R> firstToken, PatternReader reader, State state)
        {
            interpreter.Print(firstToken.Value);
            return false;
        }

        private static bool DoConstant(Interpreter interpreter, Token<R> firstToken, PatternReader reader, State state)
        {
            interpreter.Print(UnescapeConstantLiteral(firstToken.Value));
            return false;
        }

        private static bool DoEscape(Interpreter interpreter, Token<R> firstToken, PatternReader reader, State state)
        {
            interpreter.Print(Unescape(firstToken.Value, interpreter, interpreter.RNG));
            return false;
        }

        private static bool DoTag(Interpreter interpreter, Token<R> firstToken, PatternReader reader, State state)
        {
            var name = reader.ReadToken();

            switch (name.ID)
            {
                case R.Percent: // List
                    return DoListAction(interpreter, firstToken, reader, state);
                case R.Question: // Metapattern
                    state.AddPreBlueprint(new MetapatternBlueprint(interpreter));
                    interpreter.PushState(State.CreateSub(reader.Source, reader.ReadToScopeClose(R.LeftSquare, R.RightSquare, Brackets.All), interpreter));
                    return true;
                case R.Regex: // Replacer
                    return DoReplacer(name, interpreter, reader, state);
                case R.Dollar: // Subroutine
                    return reader.IsNext(R.Text) ? DoSubCall(name, interpreter, reader, state) : DoSubDefinition(name, interpreter, reader, state);
            }

            if (!ValidateName(name.Value.Trim()))
                throw new RantException(reader.Source, name, "Invalid tag name '\{name.Value}'");

            bool none = false;
            if (!reader.Take(R.Colon))
            {
                if (!reader.Take(R.RightSquare))
                    throw new RantException(reader.Source, name, "Expected ':' or ']' after tag name.");
                none = true;
            }

            if (none)
            {
                state.AddPreBlueprint(new FuncTagBlueprint(interpreter, reader.Source, name));
            }
            else
            {
                var items = reader.ReadMultiItemScope(R.LeftSquare, R.RightSquare,
                    R.Semicolon, Brackets.All).ToArray();

                state.AddPreBlueprint(new FuncTagBlueprint(interpreter, reader.Source, name, items));
            }
            return true;
        }

        private static bool DoListAction(Interpreter interpreter, Token<R> firstToken, PatternReader reader, State state)
        {
            bool create = false;
            bool createGlobal = false;
            bool clear = false;
            if (reader.TakeLoose(R.Equal))
            {
                create = createGlobal = true;
            }
            else if (reader.TakeLoose(R.Colon))
            {
                create = true;
            }
            else if (reader.TakeLoose(R.Exclamation))
            {
                clear = true;
            }

            var nameToken = reader.ReadLoose(R.Text, "list name");
            var name = nameToken.Value;

            if (!ValidateName(name))
                throw new RantException(reader.Source, nameToken, "Invalid list name '\{name}'");

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
                if (reader.TakeLoose(R.RightSquare)) return false;
            }

            if (!interpreter.LocalLists.TryGetValue(name, out list) &&
                !interpreter.Engine.GlobalLists.TryGetValue(name, out list))
                throw new RantException(reader.Source, nameToken, "Tried to access nonexistent list '\{name}'");

            if (clear)
            {
                list.Clear();
                if (reader.TakeLoose(R.RightSquare)) return false;
            }

            #region "+" Functions
            if (reader.TakeLoose(R.Plus)) // add items
            {
                var atStart = reader.TakeLoose(R.Caret);
                var fromList = reader.TakeLoose(R.Percent);

                if (fromList) // add items from other list
                {
                    var nameTokens = reader.ReadToScopeClose(R.LeftSquare, R.RightSquare, Brackets.All);
                    interpreter.PushState(State.CreateSub(reader.Source, nameTokens, interpreter));
                    state.AddPreBlueprint(new DelegateBlueprint(interpreter, _ =>
                    {
                        var srcName = _.PopResultString();
                        List<string> src;
                        if (!_.GetList(srcName, out src))
                            throw new RantException(nameTokens, reader.Source, "Tried to access nonexistent list '\{srcName}'");
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

                var items = reader.ReadMultiItemScope(R.LeftSquare, R.RightSquare,
                    R.Semicolon, Brackets.All).ToArray();
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
            if (reader.TakeLoose(R.Caret)) // add items to start
            {
                
                if (reader.Take(R.Exclamation)) // remove first item
                {
                    if (list.Any()) list.RemoveAt(0);
                    reader.ReadLoose(R.RightSquare);
                    return true;
                }

                var items = reader.ReadMultiItemScope(R.LeftSquare, R.RightSquare,
                    R.Semicolon, Brackets.All).ToArray();
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
            if (reader.TakeLoose(R.Equal))
            {
                if (reader.TakeLoose(R.At)) // set item at index to value
                {
                    var args = reader.ReadMultiItemScope(R.LeftSquare, R.RightSquare, R.Semicolon, Brackets.All).ToArray();
                    if (args.Length != 2) throw new RantException(args.SelectMany(a => a), reader.Source, "Two arguments are required for this operation.");
                    interpreter.PushState(State.CreateSub(reader.Source, args[0], interpreter)); // index
                    interpreter.PushState(State.CreateSub(reader.Source, args[1], interpreter)); // value
                    state.AddPreBlueprint(new DelegateBlueprint(interpreter, _ =>
                    {
                        var indexString = _.PopResultString();
                        var valueString = _.PopResultString();
                        int index;
                        if (!Int32.TryParse(indexString, out index))
                            throw new RantException(args[0], reader.Source, "'\{indexString}' is not a valid index. Index must be a non-negative integer.");
                        if (index >= list.Count)
                            throw new RantException(args[0], reader.Source, "Index was out of range. (\{index} > \{list.Count - 1})");

                        list[index] = valueString;
                        return false;
                    }));
                    return true;
                }

                var nameTokens = reader.ReadToScopeClose(R.LeftSquare, R.RightSquare, Brackets.All);
                interpreter.PushState(State.CreateSub(reader.Source, nameTokens, interpreter));
                state.AddPreBlueprint(new DelegateBlueprint(interpreter, _ =>
                {
                    List<string> srcList;
                    var srcName = _.PopResultString();
                    if (!_.GetList(srcName, out srcList))
                        throw new RantException(nameTokens, reader.Source, "Tried to access nonexistent list '\{srcName}'");
                    list.Clear();
                    list.AddRange(srcList);
                    return false;
                }));

                return true;
            }
            #endregion

            #region "!" functions
            if (reader.TakeLoose(R.Exclamation)) // remove item
            {
                R special;
                bool isSpecial = reader.TakeAny(out special, R.At, R.Caret, R.Dollar);

                if (!isSpecial || special == R.At) // remove by value or index
                {
                    var valueTokens = reader.ReadToScopeClose(R.LeftSquare, R.RightSquare, Brackets.All);
                    interpreter.PushState(State.CreateSub(reader.Source, valueTokens, interpreter));

                    state.AddPreBlueprint(new DelegateBlueprint(interpreter, _ =>
                    {
                        var value = _.PopResultString();

                        if (isSpecial && special == R.At)
                        {
                            int index;
                            if (!Int32.TryParse(value, out index) || index < 0)
                                throw new RantException(valueTokens, reader.Source, "'\{value}' is not a valid index. Index must be a non-negative integer.");
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
                    case R.Caret: // remove first
                        if (list.Any()) list.RemoveAt(0);
                        break;
                    case R.Dollar: // remove last
                        if (list.Any()) list.RemoveAt(list.Count - 1);
                        break;
                }

                reader.Read(R.RightSquare);

                return false;
            }
            #endregion

            #region "$" functions
            if (reader.TakeLoose(R.Dollar)) // list length
            {
                if (reader.Take(R.Caret)) // reverse list
                {
                    list.Reverse();
                }
                else
                {
                    interpreter.Print(interpreter.FormatNumber(list.Count)); // count
                }

                reader.Read(R.RightSquare);
                return false;
            }
            #endregion

            #region "@" functions
            if (reader.TakeLoose(R.At)) // get item
            {
                R special;
                bool isSpecial = reader.TakeAny(out special, R.Question, R.Caret, R.Dollar);

                if (!isSpecial || special == R.Question)
                {
                    var valueTokens = reader.ReadToScopeClose(R.LeftSquare, R.RightSquare, Brackets.All);
                    interpreter.PushState(State.CreateSub(reader.Source, valueTokens, interpreter));

                    state.AddPreBlueprint(new DelegateBlueprint(interpreter, _ =>
                    {
                        var arg = _.PopResultString();
                        if (isSpecial && special == R.Question) // index of value
                        {
                            _.Print(_.FormatNumber(list.IndexOf(arg)));
                        }
                        else // value of index
                        {
                            int index;
                            if (!Int32.TryParse(arg, out index))
                                throw new RantException(valueTokens, reader.Source, "'\{arg}' is not a valid index. Index must be a non-negative integer.");
                            if (index >= list.Count)
                                throw new RantException(valueTokens, reader.Source, "Index was out of range. (\{arg} > \{list.Count - 1})");

                            _.Print(list[index]);
                        }
                        return false;
                    }));
                    return true;
                }

                switch (special)
                {
                    case R.Caret: // get first
                        if (list.Any()) interpreter.Print(list.First());
                        break;
                    case R.Dollar: // get last
                        if (list.Any()) interpreter.Print(list.Last());
                        break;
                }

                reader.Read(R.RightSquare);

                return false;
            }
            #endregion


            throw new RantException(reader.Source, nameToken, "Expected operator after list name.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool DoSubCall(Token<R> first, Interpreter interpreter, PatternReader reader, State state)
        {
            var name = reader.ReadToken();
            if (!ValidateName(name.Value))
                throw new RantException(reader.Source, name, "Invalid subroutine name '\{name.Value}'");
            
            bool none = false;

            if (!reader.Take(R.Colon))
            {
                if (!reader.Take(R.RightSquare))
                    throw new RantException(reader.Source, name, "Expected ':' or ']' after subroutine name.");
                
                none = true;
            }

            IEnumerable<Token<R>>[] args = null;
            Subroutine sub = null;

            if (none)
            {
                if((sub = interpreter.Engine.Subroutines.Get(name.Value, 0)) == null)
                    throw new RantException(reader.Source, name, "No subroutine was found with the name '\{name.Value}' and 0 parameters.");
            }
            else
            {
                args = reader.ReadMultiItemScope(R.LeftSquare, R.RightSquare, R.Semicolon,
                    Brackets.All).ToArray();
                if((sub = interpreter.Engine.Subroutines.Get(name.Value, args.Length)) == null)
                    throw new RantException(reader.Source, name, "No subroutine was found with the name '\{name.Value}' and \{args.Length} parameter\{(args.Length != 1 ? "s" : "")}.");
            }

            state.AddPreBlueprint(new SubCallBlueprint(interpreter, reader.Source, sub, args));

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool DoSubDefinition(Token<R> first, Interpreter interpreter, PatternReader reader, State state)
        {
            bool meta = reader.Take(R.Question);
            reader.Read(R.LeftSquare);

            var parameters = new List<Tuple<string, ParamFlags>>();
            var tName = reader.Read(R.Text, "subroutine name");

            if (!ValidateName(tName.Value))
                throw new RantException(reader.Source, tName, "Invalid subroutine name: '\{tName.Value}'");
            
            if (!reader.Take(R.Colon))
            {
                reader.Read(R.RightSquare);
            }
            else
            {
                while (true)
                {
                    bool isTokens = reader.Take(R.At);
                    parameters.Add(Tuple.Create(reader.Read(R.Text, "parameter name").Value, isTokens ? ParamFlags.Code : ParamFlags.None));
                    if (reader.Take(R.RightSquare, false)) break;
                    reader.Read(R.Semicolon);
                }
            }

            reader.SkipSpace();
            reader.Read(R.Colon);

            var body = reader.ReadToScopeClose(R.LeftSquare, R.RightSquare, Brackets.All).ToArray();

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
        private static bool DoReplacer(Token<R> name, Interpreter interpreter, PatternReader reader, State state)
        {
            reader.Read(R.Colon);

            var args = reader.ReadMultiItemScope(R.LeftSquare, R.RightSquare, R.Semicolon, Brackets.All).ToArray();
            if (args.Length != 2) throw new RantException(reader.Source, name, "Replacer expected two arguments, but got \{args.Length}.");

            state.AddPreBlueprint(new ReplacerBlueprint(interpreter, ParseRegex(name.Value), args[1]));

            interpreter.PushState(State.CreateSub(reader.Source, args[0], interpreter));
            return true;
        }

        private static bool DoBlock(Interpreter interpreter, Token<R> firstToken, PatternReader reader, State state)
        {
            var attribs = interpreter.NextBlockAttribs;
            interpreter.NextBlockAttribs = new BlockAttribs();

            if (reader.Take(R.Percent)) // List as block
            {
                reader.SkipSpace();
                var listNameToken = reader.Read(R.Text, "list name");
                reader.SkipSpace();
                reader.Read(R.RightCurly);

                var listName = listNameToken.Value;

                List<string> list;
                if (!interpreter.LocalLists.TryGetValue(listName, out list) && !interpreter.Engine.GlobalLists.TryGetValue(listName, out list))
                    throw new RantException(reader.Source, listNameToken, "Tried to access nonexistent list '\{listName}'");

                var listItems = list.Any()
                    ? list.Select(str => new[] {new Token<R>(R.Text, str)}.AsEnumerable()).ToArray()
                    : new[] {Enumerable.Empty<Token<R>>()};
                var listRep = new Repeater(Block.Create(listItems), attribs);
                interpreter.PushRepeater(listRep);
                interpreter.BaseStates.Add(state);
                state.AddPreBlueprint(new RepeaterBlueprint(interpreter, listRep));
                return true;
            }

            Tuple<Block, int> block;

            // Check if the block is already cached
            if (!reader.Source.TryGetCachedBlock(firstToken, out block))
            {
                var elements = reader.ReadMultiItemScope(R.LeftCurly, R.RightCurly, R.Pipe, Brackets.All).ToArray();
                block = Tuple.Create(Block.Create(elements), reader.Position);
                reader.Source.CacheBlock(firstToken, block);
            }
            else
            {
                // If the block is cached, seek to its end
                reader.Position = block.Item2;
            }

            if (!block.Item1.Items.Any() || !interpreter.TakeChance()) return false;

            var rep = new Repeater(block.Item1, attribs);
            interpreter.PushRepeater(rep);
            interpreter.BaseStates.Add(state);
            state.AddPreBlueprint(new RepeaterBlueprint(interpreter, rep));
            return true;
        }
    }
}