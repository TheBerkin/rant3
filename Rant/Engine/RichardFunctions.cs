using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Rant.Engine.Constructs;
using Rant.Engine.Formatters;
using Rant.Engine.Metadata;
using Rant.Engine.Syntax.Expressions;
using Rant.Engine.ObjectModel;
using Rant.Engine.Syntax;
using Rant.Vocabulary;

namespace Rant.Engine
{
    internal class RichardFunctions
    {
        private static bool _loaded = false;
        private static Dictionary<Type, Dictionary<string, RantFunctionInfo>> _properties;
        private static Dictionary<string, REAObject> _globalObjects;

        static RichardFunctions()
        {
            Load();
        }

        public static void Load()
        {
            if (_loaded) return;

            _properties = new Dictionary<Type, Dictionary<string, RantFunctionInfo>>();
            _globalObjects = new Dictionary<string, REAObject>();

            foreach (var method in typeof(RichardFunctions).GetMethods(BindingFlags.Static | BindingFlags.NonPublic))
            {
                if (!method.IsStatic) continue;
                var attr = method.GetCustomAttributes().OfType<RichardProperty>().FirstOrDefault();
                if (attr == null)
                {
                    var objectAttr = method.GetCustomAttributes().OfType<RichardGlobalObject>().FirstOrDefault();
                    if (objectAttr == null) continue;
                    var name = String.IsNullOrEmpty(objectAttr.Property) ? method.Name.ToLower() : objectAttr.Property;
                    var descAttr = method.GetCustomAttributes().OfType<RantDescriptionAttribute>().FirstOrDefault();
                    var info = new RantFunctionInfo(name, descAttr?.Description ?? String.Empty, method);
                    if (Util.ValidateName(name)) RegisterGlobalObject(objectAttr.Name, objectAttr.Property, info);
                }
                else
                {
                    var name = String.IsNullOrEmpty(attr.Name) ? method.Name.ToLower() : attr.Name;
                    var descAttr = method.GetCustomAttributes().OfType<RantDescriptionAttribute>().FirstOrDefault();
                    var info = new RantFunctionInfo(name, descAttr?.Description ?? String.Empty, method);
                    if (Util.ValidateName(name)) RegisterProperty(attr.ObjectType, info);
                }
            }

            _loaded = true;
        }

        private static void RegisterProperty(Type objectType, RantFunctionInfo info)
        {
            var dictionary = _properties.ContainsKey(objectType) ?
                _properties[objectType] :
                new Dictionary<string, RantFunctionInfo>();
            dictionary.Add(info.Name, info);
            _properties[objectType] = dictionary;
        }

        public static bool HasProperty(Type objectType, string name) =>
            _properties.ContainsKey(objectType) ? _properties[objectType].ContainsKey(name) : false;

        public static RantFunctionInfo GetProperty(Type objectType, string name)
        {
            if (!HasProperty(objectType, name))
                return null;
            return _properties[objectType][name];
        }

        private static void RegisterGlobalObject(string name, string property, RantFunctionInfo info)
        {
            if (!_globalObjects.ContainsKey(name))
                _globalObjects[name] = new REAObject(null);
            _globalObjects[name].Values[property] = new REAFunctionInfo(null, info);
        }

        public static bool HasObject(string name) => _globalObjects.ContainsKey(name);

        public static REAObject GetObject(string name)
        {
            if (!HasObject(name))
                return null;
            return _globalObjects[name];
        }

        // String properties
        [RichardProperty("length", typeof(string), Returns = "number", IsFunction = false)]
        [RantDescription("Returns the length of this string.")]
        private static IEnumerator<RantAction> StringLength(Sandbox sb, RantObject that)
        {
            yield return new REANumber((that.Value as string).Length, null);
        }

        // List properties
        [RichardProperty("length", typeof(REAList), Returns = "number", IsFunction = false)]
        [RantDescription("Returns the length of this list.")]
        private static IEnumerator<RantAction> ListLength(Sandbox sb, RantObject that)
        {
            yield return that.Value as REAList;
            var list = sb.ScriptObjectStack.Pop();
            yield return new REANumber((list as REAList).Items.Count, null);
        }

        [RichardProperty("last", typeof(REAList), Returns = "any", IsFunction = false)]
        [RantDescription("Returns the last item of this list.")]
        private static IEnumerator<RantAction> ListLast(Sandbox sb, RantObject that)
        {
            yield return that.Value as REAList;
            var list = sb.ScriptObjectStack.Pop();
            yield return (list as REAList).Items.Last();
        }

        [RichardProperty("push", typeof(REAList))]
        [RantDescription("Pushes the given item onto the end of this list.")]
        [RichardPropertyArgument("item", "any", Description = "The item to push onto the list.")]
        private static IEnumerator<RantAction> ListPush(Sandbox sb, RantObject that)
        {
            yield return that.Value as REAList;
            that = new RantObject(sb.ScriptObjectStack.Pop() as REAList);
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, ListPushInner) { That = that };
        }

        private static IEnumerator<RantExpressionAction> ListPushInner(RantObject that, Sandbox sb, object[] args)
        {
            (that.Value as REAList).Items.Add(new READummy(null, args[0]));
            yield break;
        }

        [RichardProperty("pop", typeof(REAList), Returns = "any")]
        [RantDescription("Pops the last item from this list.")]
        private static IEnumerator<RantAction> ListPop(Sandbox sb, RantObject that)
        {
            yield return that.Value as REAList;
            that = new RantObject(sb.ScriptObjectStack.Pop() as REAList);
            yield return new REANativeFunction(sb.CurrentAction.Range, 0, ListPopInner) { That = that };
        }

        // the native function behind ListPop
        private static IEnumerator<RantExpressionAction> ListPopInner(RantObject that, Sandbox sb, object[] args)
        {
            var last = (that.Value as REAList).Items.Last();
            (that.Value as REAList).Items.RemoveAt((that.Value as REAList).Items.Count - 1);
            yield return last;
        }

        [RichardProperty("pushFront", typeof(REAList))]
        [RantDescription("Pushes the given item onto the front of this list.")]
        [RichardPropertyArgument("item", "any", Description = "The item to push onto the list.")]
        private static IEnumerator<RantAction> ListPushFront(Sandbox sb, RantObject that)
        {
            yield return that.Value as REAList;
            that = new RantObject(sb.ScriptObjectStack.Pop() as REAList);
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, ListPushFrontInner) { That = that };
        }

        private static IEnumerator<RantExpressionAction> ListPushFrontInner(RantObject that, Sandbox sb, object[] args)
        {
            (that.Value as REAList).Items.Insert(0, new READummy(null, args[0]));
            yield break;
        }

        [RichardProperty("popFront", typeof(REAList), Returns = "any")]
        [RantDescription("Pops the first item from this list.")]
        private static IEnumerator<RantAction> ListPopFront(Sandbox sb, RantObject that)
        {
            yield return that.Value as REAList;
            that = new RantObject(sb.ScriptObjectStack.Pop() as REAList);
            yield return new REANativeFunction(sb.CurrentAction.Range, 0, ListPopFrontInner) { That = that };
        }

        private static IEnumerator<RantExpressionAction> ListPopFrontInner(RantObject that, Sandbox sb, object[] args)
        {
            var first = (that.Value as REAList).Items.First();
            (that.Value as REAList).Items.RemoveAt(0);
            yield return first;
        }

        [RichardProperty("copy", typeof(REAList), Returns = "list")]
        [RantDescription("Returns a new list with identical items.")]
        private static IEnumerator<RantAction> ListCopy(Sandbox sb, RantObject that)
        {
            yield return that.Value as REAList;
            that = new RantObject(sb.ScriptObjectStack.Pop() as REAList);
            yield return new REANativeFunction(sb.CurrentAction.Range, 0, ListCopyInner) { That = that };
        }

        private static IEnumerator<RantExpressionAction> ListCopyInner(RantObject that, Sandbox sb, object[] args)
        {
            var newList = (that.Value as REAList).Items.ToList();
            yield return new REAList((that.Value as REAList).Range, newList, false);
        }

        [RichardProperty("fill", typeof(REAList))]
        [RantDescription("Fills every item of the list with a specified value.")]
        [RichardPropertyArgument("value", "any", Description = "The value to fill the list with.")]
        private static IEnumerator<RantAction> ListFill(Sandbox sb, RantObject that)
        {
            yield return that.Value as REAList;
            that = new RantObject(sb.ScriptObjectStack.Pop() as REAList);
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, ListFillInner) { That = that };
        }

        private static IEnumerator<RantExpressionAction> ListFillInner(RantObject that, Sandbox sb, object[] args)
        {
            var fillValue = args[0];
            (that.Value as REAList).Items =
                (that.Value as REAList).Items.Select(
                    x => (RantExpressionAction)new READummy(null, fillValue)
                ).ToList();
            yield break;
        }

        [RichardProperty("reverse", typeof(REAList))]
        [RantDescription("Reverses this list in place.")]
        private static IEnumerator<RantAction> ListReverse(Sandbox sb, RantObject that)
        {
            yield return that.Value as REAList;
            that = new RantObject(sb.ScriptObjectStack.Pop() as REAList);
            yield return new REANativeFunction(sb.CurrentAction.Range, 0, ListReverseInner) { That = that };
        }

        private static IEnumerator<RantExpressionAction> ListReverseInner(RantObject that, Sandbox sb, object[] args)
        {
            (that.Value as REAList).Items.Reverse();
            yield break;
        }

        [RichardProperty("join", typeof(REAList), Returns = "string")]
        [RantDescription("Returns the list items joined together by a specified string.")]
        [RichardPropertyArgument("seperator", "string", Description = "The string to seperate each item of the list.")]
        private static IEnumerator<RantAction> ListJoin(Sandbox sb, RantObject that)
        {
            yield return that.Value as REAList;
            that = new RantObject(sb.ScriptObjectStack.Pop() as REAList);
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, ListJoinInner) { That = that };
        }

        private static IEnumerator<RantExpressionAction> ListJoinInner(RantObject that, Sandbox sb, object[] args)
        {
            sb.ScriptObjectStack.Push(string.Join(args[0].ToString(), (that.Value as REAList).Items));
            yield break;
        }

        [RichardProperty("insert", typeof(REAList))]
        [RantDescription("Inserts an item into a specified position in this list.")]
        [RichardPropertyArgument("item", "any", Description = "The item to insert into this list.")]
        [RichardPropertyArgument("position", "number", Description = "The position in this list to insert the item into.")]
        private static IEnumerator<RantAction> ListInsert(Sandbox sb, RantObject that)
        {
            yield return that.Value as REAList;
            that = new RantObject(sb.ScriptObjectStack.Pop() as REAList);
            yield return new REANativeFunction(sb.CurrentAction.Range, 2, ListInsertInner) { That = that };
        }

        private static IEnumerator<RantExpressionAction> ListInsertInner(RantObject that, Sandbox sb, object[] args)
        {
            var obj = args[0];
            var position = args[1];
            if (!(position is double))
                throw new RantRuntimeException(sb.Pattern, null, "Position must be a number.");
            (that.Value as REAList).Items.Insert(Convert.ToInt32(position), new READummy(null, obj));
            yield break;
        }

        [RichardProperty("remove", typeof(REAList))]
        [RantDescription("Removes an item from a specified position in this list.")]
        [RichardPropertyArgument("position", "number", Description = "The position in this list to remove the item from.")]
        private static IEnumerator<RantAction> ListRemove(Sandbox sb, RantObject that)
        {
            yield return that.Value as REAList;
            that = new RantObject(sb.ScriptObjectStack.Pop() as REAList);
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, ListRemoveInner) { That = that };
        }

        private static IEnumerator<RantExpressionAction> ListRemoveInner(RantObject that, Sandbox sb, object[] args)
        {
            var position = args[0];
            if (!(position is double))
                throw new RantRuntimeException(sb.Pattern, null, "Position must be a number.");
            (that.Value as REAList).Items.RemoveAt(Convert.ToInt32(position));
            yield break;
        }


        /* 
        Global Objects
        */

        [RichardGlobalObject("Output", "print")]
        [RichardPropertyArgument("object", "any", Description = "The object to print.")]
        [RantDescription("Prints the provided object, cast to a string.")]
        private static IEnumerator<RantExpressionAction> OutputPrint(Sandbox sb)
        {
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, OutputPrintInner);
        }

        private static IEnumerator<RantExpressionAction> OutputPrintInner(RantObject that, Sandbox sb, object[] args)
        {
            var obj = args[0];
            sb.Print(obj);
            yield break;
        }

        [RichardGlobalObject("Type", "get", Returns = "string")]
        [RichardPropertyArgument("object", "any", Description = "The object whose type will be returned.")]
        [RantDescription("Checks the type of a provided object and returns it.")]
        private static IEnumerator<RantExpressionAction> TypeGet(Sandbox sb)
        {
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, TypeGetInner);
        }

        private static IEnumerator<RantExpressionAction> TypeGetInner(RantObject that, Sandbox sb, object[] args)
        {
            var obj = args[0];
            string type = Util.ScriptingObjectType(obj);
            sb.ScriptObjectStack.Push(type);
            yield break;
        }

        [RichardGlobalObject("Math", "E", Returns = "number")]
        [RantDescription("Returns the value of the mathematical constant E.")]
        private static IEnumerator<RantExpressionAction> MathE(Sandbox sb)
        {
            sb.ScriptObjectStack.Push(Math.E);
            yield break;
        }

        [RichardGlobalObject("Math", "PI", Returns = "number")]
        [RantDescription("Returns the value of the mathematical constant Pi.")]
        private static IEnumerator<RantExpressionAction> MathPI(Sandbox sb)
        {
            sb.ScriptObjectStack.Push(Math.PI);
            yield break;
        }
        [RichardGlobalObject("Math", "acos", Returns = "number")]
        [RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
        [RantDescription("Returns acos(n).")]
        private static IEnumerator<RantExpressionAction> MathAcos(Sandbox sb)
        {
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, MathAcosInner);
        }

        private static IEnumerator<RantExpressionAction> MathAcosInner(RantObject that, Sandbox sb, object[] args)
        {
            var num = args[0];
            if (!(num is double))
                throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
            sb.ScriptObjectStack.Push(Math.Acos((double)num));
            yield break;
        }

        [RichardGlobalObject("Math", "asin", Returns = "number")]
        [RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
        [RantDescription("Returns asin(n).")]
        private static IEnumerator<RantExpressionAction> MathAsin(Sandbox sb)
        {
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, MathAsinInner);
        }

        private static IEnumerator<RantExpressionAction> MathAsinInner(RantObject that, Sandbox sb, object[] args)
        {
            var num = args[0];
            if (!(num is double))
                throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
            sb.ScriptObjectStack.Push(Math.Asin((double)num));
            yield break;
        }

        [RichardGlobalObject("Math", "atan", Returns = "number")]
        [RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
        [RantDescription("Returns atan(n).")]
        private static IEnumerator<RantExpressionAction> MathAtan(Sandbox sb)
        {
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, MathAtanInner);
        }

        private static IEnumerator<RantExpressionAction> MathAtanInner(RantObject that, Sandbox sb, object[] args)
        {
            var num = args[0];
            if (!(num is double))
                throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
            sb.ScriptObjectStack.Push(Math.Atan((double)num));
            yield break;
        }
        [RichardGlobalObject("Math", "atan2", Returns = "number")]
        [RichardPropertyArgument("y", "number", Description = "The Y value of the atan2 operation.")]
        [RichardPropertyArgument("x", "number", Description = "The X value of the atan2 operation.")]
        [RantDescription("Returns atan2(n).")]
        private static IEnumerator<RantExpressionAction> MathAtan2(Sandbox sb)
        {
            yield return new REANativeFunction(sb.CurrentAction.Range, 2, MathAtan2Inner);
        }

        private static IEnumerator<RantExpressionAction> MathAtan2Inner(RantObject that, Sandbox sb, object[] args)
        {
            var y = args[0];
            if (!(y is double))
                throw new RantRuntimeException(sb.Pattern, null, "Y must be a number.");
            var x = args[1];
            if (!(x is double))
                throw new RantRuntimeException(sb.Pattern, null, "X must be a number.");
            sb.ScriptObjectStack.Push(Math.Atan2((double)y, (double)x));
            yield break;
        }

        [RichardGlobalObject("Math", "ceil", Returns = "number")]
        [RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
        [RantDescription("Returns ceil(n).")]
        private static IEnumerator<RantExpressionAction> MathCeil(Sandbox sb)
        {
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, MathCeilInner);
        }

        private static IEnumerator<RantExpressionAction> MathCeilInner(RantObject that, Sandbox sb, object[] args)
        {
            var num = args[0];
            if (!(num is double))
                throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
            sb.ScriptObjectStack.Push(Math.Ceiling((double)num));
            yield break;
        }

        [RichardGlobalObject("Math", "cos", Returns = "number")]
        [RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
        [RantDescription("Returns cos(n).")]
        private static IEnumerator<RantExpressionAction> MathCos(Sandbox sb)
        {
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, MathCosInner);
        }

        private static IEnumerator<RantExpressionAction> MathCosInner(RantObject that, Sandbox sb, object[] args)
        {
            var num = args[0];
            if (!(num is double))
                throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
            sb.ScriptObjectStack.Push(Math.Cos((double)num));
            yield break;
        }

        [RichardGlobalObject("Math", "cosh", Returns = "number")]
        [RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
        [RantDescription("Returns cosh(n).")]
        private static IEnumerator<RantExpressionAction> MathCosh(Sandbox sb)
        {
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, MathCoshInner);
        }

        private static IEnumerator<RantExpressionAction> MathCoshInner(RantObject that, Sandbox sb, object[] args)
        {
            var num = args[0];
            if (!(num is double))
                throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
            sb.ScriptObjectStack.Push(Math.Cosh((double)num));
            yield break;
        }

        [RichardGlobalObject("Math", "exp", Returns = "number")]
        [RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
        [RantDescription("Returns exp(n).")]
        private static IEnumerator<RantExpressionAction> MathExp(Sandbox sb)
        {
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, MathExpInner);
        }

        private static IEnumerator<RantExpressionAction> MathExpInner(RantObject that, Sandbox sb, object[] args)
        {
            var num = args[0];
            if (!(num is double))
                throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
            sb.ScriptObjectStack.Push(Math.Exp((double)num));
            yield break;
        }

        [RichardGlobalObject("Math", "floor", Returns = "number")]
        [RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
        [RantDescription("Returns floor(n).")]
        private static IEnumerator<RantExpressionAction> MathFloor(Sandbox sb)
        {
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, MathFloorInner);
        }

        private static IEnumerator<RantExpressionAction> MathFloorInner(RantObject that, Sandbox sb, object[] args)
        {
            var num = args[0];
            if (!(num is double))
                throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
            sb.ScriptObjectStack.Push(Math.Floor((double)num));
            yield break;
        }

        [RichardGlobalObject("Math", "mod", Returns = "number")]
        [RichardPropertyArgument("x", "number", Description = "The left hand side of the modulo operation.")]
        [RichardPropertyArgument("y", "number", Description = "The right hand side of the modulo operation.")]
        [RantDescription("Returns mod(n).")]
        private static IEnumerator<RantExpressionAction> MathMod(Sandbox sb)
        {
            yield return new REANativeFunction(sb.CurrentAction.Range, 2, MathModInner);
        }

        private static IEnumerator<RantExpressionAction> MathModInner(RantObject that, Sandbox sb, object[] args)
        {
            var num = args[0];
            if (!(num is double))
                throw new RantRuntimeException(sb.Pattern, null, "X must be a number.");
            var mod = args[1];
            if (!(mod is double))
                throw new RantRuntimeException(sb.Pattern, null, "Y must be a number.");
            sb.ScriptObjectStack.Push((double)num % (double)mod);
            yield break;
        }

        [RichardGlobalObject("Math", "log", Returns = "number")]
        [RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
        [RantDescription("Returns log(n).")]
        private static IEnumerator<RantExpressionAction> MathLog(Sandbox sb)
        {
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, MathLogInner);
        }

        private static IEnumerator<RantExpressionAction> MathLogInner(RantObject that, Sandbox sb, object[] args)
        {
            var num = args[0];
            if (!(num is double))
                throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
            sb.ScriptObjectStack.Push(Math.Log((double)num));
            yield break;
        }

        [RichardGlobalObject("Math", "log10", Returns = "number")]
        [RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
        [RantDescription("Returns log10(n).")]
        private static IEnumerator<RantExpressionAction> MathLog10(Sandbox sb)
        {
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, MathLog10Inner);
        }

        private static IEnumerator<RantExpressionAction> MathLog10Inner(RantObject that, Sandbox sb, object[] args)
        {
            var num = args[0];
            if (!(num is double))
                throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
            sb.ScriptObjectStack.Push(Math.Log10((double)num));
            yield break;
        }

        [RichardGlobalObject("Math", "round", Returns = "number")]
        [RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
        [RantDescription("Returns round(n).")]
        private static IEnumerator<RantExpressionAction> MathRound(Sandbox sb)
        {
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, MathRoundInner);
        }

        private static IEnumerator<RantExpressionAction> MathRoundInner(RantObject that, Sandbox sb, object[] args)
        {
            var num = args[0];
            if (!(num is double))
                throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
            sb.ScriptObjectStack.Push(Math.Round((double)num));
            yield break;
        }

        [RichardGlobalObject("Math", "sin", Returns = "number")]
        [RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
        [RantDescription("Returns sin(n).")]
        private static IEnumerator<RantExpressionAction> MathSin(Sandbox sb)
        {
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, MathSinInner);
        }

        private static IEnumerator<RantExpressionAction> MathSinInner(RantObject that, Sandbox sb, object[] args)
        {
            var num = args[0];
            if (!(num is double))
                throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
            sb.ScriptObjectStack.Push(Math.Sin((double)num));
            yield break;
        }

        [RichardGlobalObject("Math", "sinh", Returns = "number")]
        [RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
        [RantDescription("Returns sinh(n).")]
        private static IEnumerator<RantExpressionAction> MathSinh(Sandbox sb)
        {
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, MathSinhInner);
        }

        private static IEnumerator<RantExpressionAction> MathSinhInner(RantObject that, Sandbox sb, object[] args)
        {
            var num = args[0];
            if (!(num is double))
                throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
            sb.ScriptObjectStack.Push(Math.Sinh((double)num));
            yield break;
        }

        [RichardGlobalObject("Math", "sqrt", Returns = "number")]
        [RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
        [RantDescription("Returns sqrt(n).")]
        private static IEnumerator<RantExpressionAction> MathSqrt(Sandbox sb)
        {
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, MathSqrtInner);
        }

        private static IEnumerator<RantExpressionAction> MathSqrtInner(RantObject that, Sandbox sb, object[] args)
        {
            var num = args[0];
            if (!(num is double))
                throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
            sb.ScriptObjectStack.Push(Math.Sqrt((double)num));
            yield break;
        }

        [RichardGlobalObject("Math", "tan", Returns = "number")]
        [RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
        [RantDescription("Returns tan(n).")]
        private static IEnumerator<RantExpressionAction> MathTan(Sandbox sb)
        {
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, MathTanInner);
        }

        private static IEnumerator<RantExpressionAction> MathTanInner(RantObject that, Sandbox sb, object[] args)
        {
            var num = args[0];
            if (!(num is double))
                throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
            sb.ScriptObjectStack.Push(Math.Tan((double)num));
            yield break;
        }

        [RichardGlobalObject("Math", "tanh", Returns = "number")]
        [RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
        [RantDescription("Returns tanh(n).")]
        private static IEnumerator<RantExpressionAction> MathTanh(Sandbox sb)
        {
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, MathTanhInner);
        }

        private static IEnumerator<RantExpressionAction> MathTanhInner(RantObject that, Sandbox sb, object[] args)
        {
            var num = args[0];
            if (!(num is double))
                throw new RantRuntimeException(sb.Pattern, null, "N must be a number.");
            sb.ScriptObjectStack.Push(Math.Tanh((double)num));
            yield break;
        }

        [RichardGlobalObject("Math", "truncate", Returns = "number")]
        [RichardPropertyArgument("n", "number", Description = "The number that will be used in this operation.")]
        [RantDescription("Returns truncate(n).")]
        private static IEnumerator<RantExpressionAction> MathTruncate(Sandbox sb)
        {
            yield return new REANativeFunction(sb.CurrentAction.Range, 1, MathTruncateInner);
        }

        private static IEnumerator<RantExpressionAction> MathTruncateInner(RantObject that, Sandbox sb, object[] args)
        {
            var num = args[0];
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
