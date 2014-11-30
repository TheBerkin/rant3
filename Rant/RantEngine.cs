using System;
using System.Collections.Generic;
using Rant.Vocabulary;

namespace Rant
{
    /// <summary>
    /// The central class of the Rant engine that allows the execution of patterns.
    /// </summary>
    public sealed class RantEngine
    {
        /// <summary>
        /// The default NSFW filtering option to apply when creating Engine objects that load vocabulary from a directory.
        /// </summary>
        public static NsfwFilter DefaultNsfwFilter = NsfwFilter.Disallow;

        private static readonly RNG Seeds = new RNG();

        /// <summary>
        /// The maximum stack size allowed for a pattern.
        /// </summary>
        public static int MaxStackSize = 64;

        internal readonly Dictionary<string, Query> GlobalQueryMacros = new Dictionary<string, Query>(); 
        internal readonly Dictionary<string, List<string>> GlobalLists = new Dictionary<string, List<string>>(); 

        private readonly VarStore _vars = new VarStore();
        private readonly SubStore _subs = new SubStore();
        private readonly HashSet<string> _flags = new HashSet<string>();
        private IRantVocabulary _vocabulary;   

        internal VarStore Variables => _vars;

        internal SubStore Subroutines => _subs;

        /// <summary>
        /// The currently set flags.
        /// </summary>
        public HashSet<string> Flags => _flags;

        /// <summary>
        /// The current output formatting style for the engine.
        /// </summary>
        public RantFormatStyle FormatStyle { get; set; } = RantFormatStyle.English;

        /// <summary>
        /// The vocabulary associated with this instance.
        /// </summary>
        public IRantVocabulary Vocabulary
        {
            get { return _vocabulary; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                _vocabulary = value;
            }
        }

        /// <summary>
        /// Creates a new Engine object with no vocabulary.
        /// </summary>
        public RantEngine()
        {
            LoadVocab("", DefaultNsfwFilter);
        }

        /// <summary>
        /// Creates a new Engine object that loads vocabulary from the specified path.
        /// </summary>
        /// <param name="vocabularyPath">The path to the dictionary files to load.</param>
        public RantEngine(string vocabularyPath)
        {
            LoadVocab(vocabularyPath, DefaultNsfwFilter);
        }

        /// <summary>
        /// Creates a new Engine object that loads vocabulary from a path according to the specified filtering option.
        /// </summary>
        /// <param name="vocabularyPath">The path to the dictionary files to load.</param>
        /// <param name="filter">The filtering option to apply when loading the files.</param>
        public RantEngine(string vocabularyPath, NsfwFilter filter)
        {
            LoadVocab(vocabularyPath, filter);
        }

        /// <summary>
        /// Creates a new Engine object with the specified vocabulary.
        /// </summary>
        /// <param name="vocabulary">The vocabulary to load in this instance.</param>
        public RantEngine(IRantVocabulary vocabulary)
        {
            if (vocabulary == null) throw new ArgumentNullException("vocabulary");
            _vocabulary = vocabulary;
        }

        private void LoadVocab(string path, NsfwFilter filter)
        {
            if (_vocabulary != null) return;

            if (!String.IsNullOrEmpty(path))
            {
                _vocabulary = RantVocabulary.FromDirectory(path, filter);
            }
        }

        #region Do, DoFile
        /// <summary>
        /// Compiles the specified string into a pattern, executes it, and returns the resulting output.
        /// </summary>
        /// <param name="input">The input string to execute.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <returns></returns>
        public Output Do(string input, int charLimit = 0)
        {
            return new Interpreter(this, RantPattern.FromString(input), new RNG(Seeds.NextRaw()), charLimit).Run();
        }

        /// <summary>
        /// Loads the file located at the specified path and executes it, returning the resulting output.
        /// </summary>
        /// <param name="path">The path to the file to execute.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <returns></returns>
        public Output DoFile(string path, int charLimit = 0)
        {
            return new Interpreter(this, RantPattern.FromFile(path), new RNG(Seeds.NextRaw()), charLimit).Run();
        }

        /// <summary>
        /// Compiles the specified string into a pattern, executes it using a custom seed, and returns the resulting output.
        /// </summary>
        /// <param name="input">The input string to execute.</param>
        /// <param name="seed">The seed to generate output with.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <returns></returns>
        public Output Do(string input, long seed, int charLimit = 0)
        {
            return new Interpreter(this, RantPattern.FromString(input), new RNG(seed), charLimit).Run();
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
            return new Interpreter(this, RantPattern.FromFile(path), new RNG(seed), charLimit).Run();
        }

        /// <summary>
        /// Compiles the specified string into a pattern, executes it using a custom RNG, and returns the resulting output.
        /// </summary>
        /// <param name="input">The input string to execute.</param>
        /// <param name="rng">The random number generator to use when generating output.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <returns></returns>
        public Output Do(string input, RNG rng, int charLimit = 0)
        {
            return new Interpreter(this, RantPattern.FromString(input), rng, charLimit).Run();
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
            return new Interpreter(this, RantPattern.FromFile(path), rng, charLimit).Run();
        }

        /// <summary>
        /// Executes the specified pattern and returns the resulting output.
        /// </summary>
        /// <param name="input">The pattern to execute.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <returns></returns>
        public Output Do(RantPattern input, int charLimit = 0)
        {
            return new Interpreter(this, input, new RNG(Seeds.NextRaw()), charLimit).Run();
        }

        /// <summary>
        /// Executes the specified pattern using a custom seed and returns the resulting output.
        /// </summary>
        /// <param name="input">The pattern to execute.</param>
        /// <param name="seed">The seed to generate output with.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <returns></returns>
        public Output Do(RantPattern input, long seed, int charLimit = 0)
        {
            return new Interpreter(this, input, new RNG(seed), charLimit).Run();
        }

        /// <summary>
        /// Executes the specified pattern using a custom random number generator and returns the resulting output.
        /// </summary>
        /// <param name="input">The pattern to execute.</param>
        /// <param name="rng">The random number generator to use when generating output.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <returns></returns>
        public Output Do(RantPattern input, RNG rng, int charLimit = 0)
        {
            return new Interpreter(this, input, rng, charLimit).Run();
        }
        #endregion

        #region Hooks
        private readonly Dictionary<string, Func<string[], string>> _hooks = new Dictionary<string, Func<string[], string>>();
        private readonly HashSet<Func<string[], string>> _hookFuncs = new HashSet<Func<string[], string>>();

        internal string CallHook(string name, string[] args)
        {
            Func<string[], string> func;
            return !_hooks.TryGetValue(name, out func) ? null : func(args);
        }

        /// <summary>
        /// Adds a function to the collection with the specified name. The hook name can only contain letters, decimal digits, and underscores.
        /// </summary>
        /// <param name="name">The name of the function hook.</param>
        /// <param name="func">The function associated with the hook.</param>
        public void AddHook(string name, Func<string[], string> func)
        {
            if (!Util.ValidateName(name)) throw new FormatException("Hook name can only contain letters, decimal digits, and underscores.");
            _hooks[name] = func;
            _hookFuncs.Add(func);
        }

        /// <summary>
        /// Determines whether the HookCollection object contains a hook with the specified name.
        /// </summary>
        /// <param name="name">The name of the hook to search for.</param>
        /// <returns></returns>
        public bool HasHook(string name) => _hooks.ContainsKey(name);

        /// <summary>
        /// Determines whether the HookCollection object contains a hook with the specified function.
        /// </summary>
        /// <param name="func">The function to search for.</param>
        /// <returns></returns>
        public bool HasHook(Func<string[], string> func) => _hookFuncs.Contains(func);

        /// <summary>
        /// Removes the hook with the specified name from the collection.
        /// </summary>
        /// <param name="name">The name of the hook to remove.</param>
        public void RemoveHook(string name)
        {
            Func<string[], string> func;
            if (_hooks.TryGetValue(name, out func))
            {
                _hookFuncs.Remove(func);
            }
            _hooks.Remove(name);
        }
        #endregion
    }
}