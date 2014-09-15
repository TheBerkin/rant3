using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Stringes.Tokens;

namespace Rant.Compiler
{
    /// <summary>
    /// Represents a compiled source that can be executed by the engine.
    /// </summary>
    public sealed class Source
    {
        private readonly string _code;
        private readonly IEnumerable<Token<TokenType>> _tokens;
        private readonly SourceType _type;
        private readonly string _name;

        /// <summary>
        /// The name of the source code.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Describes the origin of the source.
        /// </summary>
        public SourceType Type
        {
            get { return _type; }
        }

        /// <summary>
        /// The code contained in the source.
        /// </summary>
        public string Code
        {
            get { return _code; }
        }

        internal IEnumerable<Token<TokenType>> Tokens
        {
            get { return _tokens; }
        }

        internal Source(string name, SourceType type, string code)
        {
            _name = name;
            _type = type;
            _code = code;
            _tokens = Lexer.GenerateTokens(code);
        }

        internal Source(string name, SourceType type, IEnumerable<Token<TokenType>> tokens, string code)
        {
            _name = name;
            _type = type;
            _code = code;
            _tokens = tokens;
        }

        /// <summary>
        /// Compiles a Source object from the specified string.
        /// </summary>
        /// <param name="code">The code to compile.</param>
        /// <returns></returns>
        public static Source FromString(string code)
        {
            return new Source("Source", SourceType.String, code);
        }

        /// <summary>
        /// Compiles a Source object from a string with the specified name.
        /// </summary>
        /// <param name="name">The name to give the source.</param>
        /// <param name="code">The code to compile.</param>
        /// <returns></returns>
        public static Source FromString(string name, string code)
        {
            return new Source(name, SourceType.String, code);
        }

        internal static Source Derived(Source source, IEnumerable<Token<TokenType>> tokens)
        {
            return new Source(source._name, source._type, tokens, source._code);
        }

        internal static Source Derived(string name, Source source, IEnumerable<Token<TokenType>> tokens)
        {
            return new Source(name, source._type, tokens, source._code);
        }

        /// <summary>
        /// Loads the file located at the specified path and creates a Source object from its contents.
        /// </summary>
        /// <param name="path">The path to the file to load.</param>
        /// <returns></returns>
        public static Source FromFile(string path)
        {
            return new Source(Path.GetFileName(path), SourceType.File, File.ReadAllText(path));
        }

        /// <summary>
        /// Returns a string describing the source.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Concat("(", Type, ") ", Name);
        }
    }
}