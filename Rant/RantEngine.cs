using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Rant.Engine;
using Rant.Engine.ObjectModel;
using Rant.Engine.Syntax;
using Rant.Formats;
using Rant.Vocabulary;

namespace Rant
{
    /// <summary>
    /// The central class of the Rant engine that allows the execution of patterns.
    /// </summary>
    public sealed class RantEngine
    {
	    static RantEngine()
	    {
		    RantFunctions.Load();
	    }

		private static readonly RNG Seeds = new RNG();

		/// <summary>
		/// The maximum stack size allowed for a pattern.
		/// </summary>
		public static int MaxStackSize = 64;

        /// <summary>
        /// The default NSFW filtering option to apply when creating Engine objects that load vocabulary from a directory.
        /// </summary>
        public static NsfwFilter DefaultNsfwFilter = NsfwFilter.Disallow;
        
        internal readonly ObjectTable Objects = new ObjectTable();

        private readonly NsfwFilter _filter = DefaultNsfwFilter;
        private readonly Dictionary<string, RantPattern> _patternCache = new Dictionary<string, RantPattern>();
		private IRantDictionary _dictionary;

        /// <summary>
        /// Returns a pattern with the specified name from the engine's cache. If the pattern doesn't exist, it is loaded from file.
        /// </summary>
        /// <param name="name">The name or path of the pattern to retrieve.</param>
        /// <returns></returns>
        internal RantPattern GetPattern(string name)
        {
            RantPattern pattern;
            if (_patternCache.TryGetValue(name, out pattern)) return pattern;
            return _patternCache[name] = RantPattern.FromFile(name);
        }

		/// <summary>
		/// Accesses global variables.
		/// </summary>
		/// <param name="name">The name of the variable to access.</param>
		/// <returns></returns>
		public RantObject this[string name]
        {
            get { return Objects[name]; }
            set { Objects[name] = value; }
        }

        /// <summary>
        /// The currently set flags.
        /// </summary>
        public readonly HashSet<string> Flags = new HashSet<string>();

        /// <summary>
        /// The current formatting settings for the engine.
        /// </summary>
        public RantFormat Format = RantFormat.English;

        /// <summary>
        /// The vocabulary associated with this instance.
        /// </summary>
        public IRantDictionary Dictionary
        {
            get { return _dictionary; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                _dictionary = value;
            }
        }

        /// <summary>
        /// Creates a new RantEngine object with no vocabulary.
        /// </summary>
        public RantEngine()
        {
        }

        /// <summary>
        /// Creates a new RantEngine object that loads vocabulary from the specified path.
        /// </summary>
        /// <param name="vocabularyPath">The path to the dictionary files to load.</param>
        public RantEngine(string vocabularyPath)
        {
            LoadVocab(vocabularyPath);
        }

        /// <summary>
        /// Creates a new RantEngine object that loads vocabulary from a path according to the specified filtering option.
        /// </summary>
        /// <param name="vocabularyPath">The path to the dictionary files to load.</param>
        /// <param name="filter">The filtering option to apply when loading the files.</param>
        public RantEngine(string vocabularyPath, NsfwFilter filter)
        {
            _filter = filter;
            LoadVocab(vocabularyPath);
        }

        /// <summary>
        /// Creates a new RantEngine object with the specified vocabulary.
        /// </summary>
        /// <param name="dictionary">The vocabulary to load in this instance.</param>
        public RantEngine(IRantDictionary dictionary)
        {
            _dictionary = dictionary;
        }

        private void LoadVocab(string path)
        {
            if (_dictionary != null) return;

            if (!String.IsNullOrEmpty(path))
            {
                _dictionary = RantDictionary.FromDirectory(path, _filter);
            }
        }

        /// <summary>
        /// Returns a boolean value indicating whether a pattern by the specified name has been loaded from a package.
        /// </summary>
        /// <param name="patternName">The name of the pattern to check.</param>
        /// <returns></returns>
        public bool PatternExists(string patternName)
        {
            if (_patternCache == null) return false;
            return _patternCache.ContainsKey(patternName);
        }

        /// <summary>
        /// Loads the specified package into the engine.
        /// </summary>
        /// <param name="package">The package to load.</param>
        /// <param name="mergeBehavior">The table merging strategy to employ.</param>
        public void LoadPackage(RantPackage package, TableMergeBehavior mergeBehavior = TableMergeBehavior.Naive)
        {
            if (package == null) throw new ArgumentNullException(nameof(package));

            var patterns = package.GetPatterns();
            var tables = package.GetTables();

            if (patterns.Any())
            {
                foreach (var pattern in patterns)
                    _patternCache[pattern.Name] = pattern;
            }

            if (tables.Any())
            {
                if (_dictionary == null)
                {
                    _dictionary = new RantDictionary(tables, mergeBehavior);
                }
                else
                {
                    foreach (var table in tables)
                    {
                        _dictionary.AddTable(table, mergeBehavior);
                    }
                }
            }
        }

        /// <summary>
        /// Loads the package at the specified file path into the engine.
        /// </summary>
        /// <param name="path">The path to the package to load.</param>
        /// <param name="mergeBehavior">The table merging strategy to employ.</param>
        public void LoadPackage(string path, TableMergeBehavior mergeBehavior = TableMergeBehavior.Naive)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentException("Path cannot be null nor empty.");

            if (String.IsNullOrEmpty(Path.GetExtension(path)))
            {
                path += ".rantpkg";
            }

            var package = RantPackage.Load(path);

            var patterns = package.GetPatterns();
            var tables = package.GetTables();

            if (patterns.Any())
            {
                foreach (var pattern in patterns)
                    _patternCache[pattern.Name] = pattern;
            }

            if (tables.Any())
            {
                if (_dictionary == null)
                {
                    _dictionary = new RantDictionary(tables, mergeBehavior);
                }
                else
                {
                    foreach (var table in tables)
                    {
                        _dictionary.AddTable(table, mergeBehavior);
                    }
                }
            }
        }

        private RantOutput RunVM(Sandbox vm, double timeout)
        {
#if EDITOR
            EventHandler<LineEventArgs> lineEvent = (sender, e) =>
            {
                ActiveLineChanged?.Invoke(this, e);
            };
            vm.ActiveLineChanged += lineEvent;

            try
            {
                return vm.Run(timeout);
            }
            finally
            {
                vm.ActiveLineChanged -= lineEvent;
            }
#else
            return vm.Run(timeout);
#endif
        }

        #region Do, DoFile
        /// <summary>
        /// Compiles the specified string into a pattern, executes it, and returns the resulting output.
        /// </summary>
        /// <param name="input">The input string to execute.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public RantOutput Do(string input, int charLimit = 0, double timeout = -1)
        {
            return RunVM(new Sandbox(this, RantPattern.FromString(input), new RNG(Seeds.NextRaw()), charLimit), timeout);
        }

        /// <summary>
        /// Loads the file located at the specified path and executes it, returning the resulting output.
        /// </summary>
        /// <param name="path">The path to the file to execute.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public RantOutput DoFile(string path, int charLimit = 0, double timeout = -1)
        {
            return RunVM(new Sandbox(this, RantPattern.FromFile(path), new RNG(Seeds.NextRaw()), charLimit), timeout);
        }

        /// <summary>
        /// Compiles the specified string into a pattern, executes it using a custom seed, and returns the resulting output.
        /// </summary>
        /// <param name="input">The input string to execute.</param>
        /// <param name="seed">The seed to generate output with.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public RantOutput Do(string input, long seed, int charLimit = 0, double timeout = -1)
        {
            return RunVM(new Sandbox(this, RantPattern.FromString(input), new RNG(seed), charLimit), timeout);
        }

        /// <summary>
        /// Loads the file located at the specified path and executes it using a custom seed, returning the resulting output.
        /// </summary>
        /// <param name="path">The path to the file to execute.</param>
        /// <param name="seed">The seed to generate output with.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public RantOutput DoFile(string path, long seed, int charLimit = 0, double timeout = -1)
        {
            return RunVM(new Sandbox(this, RantPattern.FromFile(path), new RNG(seed), charLimit), timeout);
        }

        /// <summary>
        /// Compiles the specified string into a pattern, executes it using a custom RNG, and returns the resulting output.
        /// </summary>
        /// <param name="input">The input string to execute.</param>
        /// <param name="rng">The random number generator to use when generating output.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public RantOutput Do(string input, RNG rng, int charLimit = 0, double timeout = -1)
        {
            return RunVM(new Sandbox(this, RantPattern.FromString(input), rng, charLimit), timeout);
        }

        /// <summary>
        /// Loads the file located at the specified path and executes it using a custom seed, returning the resulting output.
        /// </summary>
        /// <param name="path">The path to the file to execute.</param>
        /// <param name="rng">The random number generator to use when generating output.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public RantOutput DoFile(string path, RNG rng, int charLimit = 0, double timeout = -1)
        {
            return RunVM(new Sandbox(this, RantPattern.FromFile(path), rng, charLimit), timeout);
        }

        /// <summary>
        /// Executes the specified pattern and returns the resulting output.
        /// </summary>
        /// <param name="input">The pattern to execute.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public RantOutput Do(RantPattern input, int charLimit = 0, double timeout = -1)
        {
            return RunVM(new Sandbox(this, input, new RNG(Seeds.NextRaw()), charLimit), timeout);
        }

        /// <summary>
        /// Executes the specified pattern using a custom seed and returns the resulting output.
        /// </summary>
        /// <param name="input">The pattern to execute.</param>
        /// <param name="seed">The seed to generate output with.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public RantOutput Do(RantPattern input, long seed, int charLimit = 0, double timeout = -1)
        {
            return RunVM(new Sandbox(this, input, new RNG(seed), charLimit), timeout);
        }

        /// <summary>
        /// Executes the specified pattern using a custom random number generator and returns the resulting output.
        /// </summary>
        /// <param name="input">The pattern to execute.</param>
        /// <param name="rng">The random number generator to use when generating output.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public RantOutput Do(RantPattern input, RNG rng, int charLimit = 0, double timeout = -1)
        {
            return RunVM(new Sandbox(this, input, rng, charLimit), timeout);
        }

        /// <summary>
        /// Executes a pattern that has been loaded from a package and returns the resulting output.
        /// </summary>
        /// <param name="patternName">The name of the pattern to execute.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public RantOutput DoPackaged(string patternName, int charLimit = 0, double timeout = -1)
        {
            if (!PatternExists(patternName))
                throw new ArgumentException("Pattern doesn't exist.");

            return RunVM(new Sandbox(this, _patternCache[patternName], new RNG(Seeds.NextRaw()), charLimit), timeout);
        }

        /// <summary>
        /// Executes a pattern that has been loaded from a package and returns the resulting output.
        /// </summary>
        /// <param name="patternName">The name of the pattern to execute.</param>
        /// <param name="seed">The seed to generate output with.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public RantOutput DoPackaged(string patternName, long seed, int charLimit = 0, double timeout = -1)
        {
            if (!PatternExists(patternName))
                throw new ArgumentException("Pattern doesn't exist.");

            return RunVM(new Sandbox(this, _patternCache[patternName], new RNG(seed), charLimit), timeout);
        }

        /// <summary>
        /// Executes a pattern that has been loaded from a package using a custom random number generator and returns the resulting output.
        /// </summary>
        /// <param name="patternName">The name of the pattern to execute.</param>
        /// <param name="rng">The random number generator to use when generating output.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public RantOutput DoPackaged(string patternName, RNG rng, int charLimit = 0, double timeout = -1)
        {
            if (!PatternExists(patternName))
                throw new ArgumentException("Pattern doesn't exist.");

            return RunVM(new Sandbox(this, _patternCache[patternName], rng, charLimit), timeout);
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