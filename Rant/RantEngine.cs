using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

using Rant.Core;
using Rant.Core.Framework;
using Rant.Core.ObjectModel;
using Rant.Core.Utilities;
using Rant.Formats;
using Rant.Resources;
using Rant.Vocabulary;
using Rant.Vocabulary.Querying;

using static Rant.Localization.Txtres;

namespace Rant
{
	/// <summary>
	/// The central class of the Rant engine that allows the execution of patterns.
	/// </summary>
	public sealed class RantEngine
	{
		private readonly HashSet<RantPackageDependency> _loadedPackages = new HashSet<RantPackageDependency>();
		private readonly Dictionary<string, RantProgram> _patternCache = new Dictionary<string, RantProgram>();

		/// <summary>
		/// The currently set flags.
		/// </summary>
		public readonly HashSet<string> Flags = new HashSet<string>();

		internal readonly ObjectTable Objects = new ObjectTable();
		private CarrierState _carrierState = null;
		private RantDictionary _dictionary = new RantDictionary();
		private RantFormat _format = RantFormat.English;
		private bool _preserveCarrierState = false;
		private RantDependencyResolver _resolver = new RantDependencyResolver();
		internal Dictionary<string, RantModule> PackageModules = new Dictionary<string, RantModule>();

		/// <summary>
		/// Creates a new RantEngine object without a dictionary.
		/// </summary>
		public RantEngine()
		{
		}

		/// <summary>
		/// Creates a new RantEngine object with the specified vocabulary.
		/// </summary>
		/// <param name="dictionary">The vocabulary to load in this instance.</param>
		public RantEngine(RantDictionary dictionary)
		{
			if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
			_dictionary = dictionary;
		}

		/// <summary>
		/// User-defined Rant modules.
		/// </summary>
		public Dictionary<string, RantModule> Modules { get; } = new Dictionary<string, RantModule>();

		/// <summary>
		/// The current formatting settings for the engine.
		/// </summary>
		public RantFormat Format
		{
			get { return _format; }
			set
			{
				if (value == null) throw new ArgumentNullException(nameof(value));
				_format = value;
			}
		}

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

		/// <summary>
		/// Specifies whether to preserve carrier states between patterns.
		/// </summary>
		public bool PreserveCarrierState
		{
			get { return _preserveCarrierState; }
			set
			{
				if (!value) _carrierState = null;
				_preserveCarrierState = value;
			}
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
		/// Deletes all state data in the engine's persisted carrier state, if available.
		/// </summary>
		public void ResetCarrierState()
		{
			_carrierState?.Reset();
		}

		/// <summary>
		/// Returns a boolean value indicating whether a program by the specified name has been loaded from a package.
		/// </summary>
		/// <param name="patternName">The name of the program to check.</param>
		/// <returns></returns>
		public bool ProgramNameLoaded(string patternName)
		{
			return _patternCache != null && _patternCache.ContainsKey(patternName);
		}

		/// <summary>
		/// Used by package loader
		/// </summary>
		/// <param name="program">Program to load</param>
		/// <returns></returns>
		internal bool CacheProgramInternal(RantProgram program)
		{
			if (Util.IsNullOrWhiteSpace(program.Name) || _patternCache.ContainsKey(program.Name)) return false;
			_patternCache[program.Name] = program;
			return true;
		}

		/// <summary>
		/// Loads the specified package into the engine.
		/// </summary>
		/// <param name="package">The package to load.</param>
		public void LoadPackage(RantPackage package)
		{
			if (package == null) throw new ArgumentNullException(nameof(package));
			if (_loadedPackages.Contains(RantPackageDependency.Create(package))) return;

			foreach (var dependency in package.GetDependencies())
			{
				RantPackage pkg;
				if (!_resolver.TryResolvePackage(dependency, out pkg))
					throw new FileNotFoundException(GetString("err-unresolvable-package", package, dependency));
				LoadPackage(pkg);
			}

			foreach(var res in package.GetResources()) res.Load(this);

			_loadedPackages.Add(RantPackageDependency.Create(package));
		}

		/// <summary>
		/// Loads the package at the specified file path into the engine.
		/// </summary>
		/// <param name="path">The path to the package to load.</param>
		public void LoadPackage(string path)
		{
			if (Util.IsNullOrWhiteSpace(path))
				throw new ArgumentException(GetString("err-empty-path"));

			if (Util.IsNullOrWhiteSpace(Path.GetExtension(path)))
				path += RantPackage.EXTENSION;

			LoadPackage(RantPackage.Load(path));
		}

		/// <summary>
		/// Returns a pattern with the specified name from the engine's cache. If the pattern doesn't exist, it is loaded from
		/// file.
		/// </summary>
		/// <param name="name">The name or path of the pattern to retrieve.</param>
		/// <returns></returns>
		internal RantProgram GetProgramInternal(string name)
		{
			RantProgram pattern;
			if (_patternCache.TryGetValue(name, out pattern)) return pattern;
			return _patternCache[name] = RantProgram.CompileFile(name);
		}

		#region Static members

		private static int _maxStackSize = 64;
		private static readonly RNG Seeds = new RNG();

		static RantEngine()
		{
			ForceLoad();
			RantFunctionRegistry.Load();
		}

		/// <summary>
		/// Gets or sets the maximum stack size allowed for a pattern.
		/// </summary>
		public static int MaxStackSize
		{
			get { return _maxStackSize; }
			set
			{
				if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value), GetString("err-zero-or-negative"));
				_maxStackSize = value;
			}
		}

		#endregion

		#region Do, DoFile

#if !UNITY
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		private RantOutput RunVM(Sandbox vm, double timeout)
		{
			vm.UserModules = Modules;
			vm.PackageModules = PackageModules;
			if (_preserveCarrierState && _carrierState == null)
				_carrierState = vm.CarrierState;
			return vm.Run(timeout);
		}

		private CarrierState GetPreservedCarrierState() => _preserveCarrierState ? _carrierState : null;

		/// <summary>
		/// Compiles the specified string into a pattern, executes it, and returns the resulting output.
		/// </summary>
		/// <param name="input">The input string to execute.</param>
		/// <param name="charLimit">
		/// The maximum number of characters that can be printed. An exception will be thrown if the limit
		/// is exceeded. Set to zero or below for unlimited characters.
		/// </param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput Do(string input, int charLimit = 0, double timeout = -1, RantProgramArgs args = null) =>
			RunVM(
				new Sandbox(this, RantProgram.CompileString(input), new RNG(Seeds.NextRaw()), charLimit, GetPreservedCarrierState(),
					args), timeout);

		/// <summary>
		/// Loads the file located at the specified path and executes it, returning the resulting output.
		/// </summary>
		/// <param name="path">The path to the file to execute.</param>
		/// <param name="charLimit">
		/// The maximum number of characters that can be printed. An exception will be thrown if the limit
		/// is exceeded. Set to zero or below for unlimited characters.
		/// </param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput DoFile(string path, int charLimit = 0, double timeout = -1, RantProgramArgs args = null) =>
			RunVM(
				new Sandbox(this, RantProgram.CompileFile(path), new RNG(Seeds.NextRaw()), charLimit, GetPreservedCarrierState(), args),
				timeout);

		/// <summary>
		/// Compiles the specified string into a pattern, executes it using a custom seed, and returns the resulting output.
		/// </summary>
		/// <param name="input">The input string to execute.</param>
		/// <param name="seed">The seed to generate output with.</param>
		/// <param name="charLimit">
		/// The maximum number of characters that can be printed. An exception will be thrown if the limit
		/// is exceeded. Set to zero or below for unlimited characters.
		/// </param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput Do(string input, long seed, int charLimit = 0, double timeout = -1, RantProgramArgs args = null) =>
			RunVM(new Sandbox(this, RantProgram.CompileString(input), new RNG(seed), charLimit, GetPreservedCarrierState(), args),
				timeout);

		/// <summary>
		/// Loads the file located at the specified path and executes it using a custom seed, returning the resulting output.
		/// </summary>
		/// <param name="path">The path to the file to execute.</param>
		/// <param name="seed">The seed to generate output with.</param>
		/// <param name="charLimit">
		/// The maximum number of characters that can be printed. An exception will be thrown if the limit
		/// is exceeded. Set to zero or below for unlimited characters.
		/// </param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput DoFile(string path, long seed, int charLimit = 0, double timeout = -1, RantProgramArgs args = null)
			=>
				RunVM(new Sandbox(this, RantProgram.CompileFile(path), new RNG(seed), charLimit, GetPreservedCarrierState(), args),
					timeout);

		/// <summary>
		/// Compiles the specified string into a pattern, executes it using a custom RNG, and returns the resulting output.
		/// </summary>
		/// <param name="input">The input string to execute.</param>
		/// <param name="rng">The random number generator to use when generating output.</param>
		/// <param name="charLimit">
		/// The maximum number of characters that can be printed. An exception will be thrown if the limit
		/// is exceeded. Set to zero or below for unlimited characters.
		/// </param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput Do(string input, RNG rng, int charLimit = 0, double timeout = -1, RantProgramArgs args = null) =>
			RunVM(new Sandbox(this, RantProgram.CompileString(input), rng, charLimit, GetPreservedCarrierState(), args), timeout);

		/// <summary>
		/// Loads the file located at the specified path and executes it using a custom seed, returning the resulting output.
		/// </summary>
		/// <param name="path">The path to the file to execute.</param>
		/// <param name="rng">The random number generator to use when generating output.</param>
		/// <param name="charLimit">
		/// The maximum number of characters that can be printed. An exception will be thrown if the limit
		/// is exceeded. Set to zero or below for unlimited characters.
		/// </param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput DoFile(string path, RNG rng, int charLimit = 0, double timeout = -1, RantProgramArgs args = null) =>
			RunVM(new Sandbox(this, RantProgram.CompileFile(path), rng, charLimit, GetPreservedCarrierState(), args), timeout);

		/// <summary>
		/// Executes the specified pattern and returns the resulting output.
		/// </summary>
		/// <param name="input">The pattern to execute.</param>
		/// <param name="charLimit">
		/// The maximum number of characters that can be printed. An exception will be thrown if the limit
		/// is exceeded. Set to zero or below for unlimited characters.
		/// </param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput Do(RantProgram input, int charLimit = 0, double timeout = -1, RantProgramArgs args = null) =>
			RunVM(new Sandbox(this, input, new RNG(Seeds.NextRaw()), charLimit, GetPreservedCarrierState(), args), timeout);

		/// <summary>
		/// Executes the specified pattern using a custom seed and returns the resulting output.
		/// </summary>
		/// <param name="input">The pattern to execute.</param>
		/// <param name="seed">The seed to generate output with.</param>
		/// <param name="charLimit">
		/// The maximum number of characters that can be printed. An exception will be thrown if the limit
		/// is exceeded. Set to zero or below for unlimited characters.
		/// </param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput Do(RantProgram input, long seed, int charLimit = 0, double timeout = -1, RantProgramArgs args = null)
			=>
				RunVM(new Sandbox(this, input, new RNG(seed), charLimit, GetPreservedCarrierState(), args), timeout);

		/// <summary>
		/// Executes the specified pattern using a custom random number generator and returns the resulting output.
		/// </summary>
		/// <param name="input">The pattern to execute.</param>
		/// <param name="rng">The random number generator to use when generating output.</param>
		/// <param name="charLimit">
		/// The maximum number of characters that can be printed. An exception will be thrown if the limit
		/// is exceeded. Set to zero or below for unlimited characters.
		/// </param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput Do(RantProgram input, RNG rng, int charLimit = 0, double timeout = -1, RantProgramArgs args = null)
			=>
				RunVM(new Sandbox(this, input, rng, charLimit, GetPreservedCarrierState(), args), timeout);

		/// <summary>
		/// Executes the specified pattern and returns a series of outputs.
		/// </summary>
		/// <param name="input">The pattern to execute.</param>
		/// <param name="charLimit">
		/// The maximum number of characters that can be printed. An exception will be thrown if the limit
		/// is exceeded. Set to zero or below for unlimited characters.
		/// </param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public IEnumerable<RantOutput> DoSerial(RantProgram input, int charLimit = 0, double timeout = -1,
			RantProgramArgs args = null) =>
				new Sandbox(this, input, new RNG(Seeds.NextRaw()), charLimit, GetPreservedCarrierState(), args).RunSerial(timeout);

		/// <summary>
		/// Executes the specified pattern and returns a series of outputs.
		/// </summary>
		/// <param name="input">The patten to execute.</param>
		/// <param name="seed">The seed to generate output with.</param>
		/// <param name="charLimit">
		/// The maximum number of characters that can be printed. An exception will be thrown if the limit
		/// is exceeded. Set to zero or below for unlimited characters.
		/// </param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public IEnumerable<RantOutput> DoSerial(RantProgram input, long seed, int charLimit = 0, double timeout = -1,
			RantProgramArgs args = null) =>
				new Sandbox(this, input, new RNG(seed), charLimit, GetPreservedCarrierState(), args).RunSerial(timeout);

		/// <summary>
		/// Executes the specified pattern and returns a series of outputs.
		/// </summary>
		/// <param name="input">The pattero to execute.</param>
		/// <param name="rng">The random number generator to use when generating output.</param>
		/// <param name="charLimit">
		/// The maximum number of characters that can be printed. An exception will be thrown if the limit
		/// is exceeded. Set to zero or below for unlimited characters.
		/// </param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public IEnumerable<RantOutput> DoSerial(RantProgram input, RNG rng, int charLimit = 0, double timeout = -1,
			RantProgramArgs args = null) =>
				new Sandbox(this, input, rng, charLimit, GetPreservedCarrierState(), args).RunSerial(timeout);

		/// <summary>
		/// Executes the specified pattern and returns a series of outputs.
		/// </summary>
		/// <param name="input">The pattern to execute.</param>
		/// <param name="charLimit">
		/// The maximum number of characters that can be printed. An exception will be thrown if the limit
		/// is exceeded. Set to zero or below for unlimited characters.
		/// </param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public IEnumerable<RantOutput> DoSerial(string input, int charLimit = 0, double timeout = -1,
			RantProgramArgs args = null) =>
				new Sandbox(this, RantProgram.CompileString(input), new RNG(Seeds.NextRaw()), charLimit, GetPreservedCarrierState(),
					args).RunSerial(timeout);

		/// <summary>
		/// Executes the specified pattern and returns a series of outputs.
		/// </summary>
		/// <param name="input">The pattern to execute.</param>
		/// <param name="seed">The seed to generate output with.</param>
		/// <param name="charLimit">
		/// The maximum number of characters that can be printed. An exception will be thrown if the limit
		/// is exceeded. Set to zero or below for unlimited characters.
		/// </param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public IEnumerable<RantOutput> DoSerial(string input, long seed, int charLimit = 0, double timeout = -1,
			RantProgramArgs args = null) =>
				new Sandbox(this, RantProgram.CompileString(input), new RNG(seed), charLimit, GetPreservedCarrierState(), args)
					.RunSerial(timeout);

		/// <summary>
		/// Executes the specified pattern and returns a series of outputs.
		/// </summary>
		/// <param name="input">The pattern to execute.</param>
		/// <param name="rng">The random number generator to use when generating output.</param>
		/// <param name="charLimit">
		/// The maximum number of characters that can be printed. An exception will be thrown if the limit
		/// is exceeded. Set to zero or below for unlimited characters.
		/// </param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public IEnumerable<RantOutput> DoSerial(string input, RNG rng, int charLimit = 0, double timeout = -1,
			RantProgramArgs args = null) =>
				new Sandbox(this, RantProgram.CompileString(input), rng, charLimit, GetPreservedCarrierState(), args).RunSerial(timeout);

		/// <summary>
		/// Executes a pattern that has been loaded from a package and returns the resulting output.
		/// </summary>
		/// <param name="patternName">The name of the pattern to execute.</param>
		/// <param name="charLimit">
		/// The maximum number of characters that can be printed. An exception will be thrown if the limit
		/// is exceeded. Set to zero or below for unlimited characters.
		/// </param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput DoPackaged(string patternName, int charLimit = 0, double timeout = -1, RantProgramArgs args = null)
		{
			if (!ProgramNameLoaded(patternName))
				throw new ArgumentException("Pattern doesn't exist.");

			return
				RunVM(
					new Sandbox(this, _patternCache[patternName], new RNG(Seeds.NextRaw()), charLimit, GetPreservedCarrierState(), args),
					timeout);
		}

		/// <summary>
		/// Executes a pattern that has been loaded from a package and returns the resulting output.
		/// </summary>
		/// <param name="patternName">The name of the pattern to execute.</param>
		/// <param name="seed">The seed to generate output with.</param>
		/// <param name="charLimit">
		/// The maximum number of characters that can be printed. An exception will be thrown if the limit
		/// is exceeded. Set to zero or below for unlimited characters.
		/// </param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput DoPackaged(string patternName, long seed, int charLimit = 0, double timeout = -1,
			RantProgramArgs args = null)
		{
			if (!ProgramNameLoaded(patternName))
				throw new ArgumentException("Pattern doesn't exist.");

			return
				RunVM(new Sandbox(this, _patternCache[patternName], new RNG(seed), charLimit, GetPreservedCarrierState(), args),
					timeout);
		}

		/// <summary>
		/// Executes a pattern that has been loaded from a package using a custom random number generator and returns the resulting
		/// output.
		/// </summary>
		/// <param name="patternName">The name of the pattern to execute.</param>
		/// <param name="rng">The random number generator to use when generating output.</param>
		/// <param name="charLimit">
		/// The maximum number of characters that can be printed. An exception will be thrown if the limit
		/// is exceeded. Set to zero or below for unlimited characters.
		/// </param>
		/// <param name="timeout">The maximum number of seconds that the pattern will execute for.</param>
		/// <param name="args">The arguments to pass to the pattern.</param>
		/// <returns></returns>
		public RantOutput DoPackaged(string patternName, RNG rng, int charLimit = 0, double timeout = -1,
			RantProgramArgs args = null)
		{
			if (!ProgramNameLoaded(patternName))
				throw new ArgumentException("Pattern doesn't exist.");

			return RunVM(new Sandbox(this, _patternCache[patternName], rng, charLimit, GetPreservedCarrierState(), args), timeout);
		}

		#endregion
	}
}