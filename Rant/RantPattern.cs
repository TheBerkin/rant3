using System.Collections.Generic;
using System.IO;
using System.Linq;

using Rant.Engine.Compiler;
using Rant.Engine.Syntax;
using Rant.Engine.Constructs;
using Rant.Stringes;

namespace Rant
{
    /// <summary>
    /// Represents a compiled pattern that can be executed by the engine. It is recommended to use this class when running the same pattern multiple times.
    /// </summary>
    public sealed class RantPattern
    {
        private readonly string _code;
	    private readonly RantAction _action;
        private readonly RantPatternSource _type;
        private readonly string _name;

        /// <summary>
        /// The name of the source code.
        /// </summary>
        public string Name => _name;

		/// <summary>
		/// Describes the origin of the pattern.
		/// </summary>
		public RantPatternSource Type => _type;

		/// <summary>
		/// The code contained in the pattern.
		/// </summary>
		public string Code => _code;

	    internal RantAction Action => _action;

        internal RantPattern(string name, RantPatternSource type, string code)
        {
            _name = name;
            _type = type;
            _code = code;
	        _action = RantCompiler.Compile(name, code);
        }

        /// <summary>
        /// Compiles a pattern from the specified string.
        /// </summary>
        /// <param name="code">The code to compile.</param>
        /// <exception cref="Rant.RantCompilerException">Thrown if a syntax error is encountered.</exception>
        /// <returns></returns>
        public static RantPattern FromString(string code) => new RantPattern("Source", RantPatternSource.String, code);

        /// <summary>
        /// Compiles a pattern from a string with the specified name.
        /// </summary>
        /// <param name="name">The name to give the source.</param>
        /// <param name="code">The code to compile.</param>
        /// <exception cref="Rant.RantCompilerException">Thrown if a syntax error is encountered.</exception>
        /// <returns></returns>
        public static RantPattern FromString(string name, string code) => new RantPattern(name, RantPatternSource.String, code);

        /// <summary>
        /// Loads the file located at the specified path and compiles a pattern from its contents.
        /// </summary>
        /// <param name="path">The path to the file to load.</param>
        /// <exception cref="Rant.RantCompilerException">Thrown if a syntax error is encountered.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the file cannot be found.</exception>
        /// <returns></returns>
        public static RantPattern FromFile(string path) => new RantPattern(Path.GetFileName(path), RantPatternSource.File, File.ReadAllText(path));

		/// <summary>
		/// Returns a string describing the pattern.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{Name} [{Type}]";
    }
}