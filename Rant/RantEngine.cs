using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Rant.Engine;
using Rant.Engine.ObjectModel;
using Rant.Formats;
using Rant.Vocabulary;

namespace Rant
{
    /// <summary>
    /// The central class of the Rant engine that allows the execution of patterns.
    /// </summary>
    public sealed class RantEngine
    {
        private static int _maxStackSize = 64;
        private static readonly RNG Seeds = new RNG();

        static RantEngine()
	    {
		    RantFunctions.Load();
            RichardFunctions.Load();
	    }

        /// <summary>
        /// Gets or sets the maximum stack size allowed for a pattern.
        /// </summary>
        public static int MaxStackSize
        {
            get { return _maxStackSize; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value), "Value must be greater than zero.");
                _maxStackSize = value;
            }
        }
        
        internal readonly ObjectTable Objects = new ObjectTable();
        
        private readonly Dictionary<string, RantPattern> _patternCache = new Dictionary<string, RantPattern>();
	    private readonly HashSet<RantPackageDependency> _loadedPackages = new HashSet<RantPackageDependency>(); 
		private RantDictionary _dictionary = new RantDictionary();

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
        public RantDictionary Dictionary
        {
            get { return _dictionary; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _dictionary = value;
            }
        }

        /// <summary>
        /// Creates a new RantEngine object without a dictionary.
        /// </summary>
        public RantEngine()
        {
        }

        /// <summary>
        /// Creates a new RantEngine object that loads vocabulary from the specified path.
        /// </summary>
        /// <param name="dictionaryPath">The path to the dictionary files to load.</param>
        public RantEngine(string dictionaryPath)
        {
            if (!String.IsNullOrEmpty(dictionaryPath))
            {
                _dictionary = RantDictionary.FromDirectory(dictionaryPath);
            }
        }

        /// <summary>
        /// Creates a new RantEngine object with the specified vocabulary.
        /// </summary>
        /// <param name="dictionary">The vocabulary to load in this instance.</param>
        public RantEngine(RantDictionary dictionary)
        {
            _dictionary = dictionary;
        }

        /// <summary>
        /// Returns a boolean value indicating whether a pattern by the specified name has been loaded from a package.
        /// </summary>
        /// <param name="patternName">The name of the pattern to check.</param>
        /// <returns></returns>
        public bool PatternExists(string patternName)
        {
            return _patternCache != null && _patternCache.ContainsKey(patternName);
        }

        /// <summary>
        /// Loads the specified package into the engine.
        /// </summary>
        /// <param name="package">The package to load.</param>
        /// <param name="mergeBehavior">The table merging strategy to employ.</param>
        public void LoadPackage(RantPackage package, TableMergeBehavior mergeBehavior = TableMergeBehavior.Naive)
        {
            if (package == null) throw new ArgumentNullException(nameof(package));
	        if (_loadedPackages.Contains(RantPackageDependency.Create(package))) return;

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

	        _loadedPackages.Add(RantPackageDependency.Create(package));

	        foreach (var dependency in package.GetDependencies())
	        {
		        if (!File.Exists($"{dependency.ID}.rantpkg"))
					throw new FileNotFoundException($"Package '{package.ID} {package.Version}' cannot find dependency '{dependency}'.");
		        var pkg = RantPackage.Load($"{dependency.ID}.rantpkg");
				if (pkg.Version.Trim() != dependency.Version)
					throw new FileNotFoundException($"Package '{package.ID} {package.Version}' tried to load dependency '{dependency}' but the version did not match ({pkg.Version}).");
                LoadPackage(pkg, mergeBehavior);
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
			if (_loadedPackages.Contains(RantPackageDependency.Create(package))) return;

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

	        _loadedPackages.Add(RantPackageDependency.Create(package));

			foreach (var dependency in package.GetDependencies())
			{
				if (!File.Exists($"{dependency.ID}.rantpkg"))
					throw new FileNotFoundException($"Package '{package.ID} {package.Version}' cannot find dependency '{dependency}'.");
				var pkg = RantPackage.Load($"{dependency.ID}.rantpkg");
				if (pkg.Version.Trim() != dependency.Version)
					throw new FileNotFoundException($"Package '{package.ID} {package.Version}' tried to load dependency '{dependency}' but the version did not match ({pkg.Version}).");
				LoadPackage(pkg, mergeBehavior);
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
        public RantOutput Do(string input, int charLimit = 0, double timeout = -1) => 
            RunVM(new Sandbox(this, RantPattern.FromString(input), new RNG(Seeds.NextRaw()), charLimit), timeout);

        /// <summary>
        /// Loads the file located at the specified path and executes it, returning the resulting output.
        /// </summary>
        /// <param name="path">The path to the file to execute.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public RantOutput DoFile(string path, int charLimit = 0, double timeout = -1) => 
            RunVM(new Sandbox(this, RantPattern.FromFile(path), new RNG(Seeds.NextRaw()), charLimit), timeout);

        /// <summary>
        /// Compiles the specified string into a pattern, executes it using a custom seed, and returns the resulting output.
        /// </summary>
        /// <param name="input">The input string to execute.</param>
        /// <param name="seed">The seed to generate output with.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public RantOutput Do(string input, long seed, int charLimit = 0, double timeout = -1) => 
            RunVM(new Sandbox(this, RantPattern.FromString(input), new RNG(seed), charLimit), timeout);

        /// <summary>
        /// Loads the file located at the specified path and executes it using a custom seed, returning the resulting output.
        /// </summary>
        /// <param name="path">The path to the file to execute.</param>
        /// <param name="seed">The seed to generate output with.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public RantOutput DoFile(string path, long seed, int charLimit = 0, double timeout = -1) => 
            RunVM(new Sandbox(this, RantPattern.FromFile(path), new RNG(seed), charLimit), timeout);

        /// <summary>
        /// Compiles the specified string into a pattern, executes it using a custom RNG, and returns the resulting output.
        /// </summary>
        /// <param name="input">The input string to execute.</param>
        /// <param name="rng">The random number generator to use when generating output.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public RantOutput Do(string input, RNG rng, int charLimit = 0, double timeout = -1) => 
            RunVM(new Sandbox(this, RantPattern.FromString(input), rng, charLimit), timeout);

        /// <summary>
        /// Loads the file located at the specified path and executes it using a custom seed, returning the resulting output.
        /// </summary>
        /// <param name="path">The path to the file to execute.</param>
        /// <param name="rng">The random number generator to use when generating output.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public RantOutput DoFile(string path, RNG rng, int charLimit = 0, double timeout = -1) => 
            RunVM(new Sandbox(this, RantPattern.FromFile(path), rng, charLimit), timeout);

        /// <summary>
        /// Executes the specified pattern and returns the resulting output.
        /// </summary>
        /// <param name="input">The pattern to execute.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public RantOutput Do(RantPattern input, int charLimit = 0, double timeout = -1) => 
            RunVM(new Sandbox(this, input, new RNG(Seeds.NextRaw()), charLimit), timeout);

        /// <summary>
        /// Executes the specified pattern using a custom seed and returns the resulting output.
        /// </summary>
        /// <param name="input">The pattern to execute.</param>
        /// <param name="seed">The seed to generate output with.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public RantOutput Do(RantPattern input, long seed, int charLimit = 0, double timeout = -1) => 
            RunVM(new Sandbox(this, input, new RNG(seed), charLimit), timeout);

        /// <summary>
        /// Executes the specified pattern using a custom random number generator and returns the resulting output.
        /// </summary>
        /// <param name="input">The pattern to execute.</param>
        /// <param name="rng">The random number generator to use when generating output.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public RantOutput Do(RantPattern input, RNG rng, int charLimit = 0, double timeout = -1) => 
            RunVM(new Sandbox(this, input, rng, charLimit), timeout);

        /// <summary>
        /// Executes the specified pattern and returns a series of outputs.
        /// </summary>
        /// <param name="input">The pattern to execute.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public IEnumerable<RantOutput> DoSerial(RantPattern input, int charLimit = 0, double timeout = -1) =>
            new Sandbox(this, input, new RNG(Seeds.NextRaw()), charLimit).RunSerial(timeout);

        /// <summary>
        /// Executes the specified pattern and returns a series of outputs.
        /// </summary>
        /// <param name="input">The patten to execute.</param>
        /// <param name="seed">The seed to generate output with.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public IEnumerable<RantOutput> DoSerial(RantPattern input, long seed, int charLimit = 0, double timeout = -1) =>
            new Sandbox(this, input, new RNG(seed), charLimit).RunSerial(timeout);

        /// <summary>
        /// Executes the specified pattern and returns a series of outputs.
        /// </summary>
        /// <param name="input">The pattero to execute.</param>
        /// <param name="rng">The random number generator to use when generating output.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public IEnumerable<RantOutput> DoSerial(RantPattern input, RNG rng, int charLimit = 0, double timeout = -1) =>
            new Sandbox(this, input, rng, charLimit).RunSerial(timeout);

        /// <summary>
        /// Executes the specified pattern and returns a series of outputs.
        /// </summary>
        /// <param name="input">The pattern to execute.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public IEnumerable<RantOutput> DoSerial(string input, int charLimit = 0, double timeout = -1) =>
            new Sandbox(this, RantPattern.FromString(input), new RNG(Seeds.NextRaw()), charLimit).RunSerial(timeout);

        /// <summary>
        /// Executes the specified pattern and returns a series of outputs.
        /// </summary>
        /// <param name="input">The pattern to execute.</param>
        /// <param name="seed">The seed to generate output with.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public IEnumerable<RantOutput> DoSerial(string input, long seed, int charLimit = 0, double timeout = -1) =>
            new Sandbox(this, RantPattern.FromString(input), new RNG(seed), charLimit).RunSerial(timeout);

        /// <summary>
        /// Executes the specified pattern and returns a series of outputs.
        /// </summary>
        /// <param name="input">The pattern to execute.</param>
        /// <param name="rng">The random number generator to use when generating output.</param>
        /// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
        /// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
        /// <returns></returns>
        public IEnumerable<RantOutput> DoSerial(string input, RNG rng, int charLimit = 0, double timeout = -1) =>
            new Sandbox(this, RantPattern.FromString(input), rng, charLimit).RunSerial(timeout);

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
    }
}