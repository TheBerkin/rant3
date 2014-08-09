using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using E = Manhood.ManhoodException;

namespace Manhood
{
    internal partial class Interpreter
    {
        private static readonly Dictionary<string, Func<Interpreter, string[], bool>> TagFuncs = new Dictionary<string, Func<Interpreter, string[], bool>>();
        private static bool _funcsLoaded = false;

        public static void InitTagFuncs()
        {
            if (_funcsLoaded) return;
            _funcsLoaded = true;

            TagFuncs["caps"] = Caps;
            TagFuncs["chance"] = TagFuncs["c"] = Chance;
            TagFuncs["rep"] = TagFuncs["r"] = Rep;
            TagFuncs["repindex"] = TagFuncs["ri"] = RepIndex;
            TagFuncs["repnum"] = TagFuncs["rn"] = RepNumber;
            TagFuncs["reprem"] = TagFuncs["rr"] = RepRemaining;
            TagFuncs["repdepth"] = TagFuncs["rd"] = RepDepth;
            TagFuncs["repcount"] = TagFuncs["rc"] = RepCount;
            TagFuncs["first"] = RepFirst;
            TagFuncs["notfirst"] = RepNotFirst;
            TagFuncs["notlast"] = RepNotLast;
            TagFuncs["notmiddle"] = RepNotMiddle;
            TagFuncs["middle"] = RepMiddle;
            TagFuncs["last"] = RepLast;
            TagFuncs["odd"] = RepOdd;
            TagFuncs["even"] = RepEven;
            TagFuncs["before"] = Before;
            TagFuncs["after"] = After;
            TagFuncs["br"] = Branch;
            TagFuncs["ub"] = Unbranch;
            TagFuncs["num"] = TagFuncs["n"] = Number;
            TagFuncs["sep"] = TagFuncs["s"] = Separate;
            TagFuncs["out"] = OutChannel;
            TagFuncs["return"] = Return;
            TagFuncs["sync"] = Sync;
            TagFuncs["reseed"] = Reseed;
            TagFuncs["reset"] = Reset;
            TagFuncs["dump"] = Dump;
            TagFuncs["sub"] = Define;
            TagFuncs["call"] = Call;
            TagFuncs["icall"] = CallInterp;
            TagFuncs["isub"] = DefineInterp;
            TagFuncs["pin"] = Pin;
            TagFuncs["unpin"] = Unpin;
            TagFuncs["step"] = Step;
            TagFuncs["desync"] = Desync;
            TagFuncs["fuse"] = Fuse;
            TagFuncs["arg"] = Arg;
            TagFuncs["evalarg"] = EvalArg;
            TagFuncs["replace"] = Replace;
            TagFuncs["match"] = Match;
            TagFuncs["cut"] = Cut;
            TagFuncs["string"] = RequestString;
            TagFuncs["pick"] = Pick;
            TagFuncs["to"] = To;
            TagFuncs["past"] = Past;
            TagFuncs["alt"] = Alt;
            TagFuncs["any"] = Any;
            TagFuncs["up"] = Up;

            TagFuncs["define"] = DefineFlag;
            TagFuncs["undef"] = UndefineFlag;
            TagFuncs["ifdef"] = IfFlagDefined;
            TagFuncs["ifndef"] = IfFlagUndefined;
            TagFuncs["else"] = Else;
        }

        private static bool Else(Interpreter ii, string[] args)
        {
            E.CheckArgs("else", args, 1);
            if (!ii.State.ElseClause) return true;
            ii.State.ElseClause = false;
            ii.Do(args[0]);
            return true;
        }

        private static bool IfFlagUndefined(Interpreter ii, string[] args)
        {
            E.CheckArgs("ifndef", args, 2);
            var name = ii.Evaluate(args[0]).Trim().ToLower();
            if (!Util.ValidateName(name)) E.Throw(ii, "Invalid tag name: " + name);
            if (ii.State.ElseClause = ii.State.Flags.Contains(name))
            {
                return true;
            }
            ii.Do(args[1]);
            return true;
        }

        private static bool IfFlagDefined(Interpreter ii, string[] args)
        {
            E.CheckArgs("ifdef", args, 2);
            var name = ii.Evaluate(args[0]).Trim().ToLower();
            if (!Util.ValidateName(name)) E.Throw(ii, "Invalid tag name: " + name);
            if (ii.State.ElseClause = !ii.State.Flags.Contains(name))
            {
                return true;
            }
            ii.Do(args[1]);
            return true;
        }

        private static bool UndefineFlag(Interpreter ii, string[] args)
        {
            E.CheckArgs("undef", args, 1);
            var name = ii.Evaluate(args[0]).Trim().ToLower();
            if (!Util.ValidateName(name)) E.Throw(ii, "Invalid tag name: " + name);
            ii.State.Flags.Remove(name);
            return true;
        }

        private static bool DefineFlag(Interpreter ii, string[] args)
        {
            E.CheckArgs("define", args, 1);
            var name = ii.Evaluate(args[0]).Trim().ToLower();
            if (!Util.ValidateName(name)) E.Throw(ii, "Invalid tag name: " + name);
            ii.State.Flags.Add(name);
            return true;
        }

        private static bool RepDepth(Interpreter ii, string[] args)
        {
            E.CheckArgs("repdepth", args, 0);
            ii.Write(ii.State.RepeaterDepth);
            return true;
        }

        private static bool Up(Interpreter ii, string[] args)
        {
            E.CheckArgs("up", args, 1);
            int level;
            if (!Int32.TryParse(args[0], out level)) E.Throw(ii, "Invalid value passed to UP tag. Must be a non-negative number.");
            if (level < 0) E.Throw(ii, "UP values cannot be negative.");
            ii.State.SetContext(level);
            return true;
        }

        private static bool Any(Interpreter ii, string[] args)
        {
            E.CheckArgs("any", args, 2);
            int prevSize = ii._channels.Size;
            ii.Do(args[0]);
            if (ii._channels.Size > prevSize) ii.Do(args[1]);
            return true;
        }

        private static bool Alt(Interpreter ii, string[] args)
        {
            E.CheckArgs("alt", args, 2);
            int prevSize = ii._channels.Size;
            ii.Do(args[0]);
            if (ii._channels.Size == prevSize) ii.Do(args[1]);
            return true;
        }

        private static bool Past(Interpreter ii, string[] args)
        {
            E.CheckArgs("past", args, 2);
            if (!ii.State.PickerActive) E.Throw(ii, "Attempted to use PAST with no picker active.");

            int number;
            if (!Int32.TryParse(ii.Evaluate(args[0]), out number)) E.Throw(ii, "Invalid PAST number.");

            if (!ii.State.TestPickerThreshold(number)) return true;
            ii.Do(args[1]);
            return false; // Stop interpreting the parent expression if the condition is satisfied
        }

        private static bool To(Interpreter ii, string[] args)
        {
            E.CheckArgs("to", args, 2);
            if (!ii.State.PickerActive) if (!ii.State.PickerActive) E.Throw(ii, "Attempted to use TO with no picker active.");

            int number;
            if (!Int32.TryParse(ii.Evaluate(args[0]), out number)) E.Throw(ii, "Invalid TO number.");

            if (ii.State.TestPickerThreshold(number)) return true;
            ii.Do(args[1]);
            return false; // Stop interpreting the parent expression if the condition is satisfied
        }

        private static bool Pick(Interpreter ii, string[] args)
        {
            E.CheckArgs("pick", args, 2);
            
            int number;
            if (!Int32.TryParse(ii.Evaluate(args[0]), out number)) E.Throw(ii, "Invalid PICK number.");

            if (number < 0) E.Throw(ii, "Attempted to use negative PICK number.");

            ii.State.PushPicker(number);

            ii.Do(args[1]);

            ii.State.PopPicker();

            return true;
        }

        private static Repeater GetRepeater(Interpreter i, string type)
        {
            int level = i._state.Context;
            if (level == 0) return i.State.CurrentRepeater;
            var rep = i.State.GetRepeaterLevel(level);
            if (rep == null) E.Throw(i, type + " tag context could not locate a repeater with the specified index (" + level + ")");
            return rep;
        }

        private static bool RepNotMiddle(Interpreter ii, string[] args)
        {
            E.CheckArgs("notmiddle", args, 1);
            if (ii.State.CurrentRepeater == null) return false;
            var rep = GetRepeater(ii, "NOTMIDDLE");
            if (rep.IsLast || rep.IsFirst)
            {
                ii.Do(args[0]);
            }
            return true;
        }

        private static bool RepMiddle(Interpreter ii, string[] args)
        {
            E.CheckArgs("middle", args, 1);
            if (ii.State.CurrentRepeater == null) return false;
            var rep = GetRepeater(ii, "MIDDLE");
            if (!(rep.IsLast || rep.IsFirst))
            {
                ii.Do(args[0]);
            }
            return true;
        }

        private static bool RepNotLast(Interpreter ii, string[] args)
        {
            E.CheckArgs("notlast", args, 1);
            if (ii.State.CurrentRepeater == null) return false;
            if (!GetRepeater(ii, "NOTLAST").IsLast)
            {
                ii.Do(args[0]);
            }
            return true;
        }

        private static bool RepNotFirst(Interpreter ii, string[] args)
        {
            E.CheckArgs("notfirst", args, 1);
            if (ii.State.CurrentRepeater == null) return false;
            if (!GetRepeater(ii, "NOTFIRST").IsFirst)
            {
                ii.Do(args[0]);
            }
            return true;
        }

        private static bool After(Interpreter ii, string[] args)
        {
            E.CheckArgs("after", args, 1);
            ii._currentAttribs.AfterString = args[0];
            return true;
        }

        private static bool Before(Interpreter ii, string[] args)
        {
            E.CheckArgs("before", args, 1);
            ii._currentAttribs.BeforeString = args[0];
            return true;
        }

        private static bool RepFirst(Interpreter ii, string[] args)
        {
            E.CheckArgs("first", args, 1);
            if (ii.State.CurrentRepeater == null) return false;

            if (GetRepeater(ii, "FIRST").IsFirst)
            {
                ii.Do(args[0]);
            }
            return true;
        }

        private static bool RepLast(Interpreter ii, string[] args)
        {
            E.CheckArgs("last", args, 1);
            if (ii.State.CurrentRepeater == null) return false;

            if (GetRepeater(ii, "LAST").IsLast)
            {
                ii.Do(args[0]);
            }
            return true;
        }

        private static bool RepEven(Interpreter ii, string[] args)
        {
            E.CheckArgs("even", args, 1);
            if (ii.State.CurrentRepeater == null) return false;

            if (GetRepeater(ii, "EVEN").IsEven)
            {
                ii.Do(args[0]);
            }
            return true;
        }

        private static bool RepOdd(Interpreter ii, string[] args)
        {
            E.CheckArgs("odd", args, 1);
            if (ii.State.CurrentRepeater == null) return false;

            if (GetRepeater(ii, "ODD").IsOdd)
            {
                ii.Do(args[0]);
            }
            return true;
        }

        private static bool RepRemaining(Interpreter ii, string[] args)
        {
            E.CheckArgs("reprem", args, 0);
            if (ii.State.CurrentRepeater == null) return false;
            ii.Write(GetRepeater(ii, "REPREM").Remaining);
            return true;
        }

        private static bool RepCount(Interpreter ii, string[] args)
        {
            E.CheckArgs("repcount", args, 0);
            if (ii.State.CurrentRepeater == null) return false;
            ii.Write(GetRepeater(ii, "REPCOUNT").TotalReps);
            return true;
        }

        private static bool RepNumber(Interpreter ii, string[] args)
        {
            E.CheckArgs("repnum", args, 0);
            if (ii.State.CurrentRepeater == null) return false;
            ii.Write(GetRepeater(ii, "REPNUM").CurrentRepNumber);
            return true;
        }

        private static bool RepIndex(Interpreter ii, string[] args)
        {
            E.CheckArgs("repindex", args, 0);
            if (ii.State.CurrentRepeater == null) return false;
            ii.Write(GetRepeater(ii, "REPINDEX").CurrentRepIndex);
            return true;
        }

        private static bool RequestString(Interpreter ii, string[] args)
        {
            E.CheckArgs("string", args, 1);
            if (ii._stringRequested == null) return true;

            ii.Write(ii._stringRequested(ii._state.RNG, ii.Evaluate(args[0])) ?? "");
            return true;
        }

        private static bool Cut(Interpreter ii, string[] args)
        {
            E.CheckArgs("cut", args, 2);
            var match = Regex.Match(ii.Evaluate(args[0].Trim()), @"^(?<first>\d*)-(?<last>\d*)$");
            if (!match.Success) return false;
            var strFirst = match.Groups["first"].Value;
            int? first = String.IsNullOrEmpty(strFirst) ? null : (int?)Int32.Parse(strFirst);
            var strLast = match.Groups["last"].Value;
            int? last = String.IsNullOrEmpty(strLast) ? null : (int?)Int32.Parse(strLast);

            var input = ii.Evaluate(args[1]);

            if (first == null && last == null)
            {
                ii.Do(input);
                return true;
            }

            int p1 = first != null
                ? (first.Value < 0 ? 0 : first.Value)
                : 0;
            int p2 = last != null
                ? (last.Value > input.Length ? input.Length : last.Value)
                : input.Length;

            ii.Do(input.Substring(p1, p2 - p1));
            return true;
        }

        private static bool Match(Interpreter ii, string[] args)
        {
            switch (args.Length)
            {
                case 0:
                    ii.Write(ii.State.CurrentMatch.Value);
                    break;
                case 1:
                    ii.Write(ii.State.CurrentMatch.Groups[args[0]]);
                    break;
                default:
                    return false;
            }
            return true;
        }

        private static bool Replace(Interpreter ii, string[] args)
        {
            E.CheckArgs("replace", args, 3);
            var input = ii.Evaluate(args[0]);
            var regex = args[1].Trim();
            var replacePat = args[2];
            ii.Write(Regex.Replace(input, regex, match =>
            {
                ii.State.PushMatch(match);
                var result = ii.Evaluate(replacePat);
                ii.State.PopMatch();
                return result;
            }, RegexOptions.ExplicitCapture));
            return true;
        }

        private static bool Arg(Interpreter ii, string[] args)
        {
            E.CheckArgs("arg", args, 1);
            ii.Write(ii.State.CurrentArgs.GetArg(args[0]));
            return true;
        }

        private static bool EvalArg(Interpreter ii, string[] args)
        {
            E.CheckArgs("evalarg", args, 1);
            ii.Do(ii.State.CurrentArgs.GetArg(args[0]));
            return true;
        }

        private static bool Fuse(Interpreter ii, string[] args)
        {
            if (args.Length < 2) return false;

            var current = ii.Evaluate(args[0].Trim());
            for (int i = 1; i < args.Length; i++)
            {
                current = FuseFunc(current, ii.Evaluate(args[i].Trim()));
            }
            ii.Write(current);
            return true;
        }

        private static string FuseFunc(string a, string b)
        {
            if (a == b)
            {
                return a;
            }

            int maxFuseSize = Math.Min(a.Length, b.Length);
            if (maxFuseSize == 0)
            {
                return a + b;
            }

            int fuseSize = maxFuseSize;
            while (fuseSize > 0 && a.Substring(a.Length - fuseSize, fuseSize) != b.Substring(0, fuseSize))
            {
                fuseSize--;
            }

            if (fuseSize == 0)
            {
                return a + b;
            }
            return a + b.Substring(fuseSize);
        }

        private static bool CallInterp(Interpreter ii, string[] args)
        {
            if (args.Length < 1) return false;
            string name = ii.Evaluate(args[0]).ToLower();
            if (!Util.ValidateName(name)) return false;
            var value = ii.State.Subroutines.GetSubroutine(name);
            if (value == null) return false;
            ii.DoSub(value, args.Skip(1).ToArray());
            return true;
        }

        private static bool Desync(Interpreter ii, string[] args)
        {
            E.CheckArgs("desync", args, 0);
            ii.State.Desync();
            return true;
        }

        private static bool Step(Interpreter ii, string[] args)
        {
            E.CheckArgs("step", args, 1);
            return ii.State.Step(ii.Evaluate(args[0]));
        }

        private static bool Unpin(Interpreter ii, string[] args)
        {
            E.CheckArgs("unpin", args, 1);
            return ii.State.SetPinState(ii.Evaluate(args[0]), false);
        }

        private static bool Pin(Interpreter ii, string[] args)
        {
            E.CheckArgs("pin", args, 1);
            return ii.State.SetPinState(ii.Evaluate(args[0]), true);
        }

        private static bool DefineInterp(Interpreter ii, string[] args)
        {
            E.CheckArgs("isub", args, 2);

            var header = ii.Evaluate(args[0]).Split(new [] {' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (header.Length == 0) return false;

            var name = header[0].ToLower();
            if (!Util.ValidateName(name)) return false;

            var parameters = header.Skip(1).ToArray();

            ii.State.Subroutines.Define(name, new Subroutine(name, ii.Evaluate(args[1], false), parameters));
            
            return true;
        }

        private static bool Call(Interpreter ii, string[] args)
        {
            if (args.Length == 0) return false;
            string name = args[0].ToLower();
            if (!Util.ValidateName(name)) return false;
            var value = ii.State.Subroutines.GetSubroutine(name);
            if (value == null) return false;
            ii.DoSub(value, args.Skip(1).ToArray());
            return true;
        }

        public bool CallImpl(TagInfo info)
        {
            string name = info.Name.TrimStart('$');
            if (!Util.ValidateName(name)) return false;
            Subroutine value = State.Subroutines.GetSubroutine(name);
            if (value == null) return false;
            DoSub(value, info.Arguments);
            return true;
        }

        private static bool Define(Interpreter ii, string[] args)
        {
            E.CheckArgs("sub", args, 2);

            var header = ii.Evaluate(args[0]).Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (header.Length == 0) return false;

            var name = header[0].ToLower();
            if (!Util.ValidateName(name)) return false;

            var parameters = header.Skip(1).ToArray();

            ii.State.Subroutines.Define(name, new Subroutine(name, args[1], parameters));

            return true;
        }

        private static bool Dump(Interpreter ii, string[] args)
        {
            E.CheckArgs("dump", args, 1);
            ii.Write(ii._channels.GetOutput(args[0]));
            return true;
        }

        private static bool Reset(Interpreter ii, string[] args)
        {
            E.CheckArgs("reset", args, 1);
            ii.State.Reset(ii.Evaluate(args[0]));
            return true;
        }

        private static bool Reseed(Interpreter ii, string[] args)
        {
            E.CheckArgs("reseed", args, 2);
            ii.State.Reseed(ii.Evaluate(args[0]), ii.Evaluate(args[1]));
            return true;
        }

        private static bool Sync(Interpreter ii, string[] args)
        {
            E.CheckArgs("sync", args, 2);
            SelectorType type;
            if (!Enum.TryParse(ii.Evaluate(args[1]), true, out type)) return false;
            ii.State.Sync(ii.Evaluate(args[0]), type);
            return true;
        }

        private static bool Return(Interpreter ii, string[] args)
        {
            E.CheckArgs("return", args, 0);
            return false;
        }

        private static bool OutChannel(Interpreter ii, string[] args)
        {
            E.CheckArgs("out", args, 2);

            var name = ii.Evaluate(args[0]);
            var vis = ii.Evaluate(args[1]);

            if (vis.ToLower() == "off")
            {
                ii._channels.PopChannel(name);
                return true;
            }

            ChannelVisibility cv;
            if (!Enum.TryParse(vis, true, out cv)) cv = ChannelVisibility.Public;
            ii._channels.PushChannel(name, cv);
            return true;
        }

        private static bool Separate(Interpreter ii, string[] args)
        {
            E.CheckArgs("sep", args, 1);
            ii._currentAttribs.Separator = args[0];
            return true;
        }

        private static bool Number(Interpreter ii, string[] args)
        {
            switch (args.Length)
            {
                case 1:
                {
                    int num;
                    if (!Int32.TryParse(ii.Evaluate(args[0]), out num)) return false;
                    ii.Write(ii.State.RNG.Next(num + 1));
                    break;
                }
                case 2:
                {
                    int a, b;
                    if (!Int32.TryParse(ii.Evaluate(args[0]), out a) || !Int32.TryParse(ii.Evaluate(args[1]), out b)) return false;
                    ii.Write(ii.State.RNG.Next(a, b + 1));
                    break;
                }
                default:
                    return false;
            }
            return true;
        }

        private static bool Caps(Interpreter ii, string[] args)
        {
            E.CheckArgs("caps", args, 1);
            return Enum.TryParse(ii.Evaluate(args[0]), true, out ii._state.CurrentFormat);
        }

        private static bool Rep(Interpreter ii, string[] args)
        {
            E.CheckArgs("rep", args, 1);
            var match = Regex.Match(ii.Evaluate(args[0]), @"((?<min>\d+)\-(?<max>\d+)|(?<const>(\d+|each)))", RegexOptions.ExplicitCapture);
            if (!match.Success) return false;
            string num;
            if ((num = match.Groups["const"].Value).Length > 0)
            {
                if (String.Equals(num, "each", StringComparison.InvariantCultureIgnoreCase))
                {
                    ii._currentAttribs.Repetitions = -1;
                }
                else
                {
                    ii._currentAttribs.Repetitions = Int32.Parse(num);                    
                }
            }
            else
            {
                ii._currentAttribs.Repetitions = ii.State.RNG.Next(
                    Int32.Parse(match.Groups["min"].Value),
                    Int32.Parse(match.Groups["max"].Value) + 1);
            }
            return true;
        }

        private static bool Chance(Interpreter ii, string[] args)
        {
            E.CheckArgs("chance", args, 1);
            int num;
            if (!Int32.TryParse(ii.Evaluate(args[0]), out num)) return false;
            ii._currentAttribs.Chance = num;
            return true;
        }

        private static bool Unbranch(Interpreter ii, string[] args)
        {
            E.CheckArgs("ub", args, 0);
            if (ii.State.RNG.Depth <= 1) return false;
            ii.State.RNG.Merge();
            return true;
        }

        private static bool Branch(Interpreter ii, string[] args)
        {
            E.CheckArgs("br", args, 1);
            ii.State.RNG.Branch(ii.Evaluate(args[0]).Hash());
            return true;
        }
    }
}