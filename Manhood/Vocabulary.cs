using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Manhood
{
    /// <summary>
    /// Represents a collection of named dictionaries that can be queried by Manhood.
    /// </summary>
    public class Vocabulary
    {
        private readonly Dictionary<string, Dictionary> _wordLists;

        /// <summary>
        /// Creates a new Vocabulary object from the specified dictionary collection.
        /// </summary>
        /// <param name="dics"></param>
        public Vocabulary(IEnumerable<Dictionary> dics)
        {
            _wordLists = new Dictionary<string, Dictionary>();

            if (dics == null) return;

            foreach (var list in dics)
            {
                _wordLists[list.Name] = list;
            }
        }

        /// <summary>
        /// Adds a new Dictionary object to the collection.
        /// </summary>
        /// <param name="dictionary"></param>
        public void AddDictionary(Dictionary dictionary)
        {
            _wordLists[dictionary.Name] = dictionary;
        }

        /// <summary>
        /// Loads all dictionary (.dic) files from the specified directory and returns a Vocabulary object that contains the loaded dictionaries.
        /// </summary>
        /// <param name="directory">The directory from which to load dictionaries.</param>
        /// <param name="filter">Indicates whether dictionary entries marked with the #nsfw flag should be loaded.</param>
        /// <returns></returns>
        public static Vocabulary FromDirectory(string directory, NsfwFilter filter)
        {
            return new Vocabulary(Directory.GetFiles(directory, "*.dic").Select(file => Dictionary.FromFile(file, filter)).ToList());
        }

        internal string GetWord(Interpreter interpreter, Query wordCall)
        {
            Dictionary wordList;
            return !_wordLists.TryGetValue(wordCall.Name, out wordList) 
                ? "MISSINGDIC" 
                : wordList.GetWord(interpreter, wordCall);
        }
    }
}