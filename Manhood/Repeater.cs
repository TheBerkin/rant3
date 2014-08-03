namespace Manhood
{
    internal class Repeater
    {
        private readonly int _reps;
        private int _index;

        public Repeater(int reps)
        {
            _reps = reps;
        }

        public int TotalReps
        {
            get { return _reps; }
        }

        public int Remaining
        {
            get { return _reps - _index - 1; }
        }

        public int CurrentRepIndex
        {
            get { return _index; }
        }

        public int CurrentRepNumber
        {
            get { return _index + 1; }
        }

        public bool IsFirst
        {
            get { return _index == 0; }
        }

        public bool IsLast
        {
            get { return _index == _reps - 1; }
        }

        public bool IsEven
        {
            get { return _index % 2 != 0; }
        }

        public bool IsOdd
        {
            get { return _index % 2 == 0; }
        }

        public bool Step()
        {
            return ++_index < _reps;
        }
    }
}