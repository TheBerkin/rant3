using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Rant.Core;
using Rant.Core.Compiler.Parselets;
using Rant.Core.Framework;
using Rant.Core.ObjectModel;
using Rant.Core.Utilities;
using Rant.Formats;
using Rant.Resources;
using Rant.Vocabulary;

namespace Rant
{
    /// <summary>
    /// The central class of the Rant engine that allows the execution of patterns.
    /// </summary>
    public sealed class RantEngine
    {
		#region Static members
		private static int _maxStackSize = 64;
        private static readonly RNG Seeds = new RNG();

        static RantEngine()
	    {
		    RantFunctionRegistry.Load();
            RichardFunctions.Load();
            Parselet.Load();
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

		#endregion

		/// <summary>
		/// User-defined Rant modules.
		/// </summary>
		public Dictionary<string, RantModule> Modules => _userModules;

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
		/// Gets or sets the depdendency resolver used for packages.
		/// </summary>
		public RantDependencyResolver DependencyResolver
		{
			get { return _resolver; }
			set
			{
				if (value == null) throw new ArgumentNullException(nameof(value));
				_resolver = value;
			}
		}

		internal readonly ObjectTable Objects = new ObjectTable();
		internal Dictionary<string, RantModule> PackageModules = new Dictionary<string, RantModule>();

		private readonly Dictionary<string, RantModule> _userModules = new Dictionary<string, RantModule>();
        private readonly Dictionary<string, RantPattern> _patternCache = new Dictionary<string, RantPattern>();
	    private readonly HashSet<RantPackageDependency> _loadedPackages = new HashSet<RantPackageDependency>(); 
		private RantDependencyResolver _resolver = new RantDependencyResolver();
		private RantDictionary _dictionary = new RantDictionary();

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
				{
					_patternCache[pattern.Name] = pattern;
					if (pattern.Module != null)
						PackageModules[pattern.Name] = pattern.Module;
				}
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
		        RantPackage pkg;
				if (!_resolver.TryResolvePackage(dependency, out pkg))
					throw new FileNotFoundException($"Package '{package}' was unable to resolve dependency '{dependency}'");
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
            if (Util.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null nor empty.");

	        if (Util.IsNullOrWhiteSpace(Path.GetExtension(path)))
		        path += ".rantpkg";
			
			LoadPackage(RantPackage.Load(path), mergeBehavior);
		}

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

		#region Do, DoFile

		private RantOutput RunVM(Sandbox vm, double timeout)
		{
			vm.UserModules = _userModules;
			vm.PackageModules = PackageModules;
			return vm.Run(timeout);
		}

		/// <summary>
		/// Compiles the specified string into a pattern, executes it, and returns the resulting output.
		/// </summary>
		/// <param name="input">The input string to execute.</param>
		/// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput Do(string input, int charLimit = 0, double timeout = -1, RantPatternArgs args = null) => 
            RunVM(new Sandbox(this, RantPattern.FromString(input), new RNG(Seeds.NextRaw()), charLimit, args), timeout);

		/// <summary>
		/// Loads the file located at the specified path and executes it, returning the resulting output.
		/// </summary>
		/// <param name="path">The path to the file to execute.</param>
		/// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput DoFile(string path, int charLimit = 0, double timeout = -1, RantPatternArgs args = null) => 
            RunVM(new Sandbox(this, RantPattern.FromFile(path), new RNG(Seeds.NextRaw()), charLimit, args), timeout);

		/// <summary>
		/// Compiles the specified string into a pattern, executes it using a custom seed, and returns the resulting output.
		/// </summary>
		/// <param name="input">The input string to execute.</param>
		/// <param name="seed">The seed to generate output with.</param>
		/// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput Do(string input, long seed, int charLimit = 0, double timeout = -1, RantPatternArgs args = null) => 
            RunVM(new Sandbox(this, RantPattern.FromString(input), new RNG(seed), charLimit, args), timeout);

		/// <summary>
		/// Loads the file located at the specified path and executes it using a custom seed, returning the resulting output.
		/// </summary>
		/// <param name="path">The path to the file to execute.</param>
		/// <param name="seed">The seed to generate output with.</param>
		/// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput DoFile(string path, long seed, int charLimit = 0, double timeout = -1, RantPatternArgs args = null) => 
            RunVM(new Sandbox(this, RantPattern.FromFile(path), new RNG(seed), charLimit, args), timeout);

		/// <summary>
		/// Compiles the specified string into a pattern, executes it using a custom RNG, and returns the resulting output.
		/// </summary>
		/// <param name="input">The input string to execute.</param>
		/// <param name="rng">The random number generator to use when generating output.</param>
		/// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput Do(string input, RNG rng, int charLimit = 0, double timeout = -1, RantPatternArgs args = null) => 
            RunVM(new Sandbox(this, RantPattern.FromString(input), rng, charLimit, args), timeout);

		/// <summary>
		/// Loads the file located at the specified path and executes it using a custom seed, returning the resulting output.
		/// </summary>
		/// <param name="path">The path to the file to execute.</param>
		/// <param name="rng">The random number generator to use when generating output.</param>
		/// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput DoFile(string path, RNG rng, int charLimit = 0, double timeout = -1, RantPatternArgs args = null) => 
            RunVM(new Sandbox(this, RantPattern.FromFile(path), rng, charLimit, args), timeout);

		/// <summary>
		/// Executes the specified pattern and returns the resulting output.
		/// </summary>
		/// <param name="input">The pattern to execute.</param>
		/// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput Do(RantPattern input, int charLimit = 0, double timeout = -1, RantPatternArgs args = null) => 
            RunVM(new Sandbox(this, input, new RNG(Seeds.NextRaw()), charLimit, args), timeout);

		/// <summary>
		/// Executes the specified pattern using a custom seed and returns the resulting output.
		/// </summary>
		/// <param name="input">The pattern to execute.</param>
		/// <param name="seed">The seed to generate output with.</param>
		/// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput Do(RantPattern input, long seed, int charLimit = 0, double timeout = -1, RantPatternArgs args = null) => 
            RunVM(new Sandbox(this, input, new RNG(seed), charLimit, args), timeout);

		/// <summary>
		/// Executes the specified pattern using a custom random number generator and returns the resulting output.
		/// </summary>
		/// <param name="input">The pattern to execute.</param>
		/// <param name="rng">The random number generator to use when generating output.</param>
		/// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput Do(RantPattern input, RNG rng, int charLimit = 0, double timeout = -1, RantPatternArgs args = null) => 
            RunVM(new Sandbox(this, input, rng, charLimit, args), timeout);

		/// <summary>
		/// Executes the specified pattern and returns a series of outputs.
		/// </summary>
		/// <param name="input">The pattern to execute.</param>
		/// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public IEnumerable<RantOutput> DoSerial(RantPattern input, int charLimit = 0, double timeout = -1, RantPatternArgs args = null) =>
            new Sandbox(this, input, new RNG(Seeds.NextRaw()), charLimit, args).RunSerial(timeout);

		/// <summary>
		/// Executes the specified pattern and returns a series of outputs.
		/// </summary>
		/// <param name="input">The patten to execute.</param>
		/// <param name="seed">The seed to generate output with.</param>
		/// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public IEnumerable<RantOutput> DoSerial(RantPattern input, long seed, int charLimit = 0, double timeout = -1, RantPatternArgs args = null) =>
            new Sandbox(this, input, new RNG(seed), charLimit, args).RunSerial(timeout);

		/// <summary>
		/// Executes the specified pattern and returns a series of outputs.
		/// </summary>
		/// <param name="input">The pattero to execute.</param>
		/// <param name="rng">The random number generator to use when generating output.</param>
		/// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public IEnumerable<RantOutput> DoSerial(RantPattern input, RNG rng, int charLimit = 0, double timeout = -1, RantPatternArgs args = null) =>
            new Sandbox(this, input, rng, charLimit, args).RunSerial(timeout);

		/// <summary>
		/// Executes the specified pattern and returns a series of outputs.
		/// </summary>
		/// <param name="input">The pattern to execute.</param>
		/// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public IEnumerable<RantOutput> DoSerial(string input, int charLimit = 0, double timeout = -1, RantPatternArgs args = null) =>
            new Sandbox(this, RantPattern.FromString(input), new RNG(Seeds.NextRaw()), charLimit, args).RunSerial(timeout);

		/// <summary>
		/// Executes the specified pattern and returns a series of outputs.
		/// </summary>
		/// <param name="input">The pattern to execute.</param>
		/// <param name="seed">The seed to generate output with.</param>
		/// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public IEnumerable<RantOutput> DoSerial(string input, long seed, int charLimit = 0, double timeout = -1, RantPatternArgs args = null) =>
            new Sandbox(this, RantPattern.FromString(input), new RNG(seed), charLimit, args).RunSerial(timeout);

		/// <summary>
		/// Executes the specified pattern and returns a series of outputs.
		/// </summary>
		/// <param name="input">The pattern to execute.</param>
		/// <param name="rng">The random number generator to use when generating output.</param>
		/// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public IEnumerable<RantOutput> DoSerial(string input, RNG rng, int charLimit = 0, double timeout = -1, RantPatternArgs args = null) =>
            new Sandbox(this, RantPattern.FromString(input), rng, charLimit, args).RunSerial(timeout);

		/// <summary>
		/// Executes a pattern that has been loaded from a package and returns the resulting output.
		/// </summary>
		/// <param name="patternName">The name of the pattern to execute.</param>
		/// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput DoPackaged(string patternName, int charLimit = 0, double timeout = -1, RantPatternArgs args = null)
        {
            if (!PatternExists(patternName))
                throw new ArgumentException("Pattern doesn't exist.");

            return RunVM(new Sandbox(this, _patternCache[patternName], new RNG(Seeds.NextRaw()), charLimit, args), timeout);
        }

		/// <summary>
		/// Executes a pattern that has been loaded from a package and returns the resulting output.
		/// </summary>
		/// <param name="patternName">The name of the pattern to execute.</param>
		/// <param name="seed">The seed to generate output with.</param>
		/// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput DoPackaged(string patternName, long seed, int charLimit = 0, double timeout = -1, RantPatternArgs args = null)
        {
            if (!PatternExists(patternName))
                throw new ArgumentException("Pattern doesn't exist.");

            return RunVM(new Sandbox(this, _patternCache[patternName], new RNG(seed), charLimit, args), timeout);
        }

		/// <summary>
		/// Executes a pattern that has been loaded from a package using a custom random number generator and returns the resulting output.
		/// </summary>
		/// <param name="patternName">The name of the pattern to execute.</param>
		/// <param name="rng">The random number generator to use when generating output.</param>
		/// <param name="charLimit">The maximum number of characters that can be printed. An exception will be thrown if the limit is exceeded. Set to zero or below for unlimited characters.</param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput DoPackaged(string patternName, RNG rng, int charLimit = 0, double timeout = -1, RantPatternArgs args = null)
        {
            if (!PatternExists(patternName))
                throw new ArgumentException("Pattern doesn't exist.");

            return RunVM(new Sandbox(this, _patternCache[patternName], rng, charLimit, args), timeout);
        }
        #endregion
    }
}