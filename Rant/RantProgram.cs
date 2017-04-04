#region License

// https://github.com/TheBerkin/Rant
// 
// Copyright (c) 2017 Nicholas Fleck
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Rant.Core.Compiler;
using Rant.Core.Compiler.Syntax;
using Rant.Core.IO;
using Rant.Core.IO.Bson;
using Rant.Core.Utilities;
using Rant.Resources;

using static Rant.Localization.Txtres;

namespace Rant
{
    /// <summary>
    /// Represents a compiled pattern that can be executed by the engine. It is recommended to use this class when running the
    /// same pattern multiple times.
    /// </summary>
    public sealed class RantProgram : RantResource
    {
        private const string Magic = "RANT";
        private const string Extension = ".rantpgm";

        private static readonly HashSet<char> _invalidNameChars =
            new HashSet<char>(new[] { '$', '@', ':', '~', '%', '?', '>', '<', '[', ']', '|', '{', '}', '?' });

        private string _name;

        internal RantProgram(string name, RantProgramOrigin type, string code)
        {
            Name = name;
            Type = type;
            Code = code;
            var compiler = new RantCompiler(name, code);
            SyntaxTree = compiler.Compile();
        }

        internal RantProgram(string name, RantProgramOrigin type, RST rst)
        {
            Name = name;
            Type = type;
            Code = null;
            SyntaxTree = rst;
        }

        internal RantProgram()
        {
            // Used by serializer
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
                    throw new ArgumentException(GetString("err-bad-pattern-name", value ?? "<null>"));
                _name = string.Join("/",
                    value.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray());
            }
        }

        /// <summary>
        /// Describes the origin of the program.
        /// </summary>
        public RantProgramOrigin Type { get; }

        /// <summary>
        /// The pattern from which the program was compiled.
        /// </summary>
        public string Code { get; }

        internal RST SyntaxTree { get; private set; }

        /// <summary>
        /// Compiles a program from the specified pattern.
        /// </summary>
        /// <param name="code">The pattern to compile.</param>
        /// <exception cref="Rant.RantCompilerException">Thrown if a syntax error is encountered.</exception>
        /// <returns></returns>
        public static RantProgram CompileString(string code) => new RantProgram(GetString("pattern"), RantProgramOrigin.String, code);

        /// <summary>
        /// Compiles a program from a pattern with the specified name.
        /// </summary>
        /// <param name="name">The name to give the source.</param>
        /// <param name="code">The pattern to compile.</param>
        /// <exception cref="Rant.RantCompilerException">Thrown if a syntax error is encountered.</exception>
        /// <returns></returns>
        public static RantProgram CompileString(string name, string code)
            => new RantProgram(name, RantProgramOrigin.String, code);

        /// <summary>
        /// Loads the file located at the specified path and compiles a program from its contents.
        /// </summary>
        /// <param name="path">The path to the file to load.</param>
        /// <exception cref="Rant.RantCompilerException">Thrown if a syntax error is encountered.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the file cannot be found.</exception>
        /// <returns></returns>
        public static RantProgram CompileFile(string path)
            => new RantProgram(Path.GetFileName(path), RantProgramOrigin.File, File.ReadAllText(path));

        /// <summary>
        /// Saves the compiled program to the file at the specified path.
        /// </summary>
        /// <param name="path">The path to save the program to.</param>
        public void SaveToFile(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (!Path.HasExtension(path)) path += Extension;
            SaveToStream(File.Create(path));
        }

        /// <summary>
        /// Saves the compiled program to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to save the program to.</param>
        public void SaveToStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            using (var output = new EasyWriter(stream, Endian.Little, true))
            {
                output.WriteBytes(Encoding.ASCII.GetBytes(Magic));
                RST.SerializeRST(SyntaxTree, output);
            }
            stream.Flush();
        }

        /// <summary>
        /// Loads a compiled Rant program from the file at the specified path.
        /// </summary>
        /// <param name="path">The path to load the program from.</param>
        /// <returns></returns>
        public static RantProgram LoadFile(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return LoadStream(Path.GetFileNameWithoutExtension(path), File.Open(path, FileMode.Open));
        }

        /// <summary>
        /// Loads a compiled Rant program from the specified stream.
        /// </summary>
        /// <param name="programName">The name to give to the program.</param>
        /// <param name="stream">The stream to load the program from.</param>
        /// <returns></returns>
        public static RantProgram LoadStream(string programName, Stream stream)
        {
            if (programName == null) throw new ArgumentNullException(nameof(programName));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            using (var input = new EasyReader(stream))
            {
                if (Encoding.ASCII.GetString(input.ReadBytes(4)) != Magic)
                    throw new InvalidDataException(GetString("err-pgmload-bad-magic"));

                var rst = RST.DeserializeRST(input);

                // TODO: Use string table

                return new RantProgram(programName, RantProgramOrigin.File, rst);
            }
        }

        /// <summary>
        /// Returns a string describing the pattern.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{Name} [{Type}]";

        private static bool IsValidPatternName(string name) => !Util.IsNullOrWhiteSpace(name) && name.All(c => !_invalidNameChars.Contains(c));

        internal override void DeserializeData(BsonItem data)
        {
            Name = data["name"];
            using (var ms = new MemoryStream((byte[])data["code"].Value))
            using (var reader = new EasyReader(ms))
            {
                SyntaxTree = RST.DeserializeRST(reader);
            }
        }

        internal override BsonItem SerializeData()
        {
            var data = new BsonItem { ["name"] = Name };

            using (var ms = new MemoryStream())
            using (var writer = new EasyWriter(ms))
            {
                RST.SerializeRST(SyntaxTree, writer);
                ms.Flush();
                data["code"] = new BsonItem(ms.ToArray());
            }
            return data;
        }

        internal override void Load(RantEngine engine)
        {
            engine.CacheProgramInternal(this);
        }
    }
}