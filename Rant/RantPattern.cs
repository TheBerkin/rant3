using System.IO;

using Rant.Engine.Compiler;
using Rant.Engine.Syntax;

namespace Rant
{
    /// <summary>
    /// Represents a compiled pattern that can be executed by the engine. It is recommended to use this class when running the same pattern multiple times.
    /// </summary>
    public sealed class RantPattern
    {
        /// <summary>
        /// Gets or sets the name of the source code.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
		/// Describes the origin of the pattern.
		/// </summary>
		public RantPatternSource Type { get; }

        /// <summary>
		/// The code contained in the pattern.
		/// </summary>
		public string Code { get; }

        internal RantAction Action { get; }

        internal RantPattern(string name, RantPatternSource type, string code)
        {
            Name = name;
            Type = type;
            Code = code;
	        Action = RantCompiler.Compile(name, code);
        }

        /// <summary>
        /// Compiles a pattern from the specified string.
        /// </summary>
        /// <param name="code">The code to compile.</param>
        /// <exception cref="Rant.RantCompilerException">Thrown if a syntax error is encountered.</exception>
        /// <returns></returns>
        public static RantPattern FromString(string code) => new RantPattern("Pattern", RantPatternSource.String, code);

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