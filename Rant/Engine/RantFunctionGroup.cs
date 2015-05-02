using System;
using System.Collections.Generic;

namespace Rant.Engine
{
    internal class RantFunctionGroup
    {
        private readonly Dictionary<int, RantFunctionInfo> _functions = new Dictionary<int, RantFunctionInfo>();
        private RantFunctionInfo _paramsArrayFunc = null;

        public string Name { get; }

        public RantFunctionGroup(string name)
        {
            Name = name;
        }

        public void Add(RantFunctionInfo func)
        {
            RantFunctionInfo existing;
            if (_functions.TryGetValue(func.Parameters.Length, out existing))
                throw new ArgumentException($"Cannot load function {func} becaue its signature is ambiguous with existing function {existing}.");
            if (_paramsArrayFunc != null)
            {
                if (func.HasParamArray)
                    throw new ArgumentException($"Cannot load function {func} because another function with a parameter array was already loaded.");
                if (func.Parameters.Length >= _paramsArrayFunc.Parameters.Length)
                    throw new ArgumentException($"Cannot load function {func} because its signature is ambiguous with {_paramsArrayFunc}.");
            }

            _functions[func.Parameters.Length] = func;
            if (func.HasParamArray) _paramsArrayFunc = func;
        }

        public RantFunctionInfo GetFunction(int argc)
        {
            if (_paramsArrayFunc != null && argc >= _paramsArrayFunc.Parameters.Length - 1) return _paramsArrayFunc;
            RantFunctionInfo func;
            return _functions.TryGetValue(argc, out func) ? func : null;
        }
    }
}