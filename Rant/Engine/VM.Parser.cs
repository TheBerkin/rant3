using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using Rant.Engine.Arithmetic;
using Rant.Engine.Blueprints;
using Rant.Engine.Compiler;
using Rant.Stringes.Tokens;
using Rant.Engine.Util;
using Rant.Vocabulary;

namespace Rant.Engine
{
    internal delegate bool TokenFunc(VM interpreter, Token<R> firstToken, PatternReader reader, VM.State state);

    internal partial class VM
    {
        private static readonly Dictionary<R, TokenFunc> TokenFuncs = new Dictionary<R, TokenFunc>
        {
            {R.LeftCurly, DoBlock},
            {R.LeftSquare, DoTag},
            {R.Backtick, DoMath},
            {R.LeftAngle, DoQuery},
            {R.EscapeSequence, DoEscape},
            {R.ConstantLiteral, DoConstant},
            {R.Text, DoText}
        };

        private static bool DoMath(VM interpreter, Token<R> firstToken, PatternReader reader, State state)
        {
            var tokens = reader.ReadToTokenInParentScope(firstToken.ID, Delimiters.All);
            interpreter.PushState(State.CreateSub(reader.Source, tokens, interpreter));
            state.Pre(new DelegateBlueprint(interpreter, _ =>
            {
                var expression = _.PopResultString().Trim();
                var v = MathParser.Calculate(_, expression);                    
                if (!expression.EndsWith(";")) _.Print(_.FormatNumber(v));
                return false;
            }));
            return true;
        }

        private static bool DoQuery(VM interpreter, Token<R> firstToken, PatternReader reader, State state)
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
                
                macroName = macroNameToken.Value;

                reader.SkipSpace();

                // Check if the macro is a definition or a call.
                // A definition will start with a colon ':' or equals '=' after the name. A call will only consist of the name.
                switch (reader.ReadToken().ID)
                {
                    case R.Colon: // Local definition
                    {
                        if (!ValidateName(macroNameToken.Value))
                            throw Error(reader.Source, macroNameToken, "Invalid macro name: '\{macroNameToken.Value}'");
                        storeMacro = true;
                    }
                    break;
                    case R.Equal: // Global definition
                    {
                        if (!ValidateName(macroNameToken.Value))
                            throw Error(reader.Source, macroNameToken, "Invalid macro name: '\{macroNameToken.Value}'");
                        storeMacro = true;
                        macroIsGlobal = true;
                    }
                    break;
                    case R.RightAngle: // Call
                    {
                        Query q;                        
                        var mNameSub = macroNameToken.Value.Split(new[] { '.' }, StringSplitOptions.None);
                        if (!interpreter.LocalQueryMacros.TryGetValue(mNameSub[0], out q) && !interpreter.Engine.GlobalQueryMacros.TryGetValue(mNameSub[0], out q))
                        {
                            throw new RantException(reader.Source, macroNameToken, "Nonexistent query macro '\{macroName}'");
                        }
                        if (mNameSub.Length > 2) throw Error(reader.Source, firstToken, "Invald subtype accessor on macro call.");
                        var oldSub = q.Subtype;
                        if (mNameSub.Length == 2) q.Subtype = mNameSub[1];
                        interpreter.Print(interpreter.Engine.Dictionary?.Query(interpreter.RNG, q, interpreter.QueryState));
                        q.Subtype = oldSub;
                        return false;
                    }
                }
            }
            else if (reader.Take(R.DoubleColon)) // Carrier reset
            {
                Token<R> token;

                while((token = reader.ReadToken()).ID != R.RightAngle)
                {
                    switch(token.ID)
                    {
                        case R.At:
                            interpreter.QueryState.DeleteAssociation(reader.ReadLoose(R.Text, "associative carrier name").Value);
                            break;
                        case R.Exclamation:
                            interpreter.QueryState.DeleteUnique(reader.ReadLoose(R.Text, "unique carrier name").Value);
                            break;
                        case R.Equal:
                            interpreter.QueryState.DeleteMatch(reader.ReadLoose(R.Text, "match carrier name").Value);
                            break;
                        case R.Ampersand:
                            interpreter.QueryState.DeleteRhyme(reader.ReadLoose(R.Text, "rhyme carrier name").Value);
                            break;
                        default:
                            throw Error(reader.Source, token, "Unrecognized token in carrier reset: '\{token.Value}'");
                    }
                    reader.SkipSpace();
                }
                return false;
            }

            reader.SkipSpace();
            var namesub = reader.Read(R.Text, "dictionary name").Split(new[] { '.' }, 2).ToArray();
            reader.SkipSpace();

            bool exclusive = reader.Take(R.Dollar);
            List<Tuple<bool, string>> cfList = null;
            List<Tuple<bool, string>[]> classFilterList = null;
            List<Tuple<bool, Regex>> regList = null;
            Carrier carrier = null;
            SyllablePredicateFunc syllableRange = null;

            Token<R> queryToken = null;

            if (reader.IsNext(R.RangeLiteral))
            {
                syllableRange = SyllablePredicate.Create(reader.ReadToken());
            }

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

                    carrier = new Carrier();
                    Token<R> typeToken;
                    CarrierComponent comp = CarrierComponent.Match;
                    
                    while((typeToken = reader.ReadToken()).ID != R.RightAngle)
                    {
                        switch (typeToken.ID)
                        {
                            case R.Exclamation: // Unique
                                comp = reader.Take(R.Equal) ? CarrierComponent.MatchUnique : CarrierComponent.Unique;
                                break;
                            case R.Equal: // Match
                                comp = CarrierComponent.Match;
                                break;
                            case R.Ampersand: // Rhyme
                                comp = CarrierComponent.Rhyme;
                                break;
                            case R.At: // Associative/Relational/Dissociative/Divergent
                                {
                                    if (reader.Take(R.Question))
                                    {
                                        comp = reader.Take(R.Equal) ? CarrierComponent.MatchRelational : CarrierComponent.Relational;
                                    }
                                    else if (reader.Take(R.Exclamation))
                                    {
                                        comp = reader.Take(R.Equal) ? CarrierComponent.MatchDissociative : CarrierComponent.Dissociative;
                                    }
                                    else if (reader.Take(R.Plus))
                                    {
                                        comp = reader.Take(R.Equal) ? CarrierComponent.MatchDivergent : CarrierComponent.Divergent;
                                    }
                                    else
                                    {
                                        comp = reader.Take(R.Equal) ? CarrierComponent.MatchAssociative : CarrierComponent.Associative;
                                    }
                                }                                
                                break;
                            default:
                                throw new RantException(reader.Source, typeToken, "Unrecognized token '\{typeToken.Value}' in carrier.");
                        }
                        carrier.AddComponent(comp, reader.ReadLoose(R.Text, "carrier component name").Value);
                        reader.SkipSpace();
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
                carrier, exclusive, classFilterList, regList,
                syllableRange);

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
            interpreter.Print(interpreter.Engine.Dictionary?.Query(interpreter.RNG, query, interpreter.QueryState));

            return false;
        }

        private static bool DoText(VM interpreter, Token<R> firstToken, PatternReader reader, State state)
        {
            interpreter.Print(firstToken.Value);
            return false;
        }

        private static bool DoConstant(VM interpreter, Token<R> firstToken, PatternReader reader, State state)
        {
            interpreter.Print(UnescapeConstantLiteral(firstToken.Value));
            return false;
        }

        private static bool DoEscape(VM interpreter, Token<R> firstToken, PatternReader reader, State state)
        {
            interpreter.Print(Unescape(firstToken.Value, interpreter, interpreter.RNG));
            return false;
        }

        private static bool DoTag(VM interpreter, Token<R> firstToken, PatternReader reader, State state)
        {
            var name = reader.ReadToken();

            switch (name.ID)
            {
                case R.Percent: // List
                    return DoListAction(interpreter, firstToken, reader, state);
                case R.Question: // Metapattern
                    state.Pre(new MetapatternBlueprint(interpreter));
                    interpreter.PushState(State.CreateSub(reader.Source, reader.ReadToScopeClose(R.LeftSquare, R.RightSquare, Delimiters.All), interpreter));
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
                state.Pre(new FuncTagBlueprint(interpreter, reader.Source, name));
            }
            else
            {
                var items = reader.ReadMultiItemScope(R.LeftSquare, R.RightSquare,
                    R.Semicolon, Delimiters.All).ToArray();

                state.Pre(new FuncTagBlueprint(interpreter, reader.Source, name, items));
            }
            return true;
        }

        private static bool DoListAction(VM interpreter, Token<R> firstToken, PatternReader reader, State state)
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
                    var nameTokens = reader.ReadToScopeClose(R.LeftSquare, R.RightSquare, Delimiters.All);
                    interpreter.PushState(State.CreateSub(reader.Source, nameTokens, interpreter));
                    state.Pre(new DelegateBlueprint(interpreter, _ =>
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
                    R.Semicolon, Delimiters.All).ToArray();
                int count = items.Length;

                foreach (var item in items)
                {
                    interpreter.PushState(State.CreateSub(reader.Source, item, interpreter));
                }

                state.Pre(new DelegateBlueprint(interpreter, _ =>
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
                    R.Semicolon, Delimiters.All).ToArray();
                int count = items.Length;

                foreach (var item in items)
                {
                    interpreter.PushState(State.CreateSub(reader.Source, item, interpreter));
                }

                state.Pre(new DelegateBlueprint(interpreter, _ =>
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
                    var args = reader.ReadMultiItemScope(R.LeftSquare, R.RightSquare, R.Semicolon, Delimiters.All).ToArray();
                    if (args.Length != 2) throw new RantException(args.SelectMany(a => a), reader.Source, "Two arguments are required for this operation.");
                    interpreter.PushState(State.CreateSub(reader.Source, args[0], interpreter)); // index
                    interpreter.PushState(State.CreateSub(reader.Source, args[1], interpreter)); // value
                    state.Pre(new DelegateBlueprint(interpreter, _ =>
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

                var nameTokens = reader.ReadToScopeClose(R.LeftSquare, R.RightSquare, Delimiters.All);
                interpreter.PushState(State.CreateSub(reader.Source, nameTokens, interpreter));
                state.Pre(new DelegateBlueprint(interpreter, _ =>
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
                    var valueTokens = reader.ReadToScopeClose(R.LeftSquare, R.RightSquare, Delimiters.All);
                    interpreter.PushState(State.CreateSub(reader.Source, valueTokens, interpreter));

                    state.Pre(new DelegateBlueprint(interpreter, _ =>
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
                    var valueTokens = reader.ReadToScopeClose(R.LeftSquare, R.RightSquare, Delimiters.All);
                    interpreter.PushState(State.CreateSub(reader.Source, valueTokens, interpreter));

                    state.Pre(new DelegateBlueprint(interpreter, _ =>
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

                return false;
            }
            #endregion


            throw new RantException(reader.Source, nameToken, "Expected operator after list name.");
        }

        
        private static bool DoSubCall(Token<R> first, VM interpreter, PatternReader reader, State state)
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
                    Delimiters.All).ToArray();
                if((sub = interpreter.Engine.Subroutines.Get(name.Value, args.Length)) == null)
                    throw new RantException(reader.Source, name, "No subroutine was found with the name '\{name.Value}' and \{args.Length} parameter\{(args.Length != 1 ? "s" : "")}.");
            }

            state.Pre(new SubCallBlueprint(interpreter, reader.Source, sub, args));

            return true;
        }

        
        private static bool DoSubDefinition(Token<R> first, VM interpreter, PatternReader reader, State state)
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

            var body = reader.ReadToScopeClose(R.LeftSquare, R.RightSquare, Delimiters.All).ToArray();

            if (meta)
            {
                interpreter.PushState(State.CreateSub(reader.Source, body, interpreter));
                state.Pre(new DelegateBlueprint(interpreter, _ =>
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

        
        private static bool DoReplacer(Token<R> name, VM interpreter, PatternReader reader, State state)
        {
            reader.Read(R.Colon);

            var args = reader.ReadMultiItemScope(R.LeftSquare, R.RightSquare, R.Semicolon, Delimiters.All).ToArray();
            if (args.Length != 2) throw new RantException(reader.Source, name, "Replacer expected two arguments, but got \{args.Length}.");

            state.Pre(new ReplacerBlueprint(interpreter, ParseRegex(name.Value), args[1]));

            interpreter.PushState(State.CreateSub(reader.Source, args[0], interpreter));
            return true;
        }

        private static bool DoBlock(VM interpreter, Token<R> firstToken, PatternReader reader, State state)
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
                state.Pre(new RepeaterBlueprint(interpreter, listRep));
                return true;
            }

            Tuple<Block, int> blockInfo;

            // Check if the block is already cached
            if (!reader.Source.TryGetCachedBlock(firstToken, out blockInfo))
            {
                var elements = reader.ReadMultiItemScope(R.LeftCurly, R.RightCurly, R.Pipe, Delimiters.All).ToArray();
                blockInfo = Tuple.Create(Block.Create(elements), reader.Position);
                reader.Source.CacheBlock(firstToken, blockInfo);
            }
            else
            {
                // If the block is cached, seek to its end
                reader.Position = blockInfo.Item2;
            }

            if (!blockInfo.Item1.Items.Any() || blockInfo.Item1.WeightTotal == 0 || !interpreter.TakeChance()) return false;

            var rep = new Repeater(blockInfo.Item1, attribs);
            interpreter.PushRepeater(rep);
            interpreter.BaseStates.Add(state);
            state.Pre(new RepeaterBlueprint(interpreter, rep));
            return true;
        }
    }
}