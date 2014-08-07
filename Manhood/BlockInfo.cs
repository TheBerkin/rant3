namespace Manhood
{
    internal class BlockInfo
    {
        private readonly int _start;
        private readonly int _end;
        private readonly Range[] _ranges;

        public BlockInfo(int start, int end, params Range[] elementRanges)
        {
            _start = start;
            _end = end;
            _ranges = elementRanges;
        }

        public bool IsEmpty
        {
            get
            {
                if (_ranges.Length == 1)
                {
                    return _ranges[0].Length == 0;
                }
                return false;
            }
        }

        public int Start
        {
            get { return _start; }
        }

        public int End
        {
            get { return _end; }
        }

        public Range[] ElementRanges
        {
            get { return _ranges; }
        }
    }
}