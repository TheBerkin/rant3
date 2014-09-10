using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Manhood
{
    internal class Vocabulary
    {
        private readonly Dictionary<string, Dictionary> _wordLists;

        public Vocabulary(IEnumerable<Dictionary> wordLists)
        {
            _wordLists = new Dictionary<string, Dictionary>();

            if (wordLists == null) return;

            foreach (var list in wordLists)
            {
                _wordLists[list.Name] = list;
            }
        }

        public void AddDictionary(Dictionary dictionary)
        {
            _wordLists[dictionary.Name] = dictionary;
        }

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