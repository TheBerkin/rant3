using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Compiler;
using Rant.Stringes.Tokens;

namespace Rant
{
    internal class Block
    {
        private readonly Tuple<double, IEnumerable<Token<R>>>[] _items;

        public readonly double WeightTotal;

        private Block(IEnumerable<IEnumerable<Token<R>>> items)
        {
            _items = items.Select(item =>
            {
                double weight = 1;
                if (item.Any())
                {
                    var first = item.First();
                    if (first.ID == R.Weight)
                    {
                        weight = Double.Parse(first.Value);
                        return Tuple.Create(weight, item.Skip(1).SkipWhile(token => token.ID == R.Whitespace));
                    }

                    return Tuple.Create(weight, item.SkipWhile(token => token.ID == R.Whitespace));
                }
                return Tuple.Create(weight, item);
            }).ToArray();

            WeightTotal = _items.Sum(t => t.Item1);
        }

        public static Block Create(IEnumerable<IEnumerable<Token<R>>> items)
        {
            return new Block(items);
        }

        public Tuple<double, IEnumerable<Token<R>>>[] Items => _items;
    }
}
