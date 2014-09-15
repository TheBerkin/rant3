using System;
using System.Collections.Generic;

using Rant.Compiler;

using Stringes.Tokens;

namespace Rant
{
    internal class Subroutine
    {
        private readonly Tuple<string, ParamFlags>[] _parameters;
        private readonly int _argCount;
        private readonly Source _source;

        private Subroutine(Source source, Tuple<string, ParamFlags>[] parameters)
        {
            _source = source;
            _parameters = parameters;
            _argCount = _parameters.Length;
        }

        public Source Source
        {
            get { return _source; }
        }

        public Tuple<string, ParamFlags>[] Parameters
        {
            get { return _parameters; }
        }

        public int ParamCount
        {
            get { return _argCount; }
        }

        public static Subroutine FromTokens(string name, Source derivedSource, IEnumerable<Token<TokenType>> tokens, Tuple<string, ParamFlags>[] parameters)
        {
            return new Subroutine(Source.Derived(name, derivedSource, tokens), parameters);
        }

        public static Subroutine FromString(string name, string code, Tuple<string, ParamFlags>[] parameters)
        {
            return new Subroutine(Source.FromString(name, code), parameters);
        }
    }
}