using System.IO;
using System.Linq;

namespace Manhood
{
    /// <summary>
    /// Stores dictionaries and state information for a Manhood context object. Exposes interpretation functionality.
    /// </summary>
    public class ManhoodContext
    {
        private readonly WordBank _wordBank;
        private readonly SubStore _subStore;

        /// <summary>
        /// Invoked when the interpreter requests a string via the [string] tag.
        /// </summary>
        public StringRequestCallback StringRequested;

        /// <summary>
        /// Creates a new Manhood context.
        /// </summary>
        public ManhoodContext()
        {
            _subStore = new SubStore();
            _wordBank = new WordBank(Enumerable.Empty<ManhoodDictionary>());
        }

        /// <summary>
        /// Creates a new Manhood context and loads dictionaries from the specified directory.
        /// </summary>
        /// <param name="resourcePath">The directory from which to load dictionaries.</param>
        public ManhoodContext(string resourcePath)
        {
            _subStore = new SubStore();
            _wordBank = WordBank.FromDirectory(resourcePath);
        }

        /// <summary>
        /// Interprets the specified input pattern and returns the generated output channels.
        /// </summary>
        /// <param name="input">The pattern to interpret.</param>
        /// <param name="seed">The seed to pass to the interpreter.</param>
        /// <param name="sizeLimit">The maximum number of characters to allow the interpreter to write.</param>
        /// <returns></returns>
        public ChannelSet Do(string input, long seed, int sizeLimit = 0)
        {
            return new Interpreter(_wordBank, _subStore, seed, sizeLimit, StringRequested).InterpretToChannels(new Pattern(input));
        }

        /// <summary>
        /// Interprets the specified input pattern and returns the generated output channels.
        /// </summary>
        /// <param name="input">The pattern to interpret.</param>
        /// <param name="seed">The seed to pass to the interpreter.</param>
        /// <param name="sizeLimit">The maximum number of characters to allow the interpreter to write.</param>
        /// <returns></returns>
        public ChannelSet Do(Pattern input, long seed, int sizeLimit = 0)
        {
            return new Interpreter(_wordBank, _subStore, seed, sizeLimit, StringRequested).InterpretToChannels(input);
        }

        /// <summary>
        /// Interprets the specified input pattern and returns the generated output channels.
        /// </summary>
        /// <param name="input">The pattern to interpret.</param>
        /// <param name="sizeLimit">The maximum number of characters to allow the interpreter to write.</param>
        /// <returns></returns>
        public ChannelSet Do(string input, int sizeLimit = 0)
        {
            return new Interpreter(_wordBank, _subStore, sizeLimit, StringRequested).InterpretToChannels(new Pattern(input));
        }

        /// <summary>
        /// Interprets the specified input pattern and returns the generated output channels.
        /// </summary>
        /// <param name="input">The pattern to interpret.</param>
        /// <param name="sizeLimit">The maximum number of characters to allow the interpreter to write.</param>
        /// <returns></returns>
        public ChannelSet Do(Pattern input, int sizeLimit = 0)
        {
            return new Interpreter(_wordBank, _subStore, sizeLimit, StringRequested).InterpretToChannels(input);
        }

        /// <summary>
        /// Interprets the contents of a file and returns the generated output channels.
        /// </summary>
        /// <param name="path">The path to the file to interpret.</param>
        /// <param name="sizeLimit">The maximum number of characters to allow the interpreter to write.</param>
        /// <returns></returns>
        public ChannelSet DoFile(string path, int sizeLimit = 0)
        {
            return new Interpreter(_wordBank, _subStore, sizeLimit, StringRequested).InterpretToChannels(new Pattern(File.ReadAllText(path)));
        }

        /// <summary>
        /// Interprets the contents of a file and returns the generated output channels.
        /// </summary>
        /// <param name="path">The path to the file to interpret.</param>
        /// <param name="seed">The seed to pass to the interpreter.</param>
        /// <param name="sizeLimit">The maximum number of characters to allow the interpreter to write.</param>
        /// <returns></returns>
        public ChannelSet DoFile(string path, long seed, int sizeLimit = 0)
        {
            return new Interpreter(_wordBank, _subStore, seed, sizeLimit, StringRequested).InterpretToChannels(new Pattern(File.ReadAllText(path)));
        }
    }
}