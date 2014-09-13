using System;
using System.Collections.Generic;

using Processus.Compiler;

using Stringes.Tokens;

namespace Processus
{
    internal class Subroutine
    {
        private readonly Tuple<string, TagArgType>[] _parameters;
        private readonly int _argCount;
        private readonly Source _source;

        private Subroutine(Source source, Tuple<string, TagArgType>[] parameters)
        {
            _source = source;
            _parameters = parameters;
            _argCount = _parameters.Length;
        }

        public Source Source
        {
            get { return _source; }
        }

        public Tuple<string, TagArgType>[] Parameters
        {
            get { return _parameters; }
        }

        public int ParamCount
        {
            get { return _argCount; }
        }

        public static Subroutine FromTokens(string name, Source derivedSource, IEnumerable<Token<TokenType>> tokens, Tuple<string, TagArgType>[] parameters)
        {
            return new Subroutine(Source.Derived(name, derivedSource, tokens), parameters);
        }

        public static Subroutine FromString(string name, string code, Tuple<string, TagArgType>[] parameters)
        {
            return new Subroutine(Source.FromString(name, code), parameters);
        }
    }
}