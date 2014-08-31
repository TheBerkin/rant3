using Manhood.Compiler;

namespace Manhood
{
    public sealed partial class Engine
    {
        private readonly VarStore _vars;

        internal VarStore Variables
        {
            get { return _vars; }
        }

        public Engine()
        {
            InitializeVocab("", NsfwFilter.Allow);
            _vars = new VarStore();
        }

        public Engine(string vocabularyPath)
        {
            InitializeVocab(vocabularyPath, NsfwFilter.Allow);
            _vars = new VarStore();
        }

        public Engine(string vocabularyPath, NsfwFilter filter)
        {
            InitializeVocab(vocabularyPath, filter);
            _vars = new VarStore();
        }

        public ChannelSet Do(string input)
        {
            return new Interpreter(this, Source.FromString(input), new RNG()).Run();
        }
    }
}