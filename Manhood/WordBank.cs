using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Manhood
{
    internal class WordBank
    {
        private readonly Dictionary<string, ManhoodDictionary> _wordLists;

        public WordBank(IEnumerable<ManhoodDictionary> wordLists)
        {
            _wordLists = new Dictionary<string, ManhoodDictionary>();

            foreach (var list in wordLists)
            {
                _wordLists[list.Name] = list;
            }
        }

        public void AddDictionary(ManhoodDictionary dictionary)
        {
            _wordLists[dictionary.Name] = dictionary;
        }

        public static WordBank FromDirectory(string directory)
        {
            return new WordBank(Directory.GetFiles(directory, "*.dic").Select(ManhoodDictionary.FromFile).ToList());
        }

        internal string GetWord(Interpreter interpreter, Query wordCall)
        {
            ManhoodDictionary wordList;
            return !_wordLists.TryGetValue(wordCall.Name, out wordList) 
                ? "MISSINGLIST" 
                : wordList.GetWord(interpreter, wordCall);
        }
    }
}