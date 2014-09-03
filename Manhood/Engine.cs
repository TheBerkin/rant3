using Manhood.Compiler;

namespace Manhood
{
    /// <summary>
    /// The central class of the Manhood engine that allows the execution of patterns.
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

        internal VarStore Variables
        {
            get { return _vars; }
        }

        /// <summary>
        /// Creates a new Engine instance with no vocabulary.
        /// </summary>
        public Engine()
        {
            InitializeVocab("", DefaultNsfwFilter);
            _vars = new VarStore();
        }

        /// <summary>
        /// Creates a new Engine instance that loads vocabulary from the specified path.
        /// </summary>
        /// <param name="vocabularyPath">The path to the vocabulary files.</param>
        public Engine(string vocabularyPath)
        {
            InitializeVocab(vocabularyPath, DefaultNsfwFilter);
            _vars = new VarStore();
        }

        /// <summary>
        /// Creates a new Engine instance that loads vocabulary from a path according to the specified filtering option.
        /// </summary>
        /// <param name="vocabularyPath">The path to the vocabulary files.</param>
        /// <param name="filter">The filtering option to apply when loading the files.</param>
        public Engine(string vocabularyPath, NsfwFilter filter)
        {
            InitializeVocab(vocabularyPath, filter);
            _vars = new VarStore();
        }

        /// <summary>
        /// Executes the specified string and returns the resulting output.
        /// </summary>
        /// <param name="input">The input string to execute.</param>
        /// <returns></returns>
        public ChannelSet Do(string input)
        {
            return new Interpreter(this, Source.FromString(input), new RNG(Seeds.NextRaw()), _vocabulary).Run();
        }

        public ChannelSet DoFile(string path)
        {
            return new Interpreter(this, Source.FromFile(path), new RNG(Seeds.NextRaw()), _vocabulary).Run();
        }

        public ChannelSet Do(string input, long seed)
        {
            return new Interpreter(this, Source.FromString(input), new RNG(seed), _vocabulary).Run();
        }

        public ChannelSet DoFile(string path, long seed)
        {
            return new Interpreter(this, Source.FromFile(path), new RNG(seed), _vocabulary).Run();
        }

        public ChannelSet Do(string input, RNG rng)
        {
            return new Interpreter(this, Source.FromString(input), rng, _vocabulary).Run();
        }

        public ChannelSet DoFile(string path, RNG rng)
        {
            return new Interpreter(this, Source.FromFile(path), rng, _vocabulary).Run();
        }

        public ChannelSet Do(Source input)
        {
            return new Interpreter(this, input, new RNG(Seeds.NextRaw()), _vocabulary).Run();
        }

        public ChannelSet Do(Source input, long seed)
        {
            return new Interpreter(this, input, new RNG(seed), _vocabulary).Run();
        }

        public ChannelSet Do(Source input, RNG rng)
        {
            return new Interpreter(this, input, rng, _vocabulary).Run();
        }
    }
}