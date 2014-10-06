using System;
using System.Collections.Generic;

using Rant.Compiler;
using Rant.Stringes.Tokens;

namespace Rant
{
    internal class Subroutine
    {
        private readonly Tuple<string, ParamFlags>[] _parameters;
        private readonly int _argCount;
        private readonly Pattern _source;

        private Subroutine(Pattern source, Tuple<string, ParamFlags>[] parameters)
        {
            _source = source;
            _parameters = parameters;
            _argCount = _parameters.Length;
        }

        public Pattern Source
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

        public static Subroutine FromTokens(string name, Pattern derivedSource, IEnumerable<Token<TokenType>> tokens, Tuple<string, ParamFlags>[] parameters)
        {
            return new Subroutine(Pattern.Derived(name, derivedSource, tokens), parameters);
        }

        public static Subroutine FromString(string name, string code, Tuple<string, ParamFlags>[] parameters)
        {
            return new Subroutine(Pattern.FromString(name, code), parameters);
        }
    }
}