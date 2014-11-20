using System;
using System.Collections.Generic;
using System.Linq;

namespace Rant
{
    internal class VarStore
    {
        private readonly Dictionary<string, double> _vars;

        public VarStore()
        {
            _vars = new Dictionary<string, double>();
        }

        public double? GetVar(string name)
        {
            if (!IsValidName(name)) return null;
            double v;
            if (!_vars.TryGetValue(name, out v)) return null;
            return v;
        }

        public bool SetVar(string name, double value)
        {
            if (!IsValidName(name)) return false;
            _vars[name] = value;
            return true;
        }

        private static bool IsValidName(string name)
        {
            return !String.IsNullOrEmpty(name) && name.All(c => Char.IsLetterOrDigit(c) || c == '_');
        }
    }
}