using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
            TagFuncs["chance"] = Chance;
            TagFuncs["rep"] = Rep;
            TagFuncs["repindex"] = RepIndex;
            TagFuncs["repnum"] = RepNumber;
            TagFuncs["reprem"] = RepRemaining;
            TagFuncs["repcount"] = RepCount;
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
            TagFuncs["num"] = Number;
            TagFuncs["sep"] = Separate;
            TagFuncs["ch"] = Chan;
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
        }

        private static bool RepNotMiddle(Interpreter interpreter, string[] args)
        {
            if (args.Length != 1 || interpreter.State.CurrentRepeater == null) return false;
            if (interpreter.State.CurrentRepeater.IsLast || interpreter.State.CurrentRepeater.IsFirst)
            {
                interpreter.Do(args[0]);
            }
            return true;
        }

        private static bool RepMiddle(Interpreter interpreter, string[] args)
        {
            if (args.Length != 1 || interpreter.State.CurrentRepeater == null) return false;
            if (!(interpreter.State.CurrentRepeater.IsLast || interpreter.State.CurrentRepeater.IsFirst))
            {
                interpreter.Do(args[0]);
            }
            return true;
        }

        private static bool RepNotLast(Interpreter interpreter, string[] args)
        {
            if (args.Length != 1 || interpreter.State.CurrentRepeater == null) return false;
            if (!interpreter.State.CurrentRepeater.IsLast)
            {
                interpreter.Do(args[0]);
            }
            return true;
        }

        private static bool RepNotFirst(Interpreter interpreter, string[] args)
        {
            if (args.Length != 1 || interpreter.State.CurrentRepeater == null) return false;
            if (!interpreter.State.CurrentRepeater.IsFirst)
            {
                interpreter.Do(args[0]);
            }
            return true;
        }

        private static bool After(Interpreter interpreter, string[] args)
        {
            if (args.Length != 1) return false;
            interpreter._currentAttribs.AfterString = args[0];
            return true;
        }

        private static bool Before(Interpreter interpreter, string[] args)
        {
            if (args.Length != 1) return false;
            interpreter._currentAttribs.BeforeString = args[0];
            return true;
        }

        private static bool RepFirst(Interpreter interpreter, string[] args)
        {
            if (args.Length != 1 || interpreter.State.CurrentRepeater == null) return false;
            if (interpreter.State.CurrentRepeater.IsFirst)
            {
                interpreter.Do(args[0]);
            }
            return true;
        }

        private static bool RepLast(Interpreter interpreter, string[] args)
        {
            if (args.Length != 1 || interpreter.State.CurrentRepeater == null) return false;
            if (interpreter.State.CurrentRepeater.IsLast)
            {
                interpreter.Do(args[0]);
            }
            return true;
        }

        private static bool RepEven(Interpreter interpreter, string[] args)
        {
            if (args.Length != 1 || interpreter.State.CurrentRepeater == null) return false;
            if (interpreter.State.CurrentRepeater.IsEven)
            {
                interpreter.Do(args[0]);
            }
            return true;
        }

        private static bool RepOdd(Interpreter interpreter, string[] args)
        {
            if (args.Length != 1 || interpreter.State.CurrentRepeater == null) return false;
            if (interpreter.State.CurrentRepeater.IsOdd)
            {
                interpreter.Do(args[0]);
            }
            return true;
        }

        private static bool RepRemaining(Interpreter interpreter, string[] args)
        {
            if (args.Length != 0 || interpreter.State.CurrentRepeater == null) return false;
            interpreter.Write(interpreter.State.CurrentRepeater.Remaining);
            return true;
        }

        private static bool RepCount(Interpreter interpreter, string[] args)
        {
            if (args.Length != 0 || interpreter.State.CurrentRepeater == null) return false;
            interpreter.Write(interpreter.State.CurrentRepeater.TotalReps);
            return true;
        }

        private static bool RepNumber(Interpreter interpreter, string[] args)
        {
            if (args.Length != 0 || interpreter.State.CurrentRepeater == null) return false;
            interpreter.Write(interpreter.State.CurrentRepeater.CurrentRepNumber);
            return true;
        }

        private static bool RepIndex(Interpreter interpreter, string[] args)
        {
            if (args.Length != 0 || interpreter.State.CurrentRepeater == null) return false;
            interpreter.Write(interpreter.State.CurrentRepeater.CurrentRepIndex);
            return true;
        }

        private static bool RequestString(Interpreter interpreter, string[] args)
        {
            if (interpreter._stringRequested == null) return true;

            if (args.Length != 1) return false;

            interpreter.Write(interpreter._stringRequested(interpreter._state.RNG, interpreter.Evaluate(args[0])) ?? "");
            return true;
        }

        private static bool Cut(Interpreter interpreter, string[] args)
        {
            if (args.Length != 2) return false;
            var match = Regex.Match(interpreter.Evaluate(args[0].Trim()), @"^(?<first>\d*)-(?<last>\d*)$");
            if (!match.Success) return false;
            var strFirst = match.Groups["first"].Value;
            int? first = String.IsNullOrEmpty(strFirst) ? null : (int?)Int32.Parse(strFirst);
            var strLast = match.Groups["last"].Value;
            int? last = String.IsNullOrEmpty(strLast) ? null : (int?)Int32.Parse(strLast);

            var input = interpreter.Evaluate(args[1]);

            if (first == null && last == null)
            {
                interpreter.Do(input);
                return true;
            }

            int p1 = first != null
                ? (first.Value < 0 ? 0 : first.Value)
                : 0;
            int p2 = last != null
                ? (last.Value > input.Length ? input.Length : last.Value)
                : input.Length;

            interpreter.Do(input.Substring(p1, p2 - p1));
            return true;
        }

        private static bool Match(Interpreter interpreter, string[] args)
        {
            switch (args.Length)
            {
                case 0:
                    interpreter.Write(interpreter.State.CurrentMatch.Value);
                    break;
                case 1:
                    interpreter.Write(interpreter.State.CurrentMatch.Groups[args[0]]);
                    break;
                default:
                    return false;
            }
            return true;
        }

        private static bool Replace(Interpreter interpreter, string[] args)
        {
            if (args.Length != 3) return false;
            var input = interpreter.Evaluate(args[0]);
            var regex = args[1].Trim();
            var replacePat = args[2];
            interpreter.Write(Regex.Replace(input, regex, match =>
            {
                interpreter.State.PushMatch(match);
                var result = interpreter.Evaluate(replacePat);
                interpreter.State.PopMatch();
                return result;
            }, RegexOptions.ExplicitCapture));
            return true;
        }

        private static bool Arg(Interpreter interpreter, string[] args)
        {
            if (args.Length != 1) return false;
            interpreter.Write(interpreter.State.CurrentArgs.GetArg(args[0]));
            return true;
        }

        private static bool EvalArg(Interpreter interpreter, string[] args)
        {
            if (args.Length != 1) return false;
            interpreter.Do(interpreter.State.CurrentArgs.GetArg(args[0]));
            return true;
        }

        private static bool Fuse(Interpreter interpreter, string[] args)
        {
            if (args.Length < 2) return false;

            var current = interpreter.Evaluate(args[0].Trim());
            for (int i = 1; i < args.Length; i++)
            {
                current = FuseFunc(current, interpreter.Evaluate(args[i].Trim()));
            }
            interpreter.Write(current);
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

        private static bool CallInterp(Interpreter interpreter, string[] args)
        {
            if (args.Length < 1) return false;
            string name = interpreter.Evaluate(args[0]).ToLower();
            if (!Util.ValidateName(name)) return false;
            var value = interpreter.State.Subroutines.GetSubroutine(name);
            if (value == null) return false;
            interpreter.DoSub(value, args.Skip(1).ToArray());
            return true;
        }

        private static bool Desync(Interpreter interpreter, string[] args)
        {
            if (args.Length != 0) return false;
            interpreter.State.Desync();
            return true;
        }

        private static bool Step(Interpreter interpreter, string[] args)
        {
            if (args.Length != 1) return false;
            return interpreter.State.Step(interpreter.Evaluate(args[0]));
        }

        private static bool Unpin(Interpreter interpreter, string[] args)
        {
            if (args.Length != 1) return false;
            return interpreter.State.SetPinState(interpreter.Evaluate(args[0]), false);
        }

        private static bool Pin(Interpreter interpreter, string[] args)
        {
            if (args.Length != 1) return false;
            return interpreter.State.SetPinState(interpreter.Evaluate(args[0]), true);
        }

        private static bool DefineInterp(Interpreter interpreter, string[] args)
        {
            if (args.Length != 2) return false;

            var header = interpreter.Evaluate(args[0]).Split(new [] {' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (header.Length == 0) return false;

            var name = header[0].ToLower();
            if (!Util.ValidateName(name)) return false;

            var parameters = header.Skip(1).ToArray();

            interpreter.State.Subroutines.Define(name, new Subroutine(name, interpreter.Evaluate(args[1], false), parameters));
            
            return true;
        }

        private static bool Call(Interpreter interpreter, string[] args)
        {
            if (args.Length == 0) return false;
            string name = args[0].ToLower();
            if (!Util.ValidateName(name)) return false;
            var value = interpreter.State.Subroutines.GetSubroutine(name);
            if (value == null) return false;
            interpreter.DoSub(value, args.Skip(1).ToArray());
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

        private static bool Define(Interpreter interpreter, string[] args)
        {
            if (args.Length != 2) return false;

            var header = interpreter.Evaluate(args[0]).Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (header.Length == 0) return false;

            var name = header[0].ToLower();
            if (!Util.ValidateName(name)) return false;

            var parameters = header.Skip(1).ToArray();

            interpreter.State.Subroutines.Define(name, new Subroutine(name, args[1], parameters));

            return true;
        }

        private static bool Dump(Interpreter interpreter, string[] args)
        {
            if (args.Length != 1) return false;
            interpreter.Write(interpreter._channels.GetOutput(args[0]));
            return true;
        }

        private static bool Reset(Interpreter interpreter, string[] args)
        {
            if (args.Length != 1) return false;
            interpreter.State.Reset(interpreter.Evaluate(args[0]));
            return true;
        }

        private static bool Reseed(Interpreter interpreter, string[] args)
        {
            if (args.Length != 2) return false;
            interpreter.State.Reseed(interpreter.Evaluate(args[0]), interpreter.Evaluate(args[1]));
            return true;
        }

        private static bool Sync(Interpreter interpreter, string[] args)
        {
            if (args.Length != 2) return false;
            SelectorType type;
            if (!Enum.TryParse(interpreter.Evaluate(args[1]), true, out type)) return false;
            interpreter.State.Sync(interpreter.Evaluate(args[0]), type);
            return true;
        }

        private static bool Return(Interpreter interpreter, string[] args)
        {
            return false;
        }

        private static bool Chan(Interpreter interpreter, string[] args)
        {
            if (args.Length != 2) return false;

            var name = interpreter.Evaluate(args[0]);
            var vis = interpreter.Evaluate(args[1]);

            if (vis.ToLower() == "off")
            {
                interpreter._channels.PopChannel(name);
                return true;
            }

            ChannelVisibility cv;
            if (!Enum.TryParse(vis, true, out cv)) cv = ChannelVisibility.Public;
            interpreter._channels.PushChannel(name, cv);
            return true;
        }

        private static bool Separate(Interpreter interpreter, string[] args)
        {
            if (args.Length != 1) return false;
            interpreter._currentAttribs.Separator = args[0];
            return true;
        }

        private static bool Number(Interpreter interpreter, string[] args)
        {
            switch (args.Length)
            {
                case 1:
                {
                    int num;
                    if (!Int32.TryParse(interpreter.Evaluate(args[0]), out num)) return false;
                    interpreter.Write(interpreter.State.RNG.Next(num + 1));
                    break;
                }
                case 2:
                {
                    int a, b;
                    if (!Int32.TryParse(interpreter.Evaluate(args[0]), out a) || !Int32.TryParse(interpreter.Evaluate(args[1]), out b)) return false;
                    interpreter.Write(interpreter.State.RNG.Next(a, b + 1));
                    break;
                }
                default:
                    return false;
            }
            return true;
        }

        private static bool Caps(Interpreter interpreter, string[] args)
        {
            return args.Length == 1 && Enum.TryParse(interpreter.Evaluate(args[0]), true, out interpreter._state.CurrentFormat);
        }

        private static bool Rep(Interpreter interpreter, string[] args)
        {
            if (args.Length != 1) return false;
            var match = Regex.Match(interpreter.Evaluate(args[0]), @"((?<min>\d+)\-(?<max>\d+)|(?<const>(\d+|each)))", RegexOptions.ExplicitCapture);
            if (!match.Success) return false;
            string num;
            if ((num = match.Groups["const"].Value).Length > 0)
            {
                if (String.Equals(num, "each", StringComparison.InvariantCultureIgnoreCase))
                {
                    interpreter._currentAttribs.Repetitions = -1;
                }
                else
                {
                    interpreter._currentAttribs.Repetitions = Int32.Parse(num);                    
                }
            }
            else
            {
                interpreter._currentAttribs.Repetitions = interpreter.State.RNG.Next(
                    Int32.Parse(match.Groups["min"].Value),
                    Int32.Parse(match.Groups["max"].Value) + 1);
            }
            return true;
        }

        private static bool Chance(Interpreter interpreter, string[] args)
        {
            if (args.Length != 1) return false;
            int num;
            if (!Int32.TryParse(interpreter.Evaluate(args[0]), out num)) return false;
            interpreter._currentAttribs.Chance = num;
            return true;
        }

        private static bool Unbranch(Interpreter interpreter, string[] args)
        {
            if (args.Length != 0 || interpreter.State.RNG.Depth <= 1) return false;
            interpreter.State.RNG.Merge();
            return true;
        }

        private static bool Branch(Interpreter interpreter, string[] args)
        {
            if (args.Length != 1) return false;
            interpreter.State.RNG.Branch(interpreter.Evaluate(args[0]).Hash());
            return true;
        }
    }
}