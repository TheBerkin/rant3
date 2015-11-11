using Rant.Stringes;
using System;
using System.Collections.Generic;
using System.Linq;

using Rant.Engine.Constructs;

namespace Rant.Engine.Syntax
{
	/// <summary>
	/// Represents a block construct, which provides multiple options to the interpreter for the next sequence, one of which is chosen.
	/// </summary>
	internal class RABlock : RantAction
	{
		private readonly List<RantAction> _items = new List<RantAction>();
		private readonly int _count;

		// Item weights.
		// Dynamic weights are patterns that must be run to get the weight value.
		// Constant weights can be used directly. This is used for optimization.
		// TODO: Move _weights to local scope for thread safety
		private readonly double[] _weights = null;
		private readonly List<_<int, RantAction>> _dynamicWeights = null;
		private readonly double _constantWeightSum;
		private readonly bool _weighted = false;

		public RABlock(Stringe range, params RantAction[] items)
			: base(range)
		{
			_items.AddRange(items);
			_count = items.Length;
		}

		public RABlock(Stringe range, List<RantAction> items)
			: base(range)
		{
			_items.AddRange(items);
			_count = items.Count;
		}

		public RABlock(Stringe range, List<RantAction> items,
			List<_<int, RantAction>> dynamicWeights, List<_<int, double>> constantWeights)
			: base(range)
		{
			_items.AddRange(items);
			_count = items.Count;
			if (dynamicWeights != null && constantWeights != null)
			{
				_dynamicWeights = dynamicWeights;
				_weights = new double[_count];
				for (int i = 0; i < _count; i++) _weights[i] = 1;
				foreach (var cw in constantWeights)
				{
					_weights[cw.Item1] = cw.Item2;
				}
				_constantWeightSum = _weights.Sum() - _dynamicWeights.Count;
				_weighted = true;
			}
		}

        public override IEnumerator<RantAction> Run(Sandbox sb)
		{
			var attribs = sb.NextAttribs(this);
			int next = -1;
			int reps = attribs.RepEach ? _items.Count : attribs.Repetitions;
			var block = new BlockState(attribs.Repetitions);
			double weightSum = _constantWeightSum;

			if (attribs.Start != null) yield return attribs.Start;

			if (_weighted && attribs.Sync == null)
			{
				foreach (var dw in _dynamicWeights)
				{
					sb.AddOutputWriter();
					yield return dw.Item2;
					var strWeight = sb.Return().Main;
					if (!Double.TryParse(strWeight, out _weights[dw.Item1]))
						throw new RantRuntimeException(sb.Pattern, dw.Item2.Range,
							$"Dynamic weight returned invalid weight value: '{strWeight}'");
					weightSum += _weights[dw.Item1];
				}
			}
	        
			sb.Blocks.Push(block);
			for (int i = 0; i < reps; i++)
			{
				if (_weighted)
				{
					double choice = sb.RNG.NextDouble(weightSum);
					for (int j = 0; j < _count; j++)
					{
						if (choice < _weights[j])
						{
							next = j;
							break;
						}
						choice -= _weights[j];
					}
				}
				else
				{
					next = attribs.NextIndex(_count, sb.RNG);
				}

				if (next == -1) break;
				block.Next(next);

				sb.Blocks.Pop(); // Don't allow separator to access block state
				// Separator
				if (i > 0 && attribs.Separator != null)
				{
					if (attribs.IsSeries)
					{
						// Check if we're on the last separator in a series
						if (i == reps - 1)
						{
							// Add the oxford comma if specified
							if (attribs.EndSeparator != null)
							{
								// If there are more than two items, print it!
								if (reps > 2) yield return attribs.EndSeparator;
							}

							sb.Print(sb.Format.StandardSpace);

							// Add conjunction if specified (it normally should be, if it's a series)
							if (attribs.EndConjunction != null)
							{
								yield return attribs.EndConjunction;
								sb.Print(sb.Format.StandardSpace);
							}
						}
						else if (reps > 2)
						{
							yield return attribs.Separator;
							sb.Print(sb.Format.StandardSpace);
						}
					}
					else
					{
						yield return attribs.Separator;
					}
				}
				sb.Blocks.Push(block); // Now put it back

				// Prefix
				if (attribs.Before != null) yield return attribs.Before;

				// Content
				sb.Objects.EnterScope();
				yield return _items[next];
				sb.Objects.ExitScope();

				// Affix
				if (attribs.After != null) yield return attribs.After;
			}
			sb.Blocks.Pop();

			if (attribs.End != null) yield return attribs.End;
		}
	}
}