using System;
using System.Collections.Generic;
using System.Linq;

using Manhood.Blueprints;
using Manhood.Compiler;

using Stringes;

namespace Manhood
{
    // The return value is returned by the blueprint that executes the tag. If it's true, the interpreter will skip to the top of the state stack.
    internal delegate bool TagFunc(Interpreter interpreter, Source source, Stringe tagName, TagArg[] args);

    internal partial class Interpreter
    {
        internal static readonly Dictionary<string, TagDef> TagFuncs;

        static Interpreter()
        {
            TagFuncs = new Dictionary<string, TagDef>();

            TagFuncs["rep"] = TagFuncs["r"] = new TagDef(Repeat, TagArgType.Result);
            TagFuncs["num"] = TagFuncs["n"] = new TagDef(Number, TagArgType.Result, TagArgType.Result);
            TagFuncs["sep"] = TagFuncs["s"] = new TagDef(Separator, TagArgType.Tokens);
            TagFuncs["before"] = new TagDef(Before, TagArgType.Tokens);
            TagFuncs["after"] = new TagDef(After, TagArgType.Tokens);
            TagFuncs["chance"] = new TagDef(Chance, TagArgType.Result);
            TagFuncs["sync"] = new TagDef(Sync, TagArgType.Result, TagArgType.Result);
            TagFuncs["desync"] = new TagDef(Desync);
            TagFuncs["pin"] = new TagDef(Pin, TagArgType.Result);
            TagFuncs["unpin"] = new TagDef(Unpin, TagArgType.Result);
            TagFuncs["step"] = new TagDef(Step, TagArgType.Result);
            TagFuncs["first"] = new TagDef(First, TagArgType.Tokens);
            TagFuncs["last"] = new TagDef(Last, TagArgType.Tokens);
            TagFuncs["middle"] = new TagDef(Middle, TagArgType.Tokens);
            TagFuncs["notfirst"] = new TagDef(NotFirst, TagArgType.Tokens);
            TagFuncs["notlast"] = new TagDef(NotLast, TagArgType.Tokens);
            TagFuncs["notmiddle"] = new TagDef(NotMiddle, TagArgType.Tokens);
            TagFuncs["odd"] = new TagDef(Odd, TagArgType.Tokens);
            TagFuncs["even"] = new TagDef(Even, TagArgType.Tokens);
            TagFuncs["nth"] = new TagDef(Nth, TagArgType.Result, TagArgType.Result, TagArgType.Tokens);
            TagFuncs["repnum"] = TagFuncs["rn"] = new TagDef(RepNum);
            TagFuncs["repindex"] = TagFuncs["ri"] = new TagDef(RepIndex);
            TagFuncs["repcount"] = TagFuncs["rc"] = new TagDef(RepCount);
            TagFuncs["alt"] = new TagDef(Alt, TagArgType.Tokens, TagArgType.Tokens);
            TagFuncs["match"] = new TagDef(ReplaceMatch);
            TagFuncs["group"] = new TagDef(ReplaceGroup, TagArgType.Result);
            TagFuncs["arg"] = new TagDef(Arg, TagArgType.Result);
            TagFuncs["numfmt"] = new TagDef(NumFmt, TagArgType.Result);
            TagFuncs["caps"] = new TagDef(Caps, TagArgType.Result);
        }

        private static bool Caps(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            Capitalization caps;
            var caps_str = args[0].GetString();
            if (!Enum.TryParse(Util.NameToCamel(caps_str), out caps))
            {
                throw new ManhoodException(source, tagname, "Invalid capitalization format '" + caps_str + "'");
            }
            interpreter.CurrentState.Output.SetCaps(caps);
            return false;
        }

        private static bool NumFmt(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            NumberFormat fmt;
            var fmtstr = args[0].GetString();
            if (!Enum.TryParse(Util.NameToCamel(fmtstr), out fmt))
            {
                throw new ManhoodException(source, tagname, "Invalid number format '" + fmtstr + "'");
            }
            interpreter.NumberFormat = fmt;
            return false;
        }

        private static bool Arg(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            if (!interpreter.SubArgStack.Any())
                throw new ManhoodException(source, tagname, "Tried to access arguments outside of a subroutine body.");

            TagArg arg;
            var argName = args[0].GetString().Trim();
            if (!interpreter.SubArgStack.Peek().TryGetValue(argName, out arg))
                throw new ManhoodException(source, tagname, "Could not find argument '" + argName + "'.");

            // Argument is string
            if (arg.Type == TagArgType.Result)
            {
                interpreter.Print(arg.GetString());
                return false;
            }
            
            // Argument is tokens
            interpreter.PushState(State.CreateDerivedShared(source, arg.GetTokens(), interpreter));
            return true;
        }

        private static bool ReplaceGroup(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            interpreter.Print(interpreter.GetMatchString(args[0].GetString()));
            return false;
        }

        private static bool ReplaceMatch(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            interpreter.Print(interpreter.GetMatchString());
            return false;
        }

        private static bool Alt(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            var testState = State.CreateDerivedDistinct(source, args[0].GetTokens(), interpreter,
                interpreter.CurrentState.Output);
            testState.AddPostBlueprint(new AltBlueprint(interpreter, testState, args[1].GetTokens()));
            interpreter.PushState(testState);
            return true;
        }

        private static bool RepCount(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            if (interpreter.CurrentRepeater == null) throw new ManhoodException(source, tagname, "No active repeater.");
            interpreter.Print(interpreter.FormatNumber(interpreter.CurrentRepeater.Count));
            return false;
        }

        private static bool RepIndex(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            if (interpreter.CurrentRepeater == null) throw new ManhoodException(source, tagname, "No active repeaters.");
            interpreter.Print(interpreter.FormatNumber(interpreter.CurrentRepeater.Index));
            return false;
        }

        private static bool RepNum(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            if (interpreter.CurrentRepeater == null) throw new ManhoodException(source, tagname, "No active repeaters.");
            interpreter.Print(interpreter.FormatNumber(interpreter.CurrentRepeater.Index + 1));
            return false;
        }

        private static bool Nth(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            int offset, interval;
            if (!Int32.TryParse(args[0].GetString(), out interval))
            {
                throw new ManhoodException(source, tagname, "Invalid interval value.");
            }

            if (interval <= 0)
            {
                throw new ManhoodException(source, tagname, "Interval must be greater than zero.");
            }

            if (!Int32.TryParse(args[1].GetString(), out offset))
            {
                throw new ManhoodException(source, tagname, "Invalid offset value.");
            }

            if (interpreter.CurrentRepeater == null || !interpreter.CurrentRepeater.Nth(offset, interval)) return false;
            interpreter.PushState(State.CreateDerivedDistinct(source, args[2].GetTokens(), interpreter, interpreter.CurrentState.Output));
            return true;
        }

        private static bool Even(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            if (interpreter.CurrentRepeater == null || !interpreter.CurrentRepeater.IsEven) return false;
            interpreter.PushState(State.CreateDerivedDistinct(source, args[0].GetTokens(), interpreter, interpreter.CurrentState.Output));
            return true;
        }

        private static bool Odd(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            if (interpreter.CurrentRepeater == null || !interpreter.CurrentRepeater.IsOdd) return false;
            interpreter.PushState(State.CreateDerivedDistinct(source, args[0].GetTokens(), interpreter, interpreter.CurrentState.Output));
            return true;
        }

        private static bool NotMiddle(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            if (interpreter.CurrentRepeater == null || interpreter.CurrentRepeater.IsFirst || interpreter.CurrentRepeater.IsLast) return false;
            interpreter.PushState(State.CreateDerivedDistinct(source, args[0].GetTokens(), interpreter, interpreter.CurrentState.Output));
            return true;
        }

        private static bool NotLast(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            if (interpreter.CurrentRepeater == null || interpreter.CurrentRepeater.IsLast) return false;
            interpreter.PushState(State.CreateDerivedDistinct(source, args[0].GetTokens(), interpreter, interpreter.CurrentState.Output));
            return true;
        }

        private static bool NotFirst(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            if (interpreter.CurrentRepeater == null || interpreter.CurrentRepeater.IsFirst) return false;
            interpreter.PushState(State.CreateDerivedDistinct(source, args[0].GetTokens(), interpreter, interpreter.CurrentState.Output));
            return true;
        }

        private static bool Middle(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            if (interpreter.CurrentRepeater == null || interpreter.CurrentRepeater.IsLast || interpreter.CurrentRepeater.IsFirst) return false;
            interpreter.PushState(State.CreateDerivedDistinct(source, args[0].GetTokens(), interpreter, interpreter.CurrentState.Output));
            return true;
        }

        private static bool Last(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            if (interpreter.CurrentRepeater == null || !interpreter.CurrentRepeater.IsLast) return false;
            interpreter.PushState(State.CreateDerivedDistinct(source, args[0].GetTokens(), interpreter, interpreter.CurrentState.Output));
            return true;
        }

        private static bool First(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            if (interpreter.CurrentRepeater == null || !interpreter.CurrentRepeater.IsFirst) return false;
            interpreter.PushState(State.CreateDerivedDistinct(source, args[0].GetTokens(), interpreter, interpreter.CurrentState.Output));
            return true;
        }

        private static bool Step(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            interpreter.Step(args[0].GetString());
            return false;
        }

        private static bool Unpin(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            interpreter.Unpin(args[0].GetString());
            return false;
        }

        private static bool Pin(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            interpreter.Pin(args[0].GetString());
            return false;
        }

        private static bool Desync(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            interpreter.Desync();
            return false;
        }

        private static bool Sync(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            var typeStr = args[1].GetString();
            SyncType type;
            if (!Enum.TryParse(typeStr, true, out type))
            {
                throw new ManhoodException(source, tagname, "Invalid synchronizer type: '" + typeStr + "'");
            }
            interpreter.Sync(args[0].GetString(), type);
            return false;
        }

        private static bool Chance(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            int a;
            if (!Int32.TryParse(args[0].GetString(), out a))
            {
                throw new ManhoodException(source, tagname, "Invalid chance number.");
            }
            interpreter.SetChance(a);
            return false;
        }

        private static bool After(Interpreter interpreter, Source source, Stringe tagname, TagArg[] args)
        {
            interpreter.NextAttribs.After = args[0].GetTokens();
            return false;
        }

        private static bool Before(Interpreter interpreter, Source source, Stringe tagName, TagArg[] args)
        {
            interpreter.NextAttribs.Before = args[0].GetTokens();
            return false;
        }

        private static bool Number(Interpreter interpreter, Source source, Stringe tagName, TagArg[] args)
        {
            int a, b;
            if (!Int32.TryParse(args[0].GetString(), out a) || !Int32.TryParse(args[1].GetString(), out b))
            {
                throw new ManhoodException(source, tagName, "Range values could not be parsed. They must be numbers.");
            }
            interpreter.Print(interpreter.FormatNumber(interpreter.RNG.Next(a, b + 1)));
            return false;
        }

        private static bool Separator(Interpreter interpreter, Source source, Stringe tagName, TagArg[] args)
        {
            interpreter.NextAttribs.Separator = args[0].GetTokens();
            return false;
        }

        private static bool Repeat(Interpreter interpreter, Source source, Stringe tagName, TagArg[] args)
        {
            var reps = args[0].GetString().ToLower().Trim();
            if (reps == "each")
            {
                interpreter.NextAttribs.Repetitons = Repeater.Each;
                return false;
            }

            int num;
            if (!Int32.TryParse(reps, out num))
            {
                throw new ManhoodException(source, tagName, "Invalid repetition value '" + reps + "' - must be a number.");
            }
            if (num < 0)
            {
                throw new ManhoodException(source, tagName, "Repetition value cannot be negative.");
            }

            interpreter.NextAttribs.Repetitons = num;
            return false;
        }

        internal class TagDef
        {
            private readonly int _paramCount;
            private readonly TagArgType[] _argTypes;
            private readonly TagFunc _func;

            public TagArgType[] ArgTypes
            {
                get { return _argTypes; }
            }

            public TagDef(TagFunc func, params TagArgType[] argTypes)
            {
                _paramCount = argTypes.Length;
                _argTypes = argTypes;
                _func = func;
            }

            public void ValidateArgCount(Source source, Stringe tagName, int count)
            {
                if (count != _paramCount)
                    throw new ManhoodException(source, tagName, "Tag '" + tagName.Value + "' expected " + _paramCount + " " + (_paramCount == 1 ? "argument" : "arguments") + " but got " + count + ".");
            }

            public bool Invoke(Interpreter interpreter, Source source, Stringe tagName, TagArg[] args)
            {
                return _func(interpreter, source, tagName, args);
            }
        }
    }
}