namespace Manhood
{
    internal class Repeater
    {
        private int _index;
        private readonly int _count;

        public Repeater(int count)
        {
            _index = 0;
            _count = count;
        }
        
        public int Count
        {
            get { return _count; }
        }

        public bool Finished
        {
            get { return _index >= _count; }
        }

        public int Next()
        {
            if (Finished) return -1;
            int i = _index;
            _index++;
            return i;
        }
    }
}