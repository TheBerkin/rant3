using System;
using System.Collections.Generic;

using Processus.Compiler;

using Stringes.Tokens;

namespace Processus
{
    internal class TagArg
    {
        private readonly string _str;
        private readonly IEnumerable<Token<TokenType>> _tokens;
        private readonly TagArgType _type;

        public TagArgType Type
        {
            get { return _type; }
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

        private TagArg(string str)
        {
            _tokens = null;
            _str = str;
            _type = TagArgType.Result;
        }

        private TagArg(IEnumerable<Token<TokenType>> tokens)
        {
            _tokens = tokens;
            _str = null;
            _type = TagArgType.Tokens;
        }

        public static TagArg FromString(string value)
        {
            return new TagArg(value);
        }

        public static TagArg FromTokens(IEnumerable<Token<TokenType>> tokens)
        {
            return new TagArg(tokens);
        }
    }

    internal enum TagArgType
    {
        /// <summary>
        /// Argument is a string generated from the provided tokens.
        /// </summary>
        Result,
        /// <summary>
        /// Argument is a series of tokens.
        /// </summary>
        Tokens
    }
}