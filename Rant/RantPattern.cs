using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Rant.Core.Compiler;
using Rant.Core.Compiler.Syntax;
using Rant.Core.Utilities;
using Rant.Resources;

using static Rant.Localization.Txtres;

namespace Rant
{
	/// <summary>
	/// Represents a compiled pattern that can be executed by the engine. It is recommended to use this class when running the
	/// same pattern multiple times.
	/// </summary>
	public sealed class RantPattern
	{
		private static readonly HashSet<char> _invalidNameChars =
			new HashSet<char>(new[] { '$', '@', ':', '~', '%', '?', '>', '<', '[', ']', '|', '{', '}', '?' });

		private string _name;

		internal RantPattern(string name, RantPatternOrigin type, string code)
		{
			Name = name;
			Type = type;
			Code = code;
			var compiler = new RantCompiler(name, code);
			SyntaxTree = compiler.Compile();
			Module = compiler.HasModule ? compiler.Module : null;
		}

		/// <summary>
		/// Gets or sets the name of the source code.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set
			{
				if (!IsValidPatternName(value))
					throw new ArgumentException($"Invalid pattern name: '{value ?? "<null>"}'");
				_name = string.Join("/",
					value.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray());
			}
		}

		/// <summary>
		/// Describes the origin of the pattern.
		/// </summary>
		public RantPatternOrigin Type { get; }

		/// <summary>
		/// The code contained in the pattern.
		/// </summary>
		public string Code { get; }

		internal RST SyntaxTree { get; }
		internal RantModule Module { get; }

		/// <summary>
		/// Compiles a pattern from the specified string.
		/// </summary>
		/// <param name="code">The code to compile.</param>
		/// <exception cref="Rant.RantCompilerException">Thrown if a syntax error is encountered.</exception>
		/// <returns></returns>
		public static RantPattern CompileFromString(string code) => new RantPattern(GetString("pattern"), RantPatternOrigin.String, code);

		/// <summary>
		/// Compiles a pattern from a string with the specified name.
		/// </summary>
		/// <param name="name">The name to give the source.</param>
		/// <param name="code">The code to compile.</param>
		/// <exception cref="Rant.RantCompilerException">Thrown if a syntax error is encountered.</exception>
		/// <returns></returns>
		public static RantPattern CompileFromString(string name, string code)
			=> new RantPattern(name, RantPatternOrigin.String, code);

		/// <summary>
		/// Loads the file located at the specified path and compiles a pattern from its contents.
		/// </summary>
		/// <param name="path">The path to the file to load.</param>
		/// <exception cref="Rant.RantCompilerException">Thrown if a syntax error is encountered.</exception>
		/// <exception cref="System.IO.FileNotFoundException">Thrown if the file cannot be found.</exception>
		/// <returns></returns>
		public static RantPattern CompileFromFile(string path)
			=> new RantPattern(Path.GetFileName(path), RantPatternOrigin.File, File.ReadAllText(path));

		public void Save(string path)
		{
			
		}

		/// <summary>
		/// Returns a string describing the pattern.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{Name} [{Type}]";

		private static bool IsValidPatternName(string name) => !Util.IsNullOrWhiteSpace(name) && name.All(c => !_invalidNameChars.Contains(c));
	}
}