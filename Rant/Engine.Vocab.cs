using System;

namespace Rant
{
    public partial class Engine
    {
        private IVocabulary _vocabulary;

        /// <summary>
        /// The vocabulary associated with this instance.
        /// </summary>
        public IVocabulary Vocabulary
        {
            get { return _vocabulary; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                _vocabulary = value;
            }
        }

        private void LoadVocab(string path, NsfwFilter filter)
        {
            if (_vocabulary != null) return;

            if (String.IsNullOrEmpty(path))
            {
                _vocabulary = new Vocabulary(null);
                return;
            }

            _vocabulary = Rant.Vocabulary.FromDirectory(path, filter);
        }
    }
}