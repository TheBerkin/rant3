using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Rant.Engine.Blueprints;
using Rant.Engine.Formatters;
using Rant.Stringes;
using Rant.Engine.Util;

namespace Rant.Engine
{
    // The return value is returned by the blueprint that executes the tag. If it's true, the vm will skip to the top of the state stack.
    internal delegate bool RantFuncDelegate(VM vm, RantPattern source, Stringe tagName, Argument[] args);

#if EDITOR
    /// <summary>
    /// Contains and manages the built-in functions of the Rant language.
    /// </summary>
    public static class RantFuncs
#else
    internal static class RantFuncs
#endif

    {
        internal static readonly Dictionary<string, RantFuncSigs> F;

        /// <summary>
        /// Determines if the specified string is a valid Rant function name.
        /// </summary>
        /// <param name="functionName">The function name to check.</param>
        /// <returns></returns>
        public static bool IsValidFunctionName(string functionName) => F.ContainsKey(functionName);

        static RantFuncs()
        {
            F = new Dictionary<string, RantFuncSigs>(StringComparer.InvariantCultureIgnoreCase);

            // Repeaters, probability, and block attributes
            F["rep"] = F["r"] = new RantFunc(Repeat, ParamFlags.None);
            F["sep"] = F["s"] = new RantFunc(Separator, ParamFlags.Code);
            F["before"] = new RantFunc(Before, ParamFlags.Code);
            F["after"] = new RantFunc(After, ParamFlags.Code);
            F["chance"] = new RantFunc(Chance, ParamFlags.None);
            F["break"] = new RantFunc(Break);
            F["repnum"] = F["rn"] = new RantFunc(RepNum);
            F["repindex"] = F["ri"] = new RantFunc(RepIndex);
            F["repcount"] = F["rc"] = new RantFunc(RepCount);

            // Synchronizers
            F["sync"] = F["x"] = new RantFuncSigs(
                new RantFunc(SyncCreateApply, ParamFlags.None, ParamFlags.None),
                new RantFunc(SyncApply, ParamFlags.None)
                );
            F["xreset"] = new RantFunc(SyncReset, ParamFlags.None);
            F["xseed"] = new RantFunc(SyncReseed, ParamFlags.None, ParamFlags.None);
            F["xnone"] = new RantFunc(Desync);
            F["xnew"] = new RantFunc(SyncCreate, ParamFlags.None, ParamFlags.None);
            F["xpin"] = new RantFunc(Pin, ParamFlags.None);
            F["xunpin"] = new RantFunc(Unpin, ParamFlags.None);
            F["xstep"] = new RantFunc(Step, ParamFlags.None);

            // RNG manipulation
            F["branch"] = F["b"] = new RantFuncSigs(
                new RantFunc(Branch, ParamFlags.None),
                new RantFunc(BranchScope, ParamFlags.None, ParamFlags.Code)
                );
            F["merge"] = F["m"] = new RantFunc(Merge);
            F["generation"] = F["g"] = new RantFuncSigs(new RantFunc(Generation), new RantFunc(GenerationSet, ParamFlags.None));

            // Repeater conditionals
            F["first"] = new RantFunc(First, ParamFlags.Code);
            F["last"] = new RantFunc(Last, ParamFlags.Code);
            F["middle"] = new RantFunc(Middle, ParamFlags.Code);
            F["notfirst"] = new RantFunc(NotFirst, ParamFlags.Code);
            F["notlast"] = new RantFunc(NotLast, ParamFlags.Code);
            F["notmiddle"] = new RantFunc(NotMiddle, ParamFlags.Code);
            F["odd"] = new RantFunc(Odd, ParamFlags.Code);
            F["even"] = new RantFunc(Even, ParamFlags.Code);
            F["nth"] = new RantFuncSigs(
                new RantFunc(Nth, ParamFlags.None, ParamFlags.None, ParamFlags.Code),
                new RantFunc(NthSimple, ParamFlags.None, ParamFlags.Code)
                );

            // Quantifier conditionals
            F["alt"] = new RantFunc(Alt, ParamFlags.Code, ParamFlags.Code);
            F["any"] = new RantFunc(Any, ParamFlags.Code, ParamFlags.Code);

            // Replacers
            F["match"] = new RantFunc(ReplaceMatch);
            F["group"] = new RantFunc(ReplaceGroup, ParamFlags.None);

            // Subrotuine interaction
            F["arg"] = new RantFunc(Arg, ParamFlags.None);

            // Formatting
            F["numfmt"] = new RantFuncSigs(
                new RantFunc(NumFmt, ParamFlags.None),
                new RantFunc(NumFmtScope, ParamFlags.None, ParamFlags.Code));
            F["digits"] = new RantFuncSigs(
                new RantFunc(Digits, ParamFlags.None),
                new RantFunc(DigitsScope, ParamFlags.None, ParamFlags.Code));
            F["endian"] = new RantFunc(Endian, ParamFlags.None);
            F["caps"] = F["case"] = new RantFuncSigs(
                new RantFunc(Case, ParamFlags.None),
                new RantFunc(CaseScope, ParamFlags.None, ParamFlags.Code));
            F["capsinfer"] = new RantFunc(CapsInfer, ParamFlags.None);
            F["quot"] = F["q"] = new RantFunc(Quote, ParamFlags.Code);

            // Channels
            F["out"] = new RantFunc(Out, ParamFlags.None, ParamFlags.None);
            F["close"] = new RantFunc(Close, ParamFlags.None);

            // External interaction
            F["extern"] = F["ext"] = new RantFunc(Extern, ParamFlags.None, ParamFlags.Multi);

            // Markers and targets
            F["mark"] = new RantFunc(Mark, ParamFlags.None);
            F["dist"] = new RantFunc(Dist, ParamFlags.None, ParamFlags.None);
            F["get"] = new RantFunc(Get, ParamFlags.None);
            F["send"] = new RantFunc(Send, ParamFlags.None, ParamFlags.None);
            F["osend"] = new RantFunc(SendOverwrite, ParamFlags.None, ParamFlags.None);
            F["clrt"] = new RantFunc(ClearTarget, ParamFlags.None);
            F["copy"] = new RantFunc(Copy, ParamFlags.None, ParamFlags.None);

            // String generation, manipulation, and analysis
            F["len"] = new RantFunc(Length, ParamFlags.None);
            F["char"] = new RantFuncSigs(
                new RantFunc(Character, ParamFlags.None),
                new RantFunc(CharacterMulti, ParamFlags.None, ParamFlags.None));
            F["num"] = F["n"] = new RantFunc(Number, ParamFlags.None, ParamFlags.None);
            F["dec"] = new RantFunc(NumberDec);

            // Flags
            F["define"] = new RantFunc(DefineFlag, ParamFlags.None | ParamFlags.Multi);
            F["undef"] = new RantFunc(UndefineFlag, ParamFlags.None | ParamFlags.Multi);
            F["ifdef"] = new RantFunc(IfDef, ParamFlags.None, ParamFlags.Code);
            F["ifndef"] = new RantFunc(IfNDef, ParamFlags.None, ParamFlags.Code);
            F["else"] = new RantFunc(Else, ParamFlags.Code);

            // Comparisons
            F["cmp"] = new RantFunc(Compare, ParamFlags.None, ParamFlags.None, ParamFlags.Code);
            F["is"] = new RantFunc(CmpIs, ParamFlags.None, ParamFlags.Code);

            // Misc
            F["src"] = new RantFunc(Src);
            F["rhymemode"] = new RantFunc(RhymeMode, ParamFlags.None);
        }

        private static bool Quote(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            vm.PushState(VM.State.CreateSub(source, args[0].AsPattern(), vm, vm.CurrentState.Output)
                .Pre(new DelegateBlueprint(vm, _ =>
                {
                    vm.OpenQuote();
                    return false;
                }))
                .Post(new DelegateBlueprint(vm, _ =>
                {
                    vm.CloseQuote();
                    return false;
                })));
            return true;
        }

        private static bool Copy(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            vm.Print(vm.CopyMarkerRegion(args[0].AsString(), args[1].AsString()));
            return false;
        }

        private static bool NumberDec(VM vm, RantPattern source, Stringe tagName, Argument[] args)
        {
            vm.Print(vm.RNG.NextDouble());
            return false;
        }

        private static bool GenerationSet(VM vm, RantPattern source, Stringe tagName, Argument[] args)
        {
            var gStr = args[0].AsString();
            long g;
            if (!Int64.TryParse(gStr, out g))
            {
                throw Error(source, tagName, "Invalid generation value '\{gStr}'");
            }
            vm.RNG.Generation = g;
            return false;
        }

        private static bool Generation(VM vm, RantPattern source, Stringe tagName, Argument[] args)
        {
            vm.Print(vm.RNG.Generation);
            return false;
        }

        private static bool Merge(VM vm, RantPattern source, Stringe tagName, Argument[] args)
        {
            vm.RNG.Merge();
            return false;
        }

        private static bool BranchScope(VM vm, RantPattern source, Stringe tagName, Argument[] args)
        {
            vm.RNG.Branch(args[0].AsString().Hash());
            var state = VM.State.CreateSub(source, args[1].AsPattern(), vm, vm.CurrentState.Output);
            state.Post(new DelegateBlueprint(vm, _ =>
            {
                vm.RNG.Merge();
                return false;
            }));
            vm.PushState(state);
            return true;
        }

        private static bool Branch(VM vm, RantPattern source, Stringe tagName, Argument[] args)
        {
            vm.RNG.Branch(args[0].AsString().Hash());
            return false;
        }

        private static bool Src(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            vm.Print(source.Code);
            return false;
        }

        private static bool Break(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            if (vm.CurrentRepeater == null) return false;
            vm.CurrentRepeater.Finish();
            VM.State state = null;
            while (!vm.BaseStates.Contains(state = vm.CurrentState))
            {
                vm.PopState();
            }
            vm.BaseStates.Remove(state);
            return true;
        }

        private static bool CmpIs(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            var cmp = vm.Comparisons.Peek();
            var conStrings = args[0].AsString()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            ComparisonResult e;
            foreach (var conString in conStrings)
            {
                if (!Util.TryParseMode(conString, out e)) continue;
                if ((cmp.Result & e) != e) continue;
                vm.PushState(VM.State.CreateSub(source, args[1].AsPattern(), vm, vm.CurrentState.Output));
                return true;
            }

            return false;
        }

        private static bool Compare(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            var cmp = new Comparison(args[0].AsString(), args[1].AsString());
            vm.Comparisons.Push(cmp);

            var state = VM.State.CreateSub(source, args[2].AsPattern(), vm, vm.CurrentState.Output);
            state.Post(new DelegateBlueprint(vm, I =>
            {
                I.Comparisons.Pop();
                return false;
            }));

            vm.PushState(state);

            return true;
        }

        private static bool SyncReseed(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            vm.SyncSeed(args[0].AsString(), args[1].AsString());
            return false;
        }

        private static bool IfNDef(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            if (!vm.Engine.Flags.Contains(args[0]))
            {
                vm.PushState(VM.State.CreateSub(source, args[1].AsPattern(), vm, vm.CurrentState.Output));
                return true;
            }
            vm.SetElse();
            return false;
        }

        private static bool Else(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            if (!vm.UseElse()) return false;
            vm.PushState(VM.State.CreateSub(source, args[0].AsPattern(), vm, vm.CurrentState.Output));
            return true;
        }

        private static bool IfDef(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            if (vm.Engine.Flags.Contains(args[0]))
            {
                vm.PushState(VM.State.CreateSub(source, args[1].AsPattern(), vm, vm.CurrentState.Output));
                return true;
            }
            vm.SetElse();
            return false;
        }

        private static bool CharacterMulti(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            int count;
            if (!ParseInt(args[1], out count) || count < 0)
                throw Error(source, tagname, "Invalid character count specified. Must be a non-negative number greater than zero.");
            for (int i = 0; i < count; i++)
            {
                vm.Print(SelectFromRanges(args[0], vm.RNG));
            }
            return false;
        }

        private static bool Character(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            vm.Print(SelectFromRanges(args[0], vm.RNG));
            return false;
        }

        private static bool UndefineFlag(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            foreach (var flag in args.Where(flag => !String.IsNullOrEmpty(flag)))
            {
                if (!ValidateName(flag))
                    throw Error(source, tagname, "Invalid flag name '\{flag}'");

                vm.Engine.Flags.Remove(flag);
            }
            return false;
        }

        private static bool DefineFlag(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            foreach (var flag in args.Where(flag => !String.IsNullOrEmpty(flag)))
            {
                if (!ValidateName(flag))
                    throw Error(source, tagname, "Invalid flag name '\{flag}'");

                vm.Engine.Flags.Add(flag);
            }
            return false;
        }

        private static bool Length(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            vm.Print(args[0].AsString().Length);
            return false;
        }

        private static bool Send(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            vm.CurrentState.Output.WriteToTarget(args[0], args[1]);
            return false;
        }

        private static bool SendOverwrite(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            vm.CurrentState.Output.WriteToTarget(args[0], args[1], true);
            return false;
        }

        private static bool Get(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            vm.CurrentState.Output.CreateTarget(args[0]);
            return false;
        }

        private static bool ClearTarget(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            vm.CurrentState.Output.ClearTarget(args[0]);
            return false;
        }

        private static bool Dist(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            vm.Print(vm.GetMarkerDistance(args[0], args[1]));
            return false;
        }

        private static bool Mark(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            vm.SetMarker(args[0]);
            return false;
        }

        private static bool Extern(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            var name = args[0].AsString();
            var result = vm.Engine.CallHook(name, args.Skip(1).Select(arg => arg.AsString()).ToArray());
            if (result == null)
            {
                throw Error(source, tagname, "A hook with the name '\{name}' does not exist.");
            }
            vm.Print(result);
            return false;
        }

        private static bool SyncReset(VM vm, RantPattern source, Stringe tagName, Argument[] args)
        {
            vm.SyncReset(args[0].AsString());
            return false;
        }

        private static bool CapsInfer(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            // TODO: Make capsinfer properly infer "first" capitalization given multiple sentences. Currently, it mistakes it for "word" mode.
            var words = Regex.Matches(args[0].AsString(), @"\w+").OfType<Match>().Select(m => m.Value).ToArray();
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
                vm.CurrentState.Output.SetCase(Formatters.Case.Upper);
            }
            else if (wCount > 1 && fwCount == wCount)
            {
                vm.CurrentState.Output.SetCase(Formatters.Case.Title);
            }
            else if (firstCharIsUpper)
            {
                vm.CurrentState.Output.SetCase(Formatters.Case.First);
            }
            return false;
        }

        private static bool Close(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            vm.CurrentState.Output.PopChannel(args[0].AsString());
            return false;
        }

        private static bool Out(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            RantChannelVisibility cv;
            var cv_str = args[1].AsString();
            if (!Util.TryParseMode(cv_str, out cv))
            {
                throw Error(source, tagname, "Invalid channel visibility option '\{cv_str}'");
            }
            vm.CurrentState.Output.PushChannel(args[0].AsString(), cv, vm.Format);
            return false;
        }

        private static bool Case(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            Case caps;
            var caps_str = args[0].AsString();
            if (!Util.TryParseMode(caps_str, out caps))
            {
                throw Error(source, tagname, "Invalid case format '\{caps_str}'");
            }
            vm.CurrentState.Output.SetCase(caps);
            return false;
        }

        private static bool CaseScope(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            Case caps;
            var caps_str = args[0].AsString();
            if (!Util.TryParseMode(caps_str, out caps))
            {
                throw Error(source, tagname, "Invalid case format '\{caps_str}'");
            }
            var cases = vm.CurrentState.Output.GetCurrentCases();
            vm.CurrentState.Output.SetCase(caps);
            vm.PushState(VM.State.CreateSub(source, args[1].AsPattern(), vm, vm.CurrentState.Output)
                .Post(new DelegateBlueprint(vm,
                _ =>
                {
                    foreach (var pair in cases)
                    {
                        pair.Key.Formatter.Case = pair.Value;
                    }
                    return false;
                })));
            return true;
        }

        private static bool NumFmt(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            NumberFormat fmt;
            var fmtstr = args[0].AsString();
            if (!Util.TryParseMode(fmtstr, out fmt))
            {
                throw Error(source, tagname, "Invalid number format '\{fmtstr}'");
            }
            vm.NumberFormatter.NumberFormat = fmt;
            return false;
        }

        private static bool NumFmtScope(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            NumberFormat fmt;
            var fmtstr = args[0].AsString();
            if (!Util.TryParseMode(fmtstr, out fmt))
            {
                throw Error(source, tagname, "Invalid number format '\{fmtstr}'");
            }
            var oldFormat = vm.NumberFormatter.NumberFormat;
            vm.PushState(VM.State.CreateSub(source, args[1].AsPattern(), vm, vm.CurrentState.Output)
            .Pre(new DelegateBlueprint(vm, _ =>
            {
                _.NumberFormatter.NumberFormat = fmt;
                return false;
            }))
            .Post(new DelegateBlueprint(vm, _ =>
            {
                _.NumberFormatter.NumberFormat = oldFormat;
                return false;
            })));
            return true;
        }

        private static bool Digits(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            BinaryFormat fmtType;
            var fmtParts = args[0].AsString().Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (fmtParts.Length == 0) throw Error(source, tagname, "Expected format string.");
            if (fmtParts.Length > 2) throw Error(source, tagname, "Unrecognized format string.");
            bool hasDigitCount = fmtParts.Length == 2;
            if (!Util.TryParseMode(fmtParts[0], out fmtType))
            {
                throw Error(source, tagname, "Invalid digit format '\{fmtParts[0]}'");
            }
            vm.NumberFormatter.BinaryFormat = fmtType;
            if (hasDigitCount)
            {
                int digits;
                if (!Int32.TryParse(fmtParts[1], out digits) || digits <= 0)
                    throw Error(source, tagname, "Digit count must be an integer whose value is greater than zero.");
                vm.NumberFormatter.BinaryFormatDigits = digits;
            }
            return false;
        }

        private static bool DigitsScope(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            BinaryFormat fmt;
            var fmtstr = args[0].AsString();
            if (!Util.TryParseMode(fmtstr, out fmt))
            {
                throw Error(source, tagname, "Invalid digit format '\{fmtstr}'");
            }
            var oldfmt = vm.NumberFormatter.BinaryFormat;
            vm.PushState(VM.State.CreateSub(source, args[1].AsPattern(), vm, vm.CurrentState.Output)
                .Pre(new DelegateBlueprint(vm, _ =>
                {
                    _.NumberFormatter.BinaryFormat = fmt;
                    return false;
                }))
                .Post(new DelegateBlueprint(vm, _ =>
                {
                    _.NumberFormatter.BinaryFormat = oldfmt;
                    return false;
                })));
            return true;
        }

        private static bool Endian(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            Endianness fmt;
            var fmtstr = args[0].AsString();
            if (!Util.TryParseMode(fmtstr, out fmt))
            {
                throw Error(source, tagname, "Invalid endianness '\{fmtstr}'");
            }
            vm.NumberFormatter.Endianness = fmt;
            return false;
        }

        private static bool Arg(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            if (!vm.SubArgStack.Any())
                throw Error(source, tagname, "Tried to access arguments outside of a subroutine body.");

            Argument arg;
            var argName = args[0].AsString().Trim();
            if (!vm.SubArgStack.Peek().TryGetValue(argName, out arg))
                throw Error(source, tagname, "Could not find argument '\{argName}'.");

            // Argument is string
            if (arg.Flags == ParamFlags.None)
            {
                vm.Print(arg.AsString());
                return false;
            }

            // Argument is tokens
            vm.PushState(VM.State.CreateSub(source, arg.AsPattern(), vm, vm.CurrentState.Output));
            return true;
        }

        private static bool ReplaceGroup(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            vm.Print(vm.GetMatchString(args[0].AsString()));
            return false;
        }

        private static bool ReplaceMatch(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            vm.Print(vm.GetMatchString());
            return false;
        }

        private static bool Alt(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            var testState = VM.State.CreateSub(source, args[0].AsPattern(), vm,
                vm.CurrentState.Output);
            testState.Post(new AltBlueprint(vm, testState, args[1].AsPattern()));
            vm.PushState(testState);
            return true;
        }

        private static bool Any(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            var testState = VM.State.CreateSub(source, args[0].AsPattern(), vm,
                vm.CurrentState.Output);
            testState.Post(new AnyBlueprint(vm, testState, args[1].AsPattern()));
            vm.PushState(testState);
            return true;
        }

        private static bool RepCount(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            if (vm.CurrentRepeater == null) throw Error(source, tagname, "No active repeater.");
            vm.Print(vm.FormatNumber(vm.CurrentRepeater.Count));
            return false;
        }

        private static bool RepIndex(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            if (vm.CurrentRepeater == null) throw Error(source, tagname, "No active repeaters.");
            vm.Print(vm.FormatNumber(vm.CurrentRepeater.Index));
            return false;
        }

        private static bool RepNum(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            if (vm.CurrentRepeater == null) throw Error(source, tagname, "No active repeaters.");
            vm.Print(vm.FormatNumber(vm.CurrentRepeater.Index + 1));
            return false;
        }

        private static bool Nth(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            int offset, interval;
            if (!ParseInt(args[0].AsString(), out interval))
            {
                throw Error(source, tagname, "Invalid interval value.");
            }

            if (interval <= 0)
            {
                throw Error(source, tagname, "Interval must be greater than zero.");
            }

            if (!ParseInt(args[1].AsString(), out offset))
            {
                throw Error(source, tagname, "Invalid offset value.");
            }

            if (vm.CurrentRepeater == null || !vm.CurrentRepeater.Nth(offset, interval)) return false;
            vm.PushState(VM.State.CreateSub(source, args[2].AsPattern(), vm, vm.CurrentState.Output));
            return true;
        }

        private static bool NthSimple(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            int interval;
            if (!ParseInt(args[0].AsString(), out interval))
            {
                throw Error(source, tagname, "Invalid interval value.");
            }

            if (interval <= 0)
            {
                throw Error(source, tagname, "Interval must be greater than zero.");
            }

            if (vm.CurrentRepeater == null || !vm.CurrentRepeater.Nth(0, interval)) return false;
            vm.PushState(VM.State.CreateSub(source, args[1].AsPattern(), vm, vm.CurrentState.Output));
            return true;
        }

        private static bool Even(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            if (vm.CurrentRepeater == null || !vm.CurrentRepeater.IsEven) return false;
            vm.PushState(VM.State.CreateSub(source, args[0].AsPattern(), vm, vm.CurrentState.Output));
            return true;
        }

        private static bool Odd(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            if (vm.CurrentRepeater == null || !vm.CurrentRepeater.IsOdd) return false;
            vm.PushState(VM.State.CreateSub(source, args[0].AsPattern(), vm, vm.CurrentState.Output));
            return true;
        }

        private static bool NotMiddle(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            if (vm.CurrentRepeater == null || !(vm.CurrentRepeater.IsFirst || vm.CurrentRepeater.IsLast)) return false;
            vm.PushState(VM.State.CreateSub(source, args[0].AsPattern(), vm, vm.CurrentState.Output));
            return true;
        }

        private static bool NotLast(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            if (vm.CurrentRepeater == null || vm.CurrentRepeater.IsLast) return false;
            vm.PushState(VM.State.CreateSub(source, args[0].AsPattern(), vm, vm.CurrentState.Output));
            return true;
        }

        private static bool NotFirst(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            if (vm.CurrentRepeater == null || vm.CurrentRepeater.IsFirst) return false;
            vm.PushState(VM.State.CreateSub(source, args[0].AsPattern(), vm, vm.CurrentState.Output));
            return true;
        }

        private static bool Middle(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            if (vm.CurrentRepeater == null || vm.CurrentRepeater.IsLast || vm.CurrentRepeater.IsFirst) return false;
            vm.PushState(VM.State.CreateSub(source, args[0].AsPattern(), vm, vm.CurrentState.Output));
            return true;
        }

        private static bool Last(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            if (vm.CurrentRepeater == null || !vm.CurrentRepeater.IsLast) return false;
            vm.PushState(VM.State.CreateSub(source, args[0].AsPattern(), vm, vm.CurrentState.Output));
            return true;
        }

        private static bool First(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            if (vm.CurrentRepeater == null || !vm.CurrentRepeater.IsFirst) return false;
            vm.PushState(VM.State.CreateSub(source, args[0].AsPattern(), vm, vm.CurrentState.Output));
            return true;
        }

        private static bool Step(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            vm.SyncStep(args[0].AsString());
            return false;
        }

        private static bool Unpin(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            vm.SyncUnpin(args[0].AsString());
            return false;
        }

        private static bool Pin(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            vm.SyncPin(args[0].AsString());
            return false;
        }

        private static bool Desync(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            vm.Desync();
            return false;
        }

        private static bool SyncCreateApply(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            var typeStr = args[1].AsString();
            SyncType type;
            if (!Util.TryParseMode(typeStr, out type))
            {
                throw Error(source, tagname, "Invalid synchronizer type: '\{typeStr}'");
            }
            vm.SyncCreateApply(args[0].AsString(), type);
            return false;
        }

        private static bool SyncCreate(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            var typeStr = args[1].AsString();
            SyncType type;
            if (!Util.TryParseMode(typeStr, out type))
            {
                throw Error(source, tagname, "Invalid synchronizer type: '\{typeStr}'");
            }
            vm.SyncCreate(args[0].AsString(), type);
            return false;
        }

        private static bool SyncApply(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            var syncName = args[0].AsString();
            if (!vm.SyncApply(syncName))
            {
                throw Error(source, tagname, "Tried to use nonexistent synchronizer '\{syncName}");
            }
            return false;
        }

        private static bool SyncSeed(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {

            var syncName = args[0].AsString();
            if (!vm.SyncApply(syncName))
            {
                throw Error(source, tagname, "Tried to use nonexistent synchronizer '\{syncName}");
            }
            return false;
        }

        private static bool Chance(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            int a;
            if (!ParseInt(args[0].AsString(), out a))
            {
                throw Error(source, tagname, "Invalid chance number.");
            }
            vm.SetChance(a);
            return false;
        }

        private static bool After(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            vm.NextBlockAttribs.After = args[0].AsPattern();
            return false;
        }

        private static bool Before(VM vm, RantPattern source, Stringe tagName, Argument[] args)
        {
            vm.NextBlockAttribs.Before = args[0].AsPattern();
            return false;
        }

        private static bool Number(VM vm, RantPattern source, Stringe tagName, Argument[] args)
        {
            int a, b;
            if (!ParseInt(args[0].AsString(), out a) || !ParseInt(args[1].AsString(), out b))
            {
                throw Error(source, tagName, "Range values could not be parsed. They must be numbers.");
            }
            vm.Print(vm.FormatNumber(vm.RNG.Next(a, b + 1)));
            return false;
        }

        private static bool Separator(VM vm, RantPattern source, Stringe tagName, Argument[] args)
        {
            vm.NextBlockAttribs.Separator = args[0].AsPattern();
            return false;
        }

        private static bool Repeat(VM vm, RantPattern source, Stringe tagName, Argument[] args)
        {
            var reps = args[0].AsString().Trim();
            if (String.Equals(reps, "each", StringComparison.OrdinalIgnoreCase))
            {
                vm.NextBlockAttribs.Repetitons = Repeater.Each;
                return false;
            }

            int num;
            if (!ParseInt(reps, out num))
            {
                throw Error(source, tagName, "Invalid repetition value '\{reps}' - must be a number.");
            }
            if (num < 0)
            {
                throw Error(source, tagName, "Repetition value cannot be negative.");
            }

            vm.NextBlockAttribs.Repetitons = num;
            return false;
        }

        private static bool RhymeMode(VM vm, RantPattern source, Stringe tagname, Argument[] args)
        {
            Vocabulary.RhymeType rhyme;
            List<Vocabulary.RhymeType> allowedRhymes = new List<Vocabulary.RhymeType>();
            string[] rhymeModes = args[0].AsString().ToLower().Split(' ');
            foreach (string rhymeMode in rhymeModes)
            {
                if (!Util.TryParseMode(rhymeMode, out rhyme))
                {
                    throw Error(source, tagname, "Invalid rhyme mode '\{rhymeMode}'");
                }
                allowedRhymes.Add(rhyme);
            }
            vm.QueryState.Rhymer.AllowedRhymes = allowedRhymes.ToArray();
            return false;
        }
    }

    /// <summary>
    /// Represents a group of related Rant function signatures.
    /// </summary>
    internal class RantFuncSigs
    {
        private readonly IEnumerable<RantFunc> _defs;

        public RantFuncSigs(params RantFunc[] defs)
        {
            _defs = defs.OrderByDescending(d => d.MinArgCount).ThenBy(d => d.ParamCount);
        }

        public RantFunc GetSignature(int paramCount)
        {
            return _defs.FirstOrDefault(d => d.HasMultiFlag ? paramCount >= d.MinArgCount : paramCount == d.ParamCount);
        }

        public static implicit operator RantFuncSigs(RantFunc def)
        {
            return new RantFuncSigs(def);
        }
    }

    /// <summary>
    /// Represents a Rant function signature with a specific set of parameters.
    /// </summary>
    internal class RantFunc
    {
        private readonly int _paramCount;
        private readonly int _minArgCount;
        private readonly bool _hasMultiFlag;
        private readonly ParamFlags[] _parameters;
        private readonly RantFuncDelegate _func;

        public ParamFlags[] Parameters => _parameters;

        public int ParamCount => _paramCount;

        public int MinArgCount => _minArgCount;

        public bool HasMultiFlag => _hasMultiFlag;

        public RantFunc(RantFuncDelegate func, params ParamFlags[] parameters)
        {
            if (parameters.Where((t, i) => i < parameters.Length - 1 && (t & ParamFlags.Multi) == ParamFlags.Multi).Any())
                throw new ArgumentException("The flag 'ParamType.Multi' is only valid on the last parameter.");

            _parameters = parameters;
            _hasMultiFlag = parameters.Any() && (parameters.Last() & ParamFlags.Multi) == ParamFlags.Multi;
            _paramCount = parameters.Length;
            _minArgCount = _hasMultiFlag ? _paramCount - 1 : _paramCount;
            _func = func;
        }

        public bool Invoke(VM vm, RantPattern source, Stringe tagName, Argument[] args)
        {
            return _func(vm, source, tagName, args);
        }
    }
}