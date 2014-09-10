using System;

namespace Manhood
{
    public partial class Engine
    {
        private Vocabulary _vocabulary;

        private void LoadVocab(string path, NsfwFilter filter)
        {
            if (_vocabulary != null) return;

            if (String.IsNullOrEmpty(path))
            {
                _vocabulary = new Vocabulary(null);
                return;
            }

            _vocabulary = Vocabulary.FromDirectory(path, filter);
        }
    }
}