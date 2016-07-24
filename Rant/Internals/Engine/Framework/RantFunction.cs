using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Internals.Engine.Metadata;

namespace Rant.Internals.Engine.Framework
{
    internal class RantFunction : IRantFunctionGroup
    {
        private readonly Dictionary<int, RantFunctionSignature> _overloads = new Dictionary<int, RantFunctionSignature>();
        private RantFunctionSignature _paramsArrayFunc = null;

        public string Name { get; }

        public IEnumerable<IRantFunction> Overloads => _overloads.Select(fn => fn.Value as IRantFunction); 

        public RantFunction(string name)
        {
            Name = name;
        }

        public void Add(RantFunctionSignature func)
        {
            RantFunctionSignature existing;
            if (_overloads.TryGetValue(func.Parameters.Length, out existing))
                throw new ArgumentException($"Cannot load function {func} becaue its signature is ambiguous with existing function {existing}.");
            if (_paramsArrayFunc != null)
            {
                if (func.HasParamArray)
                    throw new ArgumentException($"Cannot load function {func} because another function with a parameter array was already loaded.");
                if (func.Parameters.Length >= _paramsArrayFunc.Parameters.Length)
                    throw new ArgumentException($"Cannot load function {func} because its signature is ambiguous with {_paramsArrayFunc}.");
            }

            _overloads[func.Parameters.Length] = func;
            if (func.HasParamArray) _paramsArrayFunc = func;
        }

        public RantFunctionSignature GetFunction(int argc)
        {
            if (_paramsArrayFunc != null && argc >= _paramsArrayFunc.Parameters.Length - 1) return _paramsArrayFunc;
            RantFunctionSignature func;
            return _overloads.TryGetValue(argc, out func) ? func : null;
        }
    }
}