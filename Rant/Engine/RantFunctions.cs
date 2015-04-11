using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Rant.Engine.Formatters;
using Rant.Engine.Syntax;

namespace Rant.Engine
{
	// Methods representing Rant functions must be marked with [RantFunction] attribute to get registered by the engine.
	// They may return either void or IEnumerator<RantAction> depending on your needs.
	internal static class RantFunctions
	{
		private static readonly Dictionary<string, RantFunctionInfo> FunctionTable = 
			new Dictionary<string, RantFunctionInfo>(StringComparer.InvariantCultureIgnoreCase);

		private static bool Loaded = false;

		internal static void Load()
		{
			if (Loaded) return;
			var methods = typeof(RantFunctions).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
			foreach (var method in methods)
			{
				if (!method.IsStatic) continue;
				var attr = method.GetCustomAttributes().OfType<RantFunctionAttribute>().FirstOrDefault();
				if (attr == null) continue;
				var name = String.IsNullOrEmpty(attr.Name) ? method.Name.ToLower() : attr.Name;
				var info = new RantFunctionInfo(name, method);
				if (info != null)
				{
					if (Util.ValidateName(name)) FunctionTable[name] = info;
					foreach(var alias in attr.Aliases.Where(Util.ValidateName))
						FunctionTable[alias] = info;
				}
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
		private static void Rep(Sandbox sb, int times)
		{
			sb.CurrentBlockAttribs.Repetitons = times;
		}

		[RantFunction("sep", "s")]
		private static void Sep(Sandbox sb, RantAction separatorAction)
		{
			sb.CurrentBlockAttribs.Separator = separatorAction;
		}

		[RantFunction]
		private static void Before(Sandbox sb, RantAction beforeAction)
		{
			sb.CurrentBlockAttribs.Before = beforeAction;
		}

		[RantFunction]
		private static void After(Sandbox sb, RantAction afterAction)
		{
			sb.CurrentBlockAttribs.After = afterAction;
		}

		[RantFunction]
		private static void Chance(Sandbox sb, int chance)
		{
			sb.CurrentBlockAttribs.Chance = chance < 0 ? 0 : chance > 100 ? 100 : chance;
		}


		[RantFunction("case", "caps")]
		private static void Case(Sandbox sb, Case textCase)
		{
			sb.CurrentOutput.SetCase(textCase);
		}
	}
}