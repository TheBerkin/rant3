using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Rant.Engine.Compiler.Syntax;

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
				if (info != null) FunctionTable[name] = info;
			}
			Loaded = true;
		}

		public static RantFunctionInfo GetFunction(string name)
		{
			RantFunctionInfo func;
			if (!FunctionTable.TryGetValue(name, out func)) return null;
			return func;
		}

		[RantFunction]
		private static void Rep(Sandbox sb, int times)
		{
			sb.CurrentBlockAttribs.Repetitons = times;
		}

		[RantFunction]
		private static void Sep(Sandbox sb, RantAction separatorAction)
		{
			sb.CurrentBlockAttribs.Separator = separatorAction;
		} 
	}
}