using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Rant.Engine.Constructs;
using Rant.Engine.Formatters;
using Rant.Engine.Metadata;
using Rant.Engine.Syntax;
using Rant.Vocabulary;

namespace Rant.Engine
{
	// Methods representing Rant functions must be marked with [RantFunction] attribute to get registered by the engine.
	// They may return either void or IEnumerator<RantAction> depending on your needs.
	internal static class RantFunctions
	{
		private static bool Loaded = false;
		private static readonly Dictionary<string, RantFunctionGroup> FunctionTable = 
			new Dictionary<string, RantFunctionGroup>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly Dictionary<string, string> AliasTable = 
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase); 

		static RantFunctions()
		{
			Load();
		}

		internal static void Load()
		{
			if (Loaded) return;
			foreach (var method in typeof(RantFunctions).GetMethods(BindingFlags.Static | BindingFlags.NonPublic))
			{
				if (!method.IsStatic) continue;
				var attr = method.GetCustomAttributes(typeof(RantFunctionAttribute), false).FirstOrDefault() as RantFunctionAttribute;
				if (attr == null) continue;
				var name = String.IsNullOrEmpty(attr.Name) ? method.Name.ToLower() : attr.Name;
				var descAttr = method.GetCustomAttributes(typeof(RantDescriptionAttribute), false).FirstOrDefault() as RantDescriptionAttribute;
				var info = new RantFunctionInfo(name, descAttr?.Description ?? String.Empty, method);
				if (Util.ValidateName(name)) RegisterFunction(info);
				foreach(var alias in attr.Aliases.Where(Util.ValidateName))
					RegisterAlias(alias, info.Name);
			}
			Loaded = true;
		}

	    public static IEnumerable<string> GetAliases(string funcName) =>
	        AliasTable.Where(pair => String.Equals(funcName, pair.Value, StringComparison.InvariantCultureIgnoreCase))
	            .Select(pair => pair.Key);

	    private static void RegisterAlias(string alias, string funcName)
	    {
	        AliasTable[alias] = funcName;
	    }

	    private static string ResolveAlias(string alias)
	    {
	        string name;
	        return AliasTable.TryGetValue(alias, out name) ? name : alias;
	    }

        private static void RegisterFunction(RantFunctionInfo func)
	    {
	        RantFunctionGroup group;
	        if (!FunctionTable.TryGetValue(func.Name, out group))
	            group = FunctionTable[func.Name] = new RantFunctionGroup(func.Name);
            group.Add(func);
	    }

		public static bool FunctionExists(string name) => 
            AliasTable.ContainsKey(name) || FunctionTable.ContainsKey(name);

	    public static IEnumerable<IRantFunctionGroup> GetFunctions() => FunctionTable.Select(item => item.Value as IRantFunctionGroup); 

		public static IEnumerable<string> GetFunctionNames() => 
            FunctionTable.Select(item => item.Key);

        public static IEnumerable<string> GetFunctionNamesAndAliases() => 
            FunctionTable.Select(item => item.Key).Concat(AliasTable.Keys);

        public static string GetFunctionDescription(string funcName, int paramc) => 
            GetFunctionGroup(funcName)?.GetFunction(paramc)?.Description ?? String.Empty;

		public static RantFunctionGroup GetFunctionGroup(string name)
		{
		    RantFunctionGroup group;
			return !FunctionTable.TryGetValue(ResolveAlias(name), out group) ? null : group;
		}

	    public static RantFunctionInfo GetFunction(string name, int paramc) =>
	        GetFunctionGroup(name)?.GetFunction(paramc);

		[RantFunction("num", "n")]
		[RantDescription("Prints a random number between the specified minimum and maximum bounds.")]
		private static void Number(Sandbox sb, 
			[RantDescription("The minimum value of the number to generate.")]
			int min, 
			[RantDescription("The maximum value of the number to generate.")]
			int max)
		{
			sb.Print(sb.RNG.Next(min, max + 1));
		}

		[RantFunction("num")]
		[RantDescription("Formats an input string using the current number format settings and prints the result.")]
		private static void Number(Sandbox sb, 
            [RantDescription("The string to convert into a number.")]
            string input)
		{
			double number;
			sb.Print(double.TryParse(input, out number) ? number : 0);
		}

		[RantFunction("numfmt")]
        [RantDescription("Sets the current number formatting mode.")]
		private static void NumberFormat(Sandbox sb, 
            [RantDescription("The number format to use.")]
            NumberFormat format)
		{
			foreach (var channel in sb.CurrentOutput.GetActive())
			{
				channel.NumberFormatter.NumberFormat = format;
			}
		}

        [RantFunction("numfmt")]
        [RantDescription("Runs the specified pattern under a specific number formatting mode.")]
        private static IEnumerator<RantAction> NumberFormatRange(Sandbox sb,
            [RantDescription("The number format to use.")]
            NumberFormat format, 
            [RantDescription("The pattern to run.")]
            RantAction rangeAction)
        {
            var oldChannelMap = sb.CurrentOutput.GetActive()
                .ToDictionary(ch => ch.Name, ch => ch.NumberFormatter.NumberFormat);

            foreach (var channel in sb.CurrentOutput.GetActive())
            {
                channel.NumberFormatter.NumberFormat = format;
            }

            yield return rangeAction;

            NumberFormat fmt;
            foreach (var channel in sb.CurrentOutput.GetActive())
            {
                if (!oldChannelMap.TryGetValue(channel.Name, out fmt)) continue;
                channel.NumberFormatter.NumberFormat = fmt;
            }
        }

        [RantFunction]
        [RantDescription("Specifies the current digit formatting mode for numbers.")]
		private static void Digits(Sandbox sb, 
            [RantDescription("The digit format to use.")]
            BinaryFormat format, 
            [RantDescription("The digit count to associate with the mode.")]
            int digits)
		{
			foreach (var channel in sb.CurrentOutput.GetActive())
			{
				channel.NumberFormatter.BinaryFormat = format;
				channel.NumberFormatter.BinaryFormatDigits = digits;
			}
		}

		[RantFunction]
        [RantDescription("Sets the current endianness for hex and binary formatted numbers.")]
		private static void Endian(Sandbox sb, 
            [RantDescription("The endianness to use.")]
            Endianness endianness)
		{
			foreach (var channel in sb.CurrentOutput.GetActive())
			{
				channel.NumberFormatter.Endianness = endianness;
			}
		}

		[RantFunction("rep", "r")]
		[RantDescription("Sets the repetition count for the next block.")]
		private static void Rep(Sandbox sb, 
			[RantDescription("The number of times to repeat the next block.")]
			int times)
		{
			sb.CurrentBlockAttribs.Repetitions = times;
		}

		[RantFunction]
		[RantDescription("Sets the repetition count to the number of items in the next block.")]
		private static void RepEach(Sandbox sb)
		{
			sb.CurrentBlockAttribs.RepEach = true;
		}

		[RantFunction("sep", "s")]
		[RantDescription("Sets the separator pattern for the next block.")]
		private static void Sep(Sandbox sb, 
			[RantDescription("The separator pattern to run between iterations of the next block.")]
			RantAction separator)
		{
			sb.CurrentBlockAttribs.Separator = separator;
		}

		[RantFunction("rs")]
        [RantDescription("Sets the repetitions and separator for the next block. A combination of rep and sep.")]
		private static void RepSep(Sandbox sb,
            [RantDescription("The number of times to repeat the next block.")]
            int times,
            [RantDescription("The separator pattern to run between iterations of the next block.")]
            RantAction separator)
		{
			sb.CurrentBlockAttribs.Repetitions = times;
			sb.CurrentBlockAttribs.Separator = separator;
		}

		[RantFunction]
		[RantDescription("Sets the prefix pattern for the next block.")]
		private static void Before(Sandbox sb, 
			[RantDescription("The pattern to run before each iteration of the next block.")]
			RantAction beforeAction)
		{
			sb.CurrentBlockAttribs.Before = beforeAction;
		}

		[RantFunction]
		[RantDescription("Sets the postfix pattern for the next block.")]
		private static void After(Sandbox sb, 
			[RantDescription("The pattern to run after each iteration of the next block.")]
			RantAction afterAction)
		{
			sb.CurrentBlockAttribs.After = afterAction;
		}

		[RantFunction]
		[RantDescription("Modifies the likelihood that the next block will execute. Specified in percentage.")]
		private static void Chance(Sandbox sb, 
			[RantDescription("The percent probability that the next block will execute.")]
			double chance)
		{
			sb.CurrentBlockAttribs.Chance = chance < 0 ? 0 : chance > 100 ? 100 : chance;
		}


		[RantFunction("case", "caps")]
		[RantDescription("Changes the capitalization mode for all open channels.")]
		private static void Case(Sandbox sb, 
            [RantDescription("The capitalization mode to use.")]
            Case textCase)
		{
			sb.CurrentOutput.SetCase(textCase);
		}

	    [RantFunction]
	    [RantDescription("Infers the capitalization of a given string and sets the capitalization mode to match it.")]
	    private static void CapsInfer(Sandbox sb, 
            [RantDescription("A string that is capitalized in the format to be set.")]
            string sample)
	    {
	        var output = sb.CurrentOutput;
	        if (String.IsNullOrEmpty(sample))
	        {
	            output.SetCase(Formatters.Case.None);
	            return;
	        }
	        var words = sample.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
	        if (words.Length == 1)
	        {
	            var word = words[0];
	            if (word.Length == 1)
	            {
	                if (Char.IsUpper(word[0]))
	                {
	                    output.SetCase(Formatters.Case.First);
	                }
	                else
	                {
	                    output.SetCase(Formatters.Case.None);
	                }
	            }
	            else if (Util.IsUppercase(word))
	            {
	                output.SetCase(Formatters.Case.Upper);
	            }
                else if (Char.IsUpper(word.SkipWhile(c => !Char.IsLetterOrDigit(c)).FirstOrDefault()))
                {
                    output.SetCase(Formatters.Case.First);
                }
                else
                {
                    output.SetCase(Formatters.Case.None);
                }
            }
	        else
	        {
                // No letters? Forget it.
	            if (!sample.Any(Char.IsLetter))
	            {
	                output.SetCase(Formatters.Case.None);
	                return;
	            }

                // Is all-caps?
	            if (Util.IsUppercase(sample))
	            {
	                output.SetCase(Formatters.Case.Upper);
	                return;
	            }
                
                var sentences = sample.Split(new[] { '.', '?', '!' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(str => str.Trim())
                    .Where(str =>
                    !String.IsNullOrEmpty(str)
                     && !Char.IsDigit(str[0])).ToArray();

                // All words capitalized?
                var lwords = words.Where(w => Char.IsLetter(w[0])).ToArray();
	            if (lwords.Any() && (sentences.Length == 1 || sentences.Any(s => s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length > 1)))
	            {
	                if (lwords.All(lw => Char.IsUpper(lw[0])))
	                {
                        output.SetCase(Formatters.Case.Word);
	                    return;
	                }

	                if (lwords.All(lw => Char.IsLower(lw[0]) == sb.Format.Excludes(lw)))
	                {
	                    output.SetCase(Formatters.Case.Title);
	                    return;
	                }
	            }

	            // All sentences capitalized?
                bool all = true;
	            bool none = true;
	            foreach (var sentence in sentences)
	            {
	                bool isCapitalized = Char.IsUpper(sentence.SkipWhile(c => !Char.IsLetter(c)).FirstOrDefault());
	                all = all && isCapitalized;
	                none = none && !isCapitalized;
	            }

	            if (sentences.Length > 1 && all)
	            {
	                output.SetCase(Formatters.Case.Sentence);
	            }
                else if (none)
                {
                    output.SetCase(Formatters.Case.Lower);
                }
                else if (Char.IsUpper(sample.SkipWhile(c => !Char.IsLetterOrDigit(c)).FirstOrDefault()))
                {
                    output.SetCase(Formatters.Case.First);
                }
                else
                {
                    output.SetCase(Formatters.Case.None);
                }
	        }
	    }

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is the first.")]
		private static IEnumerator<RantAction> First(Sandbox sb, 
            [RantDescription("The pattern to run when the condition is met.")]
            RantAction action)
		{
			if (!sb.Blocks.Any()) yield break;
			if (sb.Blocks.Peek().Iteration == 1) yield return action;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is not the first.")]
		private static IEnumerator<RantAction> NotFirst(Sandbox sb,
            [RantDescription("The pattern to run when the condition is met.")]
            RantAction action)
		{
			if (!sb.Blocks.Any()) yield break;
			if (sb.Blocks.Peek().Iteration > 1) yield return action;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is the last.")]
		private static IEnumerator<RantAction> Last(Sandbox sb,
            [RantDescription("The pattern to run when the condition is met.")]
            RantAction action)
		{
			if (!sb.Blocks.Any()) yield break;
			var block = sb.Blocks.Peek();
            if (block.Iteration == block.Count) yield return action;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is not the last.")]
		private static IEnumerator<RantAction> NotLast(Sandbox sb,
            [RantDescription("The pattern to run when the condition is met.")]
            RantAction action)
		{
			if (!sb.Blocks.Any()) yield break;
			var block = sb.Blocks.Peek();
			if (block.Iteration < block.Count) yield return action;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is neither the first nor last.")]
		private static IEnumerator<RantAction> Middle(Sandbox sb,
            [RantDescription("The pattern to run when the condition is met.")]
            RantAction action)
		{
			if (!sb.Blocks.Any()) yield break;
			var block = sb.Blocks.Peek();
			if (block.Iteration > 1 && block.Iteration < block.Count) yield return action;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is either the first or last.")]
		private static IEnumerator<RantAction> Ends(Sandbox sb,
            [RantDescription("The pattern to run when the condition is met.")]
            RantAction action)
		{
			if (!sb.Blocks.Any()) yield break;
			var block = sb.Blocks.Peek();
			if (block.Iteration == 1 || block.Iteration == block.Count) yield return action;
		}

		[RantFunction("repnum", "rn")]
		[RantDescription("Prints the iteration number of the current block.")]
		private static void RepNum(Sandbox sb)
		{
			if (!sb.Blocks.Any()) return;
			sb.Print(sb.Blocks.Peek().Iteration);
		}

		[RantFunction("repelapsed", "re")]
		[RantDescription("Prints the number of iterations remaining to be performed on the current block.")]
		private static void RepElapsed(Sandbox sb)
		{
			if (!sb.Blocks.Any()) return;
			sb.Print(sb.Blocks.Peek().Iteration - 1);
		}

		[RantFunction("repcount", "rc")]
		[RantDescription("Prints the repetition count of the current block.")]
		private static void RepCount(Sandbox sb)
		{
			if (!sb.Blocks.Any()) return;
			sb.Print(sb.Blocks.Peek().Count);
		}

		[RantFunction("reprem", "rr")]
		[RantDescription("Prints the number of remaining repetitions queued after the current one.")]
		private static void RepRem(Sandbox sb)
		{
			if (!sb.Blocks.Any()) return;
			var block = sb.Blocks.Peek();
			sb.Print(block.Count - block.Iteration);
		}

		[RantFunction("repqueued", "rq")]
		[RantDescription("Prints the number of repetitions remaining to be completed on the current block.")]
		private static void RepQueued(Sandbox sb)
		{
			if (!sb.Blocks.Any()) return;
			var block = sb.Blocks.Peek();
			sb.Print(block.Count - (block.Iteration - 1));
		}

		[RantFunction]
		[RantDescription("Prints the number of currently active blocks.")]
		private static void Depth(Sandbox sb)
		{
			sb.Print(sb.Blocks.Count);
		}

		[RantFunction("index", "i")]
		[RantDescription("Prints the zero-based index of the block item currently being executed.")]
		private static void Index(Sandbox sb)
		{
			if (!sb.Blocks.Any()) return;
			sb.Print(sb.Blocks.Peek().Index);
		}

		[RantFunction("index1", "i1")]
		[RantDescription("Prints the one-based index of the block item currently being executed.")]
		private static void IndexOne(Sandbox sb)
		{
			if (!sb.Blocks.Any()) return;
			sb.Print(sb.Blocks.Peek().Index + 1);
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is an odd number.")]
		private static IEnumerator<RantAction> Odd(Sandbox sb,
            [RantDescription("The pattern to run when the condition is met.")]
            RantAction action)
		{
			if (!sb.Blocks.Any()) yield break;
			if (sb.Blocks.Peek().Iteration % 2 != 0) yield return action;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is an even number.")]
		private static IEnumerator<RantAction> Even(Sandbox sb,
            [RantDescription("The pattern to run when the condition is met.")]
            RantAction action)
		{
			if (!sb.Blocks.Any()) yield break;
			if (sb.Blocks.Peek().Iteration % 2 == 0) yield return action;
		}

		[RantFunction]
		[RantDescription("Returns the specified argument from the current subroutine.")]
		private static IEnumerator<RantAction> Arg(Sandbox sb,
            [RantDescription("The name of the argument to retrieve.")]
            string name)
		{
			if (!sb.SubroutineArgs.Any()) yield break;
			var args = sb.SubroutineArgs.Peek();
			if (args.ContainsKey(name))
				yield return args[name];
		}

		[RantFunction]
        [RantDescription("Retrieves and prints the current match string of the active replacer.")]
        private static void Match(Sandbox sb)
		{
			if (!sb.RegexMatches.Any()) return;
			sb.Print(sb.RegexMatches.Peek().Value);
		}

		[RantFunction]
        [RantDescription("Retrieves and prints the specified group value of the current match from the active replacer.")]
        private static void Group(Sandbox sb, 
            [RantDescription("The name of the match group whose value will be retrieved.")]
            string groupName)
		{
			if (!sb.RegexMatches.Any()) return;
			sb.Print(sb.RegexMatches.Peek().Groups[groupName].Value);
		}

		[RantFunction]
        [RantDescription("Sets the current rhyming mode for queries.")]
        private static void Rhyme(Sandbox sb,
            [RantDescription("The rhyme types to use.")]
            RhymeFlags flags)
		{
			sb.QueryState.Rhymer.AllowedRhymes = flags;
		}

		[RantFunction("sync", "x")]
		[RantDescription("Creates and applies a synchronizer with the specified name and type.")]
		private static void Sync(Sandbox sb,
            [RantDescription("The name of the synchronizer.")]
            string name,
            [RantDescription("The synchronization type to use.")]
            SyncType type)
		{
			sb.SyncManager.Create(name, type, true);
		}

		[RantFunction("xpin")]
		[RantDescription("Pins a synchronizer.")]
		private static void SyncPin(Sandbox sb,
            [RantDescription("The name of the synchronizer to pin.")]
            string name)
		{
			sb.SyncManager.SetPinned(name, true);
		}

		[RantFunction("xunpin")]
		[RantDescription("Unpins a synchronizer.")]
		private static void SyncUnpin(Sandbox sb,
            [RantDescription("The name of the synchronizer to unpin.")]
            string name)
		{
			sb.SyncManager.SetPinned(name, false);
		}

		[RantFunction("xstep")]
		[RantDescription("Iterates a synchronizer.")]
		private static void SyncStep(Sandbox sb,
            [RantDescription("The name of the synchronizer to iterate.")]
            string name)
		{
			sb.SyncManager.Step(name);
		}

		[RantFunction("xreset")]
		[RantDescription("Resets a synchronizer to its initial state.")]
		private static void SyncReset(Sandbox sb,
            [RantDescription("The name of the synchronizer to reset.")]
            string name)
		{
			sb.SyncManager.Reset(name);
		}

		[RantFunction("quote", "q")]
		[RantDescription("Surrounds the specified pattern in quotes. Nested quotes use the secondary quotes defined in the format settings.")]
		private static IEnumerator<RantAction> Quote(Sandbox sb,
            [RantDescription("The pattern to run whose output will be surrounded in quotes.")]
            RantAction quoteAction)
		{
			sb.IncreaseQuote();
			sb.PrintOpeningQuote();
			yield return quoteAction;
			sb.PrintClosingQuote();
			sb.DecreaseQuote();
		}

		[RantFunction]
		[RantDescription("Opens a new output channel with the specified name and visibility.")]
		private static void Open(Sandbox sb,
            [RantDescription("The name of the channel to open.")]
            string channelName,
            [RantDescription("The visibility to assign to the channel.")]
            ChannelVisibility visibility)
		{
			sb.CurrentOutput.OpenChannel(channelName, visibility, sb.Format);
		}

		[RantFunction]
		[RantDescription("Closes the output channel with the specified name.")]
		private static void Close(Sandbox sb,
            [RantDescription("The name of the channel to close.")]
            string channelName)
		{
			sb.CurrentOutput.CloseChannel(channelName);
		}

		[RantFunction("target", "t")]
		[RantDescription("Places a target with the specified name at the current write position.")]
		private static void Target(Sandbox sb,
            [RantDescription("The name of the target.")]
            string targetName)
		{
			sb.CurrentOutput.CreateTarget(targetName);
		}

		[RantFunction]
		[RantDescription("Appends a string to the specified target's contents.")]
		private static void Send(Sandbox sb,
            [RantDescription("The name of the target to send to.")]
            string targetName,
            [RantDescription("The string to send to the target.")]
            string value)
		{
			sb.CurrentOutput.WriteToTarget(targetName, value);
		}

        [RantFunction]
        [RantDescription("Overwrites the specified target's contents with the provided value.")]
        private static void SendOver(Sandbox sb,
            [RantDescription("The name of the target to send to.")]
            string targetName,
            [RantDescription("The string to send to the target.")]
            string value)
        {
            sb.CurrentOutput.WriteToTarget(targetName, value, true);
        }

        [RantFunction("targetval")]
        [RantDescription("Prints the current value of the specified target. This function will not spawn a target.")]
	    private static void GetTargetValue(Sandbox sb,
            [RantDescription("The name of the target whose value to print.")]
	        string targetName)
	    {
	        sb.Print(sb.CurrentOutput.GetActiveChannel().GetTargetValue(targetName));
	    }

        [RantFunction("clrt")]
        [RantDescription("Clears the contents of the specified target.")]
        private static void ClearTarget(Sandbox sb,
            [RantDescription("The name of the target to be cleared.")]
            string targetName)
        {
            sb.CurrentOutput.ClearTarget(targetName);
        }

        [RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is a multiple of the specified number.")]
		private static IEnumerator<RantAction> Nth(Sandbox sb, 
            [RantDescription("The interval at which the pattern should be run.")]
            int interval, 
            [RantDescription("The pattern to run when the condition is satisfied.")]
            RantAction pattern)
		{
			if(!sb.Blocks.Any()) yield break;
			if (sb.Blocks.Peek().Iteration % interval != 0) yield break;
			yield return pattern;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is a multiple of the specified number offset by a specific amount.")]
		private static IEnumerator<RantAction> NthO(Sandbox sb, 
            [RantDescription("The interval at which the pattern should be run.")]
            int interval, 
            [RantDescription("The number of iterations to offset the interval by.")]
            int offset, 
            [RantDescription("The pattern to run when the condition is satisfied.")]
            RantAction pattern)
		{
			if (!sb.Blocks.Any()) yield break;
			if (Util.Mod(sb.Blocks.Peek().Iteration - offset, interval) != 0) yield break;
			yield return pattern;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is not a multiple of the specified number.")]
		private static IEnumerator<RantAction> NotNth(Sandbox sb, 
            [RantDescription("The interval at which the pattern should not be run.")]
            int interval, 
            [RantDescription("The pattern to run when the condition is satisfied.")]
            RantAction pattern)
		{
			if (!sb.Blocks.Any()) yield break;
			if (sb.Blocks.Peek().Iteration % interval == 0) yield break;
			yield return pattern;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is not a multiple of the specified number offset by a specific amount.")]
		private static IEnumerator<RantAction> NotNthO(Sandbox sb, 
            [RantDescription("The interval at which the pattern should not be run.")]
            int interval, 
            [RantDescription("The number of iterations to offset the interval by.")]
            int offset, 
            [RantDescription("The pattern to run when the condition is satisfied.")]
            RantAction pattern)
		{
			if (!sb.Blocks.Any()) yield break;
			if (Util.Mod(sb.Blocks.Peek().Iteration - offset, interval) == 0) yield break;
			yield return pattern;
		}

		[RantFunction]
		[RantDescription("Sets a pattern that will run before the next block.")]
		private static void Start(Sandbox sb, 
            [RantDescription("The pattern to run before the next block.")]
            RantAction beforePattern)
		{
			sb.CurrentBlockAttribs.Start = beforePattern;
		}

		[RantFunction]
		[RantDescription("Sets a pattern that will run after the next block.")]
		private static void End(Sandbox sb, 
            [RantDescription("The pattern to run after the next block.")]
            RantAction endPattern)
		{
			sb.CurrentBlockAttribs.End = endPattern;
		}

        // TODO: Finish [persist].
        //[RantFunction]
        [RantDescription("Instructs Rant not to consume the block attributes after they are used.")]
		private static void Persist(Sandbox sb, AttribPersistence persistence)
		{
			sb.CurrentBlockAttribs.Persistence = persistence;
		}

	    [RantFunction]
	    [RantDescription("Loads and runs a pattern from cache or file.")]
	    private static IEnumerator<RantAction> Import(Sandbox sb,
            [RantDescription("The name or path of the pattern to load.")]
            string name)
	    {
	        RantAction action;

	        try
	        {
	            action = sb.Engine.GetPattern(name).Action;
	        }
	        catch (RantCompilerException e)
	        {
                throw new RantRuntimeException(sb.Pattern, sb.CurrentAction.Range, 
                    $"Failed to compile imported pattern '{name}':\n{e.Message}");
	        }
	        catch (Exception e)
	        {
	            throw new RantRuntimeException(sb.Pattern, sb.CurrentAction.Range, 
                    $"Failed to import '{name}':\n{e.Message}");
	        }

	        yield return action;
	    }

	    [RantFunction]
	    [RantDescription("Defines the specified flags.")]
	    private static void Define(Sandbox sb, 
            [RantDescription("The list of flags to define.")]
            params string[] flags)
	    {
	        foreach (var flag in flags.Where(f => !Util.IsNullOrWhiteSpace(f) && Util.ValidateName(f)))
	        {
	            sb.Engine.Flags.Add(flag);
	        }
	    }

        [RantFunction]
        [RantDescription("Undefines the specified flags.")]
        private static void Undef(Sandbox sb, 
            [RantDescription("The list of flags to undefine.")]
            params string[] flags)
        {
            foreach (var flag in flags)
            {
                sb.Engine.Flags.Remove(flag);
            }
        }

        [RantFunction]
        [RantDescription("Toggles the specified flags.")]
        private static void Toggle(Sandbox sb, params string[] flags)
        {
            foreach (var flag in flags.Where(f => !Util.IsNullOrWhiteSpace(f) && Util.ValidateName(f)))
            {
                if (sb.Engine.Flags.Contains(flag))
                {
                    sb.Engine.Flags.Remove(flag);
                }
                else
                {
                    sb.Engine.Flags.Add(flag);
                }
            }
        }

        [RantFunction]
        [RantDescription("Sets the current flag condition for [then] ... [else] calls to be true if all the specified flags are set.")]
        private static void IfDef(Sandbox sb, params string[] flags)
        {
            sb.FlagConditionExpectedResult = true;
            sb.ConditionFlags.Clear();
            foreach (var flag in flags.Where(f => !Util.IsNullOrWhiteSpace(f) && Util.ValidateName(f)))
            {
                sb.ConditionFlags.Add(flag);
            }
        }

        [RantFunction]
        [RantDescription("Sets the current flag condition for [then] ... [else] calls to be true if all the specified flags are unset.")]
        private static void IfNDef(Sandbox sb, params string[] flags)
        {
            sb.FlagConditionExpectedResult = false;
            sb.ConditionFlags.Clear();
            foreach (var flag in flags.Where(f => !Util.IsNullOrWhiteSpace(f) && Util.ValidateName(f)))
            {
                sb.ConditionFlags.Add(flag);
            }
        }

	    [RantFunction]
	    [RantDescription("Executes a pattern if the current flag condition passes.")]
	    private static IEnumerator<RantAction> Then(Sandbox sb, RantAction conditionPassPattern)
	    {
	        if (sb.Engine.Flags.All(flag => sb.ConditionFlags.Contains(flag) == sb.FlagConditionExpectedResult))
	        {
	            yield return conditionPassPattern;
	        }
	    }

        [RantFunction]
        [RantDescription("Executes a pattern if the current flag condition fails.")]
        private static IEnumerator<RantAction> Else(Sandbox sb, RantAction conditionFailPattern)
        {
            if (sb.Engine.Flags.Any(flag => sb.ConditionFlags.Contains(flag) != sb.FlagConditionExpectedResult))
            {
                yield return conditionFailPattern;
            }
        }

	    [RantFunction]
	    [RantDescription("Yields the currenty written output.")]
	    private static void Yield(Sandbox sb)
	    {
	        sb.SetYield();
	    }

	    [RantFunction]
	    [RantDescription("Branches the internal RNG according to a seed.")]
	    private static void Branch(Sandbox sb, string id)
	    {
	        sb.RNG.Branch(id.Hash());
	    }

	    [RantFunction]
	    [RantDescription("Merges the topmost branch of the internal RNG, if it has been branched at least once.")]
	    private static void Merge(Sandbox sb)
	    {
	        sb.RNG.Merge();
	    }
    }
}