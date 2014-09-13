using System;

using Processus.Compiler;

namespace Processus
{
    /// <summary>
    /// The central class of the Processus engine that allows the execution of patterns.
    /// </summary>
    public sealed partial class Engine
    {
        /// <summary>
        /// The default NSFW filtering option to apply when creating Engine objects that load vocabulary from a directory.
        /// </summary>
        public static NsfwFilter DefaultNsfwFilter = NsfwFilter.Disallow;

        /// <summary>
        /// The maximum stack size allowed for a pattern.
        /// </summary>
        public static int MaxStackSize = 64;

        private static readonly RNG Seeds = new RNG();

        private readonly VarStore _vars;
        private readonly SubStore _subs;
        private readonly HookCollection _hooks;

        internal VarStore Variables
        {
            get { return _vars; }
        }

        internal SubStore Subroutines
        {
            get { return _subs; }
        }

        /// <summary>
        /// The hook collection associated with the engine.
        /// </summary>
        public HookCollection Hooks
        {
            get { return _hooks; }
        }

        /// <summary>
        /// Creates a new Engine object with no vocabulary.
        /// </summary>
        public Engine()
        {
            LoadVocab("", DefaultNsfwFilter);
            _vars = new VarStore();
            _subs = new SubStore();
        }

        /// <summary>
        /// Creates a new Engine object that loads vocabulary from the specified path.
        /// </summary>
        /// <param name="vocabularyPath">The path to the dictionary files to load.</param>
        public Engine(string vocabularyPath)
        {
            LoadVocab(vocabularyPath, DefaultNsfwFilter);
            _vars = new VarStore();
            _subs = new SubStore();
        }

        /// <summary>
        /// Creates a new Engine object that loads vocabulary from a path according to the specified filtering option.
        /// </summary>
        /// <param name="vocabularyPath">The path to the dictionary files to load.</param>
        /// <param name="filter">The filtering option to apply when loading the files.</param>
        public Engine(string vocabularyPath, NsfwFilter filter)
        {
            LoadVocab(vocabularyPath, filter);
            _vars = new VarStore();
            _subs = new SubStore();
        }

        /// <summary>
        /// Creates a new Engine object with the specified vocabulary.
        /// </summary>
        /// <param name="vocabulary">The vocabulary to load in this instance.</param>
        public Engine(IVocabulary vocabulary)
        {
            if (vocabulary == null) throw new ArgumentNullException("vocabulary");
            _vocabulary = vocabulary;
            _vars = new VarStore();
            _subs = new SubStore();
        }

        /// <summary>
        /// Executes the specified string and returns the resulting output.
        /// </summary>
        /// <param name="input">The input string to execute.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <returns></returns>
        public Output Do(string input, int charLimit = 0)
        {
            return new Interpreter(this, Source.FromString(input), new RNG(Seeds.NextRaw()), charLimit).Run();
        }

        /// <summary>
        /// Loads the file located at the specified path and executes it, returning the resulting output.
        /// </summary>
        /// <param name="path">The path to the file to execute.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <returns></returns>
        public Output DoFile(string path, int charLimit = 0)
        {
            return new Interpreter(this, Source.FromFile(path), new RNG(Seeds.NextRaw()), charLimit).Run();
        }

        /// <summary>
        /// Executes the specified string using a custom seed and returns the resulting output.
        /// </summary>
        /// <param name="input">The input string to execute.</param>
        /// <param name="seed">The seed to generate output with.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <returns></returns>
        public Output Do(string input, long seed, int charLimit = 0)
        {
            return new Interpreter(this, Source.FromString(input), new RNG(seed), charLimit).Run();
        }

        /// <summary>
        /// Loads the file located at the specified path and executes it using a custom seed, returning the resulting output.
        /// </summary>
        /// <param name="path">The path to the file to execute.</param>
        /// <param name="seed">The seed to generate output with.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <returns></returns>
        public Output DoFile(string path, long seed, int charLimit = 0)
        {
            return new Interpreter(this, Source.FromFile(path), new RNG(seed), charLimit).Run();
        }

        /// <summary>
        /// Executes the specified string using a custom random number generator and returns the resulting output.
        /// </summary>
        /// <param name="input">The input string to execute.</param>
        /// <param name="rng">The random number generator to use when generating output.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <returns></returns>
        public Output Do(string input, RNG rng, int charLimit = 0)
        {
            return new Interpreter(this, Source.FromString(input), rng, charLimit).Run();
        }

        /// <summary>
        /// Loads the file located at the specified path and executes it using a custom seed, returning the resulting output.
        /// </summary>
        /// <param name="path">The path to the file to execute.</param>
        /// <param name="rng">The random number generator to use when generating output.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <returns></returns>
        public Output DoFile(string path, RNG rng, int charLimit = 0)
        {
            return new Interpreter(this, Source.FromFile(path), rng, charLimit).Run();
        }

        /// <summary>
        /// Executes the specified source and returns the resulting output.
        /// </summary>
        /// <param name="input">The source to execute.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <returns></returns>
        public Output Do(Source input, int charLimit = 0)
        {
            return new Interpreter(this, input, new RNG(Seeds.NextRaw()), charLimit).Run();
        }

        /// <summary>
        /// Executes the specified source using a custom seed and returns the resulting output.
        /// </summary>
        /// <param name="input">The source to execute.</param>
        /// <param name="seed">The seed to generate output with.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <returns></returns>
        public Output Do(Source input, long seed, int charLimit = 0)
        {
            return new Interpreter(this, input, new RNG(seed), charLimit).Run();
        }

        /// <summary>
        /// Executes the specified source using a custom random number generator and returns the resulting output.
        /// </summary>
        /// <param name="input">The source to execute.</param>
        /// <param name="rng">The random number generator to use when generating output.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <returns></returns>
        public Output Do(Source input, RNG rng, int charLimit = 0)
        {
            return new Interpreter(this, input, rng, charLimit).Run();
        }
    }
}