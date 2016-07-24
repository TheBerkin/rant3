using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Rant.Internals.Engine.Metadata;
using Rant.Internals.Engine.Utilities;

namespace Rant.Internals.Engine
{
	internal static partial class RantFunctionRegistry
	{
		private static bool _loaded = false;
		private static readonly Dictionary<string, RantFunction> FunctionTable =
			new Dictionary<string, RantFunction>(StringComparer.InvariantCultureIgnoreCase);
		private static readonly Dictionary<string, string> AliasTable =
			new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

		static RantFunctionRegistry()
		{
			Load();
		}

		internal static void Load()
		{
			if (_loaded) return;

			// Get every single private static method in the RantFunctions class
			foreach (var method in typeof(RantFunctionRegistry).GetMethods(BindingFlags.Static | BindingFlags.NonPublic))
			{
				if (!method.IsStatic) continue;
				var attr = method.GetCustomAttributes(typeof(RantFunctionAttribute), false).FirstOrDefault() as RantFunctionAttribute;
				if (attr == null) continue; // Discard methods without a [RantFunction] attribute

				// Compile metadata into RantFunctionInfo
				var name = String.IsNullOrEmpty(attr.Name) ? method.Name.ToLower() : attr.Name;
				var descAttr = method.GetCustomAttributes(typeof(RantDescriptionAttribute), false).FirstOrDefault() as RantDescriptionAttribute;
				var info = new RantFunctionSignature(name, descAttr?.Description ?? String.Empty, method);

				if (Util.ValidateName(name)) RegisterFunction(info);

				foreach (var alias in attr.Aliases.Where(Util.ValidateName))
					RegisterAlias(alias, info.Name);
			}
			_loaded = true;
		}

		private static void RegisterFunction(RantFunctionSignature func)
		{
			RantFunction group;
			if (!FunctionTable.TryGetValue(func.Name, out group))
				group = FunctionTable[func.Name] = new RantFunction(func.Name);
			group.Add(func);
		}

		private static void RegisterAlias(string alias, string funcName)
		{
			AliasTable[alias] = funcName;
		}

		public static IEnumerable<string> GetAliases(string funcName) =>
			AliasTable.Where(pair => String.Equals(funcName, pair.Value, StringComparison.InvariantCultureIgnoreCase))
				.Select(pair => pair.Key);

		private static string ResolveAlias(string alias)
		{
			string name;
			return AliasTable.TryGetValue(alias, out name) ? name : alias;
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

		public static RantFunction GetFunctionGroup(string name)
		{
			RantFunction group;
			return !FunctionTable.TryGetValue(ResolveAlias(name), out group) ? null : group;
		}

		public static RantFunctionSignature GetFunction(string name, int paramc) =>
			GetFunctionGroup(name)?.GetFunction(paramc);
	}
}