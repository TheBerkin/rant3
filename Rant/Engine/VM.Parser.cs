using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Rant.Engine.Blueprints;
using Rant.Engine.Compiler;
using Rant.Engine.Constructs;
using static Rant.Engine.Util;
using Rant.Stringes;
using Rant.Vocabulary;
using Rant.Engine.ObjectModel;

namespace Rant.Engine
{
    internal delegate bool TokenFunc(VM interpreter, Token<R> firstToken, PatternReader reader, RantState state);

    internal partial class VM
    {
        private static readonly Dictionary<R, TokenFunc> TokenFuncs = new Dictionary<R, TokenFunc>
        {
            {R.LeftCurly, DoBlock},
            {R.LeftSquare, DoTag},
            {R.LeftAngle, DoQuery},
            {R.EscapeSequence, DoEscape},
            {R.ConstantLiteral, DoConstant},
            {R.Text, DoText},
			{R.At, DoScript}
        };

		private static bool DoScript(VM interpreter, Token<R> firstToken, PatternReader reader, RantState state)
		{
			var rave = new Rave(interpreter, reader);
			rave.Run();
			return false;
		}

		private static bool DoQuery(VM interpreter, Token<R> firstToken, PatternReader reader, RantState state)
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
                            throw Error(reader.Source, macroNameToken, $"Invalid macro name: '{macroNameToken.Value}'");
                        storeMacro = true;
                    }
                    break;
                    case R.Equal: // Global definition
                    {
                        if (!ValidateName(macroNameToken.Value))
                            throw Error(reader.Source, macroNameToken, $"Invalid macro name: '{macroNameToken.Value}'");
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
                            throw new RantException(reader.Source, macroNameToken, $"Nonexistent query macro '{macroName}'");
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
                            throw Error(reader.Source, token, $"Unrecognized token in carrier reset: '{token.Value}'");
                    }
                    reader.SkipSpace();
                }
                return false;
            }

            reader.SkipSpace();
            var namesub = reader.Read(R.Text, "dictionary name").Split(new[] { '.' }, 2).ToArray();
            reader.SkipSpace();

            bool exclusive = reader.Take(R.Dollar);
            List<_<bool, string>> cfList = null;
            List<_<bool, string>[]> classFilterList = null;
            List<_<bool, Regex>> regList = null;
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
                    (cfList ?? (cfList = new List<_<bool, string>>())).Clear();
                    do
                    {
                        bool notin = reader.Take(R.Exclamation);
                        reader.SkipSpace();
                        if (notin && exclusive)
                            throw new RantException(reader.Source, reader.PrevToken, "Cannot use the '!' modifier on exclusive class filters.");
                        cfList.Add(_.Create(!notin, reader.Read(R.Text, "class identifier").Value.Trim()));
                        reader.SkipSpace();
                    } while (reader.Take(R.Pipe));
                    (classFilterList ?? (classFilterList = new List<_<bool, string>[]>())).Add(cfList.ToArray());
                }
                else if (reader.Take(R.Question))
                {
                    reader.SkipSpace();
                    queryToken = reader.Read(R.Regex, "regex");
                    (regList ?? (regList = new List<_<bool, Regex>>())).Add(_.Create(true, ParseRegex(queryToken.Value)));
                }
                else if (reader.Take(R.Without))
                {
                    reader.SkipSpace();
                    queryToken = reader.Read(R.Regex, "regex");
                    (regList ?? (regList = new List<_<bool, Regex>>())).Add(_.Create(false, ParseRegex(queryToken.Value)));
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
                                throw new RantException(reader.Source, typeToken, $"Unrecognized token '{typeToken.Value}' in carrier.");
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
                    throw new RantException(reader.Source, t, t == null ? "Unexpected end-of-file in query." : $"Unexpected token '{t.Value}' in query.");
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
            interpreter.Print(interpreter.Engine.Dictionary?.Query(interpreter.RNG, query, interpreter.QueryState) ?? RantDictionaryTable.MissingTerm);

            return false;
        }

        private static bool DoText(VM interpreter, Token<R> firstToken, PatternReader reader, RantState state)
        {
            interpreter.Print(firstToken.Value);
            return false;
        }

        private static bool DoConstant(VM interpreter, Token<R> firstToken, PatternReader reader, RantState state)
        {
            interpreter.Print(UnescapeConstantLiteral(firstToken.Value));
            return false;
        }

        private static bool DoEscape(VM interpreter, Token<R> firstToken, PatternReader reader, RantState state)
        {
            interpreter.Print(Unescape(firstToken.Value, interpreter, interpreter.RNG));
            return false;
        }

        private static bool DoTag(VM interpreter, Token<R> firstToken, PatternReader reader, RantState state)
        {
            var name = reader.ReadToken();

            switch (name.ID)
            {	
                case R.Question: // Metapattern
                    state.Pre(new MetapatternBlueprint(interpreter));
                    interpreter.PushState(RantState.CreateSub(reader.Source, reader.ReadToScopeClose(R.LeftSquare, R.RightSquare, Delimiters.All), interpreter));
                    return true;
                case R.Regex: // Replacer
                    return DoReplacer(name, interpreter, reader, state);
                case R.Dollar: // Subroutine
                    return reader.IsNext(R.Text) ? DoSubCall(name, interpreter, reader, state) : DoSubDefinition(name, interpreter, reader, state);
            }

            if (!ValidateName(name.Value.Trim()))
                throw new RantException(reader.Source, name, $"Invalid tag name '{name.Value}'");

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

        
        private static bool DoSubCall(Token<R> first, VM interpreter, PatternReader reader, RantState state)
        {
            var name = reader.ReadToken();
            if (!ValidateName(name.Value))
                throw new RantException(reader.Source, name, $"Invalid subroutine name '{name.Value}'");
            
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
                    throw new RantException(reader.Source, name, $"No subroutine was found with the name '{name.Value}' and 0 parameters.");
            }
            else
            {
                args = reader.ReadMultiItemScope(R.LeftSquare, R.RightSquare, R.Semicolon,
                    Delimiters.All).ToArray();
                if((sub = interpreter.Engine.Subroutines.Get(name.Value, args.Length)) == null)
                    throw new RantException(reader.Source, name, $"No subroutine was found with the name '{name.Value}' and {args.Length} parameter{(args.Length != 1 ? "s" : "")}.");
            }

            state.Pre(new SubCallBlueprint(interpreter, reader.Source, sub, args));

            return true;
        }

        
        private static bool DoSubDefinition(Token<R> first, VM interpreter, PatternReader reader, RantState state)
        {
            bool meta = reader.Take(R.Question);
            reader.Read(R.LeftSquare);

            var parameters = new List<_<string, ParamFlags>>();
            var tName = reader.Read(R.Text, "subroutine name");

            if (!ValidateName(tName.Value))
                throw new RantException(reader.Source, tName, $"Invalid subroutine name: '{tName.Value}'");
            
            if (!reader.Take(R.Colon))
            {
                reader.Read(R.RightSquare);
            }
            else
            {
                while (true)
                {
                    bool isTokens = reader.Take(R.At);
                    parameters.Add(_.Create(reader.Read(R.Text, "parameter name").Value, isTokens ? ParamFlags.Code : ParamFlags.None));
                    if (reader.Take(R.RightSquare, false)) break;
                    reader.Read(R.Semicolon);
                }
            }

            reader.SkipSpace();
            reader.Read(R.Colon);

            var body = reader.ReadToScopeClose(R.LeftSquare, R.RightSquare, Delimiters.All).ToArray();

            if (meta)
            {
                interpreter.PushState(RantState.CreateSub(reader.Source, body, interpreter));
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

        
        private static bool DoReplacer(Token<R> name, VM interpreter, PatternReader reader, RantState state)
        {
            reader.Read(R.Colon);

            var args = reader.ReadMultiItemScope(R.LeftSquare, R.RightSquare, R.Semicolon, Delimiters.All).ToArray();
            if (args.Length != 2) throw new RantException(reader.Source, name, $"Replacer expected two arguments, but got {args.Length}.");

            state.Pre(new ReplacerBlueprint(interpreter, ParseRegex(name.Value), args[1]));

            interpreter.PushState(RantState.CreateSub(reader.Source, args[0], interpreter));
            return true;
        }

        private static bool DoBlock(VM interpreter, Token<R> firstToken, PatternReader reader, RantState state)
        {
            var attribs = interpreter.NextBlockAttribs;
            interpreter.NextBlockAttribs = new BlockAttribs();

            _<Block, int> blockInfo;

            // Check if the block is already cached
            if (!reader.Source.TryGetCachedBlock(firstToken, out blockInfo))
            {
                var elements = reader.ReadMultiItemScope(R.LeftCurly, R.RightCurly, R.Pipe, Delimiters.All).ToArray();
                blockInfo = _.Create(Block.Create(elements), reader.Position);
                reader.Source.CacheBlock(firstToken, blockInfo);
            }
            else
            {
                // If the block is cached, seek to its end
                reader.Position = blockInfo.Item2;
            }

            if (!blockInfo.Item1.Items.Any() || blockInfo.Item1.WeightTotal == 0 || !interpreter.TakeChance()) return false;

            var rep = new Repeater(blockInfo.Item1, attribs);
            interpreter.Objects.EnterScope();
            interpreter.PushRepeater(rep);
            interpreter.BaseStates.Add(state);
            state.Pre(new RepeaterBlueprint(interpreter, rep));
            return true;
        }
    }
}