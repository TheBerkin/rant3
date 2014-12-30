using System;
using System.Collections.Generic;
using System.IO;

using Rant.Engine;
using Rant.Engine.Compiler;
using Rant.Engine.Stringes.Tokens;

namespace Rant
{
    /// <summary>
    /// Represents a compiled pattern that can be executed by the engine. It is recommended to use this class when running the same patern multiple times.
    /// </summary>
    public sealed class RantPattern
    {
        private readonly string _code;
        private readonly IEnumerable<Token<R>> _tokens;
        private readonly RantPatternSource _type;
        private readonly string _name;

        // This is used to cache item locations within blocks, which eliminates unnecessary multiple traversals of the blocks' tokens.
        // Item1 = Block items
        // Item2 = End position of block
        private readonly Dictionary<Token<R>, Tuple<Block, int>> _blockJumpTable = new Dictionary<Token<R>, Tuple<Block, int>>();

        /// <summary>
        /// The name of the source code.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Describes the origin of the source.
        /// </summary>
        public RantPatternSource Type => _type;

        /// <summary>
        /// The code contained in the source.
        /// </summary>
        public string Code => _code;

        internal void CacheBlock(Token<R> start, Tuple<Block, int> block) => _blockJumpTable[start] = block;

        internal bool TryGetCachedBlock(Token<R> start, out Tuple<Block, int> block) => _blockJumpTable.TryGetValue(start, out block);

        internal IEnumerable<Token<R>> Tokens => _tokens;

        internal RantPattern(string name, RantPatternSource type, string code)
        {
            _name = name;
            _type = type;
            _code = code;
            _tokens = RantLexer.GenerateTokens(code);
        }

        internal RantPattern(RantPattern derived, IEnumerable<Token<R>> sub)
        {
            _name = derived._name;
            _type = derived._type;
            _code = derived._code;
            _blockJumpTable = derived._blockJumpTable;
            _tokens = sub;
        }

        internal RantPattern(string name, RantPattern derived, IEnumerable<Token<R>> sub)
        {
            _name = name;
            _type = derived._type;
            _code = derived._code;
            _blockJumpTable = derived._blockJumpTable;
            _tokens = sub;
        }

        /// <summary>
        /// Compiles a Source object from the specified string.
        /// </summary>
        /// <param name="code">The code to compile.</param>
        /// <returns></returns>
        public static RantPattern FromString(string code) => new RantPattern("Source", RantPatternSource.String, code);

        /// <summary>
        /// Compiles a Source object from a string with the specified name.
        /// </summary>
        /// <param name="name">The name to give the source.</param>
        /// <param name="code">The code to compile.</param>
        /// <returns></returns>
        public static RantPattern FromString(string name, string code) => new RantPattern(name, RantPatternSource.String, code);

        internal static RantPattern Derived(RantPattern source, IEnumerable<Token<R>> tokens) => new RantPattern(source, tokens);

        // Used for applying a different name to subroutines
        internal static RantPattern Derived(string name, RantPattern source, IEnumerable<Token<R>> tokens) => new RantPattern(name, source, tokens);

        /// <summary>
        /// Loads the file located at the specified path and creates a Source object from its contents.
        /// </summary>
        /// <param name="path">The path to the file to load.</param>
        /// <returns></returns>
        public static RantPattern FromFile(string path) => new RantPattern(Path.GetFileName(path), RantPatternSource.File, File.ReadAllText(path));

        /// <summary>
        /// Returns a string describing the source.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => "(\{Type}) \{Name}";
    }
}