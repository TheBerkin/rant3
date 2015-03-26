using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Engine.Compiler;
using Rant.Stringes;

namespace Rant.Engine
{
    internal class Block
    {
        private readonly _<double, IEnumerable<Token<R>>>[] _items;

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
                        return _.Create(weight, item.Skip(1).SkipWhile(token => token.ID == R.Whitespace));
                    }

                    return _.Create(weight, item.SkipWhile(token => token.ID == R.Whitespace));
                }
                return _.Create(weight, item);
            }).ToArray();

            WeightTotal = _items.Sum(t => t.Item1);
        }

        public static Block Create(IEnumerable<IEnumerable<Token<R>>> items)
        {
            return new Block(items);
        }

        public _<double, IEnumerable<Token<R>>>[] Items => _items;
    }
}
