using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Rant.Engine.Formatters;
using Rant.Engine.Metadata;
using Rant.Engine.Syntax;

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
		[RantDescription("Runs a pattern if the current block iteration is the last.")]
		private static IEnumerator<RantAction> Last(Sandbox sb, RantAction action)
		{
			if (!sb.Blocks.Any()) yield break;
			var block = sb.Blocks.Peek();
            if (block.Iteration == block.Count) yield return action;
		}

		[RantFunction("repnum", "rn")]
		[RantDescription("Prints the iteration number of the current block.")]
		private static void RepNum(Sandbox sb)
		{
			if (!sb.Blocks.Any()) return;
			sb.Print(sb.Blocks.Peek().Iteration);
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
	}
}