using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rant
{
    /// <summary>
    /// Represents a collection of named, queryable dictionaries. This is the default vocabulary type used by Rant.
    /// </summary>
    public sealed class Vocabulary : IVocabulary
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

        /// <summary>
        /// Queries the vocabulary according to the specified criteria and returns a random match.
        /// </summary>
        /// <param name="rng">The random number generator to randomize the match with.</param>
        /// <param name="query">The search criteria to use.</param>
        /// <returns></returns>
        public string Query(RNG rng, Query query)
        {
            Dictionary wordList;
            return !_wordLists.TryGetValue(query.Name, out wordList) 
                ? "MISSINGDIC" 
                : wordList.Query(rng, query);
        }
    }
}