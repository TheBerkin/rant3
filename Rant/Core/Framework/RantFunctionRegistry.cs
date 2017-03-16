#region License

// https://github.com/TheBerkin/Rant
// 
// Copyright (c) 2017 Nicholas Fleck
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Rant.Core.Compiler.Syntax;
using Rant.Core.Constructs;
using Rant.Core.Formatting;
using Rant.Core.Output;
using Rant.Core.Utilities;
using Rant.Metadata;
using Rant.Vocabulary.Utilities;
using Rant.Vocabulary.Querying;
using Rant.Core.ObjectModel;

// ReSharper disable UnusedMember.Local

namespace Rant.Core.Framework
{
	// Methods representing Rant functions must be marked with [RantFunction] attribute to get registered by the engine.
	// They may return either void or IEnumerator<RantAction> depending on your needs.
	internal static partial class RantFunctionRegistry
	{
		[RantFunction("num", "n")]
		[RantDescription("Prints a random number between the specified minimum and maximum bounds.")]
		private static void Number(Sandbox sb,
			[RantDescription("The minimum value of the number to generate.")] int min,
			[RantDescription("The maximum value of the number to generate.")] int max) => sb.Print(sb.RNG.Next(min, max + 1));

		[RantFunction("num")]
		[RantDescription("Formats an input string using the current number format settings and prints the result.")]
		private static void Number(Sandbox sb,
			[RantDescription("The string to convert into a number.")] string input)
		{
			sb.Print(double.TryParse(input, out double number) ? number : 0);
		}

		[RantFunction("numfmt")]
		[RantDescription("Sets the current number formatting mode.")]
		private static void NumberFormat(Sandbox sb,
			[RantDescription("The number format to use.")] NumberFormat format) => sb.Output.Do(chain => chain.Last.NumberFormatter.NumberFormat = format);

		[RantFunction("numfmt")]
		[RantDescription("Runs the specified pattern under a specific number formatting mode.")]
		private static IEnumerator<RST> NumberFormatRange(Sandbox sb,
			[RantDescription("The number format to use.")] NumberFormat format,
			[RantDescription("The pattern to run.")] RST rangeAction)
		{
			var oldFmtMap = new Dictionary<OutputChain, NumberFormat>();

			sb.Output.Do(chain =>
			{
				oldFmtMap[chain] = chain.Last.NumberFormatter.NumberFormat;
				chain.Last.NumberFormatter.NumberFormat = format;
			});

			yield return rangeAction;

			sb.Output.Do(chain =>
			{
				if (!oldFmtMap.TryGetValue(chain, out NumberFormat fmt)) return;
				chain.Last.NumberFormatter.NumberFormat = fmt;
			});
		}

		[RantFunction]
		[RantDescription("Specifies the current digit formatting mode for numbers.")]
		private static void Digits(Sandbox sb,
			[RantDescription("The digit format to use.")] BinaryFormat format,
			[RantDescription("The digit count to associate with the mode.")] int digits) => sb.Output.Do(chain =>
		{
			chain.Last.NumberFormatter.BinaryFormat = format;
			chain.Last.NumberFormatter.BinaryFormatDigits = digits;
		});

		[RantFunction]
		[RantDescription("Sets the current endianness for hex and binary formatted numbers.")]
		private static void Endian(Sandbox sb,
			[RantDescription("The endianness to use.")] Endianness endianness) => sb.Output.Do(chain => chain.Last.NumberFormatter.Endianness = endianness);

		[RantFunction("init")]
		[RantDescription("Sets the index of the element to execute on the next block. Set to -1 to disable.")]
		private static void Initial(Sandbox sb, int index) => sb.CurrentBlockAttribs.StartIndex = index;

		[RantFunction("rep", "r")]
		[RantDescription("Sets the repetition count for the next block.")]
		private static void Rep(Sandbox sb,
			[RantDescription("The number of times to repeat the next block.")] int times) => sb.CurrentBlockAttribs.Repetitions = times;

		[RantFunction]
		[RantDescription("Sets the repetition count to the number of items in the next block.")]
		private static void RepEach(Sandbox sb) => sb.CurrentBlockAttribs.RepEach = true;

		[RantFunction("sep")]
		private static IEnumerator<RST> PrintSep(Sandbox sb)
		{
			yield return sb.BlockManager.GetPrevious().Separator;
		}

		[RantFunction("sep", "s")]
		[RantDescription("Sets the separator pattern for the next block.")]
		private static void Sep(Sandbox sb,
			[RantDescription("The separator pattern to run between iterations of the next block.")] RST separator)
		{
			sb.CurrentBlockAttribs.IsSeries = false;
			sb.CurrentBlockAttribs.Separator = separator;
		}

		[RantFunction("sep", "s")]
		[RantDescription("Flags the next block as a series and sets the separator and conjunction patterns.")]
		private static void Sep(Sandbox sb,
			[RantDescription("The separator pattern to run between items.")] RST separator,
			[RantDescription("The conjunction pattern to run before the last item.")] RST conjunction)
		{
			sb.CurrentBlockAttribs.IsSeries = true;
			sb.CurrentBlockAttribs.Separator = separator;
			sb.CurrentBlockAttribs.EndConjunction = conjunction;
		}

		[RantFunction("sep", "s")]
		[RantDescription("Sets the separator, Oxford comma, and conjunction patterns for the next series.")]
		private static void Sep(Sandbox sb,
			[RantDescription("The separator pattern to run between items.")] RST separator,
			[RantDescription("The Oxford comma pattern to run before the last item.")] RST oxford,
			[RantDescription("The conjunction pattern to run before the last item in the series.")] RST conjunction)
		{
			sb.CurrentBlockAttribs.IsSeries = true;
			sb.CurrentBlockAttribs.Separator = separator;
			sb.CurrentBlockAttribs.EndSeparator = oxford;
			sb.CurrentBlockAttribs.EndConjunction = conjunction;
		}

		[RantFunction("rs")]
		[RantDescription("Sets the repetitions and separator for the next block. A combination of rep and sep.")]
		private static void RepSep(Sandbox sb,
			[RantDescription("The number of times to repeat the next block.")] int times,
			[RantDescription("The separator pattern to run between iterations of the next block.")] RST separator)
		{
			sb.CurrentBlockAttribs.IsSeries = false;
			sb.CurrentBlockAttribs.Repetitions = times;
			sb.CurrentBlockAttribs.Separator = separator;
		}

		[RantFunction]
		[RantDescription("Sets the prefix pattern for the next block.")]
		private static void Before(Sandbox sb,
			[RantDescription("The pattern to run before each iteration of the next block.")] RST beforeAction) => sb.CurrentBlockAttribs.Before = beforeAction;

		[RantFunction]
		[RantDescription("Sets the postfix pattern for the next block.")]
		private static void After(Sandbox sb,
			[RantDescription("The pattern to run after each iteration of the next block.")] RST afterAction) => sb.CurrentBlockAttribs.After = afterAction;

		[RantFunction]
		[RantDescription("Modifies the likelihood that the next block will execute. Specified in percentage.")]
		private static void Chance(Sandbox sb,
			[RantDescription("The percent probability that the next block will execute.")] double chance) => sb.CurrentBlockAttribs.Chance = chance < 0 ? 0 : chance > 100 ? 100 : chance;

		[RantFunction("case", "caps")]
		[RantDescription("Changes the capitalization mode for all open channels.")]
		private static void Case(Sandbox sb,
			[RantDescription("The capitalization mode to use.")] Capitalization mode) => sb.Output.Capitalize(mode);

		[RantFunction("txtfmt")]
		[RantDescription("Sets the text conversion format for all open channels.")]
		private static void TxtFmt(Sandbox sb, 
			[RantDescription("The conversion mode to use.")]
			CharConversion format) => sb.Output.SetConversion(format);

		[RantFunction]
		[RantDescription("Infers the capitalization of a given string and sets the capitalization mode to match it.")]
		private static void CapsInfer(Sandbox sb,
			[RantDescription("A string that is capitalized in the format to be set.")] string sample)
		{
			var output = sb.Output;
			if (string.IsNullOrEmpty(sample))
			{
				output.Capitalize(Capitalization.None);
				return;
			}
			var words = sample.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (words.Length == 1)
			{
				string word = words[0];
				if (word.Length == 1)
					output.Capitalize(char.IsUpper(word[0]) ? Capitalization.First : Capitalization.None);
				else if (Util.IsUppercase(word))
					output.Capitalize(Capitalization.Upper);
				else if (char.IsUpper(word.SkipWhile(c => !char.IsLetterOrDigit(c)).FirstOrDefault()))
					output.Capitalize(Capitalization.First);
				else
					output.Capitalize(Capitalization.None);
			}
			else
			{
				// No letters? Forget it.
				if (!sample.Any(char.IsLetter))
				{
					output.Capitalize(Capitalization.None);
					return;
				}

				// Is all-caps?
				if (Util.IsUppercase(sample))
				{
					output.Capitalize(Capitalization.Upper);
					return;
				}

				var sentences = sample.Split(new[] { '.', '?', '!' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(str => str.Trim())
					.Where(str =>
						!string.IsNullOrEmpty(str)
						&& !char.IsDigit(str[0])).ToArray();

				// All words capitalized?
				var lwords = words.Where(w => char.IsLetter(w[0])).ToArray();
				if (lwords.Any() &&
					(sentences.Length == 1 ||
					 sentences.Any(s => s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length > 1)))
				{
					if (lwords.All(lw => char.IsUpper(lw[0])))
					{
						output.Capitalize(Capitalization.Word);
						return;
					}

					if (lwords.All(lw => char.IsLower(lw[0]) == sb.Format.Excludes(lw)))
					{
						output.Capitalize(Capitalization.Title);
						return;
					}
				}

				// All sentences capitalized?
				bool all = true;
				bool none = true;
				foreach (string sentence in sentences)
				{
					bool isCapitalized = char.IsUpper(sentence.SkipWhile(c => !char.IsLetter(c)).FirstOrDefault());
					all = all && isCapitalized;
					none = none && !isCapitalized;
				}

				if (sentences.Length > 1 && all)
					output.Capitalize(Capitalization.Sentence);
				else if (none)
					output.Capitalize(Capitalization.Lower);
				else if (char.IsUpper(sample.SkipWhile(c => !char.IsLetterOrDigit(c)).FirstOrDefault()))
					output.Capitalize(Capitalization.First);
				else
					output.Capitalize(Capitalization.None);
			}
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is the first.")]
		private static IEnumerator<RST> First(Sandbox sb,
			[RantDescription("The pattern to run when the condition is met.")] RST action)
		{
			if (!sb.Blocks.Any()) yield break;
			if (sb.Blocks.Peek().Iteration == 1) yield return action;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is not the first.")]
		private static IEnumerator<RST> NotFirst(Sandbox sb,
			[RantDescription("The pattern to run when the condition is met.")] RST action)
		{
			if (!sb.Blocks.Any()) yield break;
			if (sb.Blocks.Peek().Iteration > 1) yield return action;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is the last.")]
		private static IEnumerator<RST> Last(Sandbox sb,
			[RantDescription("The pattern to run when the condition is met.")] RST action)
		{
			if (!sb.Blocks.Any()) yield break;
			var block = sb.Blocks.Peek();
			if (block.Iteration == block.Count) yield return action;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is not the last.")]
		private static IEnumerator<RST> NotLast(Sandbox sb,
			[RantDescription("The pattern to run when the condition is met.")] RST action)
		{
			if (!sb.Blocks.Any()) yield break;
			var block = sb.Blocks.Peek();
			if (block.Iteration < block.Count) yield return action;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is neither the first nor last.")]
		private static IEnumerator<RST> Middle(Sandbox sb,
			[RantDescription("The pattern to run when the condition is met.")] RST action)
		{
			if (!sb.Blocks.Any()) yield break;
			var block = sb.Blocks.Peek();
			if (block.Iteration > 1 && block.Iteration < block.Count) yield return action;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is either the first or last.")]
		private static IEnumerator<RST> Ends(Sandbox sb,
			[RantDescription("The pattern to run when the condition is met.")] RST action)
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
		private static void Depth(Sandbox sb) => sb.Print(sb.Blocks.Count);

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
		private static IEnumerator<RST> Odd(Sandbox sb,
			[RantDescription("The pattern to run when the condition is met.")] RST action)
		{
			if (!sb.Blocks.Any()) yield break;
			if (sb.Blocks.Peek().Iteration % 2 != 0) yield return action;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is an even number.")]
		private static IEnumerator<RST> Even(Sandbox sb,
			[RantDescription("The pattern to run when the condition is met.")] RST action)
		{
			if (!sb.Blocks.Any()) yield break;
			if (sb.Blocks.Peek().Iteration % 2 == 0) yield return action;
		}

		[RantFunction]
		[RantDescription("Returns the specified argument from the current subroutine.")]
		private static IEnumerator<RST> Arg(Sandbox sb,
			[RantDescription("The name of the argument to retrieve.")] string name)
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
			[RantDescription("The name of the match group whose value will be retrieved.")] string groupName)
		{
			if (!sb.RegexMatches.Any()) return;
			sb.Print(sb.RegexMatches.Peek().Groups[groupName].Value);
		}

		[RantFunction]
		[RantDescription("Sets the current rhyming mode for queries.")]
		private static void Rhyme(Sandbox sb,
			[RantDescription("The rhyme types to use.")] RhymeFlags flags) => sb.CarrierState.Rhymer.AllowedRhymes = flags;

		[RantFunction("sync", "x")]
		[RantDescription("Creates and applies a synchronizer with the specified name and type.")]
		private static void Sync(Sandbox sb,
			[RantDescription("The name of the synchronizer.")] string name,
			[RantDescription("The synchronization type to use.")] SyncType type) => sb.SyncManager.Create(name, type, true);

		[RantFunction("xpin")]
		[RantDescription("Pins a synchronizer.")]
		private static void SyncPin(Sandbox sb,
			[RantDescription("The name of the synchronizer to pin.")] string name) => sb.SyncManager.SetPinned(name, true);

		[RantFunction("xunpin")]
		[RantDescription("Unpins a synchronizer.")]
		private static void SyncUnpin(Sandbox sb,
			[RantDescription("The name of the synchronizer to unpin.")] string name) => sb.SyncManager.SetPinned(name, false);

		[RantFunction("xstep")]
		[RantDescription("Iterates a synchronizer.")]
		private static void SyncStep(Sandbox sb,
			[RantDescription("The name of the synchronizer to iterate.")] string name) => sb.SyncManager.Step(name);

		[RantFunction("xreset")]
		[RantDescription("Resets a synchronizer to its initial state.")]
		private static void SyncReset(Sandbox sb,
			[RantDescription("The name of the synchronizer to reset.")] string name) => sb.SyncManager.Reset(name);

		[RantFunction("quote", "q")]
		[RantDescription(
			"Surrounds the specified pattern in quotes. Nested quotes use the secondary quotes defined in the format settings.")]
		private static IEnumerator<RST> Quote(Sandbox sb,
			[RantDescription("The pattern to run whose output will be surrounded in quotes.")] RST quoteAction)
		{
			sb.IncreaseQuote();
			sb.PrintOpeningQuote();
			yield return quoteAction;
			sb.PrintClosingQuote();
			sb.DecreaseQuote();
		}

		[RantFunction]
		[RantDescription("Opens a channel for writing and executes the specified pattern inside of it.")]
		private static IEnumerator<RST> Chan(Sandbox sb, string channelName, ChannelVisibility visibility, RST pattern)
		{
			sb.Output.OpenChannel(channelName, visibility);
			yield return pattern;
			sb.Output.CloseChannel();
		}

		[RantFunction("target", "t")]
		[RantDescription("Places a target with the specified name at the current write position.")]
		private static void Target(Sandbox sb,
			[RantDescription("The name of the target.")] string targetName) => sb.Output.InsertTarget(targetName);

		[RantFunction]
		[RantDescription("Appends a string to the specified target's contents.")]
		private static void Send(Sandbox sb,
			[RantDescription("The name of the target to send to.")] string targetName,
			[RantDescription("The string to send to the target.")] string value) => sb.Output.PrintToTarget(targetName, value);

		[RantFunction]
		[RantDescription("Overwrites the specified target's contents with the provided value.")]
		private static void SendOver(Sandbox sb,
			[RantDescription("The name of the target to send to.")] string targetName,
			[RantDescription("The string to send to the target.")] string value)
		{
			sb.Output.Do(chain => chain.ClearTarget(targetName));
			sb.Output.PrintToTarget(targetName, value);
		}

		[RantFunction("targetval")]
		[RantDescription("Prints the current value of the specified target. This function will not spawn a target.")]
		private static void GetTargetValue(Sandbox sb,
			[RantDescription("The name of the target whose value to print.")] string targetName) => sb.Output.Do(chain => chain.Print(chain.GetTargetValue(targetName)));

		[RantFunction("clrt")]
		[RantDescription("Clears the contents of the specified target.")]
		private static void ClearTarget(Sandbox sb,
			[RantDescription("The name of the target to be cleared.")] string targetName) => sb.Output.Do(chain => chain.ClearTarget(targetName));

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is a multiple of the specified number.")]
		private static IEnumerator<RST> Nth(Sandbox sb,
			[RantDescription("The interval at which the pattern should be run.")] int interval,
			[RantDescription("The pattern to run when the condition is satisfied.")] RST pattern)
		{
			if (!sb.Blocks.Any()) yield break;
			if (sb.Blocks.Peek().Iteration % interval != 0) yield break;
			yield return pattern;
		}

		[RantFunction]
		[RantDescription(
			"Runs a pattern if the current block iteration is a multiple of the specified number offset by a specific amount.")]
		private static IEnumerator<RST> NthO(Sandbox sb,
			[RantDescription("The interval at which the pattern should be run.")] int interval,
			[RantDescription("The number of iterations to offset the interval by.")] int offset,
			[RantDescription("The pattern to run when the condition is satisfied.")] RST pattern)
		{
			if (!sb.Blocks.Any()) yield break;
			if (Util.Mod(sb.Blocks.Peek().Iteration - offset, interval) != 0) yield break;
			yield return pattern;
		}

		[RantFunction]
		[RantDescription("Runs a pattern if the current block iteration is not a multiple of the specified number.")]
		private static IEnumerator<RST> NotNth(Sandbox sb,
			[RantDescription("The interval at which the pattern should not be run.")] int interval,
			[RantDescription("The pattern to run when the condition is satisfied.")] RST pattern)
		{
			if (!sb.Blocks.Any()) yield break;
			if (sb.Blocks.Peek().Iteration % interval == 0) yield break;
			yield return pattern;
		}

		[RantFunction]
		[RantDescription(
			"Runs a pattern if the current block iteration is not a multiple of the specified number offset by a specific amount."
		)]
		private static IEnumerator<RST> NotNthO(Sandbox sb,
			[RantDescription("The interval at which the pattern should not be run.")] int interval,
			[RantDescription("The number of iterations to offset the interval by.")] int offset,
			[RantDescription("The pattern to run when the condition is satisfied.")] RST pattern)
		{
			if (!sb.Blocks.Any()) yield break;
			if (Util.Mod(sb.Blocks.Peek().Iteration - offset, interval) == 0) yield break;
			yield return pattern;
		}

		[RantFunction]
		[RantDescription("Sets a pattern that will run before the next block.")]
		private static void Start(Sandbox sb,
			[RantDescription("The pattern to run before the next block.")] RST beforePattern) => sb.CurrentBlockAttribs.Start = beforePattern;

		[RantFunction]
		[RantDescription("Sets a pattern that will run after the next block.")]
		private static void End(Sandbox sb,
			[RantDescription("The pattern to run after the next block.")] RST endPattern) => sb.CurrentBlockAttribs.End = endPattern;

		// TODO: Finish [persist].
		//[RantFunction]
		[RantDescription("Instructs Rant not to consume the block attributes after they are used.")]
		private static void Persist(Sandbox sb, AttribPersistence persistence) => sb.CurrentBlockAttribs.Persistence = persistence;

		[RantFunction]
		[RantDescription("Loads and runs a pattern from cache or file.")]
		private static IEnumerator<RST> Import(Sandbox sb,
			[RantDescription("The name or path of the pattern to load.")] string name)
		{
			RST action;

			try
			{
				action = sb.Engine.GetProgramInternal(name).SyntaxTree;
			}
			catch (RantCompilerException e)
			{
				throw new RantRuntimeException(sb.Pattern, sb.CurrentAction.Location,
					$"Failed to compile imported pattern '{name}':\n{e.Message}");
			}
			catch (Exception e)
			{
				throw new RantRuntimeException(sb.Pattern, sb.CurrentAction.Location,
					$"Failed to import '{name}':\n{e.Message}");
			}

			yield return action;
		}

		[RantFunction]
		[RantDescription("Defines the specified flags.")]
		private static void Define(Sandbox sb,
			[RantDescription("The list of flags to define.")] params string[] flags)
		{
			foreach (string flag in flags.Where(f => !Util.IsNullOrWhiteSpace(f) && Util.ValidateName(f)))
				sb.Engine.Flags.Add(flag);
		}

		[RantFunction]
		[RantDescription("Undefines the specified flags.")]
		private static void Undef(Sandbox sb,
			[RantDescription("The list of flags to undefine.")] params string[] flags)
		{
			foreach (string flag in flags) sb.Engine.Flags.Remove(flag);
		}

		[RantFunction]
		[RantDescription("Toggles the specified flags.")]
		private static void Toggle(Sandbox sb, params string[] flags)
		{
			foreach (string flag in flags.Where(f => !Util.IsNullOrWhiteSpace(f) && Util.ValidateName(f)))
			{
				if (sb.Engine.Flags.Contains(flag))
					sb.Engine.Flags.Remove(flag);
				else
					sb.Engine.Flags.Add(flag);
			}
		}

		[RantFunction]
		[RantDescription(
			"Sets the current flag condition for [then] ... [else] calls to be true if all the specified flags are set.")]
		private static void IfDef(Sandbox sb, params string[] flags)
		{
			sb.FlagConditionExpectedResult = true;
			sb.ConditionFlags.Clear();
			foreach (string flag in flags.Where(f => !Util.IsNullOrWhiteSpace(f) && Util.ValidateName(f)))
				sb.ConditionFlags.Add(flag);
		}

		[RantFunction]
		[RantDescription(
			"Sets the current flag condition for [then] ... [else] calls to be true if all the specified flags are unset.")]
		private static void IfNDef(Sandbox sb, params string[] flags)
		{
			sb.FlagConditionExpectedResult = false;
			sb.ConditionFlags.Clear();
			foreach (string flag in flags.Where(f => !Util.IsNullOrWhiteSpace(f) && Util.ValidateName(f)))
				sb.ConditionFlags.Add(flag);
		}

		[RantFunction]
		[RantDescription("Executes a pattern if the current flag condition passes.")]
		private static IEnumerator<RST> Then(Sandbox sb, RST conditionPassPattern)
		{
			if (sb.Engine.Flags.All(flag => sb.ConditionFlags.Contains(flag) == sb.FlagConditionExpectedResult))
				yield return conditionPassPattern;
		}

		[RantFunction]
		[RantDescription("Executes a pattern if the current flag condition fails.")]
		private static IEnumerator<RST> Else(Sandbox sb, RST conditionFailPattern)
		{
			if (sb.Engine.Flags.Any(flag => sb.ConditionFlags.Contains(flag) != sb.FlagConditionExpectedResult))
				yield return conditionFailPattern;
		}

		[RantFunction]
		[RantDescription("Yields the currenty written output.")]
		private static void Yield(Sandbox sb) => sb.SetYield();

		[RantFunction]
		[RantDescription("Branches the internal RNG according to a seed.")]
		private static void Branch(Sandbox sb,
			[RantDescription("The seed for the branch.")] string seed) => sb.RNG.Branch(seed.Hash());

		[RantFunction]
		[RantDescription("Branches the internal RNG, executes the specified pattern, and then merges the branch.")]
		private static IEnumerator<RST> Branch(Sandbox sb,
			[RantDescription("The seed for the branch.")] string seed,
			[RantDescription("The pattern to run on the branch.")] RST branchAction)
		{
			sb.RNG.Branch(seed.Hash());
			yield return branchAction;
			sb.RNG.Merge();
		}

		[RantFunction]
		[RantDescription("Merges the topmost branch of the internal RNG, if it has been branched at least once.")]
		private static void Merge(Sandbox sb) => sb.RNG.Merge();

		[RantFunction("in")]
		[RantDescription("Prints the value of the specified pattern argument.")]
		private static void PatternArg(Sandbox sb,
			[RantDescription("The name of the argument to access.")] string argName)
		{
			if (sb.PatternArgs == null) return;
			sb.Output.Print(sb.PatternArgs[argName]);
		}

		[RantFunction("tm")]
		[RantDescription("Prints the trademark symbol.")]
		private static void Trademark(Sandbox sb) => sb.Print("\x2122");

		[RantFunction("reg")]
		[RantDescription("Prints the registered trademark symbol.")]
		private static void RegisteredTrademark(Sandbox sb) => sb.Print("\x00ae");

		[RantFunction("c")]
		[RantDescription("Prints the copyright symbol.")]
		private static void Copyright(Sandbox sb) => sb.Print("\x00a9");

		[RantFunction("em")]
		[RantDescription("Prints an emdash.")]
		private static void Emdash(Sandbox sb) => sb.Print("\x2014");

		[RantFunction("en")]
		[RantDescription("Prints an endash.")]
		private static void Endash(Sandbox sb) => sb.Print("\x2013");

		[RantFunction("b")]
		[RantDescription("Prints a bullet character.")]
		private static void Bullet(Sandbox sb) => sb.Print("\x2022");

		[RantFunction("ss")]
		[RantDescription("Prints an eszett (ß).")]
		private static void Eszett(Sandbox sb) => sb.Print("\x00df");

		[RantFunction("emoji")]
		[RantDescription("Takes an emoji shortcode and prints the corresponding emoji.")]
		private static void PrintEmoji(Sandbox sb,
			[RantDescription("The emoji shortcode to use, without colons.")] string shortcode)
		{
			shortcode = shortcode.ToLower();
			if (!Emoji.Shortcodes.ContainsKey(shortcode))
			{
				sb.Print("[missing emoji]");
				return;
			}
			sb.Print(char.ConvertFromUtf32(Emoji.Shortcodes[shortcode]));
		}

		[RantFunction("plural", "pl")]
		[RantDescription("Infers and prints the plural form of the specified word.")]
		private static void Plural(Sandbox sb, string word) => sb.Print(sb.Format.Pluralizer.Pluralize(word));

		[RantFunction("use")]
		[RantDescription("Loads a module from the file name.module.rant, name.rant, or name, in that order.")]
		private static void Use(Sandbox sb, string name)
		{
			if (sb.UserModules.ContainsKey(name))
			{
				sb.Modules[name] = sb.UserModules[name];
				return;
			}
			if (sb.PackageModules.ContainsKey(name))
			{
				sb.Modules[name] = sb.PackageModules[name];
				return;
			}
			string file;
			if (File.Exists(name + ".module.rant"))
				file = name + ".module.rant";
			else if (File.Exists(name + ".rant"))
				file = name + ".rant";
			else if (File.Exists(name))
				file = name;
			else
				throw new RantRuntimeException(sb.Pattern, sb.CurrentAction.Location, $"Could not find module '{name}'.");
			var pattern = RantProgram.CompileFile(file);
			if (pattern.Module == null)
				throw new RantRuntimeException(sb.Pattern, sb.CurrentAction.Location, $"No module is defined in {file}.");
			sb.Modules[Path.GetFileNameWithoutExtension(name)] = pattern.Module;
		}

		[RantFunction]
		[RantDescription("Prints the current length of the specified channel, in characters.")]
		private static void Len(Sandbox sb,
			[RantDescription("The channel for which to retrieve the length.")] string channelName) => sb.Print(sb.Output.GetChannelLength(channelName));

		[RantFunction("rev")]
		[RantDescription("Reverses the specified string and prints it to the output.")]
		private static void Reverse(Sandbox sb,
			[RantDescription("The string to reverse.")] string input)
		{
			if (string.IsNullOrEmpty(input)) return;
			var buffer = new char[input.Length];
			int numCombiners = 0;
			int lastIndex = input.Length - 1;
			for (int i = lastIndex; i >= 0; i--)
			{
				if (CharUnicodeInfo.GetUnicodeCategory(input[i]) == UnicodeCategory.NonSpacingMark)
				{
					// It's combining, so increase the combiner count until we hit a regular char
					numCombiners++;
				}
				else if (numCombiners > 0)
				{
					// We've hit a non-combining character with combiners added.
					// First thing to do is add the character to the buffer.
					buffer[lastIndex - i - numCombiners] = input[i];

					// Then we insert all the combining characters that come after it.
					for (int j = 1; j <= numCombiners; j++)
						buffer[lastIndex - i - numCombiners + j] = input[i + j];
					numCombiners = 0;
				}
				else if (char.IsLowSurrogate(input[i]))
					buffer[lastIndex - i + 1] = input[i];
				else if (char.IsHighSurrogate(input[i]))
					buffer[lastIndex - i - 1] = input[i];
				else
					buffer[lastIndex - i] = input[i];
			}
			sb.Print(new string(buffer));
		}

		[RantFunction("revx")]
		[RantDescription("Reverses the specified string and inverts common brackets and quotation marks, then prints the result to the output.")]
		private static void ReverseEx(Sandbox sb,
			[RantDescription("The string to reverse.")] string input)
		{
			if (string.IsNullOrEmpty(input)) return;
			var buffer = new char[input.Length];
			int numCombiners = 0;
			int lastIndex = input.Length - 1;
			for (int i = lastIndex; i >= 0; i--)
			{
				if (CharUnicodeInfo.GetUnicodeCategory(input[i]) == UnicodeCategory.NonSpacingMark)
				{
					// It's combining, so increase the combiner count until we hit a regular char
					numCombiners++;
				}
				else if (numCombiners > 0)
				{
					// We've hit a non-combining character with combiners added.
					// First thing to do is add the character to the buffer.
					buffer[lastIndex - i - numCombiners] = Util.ReverseChar(input[i]);

					// Then we insert all the combining characters that come after it.
					for (int j = 1; j <= numCombiners; j++)
						buffer[lastIndex - i - numCombiners + j] = input[i + j];
					numCombiners = 0;
				}
				else if (char.IsLowSurrogate(input[i]))
					buffer[lastIndex - i + 1] = input[i];
				else if (char.IsHighSurrogate(input[i]))
					buffer[lastIndex - i - 1] = input[i];
				else
					buffer[lastIndex - i] = Util.ReverseChar(input[i]);
			}
			sb.Print(new string(buffer));
		}

		[RantFunction("accent")]
		[RantDescription("Accents the previous character.")]
		private static void AddAccent(Sandbox sb, Accent accent) => sb.Print(accent.GetAccentChar());

		[RantFunction("accent")]
		[RantDescription("Accents the specified character.")]
		private static void AddAccent(Sandbox sb, string character, Accent accent) => sb.Print($"{character}{accent.GetAccentChar()}".Normalize(NormalizationForm.FormC));

		[RantFunction("acute", "act")]
		[RantDescription("Accents the specified character with an acute (a\u0301) accent.")]
		private static void AccentAcute(Sandbox sb, string character) => sb.Print($"{character}\u0301".Normalize(NormalizationForm.FormC));

		[RantFunction("circumflex", "cflex")]
		[RantDescription("Accents the specified character with a circumflex (a\u0302) accent.")]
		private static void AccentCircumflex(Sandbox sb, string character) => sb.Print($"{character}\u0302".Normalize(NormalizationForm.FormC));

		[RantFunction("grave", "grv")]
		[RantDescription("Accents the specified character with a grave (a\u0300) accent.")]
		private static void AccentGrave(Sandbox sb, string character) => sb.Print($"{character}\u0300".Normalize(NormalizationForm.FormC));

		[RantFunction("ring")]
		[RantDescription("Accents the specified character with a ring (a\u030A) accent.")]
		private static void AccentRing(Sandbox sb, string character) => sb.Print($"{character}\u030A".Normalize(NormalizationForm.FormC));

		[RantFunction("tilde", "tld")]
		[RantDescription("Accents the specified character with a tilde (a\u0303) accent.")]
		private static void AccentTilde(Sandbox sb, string character) => sb.Print($"{character}\u0303".Normalize(NormalizationForm.FormC));

		[RantFunction("diaeresis", "dia")]
		[RantDescription("Accents the specified character with a diaeresis (a\u0308) accent.")]
		private static void AccentDiaeresis(Sandbox sb, string character) => sb.Print($"{character}\u0308".Normalize(NormalizationForm.FormC));

		[RantFunction("caron", "crn")]
		[RantDescription("Accents the specified character with a caron (c\u030C) accent.")]
		private static void AccentCaron(Sandbox sb, string character) => sb.Print($"{character}\u030C".Normalize(NormalizationForm.FormC));

		[RantFunction("macron", "mcn")]
		[RantDescription("Accents the specified character with a macron (c\u0304) accent.")]
		private static void AccentMacron(Sandbox sb, string character) => sb.Print($"{character}\u0304".Normalize(NormalizationForm.FormC));

		[RantFunction("cedilla", "ced")]
		[RantDescription("Accents the specified character with a cedilla (c\u0327) accent.")]
		private static void AccentCedilla(Sandbox sb, string character) => sb.Print($"{character}\u0327".Normalize(NormalizationForm.FormC));

		[RantFunction("char")]
		[RantDescription("Prints a Unicode character given its official Unicode-designated name (e.g. 'LATIN CAPITAL LETTER R' -> 'R').")]
		private static void Character(Sandbox sb,
			[RantDescription("The name of the character to print (case-insensitive).")] string name) => sb.Print(Unicode.GetByName(name));

		[RantFunction("rcc")]
		[RantDescription("Resets the specified carrier components.")]
		private static void ResetCarrier(Sandbox sb,
			[RantDescription("The list of carrier component identifiers to reset.")]
			params string[] ids)
		{
			foreach (var id in ids)
			{
				if (String.IsNullOrWhiteSpace(id)) continue;
				sb.CarrierState.DeleteAssociation(id);
				sb.CarrierState.DeleteMatch(id);
				sb.CarrierState.DeleteRhyme(id);
				sb.CarrierState.DeleteUnique(id);
			}
		}

		[RantFunction("query")]
		private static IEnumerator<RST> QueryRun(Sandbox sb)
		{
			return sb.QueryBuilder.CurrentQuery?.Run(sb);
		}

		[RantFunction("qname")]
		private static void QueryName(Sandbox sb, string id, string name)
		{
			sb.QueryBuilder.GetQuery(id).Name = name;
		}

		[RantFunction("qsub")]
		private static void QuerySubtype(Sandbox sb, string id, string name)
		{
			sb.QueryBuilder.GetQuery(id).Subtype = name;
		}

		[RantFunction("qcf")]
		private static void QueryClassFilterPositive(Sandbox sb, string id, params string[] classes)
		{
			Query q;
			ClassFilter cf;
			cf = (q = sb.QueryBuilder.GetQuery(id)).GetFilters().FirstOrDefault(f => f is ClassFilter) as ClassFilter;
			if (cf == null) q.AddFilter(cf = new ClassFilter());
			foreach (var cl in classes)
			{
				cf.AddRule(new ClassFilterRule(cl, true));
			}
		}

		[RantFunction("qcfn")]
		private static void QueryClassFilterNegative(Sandbox sb, string id, params string[] classes)
		{
			Query q;
			ClassFilter cf;
			cf = (q = sb.QueryBuilder.GetQuery(id)).GetFilters().FirstOrDefault(f => f is ClassFilter) as ClassFilter;
			if (cf == null) q.AddFilter(cf = new ClassFilter());
			foreach (var cl in classes)
			{
				cf.AddRule(new ClassFilterRule(cl, false));
			}
		}

		[RantFunction("pipe")]
		private static void Redirect(Sandbox sb, RST redirectCallback)
		{
			sb.CurrentBlockAttribs.Redirect = redirectCallback;
		}

		[RantFunction("item")]
		private static void RedirectedItem(Sandbox sb)
		{
			sb.Print(sb.GetRedirectedOutput().Main);
		}

		[RantFunction("item")]
		private static void RedirectedItem(Sandbox sb, string channel)
		{
			sb.Print(sb.GetRedirectedOutput()[channel]);
		}

		[RantFunction("vs")]
		private static void VariableSet(Sandbox sb, string name, string value)
		{
			sb.Objects[name] = new RantObject(value);
		}

		[RantFunction("vn")]
		private static void VariableSet(Sandbox sb, string name, double value)
		{
			sb.Objects[name] = new RantObject(value);
		}

		[RantFunction("vn")]
		private static void VariableSet(Sandbox sb, string name, int min, int max)
		{
			sb.Objects[name] = new RantObject(sb.RNG.Next(min, max + 1));
		}

		[RantFunction("vp")]
		private static void VariableSetLazy(Sandbox sb, string name, RST value)
		{
			sb.Objects[name] = new RantObject(value);
		}

		[RantFunction("vcpy")]
		private static void VariableCopy(Sandbox sb, string a, string b)
		{
			sb.Objects[b] = sb.Objects[a].Clone();
		}

		[RantFunction("v")]
		private static IEnumerator<RST> VariableGet(Sandbox sb, string name)
		{
			var o = sb.Objects[name];
			if (o == null)
			{
				throw new RantRuntimeException(sb.Pattern, sb.CurrentAction, "err-runtime-missing-var", name);
			}

			if (o.Type == RantObjectType.Action)
			{
				yield return o.Value as RST;
			}
			else
			{
				sb.Print(o);
			}
		}
	}
}