using System;

namespace Rant
{
    internal sealed class Limit<T>
    {
        private T _value;
        private readonly T _limit;
        private readonly Func<T, T, T> _accumulatorFunc;
        private readonly Func<T, T, bool> _checkFunc;

        public Limit(T startValue, T limit, Func<T, T, T> accumulator, Func<T, T, bool> limitChecker)
        {
            _value = startValue;
            _limit = limit;
            _accumulatorFunc = accumulator;
            _checkFunc = limitChecker;
        }

        public T LimitValue
        {
            get { return _limit; }
        }

        public T CurrentValue
        {
            get { return _value; }
        }

        public bool Accumulate(T value)
        {
            _value = _accumulatorFunc(_value, value);
            return _checkFunc(_value, _limit);
        }
    }
}