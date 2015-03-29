using System.Collections.Generic;

using Rant.Engine.Compiler;
using Rant.Stringes;

namespace Rant.Engine.Constructs
{
    internal class Subroutine
    {
        private readonly _<string, ParamFlags>[] _parameters;
        private readonly int _argCount;
        private readonly RantPattern _source;

        private Subroutine(RantPattern source, _<string, ParamFlags>[] parameters)
        {
            _source = source;
            _parameters = parameters;
            _argCount = _parameters.Length;
        }

        public RantPattern Source
        {
            get { return _source; }
        }

        public _<string, ParamFlags>[] Parameters
        {
            get { return _parameters; }
        }

        public int ParamCount
        {
            get { return _argCount; }
        }

        public static Subroutine FromTokens(string name, RantPattern derivedSource, IEnumerable<Token<R>> tokens, _<string, ParamFlags>[] parameters)
        {
            return new Subroutine(RantPattern.Derived(name, derivedSource, tokens), parameters);
        }

        public static Subroutine FromString(string name, string code, _<string, ParamFlags>[] parameters)
        {
            return new Subroutine(RantPattern.FromString(name, code), parameters);
        }
    }
}