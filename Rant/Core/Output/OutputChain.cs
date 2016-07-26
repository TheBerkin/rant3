using System;
using System.Collections.Generic;
using System.Text;

namespace Rant.Core.Output
{
	/// <summary>
	/// Specially designed linked list for storing targets and output buffers, with support for change events for auto-formatting functionality.
	/// </summary>
	internal class OutputChain
	{
		// Engine
		private readonly Sandbox sandbox;

		// Targets
		private readonly Dictionary<string, OutputChainBuffer> targets = new Dictionary<string, OutputChainBuffer>();

		// Buffer endpoint references
		private readonly OutputChainBuffer _first;
		private OutputChainBuffer _last;

		// Public
		public OutputChainBuffer First => _first;
		public OutputChainBuffer Last => _last;
		public ChannelVisibility Visibility { get; set; } = ChannelVisibility.Public;
		public string Name { get; }

		public OutputChain(Sandbox sb, string name)
		{
			sandbox = sb;
			_first = new OutputChainBuffer(sb, null);
			_last = _first;
			Name = name;
		}

		public OutputChainBuffer AddBuffer()
		{
			return _last = new OutputChainBuffer(sandbox, _last);
		}

		public void InsertTarget(string targetName)
		{
			// Check if the buffer was already created
			OutputChainBuffer buffer;
			if (!targets.TryGetValue(targetName, out buffer))
			{
				// If not, make a new one and insert it
				buffer = targets[targetName] = AddBuffer();
			}
			else
			{
				// If it does exist, just create a new instance of it with the same buffer and add it in.
				_last = new OutputChainBuffer(sandbox, _last, buffer);
			}

			// Then add an empty buffer after it so we don't start printing onto the target.
			AddBuffer();
		}

		public void PrintToTarget(string targetName, string value)
		{
			OutputChainBuffer buffer;
			if (!targets.TryGetValue(targetName, out buffer))
			{
				buffer = targets[targetName] = new OutputChainBuffer(sandbox, null);
			}

			buffer.Print(value);
		}

		public void ClearTarget(string targetName)
		{
			OutputChainBuffer buffer;
			if (targets.TryGetValue(targetName, out buffer))
			{
				buffer.Clear();
			}
		}

		public string GetTargetValue(string targetName)
		{
			OutputChainBuffer buffer;
			return targets.TryGetValue(targetName, out buffer) ? buffer.ToString() : String.Empty;
		}

		public void Print(string value)
		{
			if (_last.GetType() != typeof(OutputChainBuffer)) AddBuffer();
			_last.Print(value);
		}

		public void Print(object obj)
		{
			if (_last.GetType() != typeof(OutputChainBuffer)) AddBuffer();
			_last.Print(obj);
		}

		public OutputChainBuffer AddArticleBuffer()
		{
			// If the last buffer is empty, just replace it.
			var b = _last = new OutputChainArticleBuffer(sandbox, _last);
			return b;
		}

		public override string ToString()
		{
			var sb = new StringBuilder(256);
			var buffer = _first;
			while (buffer != null)
			{
				sb.Append(buffer);
				buffer = buffer.Next;
			}
			return sb.ToString();
		}
	}
}