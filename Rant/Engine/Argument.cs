using System;
using System.Collections.Generic;

using Rant.Engine.Compiler;
using Rant.Stringes.Tokens;

namespace Rant.Engine
{
    internal class Argument
    {
        private readonly string _str;
        private readonly IEnumerable<Token<R>> _tokens;
        private readonly ParamFlags _flags;

        public ParamFlags Flags => _flags;

        public string AsString()
        {
            if (_str == null) throw new InvalidOperationException("Tried to use a 'pattern' argument as a 'string' argument.");
            return _str;
        }

        public IEnumerable<Token<R>> AsPattern()
        {
            if(_tokens == null) throw new InvalidOperationException("Tried to use a 'string' argument as a 'pattern' argument.");
            return _tokens;
        }

        private Argument(string str)
        {
            _tokens = null;
            _str = str;
            _flags = ParamFlags.None;
        }

        private Argument(IEnumerable<Token<R>> tokens)
        {
            _tokens = tokens;
            _str = null;
            _flags = ParamFlags.Code;
        }

        public static Argument FromString(string value)
        {
            return new Argument(value);
        }

        public static Argument FromTokens(IEnumerable<Token<R>> tokens)
        {
            return new Argument(tokens);
        }

        public static implicit operator string(Argument arg)
        {
            return arg.AsString();
        }
    }
}