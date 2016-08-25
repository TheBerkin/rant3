using System.Collections.Generic;
using System.Linq;

using Rant.Core.Constructs;
using Rant.Core.IO;
using Rant.Core.Stringes;

using static Rant.Localization.Txtres;

namespace Rant.Core.Compiler.Syntax
{
	/// <summary>
	/// Represents a block construct, which provides multiple options to the interpreter for the next sequence, one of which is
	/// chosen.
	/// </summary>
	[RST("bloc")]
	internal class RstBlock : RST
	{
		private double _constantWeightSum;
		private int _count;
		private List<_<int, RST>> _dynamicWeights = null;
		private List<RST> _elements = new List<RST>();
		private bool _weighted = false;
		// Item weights.
		// Dynamic weights are patterns that must be run to get the weight value.
		// Constant weights can be used directly. This is used for optimization.
		// TODO: Move _weights to local scope for thread safety
		private double[] _weights = null;

		public RstBlock(Stringe location, params RST[] items)
			: base(location)
		{
			_elements.AddRange(items);
			_count = items.Length;
		}

		public RstBlock(Stringe location, List<RST> items)
			: base(location)
		{
			_elements.AddRange(items);
			_count = items.Count;
		}

		public RstBlock(Stringe location, List<RST> items,
			List<_<int, RST>> dynamicWeights, List<_<int, double>> constantWeights)
			: base(location)
		{
			_elements.AddRange(items);
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

		public override IEnumerator<RST> Run(Sandbox sb)
		{
			var attribs = sb.NextAttribs(this);

			// Skip if chance doesn't fall within range
			if (attribs.Chance < 100 && sb.RNG.NextDouble(0, 100) > attribs.Chance)
			{
				yield break;
			}

			int next = -1;
			int reps = attribs.RepEach ? _elements.Count : attribs.Repetitions;
			var block = new BlockState(attribs.Repetitions);
			double weightSum = _constantWeightSum;

			if (attribs.Start != null) yield return attribs.Start;

			if (_weighted && attribs.Sync == null)
			{
				foreach (var dw in _dynamicWeights)
				{
					sb.AddOutputWriter();
					yield return dw.Item2;
					string strWeight = sb.Return().Main;
					if (!double.TryParse(strWeight, out _weights[dw.Item1]))
						throw new RantRuntimeException(sb.Pattern, dw.Item2.Location,
							GetString("err-runtime-invalid-dynamic-weight", strWeight));

					weightSum += _weights[dw.Item1];
				}
			}

			if (attribs.Sync?.Index == 0 && attribs.StartIndex >= 0)
				attribs.Sync.Index = attribs.StartIndex;

			sb.Blocks.Push(block);
			for (int i = 0; i < reps; i++)
			{
				if (i == 0 && attribs.StartIndex >= 0 && attribs.Sync == null)
				{
					next = attribs.StartIndex > _count ? _count - 1 : attribs.StartIndex;
				}
				else if (_weighted)
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

				block.Next(next); // Set next block index

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
				yield return _elements[next];
				sb.Objects.ExitScope();

				// Affix
				if (attribs.After != null) yield return attribs.After;
			}
			sb.Blocks.Pop();

			if (attribs.End != null) yield return attribs.End;
		}

		protected override IEnumerator<RST> Serialize(EasyWriter output)
		{
			output.Write(_count);
			output.Write(_weighted);
			if (_weighted)
			{
				// Write constant weights
				if (_weights != null)
				{
					output.Write(true);
					output.Write(_constantWeightSum);
					for (int i = 0; i < _count; i++)
					{
						output.Write(_weights[i]);
					}
				}
				else
				{
					output.Write(false);
				}

				// Write dynamic weights
				if (_dynamicWeights != null)
				{
					output.Write(_dynamicWeights.Count);
					foreach (var dw in _dynamicWeights)
					{
						output.Write(dw.Item1);
						yield return dw.Item2;
					}
				}
				else
				{
					output.Write(0);
				}
			}

			// Block elements
			foreach (var e in _elements) yield return e;
		}

		protected override IEnumerator<DeserializeRequest> Deserialize(EasyReader input)
		{
			input.ReadInt32(out _count);
			input.ReadBoolean(out _weighted);
			if (_weighted)
			{
				// Read constant weights
				if (input.ReadBoolean())
				{
					input.ReadDouble(out _constantWeightSum);
					_weights = new double[_count];
					for (int i = 0; i < _count; i++)
					{
						input.ReadDouble(out _weights[i]);
					}
				}

				// Read dynamic weights
				int numDW = input.ReadInt32();
				if (numDW > 0)
				{
					_dynamicWeights = new List<_<int, RST>>(numDW);
					for (int i = 0; i < numDW; i++)
					{
						int index = input.ReadInt32();
						var request = new DeserializeRequest(input.ReadUInt32());
						yield return request;
						_dynamicWeights.Add(new _<int, RST>(index, request.Result));
					}
				}
			}

			// Read elements
			_elements = new List<RST>(_count);
			for (int i = 0; i < _count; i++)
			{
				var request = new DeserializeRequest(input.ReadUInt32());
				yield return request;
				_elements.Add(request.Result);
			}
		}
	}
}