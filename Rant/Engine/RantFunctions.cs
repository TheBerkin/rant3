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
		private static readonly Dictionary<string, RantFunctionInfo> FunctionTable = 
			new Dictionary<string, RantFunctionInfo>(StringComparer.InvariantCultureIgnoreCase);

		internal static void Load()
		{
			if (Loaded) return;
			foreach (var method in typeof(RantFunctions).GetMethods(BindingFlags.Static | BindingFlags.NonPublic))
			{
				if (!method.IsStatic) continue;
				var attr = method.GetCustomAttributes().OfType<RantFunctionAttribute>().FirstOrDefault();
				if (attr == null) continue;
				var name = String.IsNullOrEmpty(attr.Name) ? method.Name.ToLower() : attr.Name;
				var info = new RantFunctionInfo(name, method);
				if (Util.ValidateName(name)) FunctionTable[name] = info;
				foreach(var alias in attr.Aliases.Where(Util.ValidateName))
					FunctionTable[alias] = info;
			}
			Loaded = true;
		}

		public static RantFunctionInfo GetFunction(string name)
		{
			RantFunctionInfo func;
			if (!FunctionTable.TryGetValue(name, out func)) return null;
			return func;
		}

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

		[RantFunction("tonum")]
		[RantDescription("Formats an input string using the current number format settings and prints the result.")]
		private static void Number(Sandbox sb, string input)
		{
			double number;
			sb.Print(double.TryParse(input, out number) ? number : 0);
		}

		[RantFunction("numfmt")]
		private static void NumberFormat(Sandbox sb, NumberFormat format)
		{
			foreach (var channel in sb.CurrentOutput.GetActive())
			{
				channel.NumberFormatter.NumberFormat = format;
			}
		}

		[RantFunction]
		private static void Digits(Sandbox sb, BinaryFormat format, int digits)
		{
			foreach (var channel in sb.CurrentOutput.GetActive())
			{
				channel.NumberFormatter.BinaryFormat = format;
				channel.NumberFormatter.BinaryFormatDigits = digits;
			}
		}

		[RantFunction]
		private static void Endian(Sandbox sb, Endianness endianness)
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
			sb.CurrentBlockAttribs.Repetitons = times;
		}

		[RantFunction("sep", "s")]
		[RantDescription("Sets the separator pattern for the next block.")]
		private static void Sep(Sandbox sb, 
			[RantDescription("The separator pattern to run between iterations of the next block.")]
			RantAction separatorAction)
		{
			sb.CurrentBlockAttribs.Separator = separatorAction;
		}

		[RantFunction("rs")]
		private static void RepSep(Sandbox sb, int times, RantAction separator)
		{
			sb.CurrentBlockAttribs.Repetitons = times;
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
			int chance)
		{
			sb.CurrentBlockAttribs.Chance = chance < 0 ? 0 : chance > 100 ? 100 : chance;
		}


		[RantFunction("case", "caps")]
		[RantDescription("Changes the capitalization mode for all open channels.")]
		private static void Case(Sandbox sb, Case textCase)
		{
			sb.CurrentOutput.SetCase(textCase);
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is the first.")]
		private static IEnumerator<RantAction> First(Sandbox sb, RantAction action)
		{
			if (!sb.Blocks.Any()) yield break;
			if (sb.Blocks.Peek().Iteration == 1) yield return action;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is not the first.")]
		private static IEnumerator<RantAction> NotFirst(Sandbox sb, RantAction action)
		{
			if (!sb.Blocks.Any()) yield break;
			if (sb.Blocks.Peek().Iteration > 1) yield return action;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is the last.")]
		private static IEnumerator<RantAction> Last(Sandbox sb, RantAction action)
		{
			if (!sb.Blocks.Any()) yield break;
			var block = sb.Blocks.Peek();
            if (block.Iteration == block.Count) yield return action;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is not the last.")]
		private static IEnumerator<RantAction> NotLast(Sandbox sb, RantAction action)
		{
			if (!sb.Blocks.Any()) yield break;
			var block = sb.Blocks.Peek();
			if (block.Iteration < block.Count) yield return action;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is neither the first nor last.")]
		private static IEnumerator<RantAction> Middle(Sandbox sb, RantAction action)
		{
			if (!sb.Blocks.Any()) yield break;
			var block = sb.Blocks.Peek();
			if (block.Iteration > 1 && block.Iteration < block.Count) yield return action;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is either the first or last.")]
		private static IEnumerator<RantAction> Ends(Sandbox sb, RantAction action)
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
		private static IEnumerator<RantAction> Odd(Sandbox sb, RantAction action)
		{
			if (!sb.Blocks.Any()) yield break;
			if (sb.Blocks.Peek().Iteration % 2 != 0) yield return action;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is an even number.")]
		private static IEnumerator<RantAction> Even(Sandbox sb, RantAction action)
		{
			if (!sb.Blocks.Any()) yield break;
			if (sb.Blocks.Peek().Iteration % 2 == 0) yield return action;
		}

		[RantFunction]
		[RantDescription("Returns the specified argument from the current subroutine.")]
		private static IEnumerator<RantAction> Arg(Sandbox sb, string name)
		{
			if (!sb.SubroutineArgs.Any()) yield break;
			var args = sb.SubroutineArgs.Peek();
			if (args.ContainsKey(name))
				yield return args[name];
		}

		[RantFunction]
		private static void Match(Sandbox sb)
		{
			if (!sb.RegexMatches.Any()) return;
			sb.Print(sb.RegexMatches.Peek().Value);
		}

		[RantFunction]
		private static void Group(Sandbox sb, string groupName)
		{
			if (!sb.RegexMatches.Any()) return;
			sb.Print(sb.RegexMatches.Peek().Groups[groupName].Value);
		}

		[RantFunction]
		private static void Rhyme(Sandbox sb, RhymeFlags flags)
		{
			sb.QueryState.Rhymer.AllowedRhymes = flags;
		}

		[RantFunction("sync", "x")]
		[RantDescription("Creates and applies a synchronizer with the specified name and type.")]
		private static void Sync(Sandbox sb, string name, SyncType type)
		{
			sb.SyncManager.Create(name, type, true);
		}

		[RantFunction("xpin")]
		[RantDescription("Pins a synchronizer.")]
		private static void SyncPin(Sandbox sb, string name)
		{
			sb.SyncManager.SetPinned(name, true);
		}

		[RantFunction("xunpin")]
		[RantDescription("Pins a synchronizer.")]
		private static void SyncUnpin(Sandbox sb, string name)
		{
			sb.SyncManager.SetPinned(name, false);
		}

		[RantFunction("xstep")]
		[RantDescription("Iterates a synchronizer.")]
		private static void SyncStep(Sandbox sb, string name)
		{
			sb.SyncManager.Step(name);
		}

		[RantFunction]
		[RantDescription("Resets a synchronizer.")]
		private static void SyncReset(Sandbox sb, string name)
		{
			sb.SyncManager.Reset(name);
		}
	}
}