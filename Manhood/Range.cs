using System;

namespace Manhood
{
    internal struct Range
    {
        private int _start, _length;

        public Range(int start, int length)
        {
            if (length < 0)
            {
                throw new ArgumentException("Length cannot be less than zero.");
            }
            if (start < 0)
            {
                throw new ArgumentException("Start index cannot be less than zero.");
            }
            _start = start;
            _length = length;
        }

        public int Start
        {
            get { return _start; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Start index cannot be less than zero.");
                }
                _start = value;
            }
        }

        public int Length
        {
            get { return _length; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Length cannot be less than zero.");
                }
                _length = value;
            }
        }

        public int End
        {
            get { return _start + _length; }
            set
            {
                if (value < _start)
                {
                    throw new ArgumentException("End index cannot be less than start index.");
                }
                _length = value - _start;
            }
        }
    }
}