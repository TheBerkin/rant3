using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Rant.Core.Compiler.Syntax;
using Rant.Core.Compiler.Syntax.Richard;
using Rant.Core.Constructs;
using Rant.Core.ObjectModel;
using Rant.Core.Utilities;
using Rant.Metadata;

namespace Rant.Core.Framework
{
	internal class RichardFunctions
	{
		private static bool _loaded = false;
		private static Dictionary<Type, Dictionary<string, RantFunctionSignature>> _properties;
		private static Dictionary<string, RichObject> _globalObjects;

		static RichardFunctions()
		{
			Load();
		}

		public static void Load()
		{
			if (_loaded) return;

			_properties = new Dictionary<Type, Dictionary<string, RantFunctionSignature>>();
			_globalObjects = new Dictionary<string, RichObject>();

			foreach (var method in typeof(RichardFunctions).GetMethods(BindingFlags.Static | BindingFlags.NonPublic))
			{
				if (!method.IsStatic) continue;
				var attr = method.GetCustomAttributes(typeof(RichardProperty), true).OfType<RichardProperty>().FirstOrDefault();
				if (attr == null)
				{
					var objectAttr = method.GetCustomAttributes(typeof(RichardGlobalObject), true).OfType<RichardGlobalObject>().FirstOrDefault();
					if (objectAttr == null) continue;
					var name = String.IsNullOrEmpty(objectAttr.Property) ? method.Name.ToLower() : objectAttr.Property;
					var descAttr = method.GetCustomAttributes(typeof(RantDescriptionAttribute), true).OfType<RantDescriptionAttribute>().FirstOrDefault();
					var info = new RantFunctionSignature(name, descAttr?.Description ?? String.Empty, method);
					info.TreatAsRichardFunction = objectAttr.IsFunction;
					if (Util.ValidateName(name)) RegisterGlobalObject(objectAttr.Name, objectAttr.Property, info);
				}
				else
				{
					var name = String.IsNullOrEmpty(attr.Name) ? method.Name.ToLower() : attr.Name;
					var descAttr = method.GetCustomAttributes(typeof(RantDescriptionAttribute), true).OfType<RantDescriptionAttribute>().FirstOrDefault();
					var info = new RantFunctionSignature(name, descAttr?.Description ?? String.Empty, method);
					info.TreatAsRichardFunction = attr.IsFunction;
					if (Util.ValidateName(name)) RegisterProperty(attr.ObjectType, info);
				}
			}

			_loaded = true;
		}

		private static void RegisterProperty(Type objectType, RantFunctionSignature info)
		{
			var dictionary = _properties.ContainsKey(objectType) ?
				_properties[objectType] :
				new Dictionary<string, RantFunctionSignature>();
			dictionary.Add(info.Name, info);
			_properties[objectType] = dictionary;
		}

		public static bool HasProperty(Type objectType, string name) =>
			_properties.ContainsKey(objectType) ? _properties[objectType].ContainsKey(name) : false;

		public static RantFunctionSignature GetProperty(Type objectType, string name)
		{
			if (!HasProperty(objectType, name))
				return null;
			return _properties[objectType][name];
		}

		private static void RegisterGlobalObject(string name, string property, RantFunctionSignature info)
		{
			if (!_globalObjects.ContainsKey(name))
				_globalObjects[name] = new RichObject(null);
			if (info.TreatAsRichardFunction)
				_globalObjects[name].Values[property] = new RichNativeFunction(null, info.ParamCount - 1, info);
			else
				_globalObjects[name].Values[property] = new RichFunctionInfo(null, info);
		}

		public static bool HasObject(string name) => _globalObjects.ContainsKey(name);

		public static RichObject GetObject(string name)
		{
			if (!HasObject(name))
				return null;
			return _globalObjects[name];
		}

		public static IEnumerable<Type> GetPropertyTypes() => _properties.Keys;
		public static IEnumerable<string> GetProperties(Type type) => _properties[type].Keys;
		public static RantFunctionSignature GetPropertyFunction(Type type, string name) => _properties[type][name];

		public static IEnumerable<string> GetGlobalObjects() => _globalObjects.Keys;
		public static IEnumerable<string> GetObjectProperties(string name) => _globalObjects[name].Values.Keys;
		public static RantFunctionSignature GetObjectProperty(string name, string prop)
		{
			var property = _globalObjects[name].Values[prop];
			if (property is RichFunctionInfo)
				return (property as RichFunctionInfo).Function;
			return (property as RichNativeFunction).Function;
		}

		// String properties
		[RichardProperty("length", typeof(string), Returns = "number", IsFunction = false)]
		[RantDescription("Returns the length of this string.")]
		private static IEnumerator<RantAction> StringLength(Sandbox sb, RantObject that)
		{
			yield return new RichNumber((that.Value as string).Length, null);
		}

		// List properties
		[RichardProperty("length", typeof(RichList), Returns = "number", IsFunction = false)]
		[RantDescription("Returns the length of this list.")]
		private static IEnumerator<RantAction> ListLength(Sandbox sb, RantObject that)
		{
			yield return that.Value as RichList;
			var list = sb.ScriptObjectStack.Pop();
			yield return new RichNumber((list as RichList).Items.Count, null);
		}

		[RichardProperty("last", typeof(RichList), Returns = "any", IsFunction = false)]
		[RantDescription("Returns the last item of this list.")]
		private static IEnumerator<RantAction> ListLast(Sandbox sb, RantObject that)
		{
			yield return that.Value as RichList;
			var list = sb.ScriptObjectStack.Pop();
			yield return (list as RichList).Items.Last();
		}

		[RichardProperty("push", typeof(RichList))]
		[RantDescription("Pushes the given item onto the end of this list.")]
		private static IEnumerator<RantAction> ListPush(Sandbox sb, RantObject that,
			[RichardPropertyArgument("item", "any", Description = "The item to push onto the list.")]
			RantObject obj)
		{
			(that.Value as RichList).InternalItems.Add(new RichDummy(null, obj.Value));
			(that.Value as RichList).ItemsChanged = true;
			yield break;
		}

		[RichardProperty("pop", typeof(RichList), Returns = "any")]
		[RantDescription("Pops the last item from this list.")]
		private static IEnumerator<RantAction> ListPop(Sandbox sb, RantObject that)
		{
			var last = (that.Value as RichList).Items.Last();
			(that.Value as RichList).InternalItems.RemoveAt((that.Value as RichList).Items.Count - 1);
			(that.Value as RichList).ItemsChanged = true;
			yield return last;
		}

		[RichardProperty("pushFront", typeof(RichList))]
		[RantDescription("Pushes the given item onto the front of this list.")]
		private static IEnumerator<RantAction> ListPushFront(Sandbox sb, RantObject that,
			[RichardPropertyArgument("item", "any", Description = "The item to push onto the list.")]
			RantObject obj)
		{
			(that.Value as RichList).InternalItems.Insert(0, new RichDummy(null, obj.Value));
			(that.Value as RichList).ItemsChanged = true;
			yield break;
		}

		[RichardProperty("popFront", typeof(RichList), Returns = "any")]
		[RantDescription("Pops the first item from this list.")]
		private static IEnumerator<RantAction> ListPopFront(Sandbox sb, RantObject that)
		{
			var first = (that.Value as RichList).Items.First();
			(that.Value as RichList).InternalItems.RemoveAt(0);
			(that.Value as RichList).ItemsChanged = true;
			yield return first;
		}

		[RichardProperty("copy", typeof(RichList), Returns = "list")]
		[RantDescription("Returns a new list with identical items.")]
		private static IEnumerator<RantAction> ListCopy(Sandbox sb, RantObject that)
		{
			var newList = (that.Value as RichList).Items.ToList();
			yield return new RichList((that.Value as RichList).Range, newList, false);
		}

		[RichardProperty("fill", typeof(RichList))]
		[RantDescription("Fills every item of the list with a specified value.")]
		private static IEnumerator<RantAction> ListFill(Sandbox sb, RantObject that,
			[RichardPropertyArgument("value", "any", Description = "The value to fill the list with.")]
			RantObject obj)
		{
			var fillValue = obj.Value;
			(that.Value as RichList).InternalItems =
				(that.Value as RichList).Items.Select(
					x => (RichActionBase)new RichDummy(null, fillValue)
				).ToList();
			(that.Value as RichList).ItemsChanged = true;
			yield break;
		}

		[RichardProperty("reverse", typeof(RichList))]
		[RantDescription("Reverses this list in place.")]
		private static IEnumerator<RantAction> ListReverse(Sandbox sb, RantObject that)
		{
			(that.Value as RichList).InternalItems.Reverse();
			(that.Value as RichList).ItemsChanged = true;
			yield break;
		}

		[RichardProperty("join", typeof(RichList), Returns = "string")]
		[RantDescription("Returns the list items joined together by a specified string.")]
		private static IEnumerator<RantAction> ListJoin(Sandbox sb, RantObject that,
			[RichardPropertyArgument("seperator", "string", Description = "The string to seperate each item of the list.")]
			RantObject obj)
		{
			yield return (that.Value as RichList);
			var list = sb.ScriptObjectStack.Pop();
			sb.ScriptObjectStack.Push(string.Join(obj.ToString(), (list as RichList).Items.Select(item => item.ToString()).ToArray()));
			yield break;
		}

		[RichardProperty("insert", typeof(RichList))]
		[RantDescription("Inserts an item into a specified position in this list.")]
		private static IEnumerator<RantAction> ListInsert(Sandbox sb, RantObject that,
			[RichardPropertyArgument("item", "any", Description = "The item to insert into this list.")]
			RantObject item,
			[RichardPropertyArgument("position", "number", Description = "The position in this list to insert the item into.")]
			RantObject pos)
		{
			var obj = item.Value;
			var position = pos.Value;
			if (!(position is double))
				throw new RantRuntimeException(sb.Pattern, null, "Position must be a number.");
			(that.Value as RichList).InternalItems.Insert(Convert.ToInt32(position), new RichDummy(null, obj));
			(that.Value as RichList).ItemsChanged = true;
			yield break;
		}

		[RichardProperty("remove", typeof(RichList))]
		[RantDescription("Removes an item from a specified position in this list.")]
		private static IEnumerator<RantAction> ListRemove(Sandbox sb, RantObject that,
			[RichardPropertyArgument("position", "number", Description = "The position in this list to remove the item from.")]
			RantObject pos)
		{
			var position = pos.Value;
			if (!(position is double))
				throw new RantRuntimeException(sb.Pattern, null, "Position must be a number.");
			(that.Value as RichList).InternalItems.RemoveAt(Convert.ToInt32(position));
			(that.Value as RichList).ItemsChanged = true;
			yield break;
		}

		[RichardProperty("filter", typeof(RichList))]
		[RantDescription("Filters the items of this list according to the results of a test function.")]
		private static IEnumerator<RantAction> ListFilter(Sandbox sb, RantObject that,
			[RichardPropertyArgument("test", "function", Description = "The test function which will be called for every item in this list.")]
			RantObject test)
		{
			if (!(test.Value is RichFunction))
				throw new RantRuntimeException(sb.Pattern, null, "Test must be a function.");
			List<RichActionBase> filteredObjects = new List<RichActionBase>();
			var testFunction = (test.Value as RichFunction);
			foreach (RichActionBase action in (that.Value as RichList).Items)
			{
				var count = sb.ScriptObjectStack.Count;
				yield return action;
				object currentObj = null;
				if (count < sb.ScriptObjectStack.Count)
					currentObj = sb.ScriptObjectStack.Peek();
				count = sb.ScriptObjectStack.Count;
				if (testFunction.ArgCount != 1)
					throw new RantRuntimeException(sb.Pattern, testFunction.Range, "Expected test function to accept only one argument.");
				var enumerator = testFunction.Execute(sb);
				while (enumerator.MoveNext())
					yield return enumerator.Current;
				if (count > sb.ScriptObjectStack.Count)
					throw new RantRuntimeException(sb.Pattern, testFunction.Range, "Expected boolean value from test function, got nothing.");
				var testValue = sb.ScriptObjectStack.Pop();
				if (!(testValue is bool))
					throw new RantRuntimeException(sb.Pattern, testFunction.Range, $"Expected boolean value from test function, got {Util.ScriptingObjectType(testValue)}.");
				var testBool = (bool)testValue;
				if (testBool)
					filteredObjects.Add(new RichDummy(testFunction.Range, currentObj));
			}
			sb.ScriptObjectStack.Push(new RichList(sb.CurrentAction.Range, filteredObjects, false));
			yield break;
		}


		/* 
        Global Objects
        */

		[RichardGlobalObject("Target", "send")]
		[RantDescription("Sends the provided value to the named target.")]
		private static IEnumerator<RichActionBase> TargetSend(Sandbox sb, RantObject that,
			[RichardPropertyArgument("targetName", "string", Description = "The name of the target to write to.")]
			RantObject obj1,
			[RichardPropertyArgument("value", "string", Description = "The value to write to the target.")]
			RantObject obj2
		)
		{
			string targetName = obj1.Value as string;
			string value = obj2.Value as string;
			if (targetName == null)
				throw new RantRuntimeException(sb.Pattern, null, "targetName must be a string.");
			if (value == null)
				throw new RantRuntimeException(sb.Pattern, null, "value must be a string.");
			sb.Output.PrintToTarget(targetName, value);
			yield break;
		}

		[RichardGlobalObject("Target", "clear")]
		[RantDescription("Clears the named target.")]
		private static IEnumerator<RichActionBase> TargetClear(Sandbox sb, RantObject that,
			[RichardPropertyArgument("targetName", "string", Description = "The target name to clear.")]
			RantObject obj1)
		{
			string targetName = obj1.Value as string;
			if (targetName == null)
				throw new RantRuntimeException(sb.Pattern, null, "targetName must be a string.");
			sb.Output.Do(chain => chain.ClearTarget(targetName));
			yield break;
		}

		[RichardGlobalObject("Sync", "set")]
		[RantDescription("Creates and applies a synchronizer with the specified name and type.")]
		private static IEnumerator<RichActionBase> SyncSet(Sandbox sb, RantObject that,
			[RichardPropertyArgument("syncName", "string", Description = "The name of the synchronizer to create.")]
			RantObject obj1,
			[RichardPropertyArgument("syncType", "string", Description = "The type of synchronizer to create.")]
			RantObject obj2)
		{
			string syncName = obj1.Value as string;
			string syncType = obj2.Value as string;
			if (syncName == null)
				throw new RantRuntimeException(sb.Pattern, null, "syncName must be a string.");
			if (syncType == null)
				throw new RantRuntimeException(sb.Pattern, null, "syncType must be a string.");
			object type;
			Util.TryParseEnum(typeof(SyncType), syncType, out type);
			sb.SyncManager.Create(syncName, (SyncType)type, true);
			yield break;
		}

		[RichardGlobalObject("Sync", "reset")]
		[RantDescription("Resets the named synchronizer.")]
		private static IEnumerator<RichActionBase> SyncReset(Sandbox sb, RantObject that,
			[RichardPropertyArgument("syncName", "string", Description = "The name of the synchronizer to reset.")]
			RantObject obj1)
		{
			string syncName = obj1.Value as string;
			if (syncName == null)
				throw new RantRuntimeException(sb.Pattern, null, "syncName must be a string.");
			sb.SyncManager.Reset(syncName);
			yield break;
		}

		[RichardGlobalObject("Sync", "reseed")]
		[RantDescription("Reseeds the named synchronizer with a provided seed.")]
		private static IEnumerator<RichActionBase> SyncReseed(Sandbox sb, RantObject that,
			[RichardPropertyArgument("syncName", "string", Description = "The name of the synchronizer to reseed.")]
			RantObject obj1,
			[RichardPropertyArgument("seed", "string", Description = "The value that the synchronizer will be seeded with.")]
			RantObject obj2)
		{
			string syncName = obj1.Value as string;
			string seed = obj2.Value as string;
			if (syncName == null)
				throw new RantRuntimeException(sb.Pattern, null, "syncName must be a string.");
			if (seed == null)
				throw new RantRuntimeException(sb.Pattern, null, "seed must be a string.");
			if (!sb.SyncManager.SynchronizerExists(syncName))
				throw new RantRuntimeException(sb.Pattern, null, "Synchronizer does not exist.");
			sb.SyncManager[syncName].Reseed(seed);
			yield break;
		}

		[RichardGlobalObject("Sync", "pin")]
		[RantDescription("Pins the named synchronizer.")]
		private static IEnumerator<RichActionBase> SyncPin(Sandbox sb, RantObject that,
			[RichardPropertyArgument("syncName", "string", Description = "The name of the synchronizer to pin.")]
			RantObject obj1)
		{
			string syncName = obj1.Value as string;
			if (syncName == null)
				throw new RantRuntimeException(sb.Pattern, null, "syncName must be a string.");
			sb.SyncManager.SetPinned(syncName, true);
			yield break;
		}

		[RichardGlobalObject("Sync", "unpin")]
		[RantDescription("Unpins the named synchronizer.")]
		private static IEnumerator<RichActionBase> SyncUnpin(Sandbox sb, RantObject that,
			[RichardPropertyArgument("syncName", "string", Description = "The name of the synchronizer to unpin.")]
			RantObject obj1)
		{
			string syncName = obj1.Value as string;
			if (syncName == null)
				throw new RantRuntimeException(sb.Pattern, null, "syncName must be a string.");
			sb.SyncManager.SetPinned(syncName, false);
			yield break;
		}

		[RichardGlobalObject("Sync", "isPinned", Returns = "bool")]
		[RantDescription("Returns whether or not the named synchronizer is pinned.")]
		private static IEnumerator<RichActionBase> SyncIsPinned(Sandbox sb, RantObject that,
			[RichardPropertyArgument("syncName", "string", Description = "The name of the synchronizer to check for pinnedness. Pinfulness?")]
			RantObject obj1)
		{
			string syncName = obj1.Value as string;
			if (syncName == null)
				throw new RantRuntimeException(sb.Pattern, null, "syncName must be a string.");
			if (!sb.SyncManager.SynchronizerExists(syncName))
				throw new RantRuntimeException(sb.Pattern, null, "Synchronizer does not exist.");
			sb.ScriptObjectStack.Push(sb.SyncManager[syncName].Pinned);
			yield break;
		}

		[RichardGlobalObject("Sync", "exists", Returns = "bool")]
		[RantDescription("Returns whether or not the named synchronizer exists.")]
		private static IEnumerator<RichActionBase> SyncExists(Sandbox sb, RantObject that,
			[RichardPropertyArgument("syncName", "string", Description = "The name of the synchronizer whose existence will be checked.")]
			RantObject obj1)
		{
			string syncName = obj1.Value as string;
			if (syncName == null)
				throw new RantRuntimeException(sb.Pattern, null, "syncName must be a string.");
			sb.ScriptObjectStack.Push(sb.SyncManager.SynchronizerExists(syncName));
			yield break;
		}

		[RichardGlobalObject("Sync", "setPinnedState")]
		[RantDescription("Sets the pinned state of the named synchronizer.")]
		private static IEnumerator<RichActionBase> SyncSetPinnedState(Sandbox sb, RantObject that,
			[RichardPropertyArgument("syncName", "string", Description = "The name of the synchronizer to set pinned state on.")]
			RantObject obj1,
			[RichardPropertyArgument("pinnedState", "bool", Description = "The pinned state that will be assigned to this synchronizer.")]
			RantObject obj2)
		{
			string syncName = obj1.Value as string;
			object pinnedState = obj2.Value;
			if (syncName == null)
				throw new RantRuntimeException(sb.Pattern, null, "syncName must be a string.");
			if (!(pinnedState is bool))
				throw new RantRuntimeException(sb.Pattern, null, "pinnedState must be a bool.");
			sb.SyncManager.SetPinned(syncName, (bool)pinnedState);
			yield break;
		}

		[RichardGlobalObject("Convert", "toString", Returns = "string")]
		[RantDescription("Converts the specified object to a string.")]
		private static IEnumerator<RichActionBase> ConvertToString(Sandbox sb, RantObject that,
			[RichardPropertyArgument("object", "any", Description = "The object to convert to a string.")]
			RantObject obj)
		{
			sb.ScriptObjectStack.Push(obj.ToString());
			yield break;
		}

		[RichardGlobalObject("Convert", "toNumber", Returns = "number")]
		[RantDescription("Converts the specified object to a number.")]
		private static IEnumerator<RichActionBase> ConvertToNumber(Sandbox sb, RantObject that,
			[RichardPropertyArgument("object", "any", Description = "The object to convert to a number.")]
			RantObject obj)
		{
			string val = obj.ToString();
			double n;
			if (!Util.ParseDouble(val, out n))
				sb.ScriptObjectStack.Push(new RantObject());
			sb.ScriptObjectStack.Push(n);
			yield break;
		}

		[RichardGlobalObject("Output", "print")]
		[RantDescription("Prints the provided object, cast to a string.")]
		private static IEnumerator<RichActionBase> OutputPrint(Sandbox sb, RantObject that,
			[RichardPropertyArgument("object", "any", Description = "The object to print.")]
			RantObject obj)
		{
			sb.Print(obj.ToString());
			yield break;
		}

		[RichardGlobalObject("Type", "get", Returns = "string")]
		[RantDescription("Checks the type of a provided object and returns it.")]
		private static IEnumerator<RichActionBase> TypeGet(Sandbox sb, RantObject that,
			[RichardPropertyArgument("object", "any", Description = "The object whose type will be returned.")]
			RantObject obj)
		{
			string type = Util.ScriptingObjectType(obj);
			sb.ScriptObjectStack.Push(type);
			yield break;
		}

		[RichardGlobalObject("Math", "E", Returns = "number", IsFunction = false)]
		[RantDescription("Returns the value of the mathematical constant E.")]
		private static IEnumerator<RichActionBase> MathE(Sandbox sb)
		{
			sb.ScriptObjectStack.Push(Math.E);
			yield break;
		}

		[RichardGlobalObject("Math", "PI", Returns = "number", IsFunction = false)]
		[RantDescription("Returns the value of the mathematical constant Pi.")]
		private static IEnumerator<RichActionBase> MathPI(Sandbox sb)
		{
			sb.ScriptObjectStack.Push(Math.PI);
			yield break;
		}
		[RichardGlobalObject("Math", "acos", Returns = "number")]
		[RantDescription("Returns acos(n).")]
		private static IEnumerator<RichActionBase> MathAcos(Sandbox sb, RantObject that,
			[RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
			RantObject obj)
		{
			var num = obj.Value;
			if (!(num is double))
				throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
			sb.ScriptObjectStack.Push(Math.Acos((double)num));
			yield break;
		}

		[RichardGlobalObject("Math", "asin", Returns = "number")]
		[RantDescription("Returns asin(n).")]
		private static IEnumerator<RichActionBase> MathAsin(Sandbox sb, RantObject that,
			[RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
			RantObject obj)
		{
			var num = obj.Value;
			if (!(num is double))
				throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
			sb.ScriptObjectStack.Push(Math.Asin((double)num));
			yield break;
		}

		[RichardGlobalObject("Math", "atan", Returns = "number")]
		[RantDescription("Returns atan(n).")]
		private static IEnumerator<RichActionBase> MathAtan(Sandbox sb, RantObject that,
			[RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
			RantObject obj)
		{
			var num = obj.Value;
			if (!(num is double))
				throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
			sb.ScriptObjectStack.Push(Math.Atan((double)num));
			yield break;
		}
		[RichardGlobalObject("Math", "atan2", Returns = "number")]
		[RantDescription("Returns atan2(y, n).")]
		private static IEnumerator<RichActionBase> MathAtan2(Sandbox sb, RantObject that,
			[RichardPropertyArgument("y", "number", Description = "The Y value of the atan2 operation.")]
			RantObject arg1,
			[RichardPropertyArgument("x", "number", Description = "The X value of the atan2 operation.")]
			RantObject arg2)
		{
			var y = arg1.Value;
			if (!(y is double))
				throw new RantRuntimeException(sb.Pattern, null, "Y must be a number.");
			var x = arg2.Value;
			if (!(x is double))
				throw new RantRuntimeException(sb.Pattern, null, "X must be a number.");
			sb.ScriptObjectStack.Push(Math.Atan2((double)y, (double)x));
			yield break;
		}

		[RichardGlobalObject("Math", "ceil", Returns = "number")]
		[RantDescription("Returns ceil(n).")]
		private static IEnumerator<RichActionBase> MathCeil(Sandbox sb, RantObject that,
			[RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
			RantObject obj)
		{
			var num = obj.Value;
			if (!(num is double))
				throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
			sb.ScriptObjectStack.Push(Math.Ceiling((double)num));
			yield break;
		}

		[RichardGlobalObject("Math", "cos", Returns = "number")]
		[RantDescription("Returns cos(n).")]
		private static IEnumerator<RichActionBase> MathCos(Sandbox sb, RantObject that,
			[RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
			RantObject obj)
		{
			var num = obj.Value;
			if (!(num is double))
				throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
			sb.ScriptObjectStack.Push(Math.Cos((double)num));
			yield break;
		}

		[RichardGlobalObject("Math", "cosh", Returns = "number")]
		[RantDescription("Returns cosh(n).")]
		private static IEnumerator<RichActionBase> MathCosh(Sandbox sb, RantObject that,
			[RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
			RantObject obj)
		{
			var num = obj.Value;
			if (!(num is double))
				throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
			sb.ScriptObjectStack.Push(Math.Cosh((double)num));
			yield break;
		}

		[RichardGlobalObject("Math", "exp", Returns = "number")]
		[RantDescription("Returns exp(n).")]
		private static IEnumerator<RichActionBase> MathExp(Sandbox sb, RantObject that,
			[RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
			RantObject obj)
		{
			var num = obj.Value;
			if (!(num is double))
				throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
			sb.ScriptObjectStack.Push(Math.Exp((double)num));
			yield break;
		}

		[RichardGlobalObject("Math", "floor", Returns = "number")]
		[RantDescription("Returns floor(n).")]
		private static IEnumerator<RichActionBase> MathFloor(Sandbox sb, RantObject that,
			[RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
			RantObject obj)
		{
			var num = obj.Value;
			if (!(num is double))
				throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
			sb.ScriptObjectStack.Push(Math.Floor((double)num));
			yield break;
		}

		[RichardGlobalObject("Math", "log", Returns = "number")]
		[RantDescription("Returns log(n).")]
		private static IEnumerator<RichActionBase> MathLog(Sandbox sb, RantObject that,
			[RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
			RantObject obj)
		{
			var num = obj.Value;
			if (!(num is double))
				throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
			sb.ScriptObjectStack.Push(Math.Log((double)num));
			yield break;
		}

		[RichardGlobalObject("Math", "log10", Returns = "number")]
		[RantDescription("Returns log10(n).")]
		private static IEnumerator<RichActionBase> MathLog10(Sandbox sb, RantObject that,
			[RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
			RantObject obj)
		{
			var num = obj.Value;
			if (!(num is double))
				throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
			sb.ScriptObjectStack.Push(Math.Log10((double)num));
			yield break;
		}

		[RichardGlobalObject("Math", "max", Returns = "number")]
		[RantDescription("Returns the largest of the two provided numbers.")]
		private static IEnumerator<RichActionBase> MathMax(Sandbox sb, RantObject that,
			[RichardPropertyArgument("a", "number")]
			RantObject arg1,
			[RichardPropertyArgument("b", "number")]
			RantObject arg2)
		{
			var a = arg1.Value;
			if (!(a is double))
				throw new RantRuntimeException(sb.Pattern, null, "A must be a number.");
			var b = arg2.Value;
			if (!(b is double))
				throw new RantRuntimeException(sb.Pattern, null, "B must be a number.");
			sb.ScriptObjectStack.Push(Math.Max((double)a, (double)b));
			yield break;
		}

		[RichardGlobalObject("Math", "min", Returns = "number")]
		[RantDescription("Returns the smallest of the two provided numbers.")]
		private static IEnumerator<RichActionBase> MathMin(Sandbox sb, RantObject that,
			[RichardPropertyArgument("a", "number")]
			RantObject arg1,
			[RichardPropertyArgument("b", "number")]
			RantObject arg2)
		{
			var a = arg1.Value;
			if (!(a is double))
				throw new RantRuntimeException(sb.Pattern, null, "A must be a number.");
			var b = arg2.Value;
			if (!(b is double))
				throw new RantRuntimeException(sb.Pattern, null, "B must be a number.");
			sb.ScriptObjectStack.Push(Math.Min((double)a, (double)b));
			yield break;
		}

		[RichardGlobalObject("Math", "pow", Returns = "number")]
		[RantDescription("Raises base to exponent and returns the resulting number.")]
		private static IEnumerator<RichActionBase> MathPow(Sandbox sb, RantObject that,
			[RichardPropertyArgument("base", "number", Description = "The base number of the exponent operation.")]
			RantObject baseObj,
			[RichardPropertyArgument("exponent", "number", Description = "The exponent that base will be raised to.")]
			RantObject expObj)
		{
			if (baseObj.Type != RantObjectType.Number)
				throw new RantRuntimeException(sb.Pattern, null, "Base must be a number.");
			if (expObj.Type != RantObjectType.Number)
				throw new RantRuntimeException(sb.Pattern, null, "Exponent must be a number.");
			sb.ScriptObjectStack.Push(Math.Pow((double)baseObj.Value, (double)expObj.Value));
			yield break;
		}

		[RichardGlobalObject("Math", "round", Returns = "number")]
		[RantDescription("Returns round(n).")]
		private static IEnumerator<RichActionBase> MathRound(Sandbox sb, RantObject that,
			[RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
			RantObject obj)
		{
			var num = obj.Value;
			if (!(num is double))
				throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
			sb.ScriptObjectStack.Push(Math.Round((double)num));
			yield break;
		}

		[RichardGlobalObject("Math", "sin", Returns = "number")]
		[RantDescription("Returns sin(n).")]
		private static IEnumerator<RichActionBase> MathSin(Sandbox sb, RantObject that,
			[RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
			RantObject obj)
		{
			var num = obj.Value;
			if (!(num is double))
				throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
			sb.ScriptObjectStack.Push(Math.Sin((double)num));
			yield break;
		}

		[RichardGlobalObject("Math", "sinh", Returns = "number")]
		[RantDescription("Returns sinh(n).")]
		private static IEnumerator<RichActionBase> MathSinh(Sandbox sb, RantObject that,
			[RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
			RantObject obj)
		{
			var num = obj.Value;
			if (!(num is double))
				throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
			sb.ScriptObjectStack.Push(Math.Sinh((double)num));
			yield break;
		}

		[RichardGlobalObject("Math", "sqrt", Returns = "number")]
		[RantDescription("Returns sqrt(n).")]
		private static IEnumerator<RichActionBase> MathSqrt(Sandbox sb, RantObject that,
			[RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
			RantObject obj)
		{
			var num = obj.Value;
			if (!(num is double))
				throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
			sb.ScriptObjectStack.Push(Math.Sqrt((double)num));
			yield break;
		}

		[RichardGlobalObject("Math", "tan", Returns = "number")]
		[RantDescription("Returns tan(n).")]
		private static IEnumerator<RichActionBase> MathTan(Sandbox sb, RantObject that,
			[RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
			RantObject obj)
		{
			var num = obj.Value;
			if (!(num is double))
				throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
			sb.ScriptObjectStack.Push(Math.Tan((double)num));
			yield break;
		}

		[RichardGlobalObject("Math", "tanh", Returns = "number")]
		[RantDescription("Returns tanh(n).")]
		private static IEnumerator<RichActionBase> MathTanh(Sandbox sb, RantObject that,
			[RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
			RantObject obj)
		{
			var num = obj.Value;
			if (!(num is double))
				throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
			sb.ScriptObjectStack.Push(Math.Tanh((double)num));
			yield break;
		}

		[RichardGlobalObject("Math", "truncate", Returns = "number")]
		[RantDescription("Returns truncate(n).")]
		private static IEnumerator<RichActionBase> MathTruncate(Sandbox sb, RantObject that,
			[RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
			RantObject obj)
		{
			var num = obj.Value;
			if (!(num is double))
				throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
			sb.ScriptObjectStack.Push(Math.Truncate((double)num));
			yield break;
		}
	}

	internal class RichardProperty : Attribute
	{
		public string Name;
		public Type ObjectType;
		public string Returns;
		public bool IsFunction = true;

		public RichardProperty(string name, Type objectType)
		{
			Name = name;
			ObjectType = objectType;
		}
	}

	[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	internal class RichardPropertyArgument : Attribute
	{
		public string Name;
		public string Description;
		public string Type;

		public RichardPropertyArgument(string name, string type)
		{
			Name = name;
			Type = type;
		}
	}

	internal class RichardGlobalObject : Attribute
	{
		public string Name;
		public string Property;
		public string Returns;
		public bool IsFunction = true;

		public RichardGlobalObject(string name, string property)
		{
			Name = name;
			Property = property;
		}
	}
}
