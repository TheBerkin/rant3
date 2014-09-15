using System;
using System.Collections.Generic;

using Rant.Compiler;

using Stringes.Tokens;

namespace Rant
{
    internal class Argument
    {
        private readonly string _str;
        private readonly IEnumerable<Token<TokenType>> _tokens;
        private readonly ParamFlags _flags;

        public ParamFlags Flags
        {
            get { return _flags; }
        }

        public string GetString()
        {
            if (_str == null) throw new InvalidOperationException("Tried to use a 'tokens' argument as a 'string' argument.");
            return _str;
        }

        public IEnumerable<Token<TokenType>> GetTokens()
        {
            if(_tokens == null) throw new InvalidOperationException("Tried to use a 'string' argument as a 'tokens' argument.");
            return _tokens;
        }

        private Argument(string str)
        {
            _tokens = null;
            _str = str;
            _flags = ParamFlags.None;
        }

        private Argument(IEnumerable<Token<TokenType>> tokens)
        {
            _tokens = tokens;
            _str = null;
            _flags = ParamFlags.Code;
        }

        public static Argument FromString(string value)
        {
            return new Argument(value);
        }

        public static Argument FromTokens(IEnumerable<Token<TokenType>> tokens)
        {
            return new Argument(tokens);
        }

        public static implicit operator string(Argument arg)
        {
            return arg.GetString();
        }
    }
}