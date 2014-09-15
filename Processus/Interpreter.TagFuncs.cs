using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Processus.Blueprints;
using Processus.Compiler;

using Stringes;

namespace Processus
{
    // The return value is returned by the blueprint that executes the tag. If it's true, the interpreter will skip to the top of the state stack.
    internal delegate bool FuncTagCallback(Interpreter interpreter, Source source, Stringe tagName, Argument[] args);

    internal partial class Interpreter
    {
        internal static readonly Dictionary<string, FuncTagDef> TagFuncs;

        static Interpreter()
        {
            TagFuncs = new Dictionary<string, FuncTagDef>();

            TagFuncs["rep"] = TagFuncs["r"] = new FuncTagSig(Repeat, ParamFlags.None);
            TagFuncs["num"] = TagFuncs["n"] = new FuncTagSig(Number, ParamFlags.None, ParamFlags.None);
            TagFuncs["sep"] = TagFuncs["s"] = new FuncTagSig(Separator, ParamFlags.Code);
            TagFuncs["before"] = new FuncTagSig(Before, ParamFlags.Code);
            TagFuncs["after"] = new FuncTagSig(After, ParamFlags.Code);
            TagFuncs["chance"] = new FuncTagSig(Chance, ParamFlags.None);
            TagFuncs["sync"] = new FuncTagSig(Sync, ParamFlags.None, ParamFlags.None);
            TagFuncs["desync"] = new FuncTagSig(Desync);
            TagFuncs["pin"] = new FuncTagSig(Pin, ParamFlags.None);
            TagFuncs["unpin"] = new FuncTagSig(Unpin, ParamFlags.None);
            TagFuncs["step"] = new FuncTagSig(Step, ParamFlags.None);
            TagFuncs["reset"] = new FuncTagSig(Reset, ParamFlags.None);
            TagFuncs["first"] = new FuncTagSig(First, ParamFlags.Code);
            TagFuncs["last"] = new FuncTagSig(Last, ParamFlags.Code);
            TagFuncs["middle"] = new FuncTagSig(Middle, ParamFlags.Code);
            TagFuncs["notfirst"] = new FuncTagSig(NotFirst, ParamFlags.Code);
            TagFuncs["notlast"] = new FuncTagSig(NotLast, ParamFlags.Code);
            TagFuncs["notmiddle"] = new FuncTagSig(NotMiddle, ParamFlags.Code);
            TagFuncs["odd"] = new FuncTagSig(Odd, ParamFlags.Code);
            TagFuncs["even"] = new FuncTagSig(Even, ParamFlags.Code);
            TagFuncs["nth"] = new FuncTagSig(Nth, ParamFlags.None, ParamFlags.None, ParamFlags.Code);
            TagFuncs["repnum"] = TagFuncs["rn"] = new FuncTagSig(RepNum);
            TagFuncs["repindex"] = TagFuncs["ri"] = new FuncTagSig(RepIndex);
            TagFuncs["repcount"] = TagFuncs["rc"] = new FuncTagSig(RepCount);
            TagFuncs["alt"] = new FuncTagSig(Alt, ParamFlags.Code, ParamFlags.Code);
            TagFuncs["match"] = new FuncTagSig(ReplaceMatch);
            TagFuncs["group"] = new FuncTagSig(ReplaceGroup, ParamFlags.None);
            TagFuncs["arg"] = new FuncTagSig(Arg, ParamFlags.None);
            TagFuncs["numfmt"] = new FuncTagSig(NumFmt, ParamFlags.None);
            TagFuncs["caps"] = new FuncTagSig(Caps, ParamFlags.None);
            TagFuncs["capsinfer"] = new FuncTagSig(CapsInfer, ParamFlags.None);
            TagFuncs["out"] = new FuncTagSig(Out, ParamFlags.None, ParamFlags.None);
            TagFuncs["close"] = new FuncTagSig(Close, ParamFlags.None);
            TagFuncs["extern"] = new FuncTagSig(Extern, ParamFlags.None);
            TagFuncs["mark"] = new FuncTagSig(Mark, ParamFlags.None);
            TagFuncs["dist"] = new FuncTagSig(Dist, ParamFlags.None, ParamFlags.None);
            TagFuncs["take"] = new FuncTagSig(Take, ParamFlags.None);
            TagFuncs["send"] = new FuncTagSig(Send, ParamFlags.None, ParamFlags.None);
            TagFuncs["len"] = new FuncTagSig(Length, ParamFlags.None);
        }

        private static bool Length(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            interpreter.Print(args[0].GetString().Length);
            return false;
        }

        private static bool Send(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            interpreter.CurrentState.Output.WriteToPoint(args[0], args[1]);
            return false;
        }

        private static bool Take(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            interpreter.CurrentState.Output.SetWritePoint(args[0]);
            return false;
        }

        private static bool Dist(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            interpreter.Print(interpreter.GetMarkerDistance(args[0], args[1]));
            return false;
        }

        private static bool Mark(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            interpreter.SetMarker(args[0]);
            return false;
        }

        private static bool Extern(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            var name = args[0].GetString();
            var result = interpreter.Engine.Hooks.Call(name);
            if (result == null)
            {
                throw new ProcessusException(source, tagname, "A hook with the name '" + name + "' does not exist.");
            }
            interpreter.Print(result);
            return false;
        }

        private static bool Reset(Interpreter interpreter, Source source, Stringe tagName, Argument[] args)
        {
            interpreter.Reset(args[0].GetString());
            return false;
        }

        private static bool CapsInfer(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            // TODO: Make capsinfer properly infer "first" capitalization given multiple sentences. Currently, it mistakes it for "word" mode.
            var words = Regex.Matches(args[0].GetString(), @"\w+").OfType<Match>().Select(m => m.Value).ToArray();
            int wCount = words.Length;
            int uCount = 0;
            int fwCount = 0;
            bool firstCharIsUpper = false;
            for (int i = 0; i < wCount; i++)
            {
                if (words[i].All(Char.IsUpper))
                {
                    uCount++;
                }
                if (Char.IsUpper(words[i][0]))
                {
                    fwCount++;
                    if (i == 0) firstCharIsUpper = true;
                }
            }
            if (uCount == wCount)
            {
                interpreter.CurrentState.Output.SetCaps(Capitalization.Upper);
            }
            else if (wCount > 1 && fwCount == wCount)
            {
                interpreter.CurrentState.Output.SetCaps(Capitalization.Word);
            }
            else if (firstCharIsUpper)
            {
                interpreter.CurrentState.Output.SetCaps(Capitalization.First);
            }
            return false;
        }

        private static bool Close(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            interpreter.CurrentState.Output.PopChannel(args[0].GetString());
            return false;
        }

        private static bool Out(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            ChannelVisibility cv;
            var cv_str = args[1].GetString();
            if (!Enum.TryParse(Util.NameToCamel(cv_str), out cv))
            {
                throw new ProcessusException(source, tagname, "Invalid channel visibility option '" + cv_str + "'");
            }
            interpreter.CurrentState.Output.PushChannel(args[0].GetString(), cv);
            return false;
        }

        private static bool Caps(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            Capitalization caps;
            var caps_str = args[0].GetString();
            if (!Enum.TryParse(Util.NameToCamel(caps_str), out caps))
            {
                throw new ProcessusException(source, tagname, "Invalid capitalization format '" + caps_str + "'");
            }
            interpreter.CurrentState.Output.SetCaps(caps);
            return false;
        }

        private static bool NumFmt(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            NumberFormat fmt;
            var fmtstr = args[0].GetString();
            if (!Enum.TryParse(Util.NameToCamel(fmtstr), out fmt))
            {
                throw new ProcessusException(source, tagname, "Invalid number format '" + fmtstr + "'");
            }
            interpreter.NumberFormat = fmt;
            return false;
        }

        private static bool Arg(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            if (!interpreter.SubArgStack.Any())
                throw new ProcessusException(source, tagname, "Tried to access arguments outside of a subroutine body.");

            Argument arg;
            var argName = args[0].GetString().Trim();
            if (!interpreter.SubArgStack.Peek().TryGetValue(argName, out arg))
                throw new ProcessusException(source, tagname, "Could not find argument '" + argName + "'.");

            // Argument is string
            if (arg.Flags == ParamFlags.None)
            {
                interpreter.Print(arg.GetString());
                return false;
            }
            
            // Argument is tokens
            interpreter.PushState(State.CreateDerivedShared(source, arg.GetTokens(), interpreter));
            return true;
        }

        private static bool ReplaceGroup(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            interpreter.Print(interpreter.GetMatchString(args[0].GetString()));
            return false;
        }

        private static bool ReplaceMatch(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            interpreter.Print(interpreter.GetMatchString());
            return false;
        }

        private static bool Alt(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            var testState = State.CreateDerivedDistinct(source, args[0].GetTokens(), interpreter,
                interpreter.CurrentState.Output);
            testState.AddPostBlueprint(new AltBlueprint(interpreter, testState, args[1].GetTokens()));
            interpreter.PushState(testState);
            return true;
        }

        private static bool RepCount(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            if (interpreter.CurrentRepeater == null) throw new ProcessusException(source, tagname, "No active repeater.");
            interpreter.Print(interpreter.FormatNumber(interpreter.CurrentRepeater.Count));
            return false;
        }

        private static bool RepIndex(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            if (interpreter.CurrentRepeater == null) throw new ProcessusException(source, tagname, "No active repeaters.");
            interpreter.Print(interpreter.FormatNumber(interpreter.CurrentRepeater.Index));
            return false;
        }

        private static bool RepNum(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            if (interpreter.CurrentRepeater == null) throw new ProcessusException(source, tagname, "No active repeaters.");
            interpreter.Print(interpreter.FormatNumber(interpreter.CurrentRepeater.Index + 1));
            return false;
        }

        private static bool Nth(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            int offset, interval;
            if (!Int32.TryParse(args[0].GetString(), out interval))
            {
                throw new ProcessusException(source, tagname, "Invalid interval value.");
            }

            if (interval <= 0)
            {
                throw new ProcessusException(source, tagname, "Interval must be greater than zero.");
            }

            if (!Int32.TryParse(args[1].GetString(), out offset))
            {
                throw new ProcessusException(source, tagname, "Invalid offset value.");
            }

            if (interpreter.CurrentRepeater == null || !interpreter.CurrentRepeater.Nth(offset, interval)) return false;
            interpreter.PushState(State.CreateDerivedDistinct(source, args[2].GetTokens(), interpreter, interpreter.CurrentState.Output));
            return true;
        }

        private static bool Even(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            if (interpreter.CurrentRepeater == null || !interpreter.CurrentRepeater.IsEven) return false;
            interpreter.PushState(State.CreateDerivedDistinct(source, args[0].GetTokens(), interpreter, interpreter.CurrentState.Output));
            return true;
        }

        private static bool Odd(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            if (interpreter.CurrentRepeater == null || !interpreter.CurrentRepeater.IsOdd) return false;
            interpreter.PushState(State.CreateDerivedDistinct(source, args[0].GetTokens(), interpreter, interpreter.CurrentState.Output));
            return true;
        }

        private static bool NotMiddle(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            if (interpreter.CurrentRepeater == null || interpreter.CurrentRepeater.IsFirst || interpreter.CurrentRepeater.IsLast) return false;
            interpreter.PushState(State.CreateDerivedDistinct(source, args[0].GetTokens(), interpreter, interpreter.CurrentState.Output));
            return true;
        }

        private static bool NotLast(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            if (interpreter.CurrentRepeater == null || interpreter.CurrentRepeater.IsLast) return false;
            interpreter.PushState(State.CreateDerivedDistinct(source, args[0].GetTokens(), interpreter, interpreter.CurrentState.Output));
            return true;
        }

        private static bool NotFirst(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            if (interpreter.CurrentRepeater == null || interpreter.CurrentRepeater.IsFirst) return false;
            interpreter.PushState(State.CreateDerivedDistinct(source, args[0].GetTokens(), interpreter, interpreter.CurrentState.Output));
            return true;
        }

        private static bool Middle(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            if (interpreter.CurrentRepeater == null || interpreter.CurrentRepeater.IsLast || interpreter.CurrentRepeater.IsFirst) return false;
            interpreter.PushState(State.CreateDerivedDistinct(source, args[0].GetTokens(), interpreter, interpreter.CurrentState.Output));
            return true;
        }

        private static bool Last(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            if (interpreter.CurrentRepeater == null || !interpreter.CurrentRepeater.IsLast) return false;
            interpreter.PushState(State.CreateDerivedDistinct(source, args[0].GetTokens(), interpreter, interpreter.CurrentState.Output));
            return true;
        }

        private static bool First(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            if (interpreter.CurrentRepeater == null || !interpreter.CurrentRepeater.IsFirst) return false;
            interpreter.PushState(State.CreateDerivedDistinct(source, args[0].GetTokens(), interpreter, interpreter.CurrentState.Output));
            return true;
        }

        private static bool Step(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            interpreter.Step(args[0].GetString());
            return false;
        }

        private static bool Unpin(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            interpreter.Unpin(args[0].GetString());
            return false;
        }

        private static bool Pin(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            interpreter.Pin(args[0].GetString());
            return false;
        }

        private static bool Desync(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            interpreter.Desync();
            return false;
        }

        private static bool Sync(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            var typeStr = args[1].GetString();
            SyncType type;
            if (!Enum.TryParse(typeStr, true, out type))
            {
                throw new ProcessusException(source, tagname, "Invalid synchronizer type: '" + typeStr + "'");
            }
            interpreter.Sync(args[0].GetString(), type);
            return false;
        }

        private static bool Chance(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            int a;
            if (!Int32.TryParse(args[0].GetString(), out a))
            {
                throw new ProcessusException(source, tagname, "Invalid chance number.");
            }
            interpreter.SetChance(a);
            return false;
        }

        private static bool After(Interpreter interpreter, Source source, Stringe tagname, Argument[] args)
        {
            interpreter.NextAttribs.After = args[0].GetTokens();
            return false;
        }

        private static bool Before(Interpreter interpreter, Source source, Stringe tagName, Argument[] args)
        {
            interpreter.NextAttribs.Before = args[0].GetTokens();
            return false;
        }

        private static bool Number(Interpreter interpreter, Source source, Stringe tagName, Argument[] args)
        {
            int a, b;
            if (!Int32.TryParse(args[0].GetString(), out a) || !Int32.TryParse(args[1].GetString(), out b))
            {
                throw new ProcessusException(source, tagName, "Range values could not be parsed. They must be numbers.");
            }
            interpreter.Print(interpreter.FormatNumber(interpreter.RNG.Next(a, b + 1)));
            return false;
        }

        private static bool Separator(Interpreter interpreter, Source source, Stringe tagName, Argument[] args)
        {
            interpreter.NextAttribs.Separator = args[0].GetTokens();
            return false;
        }

        private static bool Repeat(Interpreter interpreter, Source source, Stringe tagName, Argument[] args)
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
                throw new ProcessusException(source, tagName, "Invalid repetition value '" + reps + "' - must be a number.");
            }
            if (num < 0)
            {
                throw new ProcessusException(source, tagName, "Repetition value cannot be negative.");
            }

            interpreter.NextAttribs.Repetitons = num;
            return false;
        }

        internal class FuncTagDef
        {
            private readonly IEnumerable<FuncTagSig> _defs; 

            public FuncTagDef(params FuncTagSig[] defs)
            {
                _defs = defs.OrderByDescending(d => d.MinArgCount).ThenBy(d => d.ParamCount);
            }

            public FuncTagSig GetDef(int paramCount)
            {
                return _defs.FirstOrDefault(d => d.HasMultiFlag ? paramCount >= d.MinArgCount : paramCount == d.ParamCount);
            }

            public static implicit operator FuncTagDef(FuncTagSig def)
            {
                return new FuncTagDef(def);
            }
        }

        internal class FuncTagSig
        {
            private readonly int _paramCount;
            private readonly int _minArgCount;
            private readonly bool _hasMultiFlag;
            private readonly ParamFlags[] _parameters;
            private readonly FuncTagCallback _func;
            
            public ParamFlags[] Parameters
            {
                get { return _parameters; }
            }

            public int ParamCount
            {
                get { return _paramCount; }
            }

            public int MinArgCount
            {
                get { return _minArgCount; }
            }

            public bool HasMultiFlag
            {
                get { return _hasMultiFlag; }
            }

            public FuncTagSig(FuncTagCallback func, params ParamFlags[] parameters)
            {
                if (parameters.Where((t, i) => i < parameters.Length - 1 && t.HasFlag(ParamFlags.Multi)).Any())
                    throw new ArgumentException("The flag 'ParamType.Multi' is only valid on the last parameter.");

                _parameters = parameters;
                _hasMultiFlag = parameters.Any() && parameters.Last().HasFlag(ParamFlags.Multi);
                _paramCount = parameters.Length;
                _minArgCount = _hasMultiFlag ? _paramCount - 1 : _paramCount;
                _func = func;
            }

            public bool Invoke(Interpreter interpreter, Source source, Stringe tagName, Argument[] args)
            {
                return _func(interpreter, source, tagName, args);
            }
        }
    }
}