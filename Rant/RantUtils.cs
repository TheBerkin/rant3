using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Engine;
using Rant.Engine.Metadata;
using Rant.Engine.Syntax.Richard;

namespace Rant
{
    /// <summary>
    /// Contains miscellaneous utility methods that provide information about the Rant engine.
    /// </summary>
    public static class RantUtils
    {
        /// <summary>
        /// Determines whether a function with the specified name is defined in the current engine version.
        /// </summary>
        /// <param name="functionName">The name of the function to search for. Argument is not case-sensitive.</param>
        /// <returns></returns>
        public static bool FunctionExists(string functionName)
        {
            return RantFunctions.FunctionExists(functionName);
        }

        /// <summary>
        /// Returns the function with the specified name. The return value will be null if the function is not found.
        /// </summary>
        /// <param name="functionName">The name of the function to retrieve.</param>
        /// <returns></returns>
        public static IRantFunctionGroup GetFunction(string functionName) => RantFunctions.GetFunctionGroup(functionName);

        /// <summary>
        /// Enumerates the names of all available Rant functions.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetFunctionNames() => RantFunctions.GetFunctionNames();

        /// <summary>
        /// Enumerates all function names and their aliases.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetFunctionNamesAndAliases() => RantFunctions.GetFunctionNamesAndAliases(); 

        /// <summary>
        /// Enumerates the available functions.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IRantFunctionGroup> GetFunctions() => RantFunctions.GetFunctions();

        /// <summary>
        /// Returns the description for the function with the specified name.
        /// </summary>
        /// <param name="functionName">The name of the function to get the description for.</param>
        /// <param name="argc">The number of arguments in the overload to get the description for.</param>
        /// <returns></returns>
        public static string GetFunctionDescription(string functionName, int argc) => RantFunctions.GetFunctionDescription(functionName, argc);
        
        /// <summary>
        /// Enumerates the aliases assigned to the specified function name.
        /// </summary>
        /// <param name="functionName">The function name to retrieve aliases for.</param>
        /// <returns></returns>
        public static IEnumerable<string> GetFunctionAliases(string functionName) => RantFunctions.GetAliases(functionName);


        public static IEnumerable<Type> GetRichardPropertyTypes() => RichardFunctions.GetPropertyTypes();
        public static IEnumerable<string> GetRichardProperties(Type type) => RichardFunctions.GetProperties(type);
        public static bool IsRichardPropertyFunction(Type type, string name) => RichardFunctions.GetPropertyFunction(type, name).TreatAsRichardFunction;
        public static RichardArgumentInfo[] GetRichardPropertyArguments(Type type, string name)
        {
            return RichardFunctions
                .GetPropertyFunction(type, name)
                .RawParameters
                .Select(x =>
                    x.GetCustomAttributes(typeof(RichardPropertyArgument), false).FirstOrDefault() as RichardPropertyArgument
                )
                .Where(x => x != null)
                .Select(x => new RichardArgumentInfo() { Name = x.Name, Description = x.Description, Type = x.Type })
                .ToArray();
        }
        public static RichardPropertyInfo GetRichardPropertyInfo(Type type, string name)
        {
            var prop = RichardFunctions.GetPropertyFunction(type, name);
            var propAttribute = prop.RawMethod.GetCustomAttributes(typeof(RichardProperty), false)[0] as RichardProperty;
            return new RichardPropertyInfo()
            {
                Name = prop.Name,
                Description = prop.Description,
                IsFunction = prop.TreatAsRichardFunction,
                Returns = propAttribute.Returns
            };
        }
        

        public static IEnumerable<string> GetRichardGlobalObjects() => RichardFunctions.GetGlobalObjects();
        public static IEnumerable<string> GetRichardObjectProperties(string name) => RichardFunctions.GetObjectProperties(name);
        public static bool IsRichardGlobalObjectPropertyFunction(string name, string prop) => RichardFunctions.GetObjectProperty(name, prop).TreatAsRichardFunction;
        public static RichardArgumentInfo[] GetRichardObjectPropertyArguments(string name, string prop)
        {
            return RichardFunctions
                .GetObjectProperty(name, prop)
                .RawParameters
                .Select(x =>
                    x.GetCustomAttributes(typeof(RichardPropertyArgument), false).FirstOrDefault() as RichardPropertyArgument
                )
                .Where(x => x != null)
                .Select(x => new RichardArgumentInfo() { Name = x.Name, Description = x.Description, Type = x.Type })
                .ToArray();
        }
        public static RichardPropertyInfo GetRichardObjectPropertyInfo(string name, string prop)
        {
            var property = RichardFunctions.GetObjectProperty(name, prop);
            var propAttribute = property.RawMethod.GetCustomAttributes(typeof(RichardGlobalObject), false)[0] as RichardGlobalObject;
            return new RichardPropertyInfo()
            {
                Name = property.Name,
                Description = property.Description,
                IsFunction = property.TreatAsRichardFunction,
                Returns = propAttribute.Returns
            };
        }
        
        public static string GetRichardTypeName(Type val)
        {
            string type = "No";
            if(val == typeof(string))
                type = "String";
            else if(val == typeof(double))
                type = "Number";
            else if(val == typeof(REAList))
                type = "List";
            else if(val == typeof(REAObject))
                type = "Object";
            else if(val == typeof(REAFunction))
                type = "Function";
            else if(val == typeof(REAPatternString))
                type = "Pattern String";
            else if(val == typeof(bool))
                type = "Bool";
            return type;
        }

        public class RichardPropertyInfo
        {
            public string Name;
            public string Description;
            public bool IsFunction;
            public string Returns;
        }

        public class RichardArgumentInfo
        {
            public string Name;
            public string Description;
            public string Type;
        }
    }
}
