using System;
using System.Text;

using Rant.Engine.Formatters;
using Rant.IO;

namespace Rant.Engine.Output
{
	internal class OutputChainBuffer
	{
		private const int InitialCapacity = 256;

		private readonly Sandbox sandbox;
		private readonly OutputChainBuffer _prevItem;
		private OutputChainBuffer _nextItem;
		private NumberFormatter _numberFormatter;
		protected readonly StringBuilder buffer;

		public OutputChainBuffer(Sandbox sb, OutputChainBuffer prev)
		{
			_prevItem = prev;
#if DEBUG
			if (prev._nextItem != null)
			{
				Console.WriteLine("WARNING: Duplicate output chain buffer created.");
			}
#endif
			prev._nextItem = this;
			buffer = new StringBuilder(InitialCapacity);
			sandbox = sb;
		}

		public OutputChainBuffer(Sandbox sb, OutputChainBuffer prev, OutputChainBuffer targetOrigin)
		{
			_prevItem = prev;
#if DEBUG
			if (prev._nextItem != null)
			{
				Console.WriteLine("WARNING: Duplicate output chain buffer created.");
			}
#endif
			prev._nextItem = this;
			buffer = targetOrigin.buffer;
			sandbox = sb;
		}

		public NumberFormatter NumberFormatter
		{
			get { return _numberFormatter; }
			set
			{
				if (value == null) throw new ArgumentNullException(nameof(value));
				_numberFormatter = value;
			}
		}

		public OutputChainBuffer Next => _nextItem;

		public OutputChainBuffer Prev => _prevItem;

		public int Length => buffer.Length;

		protected virtual void OnPrevBufferChange()
		{
		}

		protected virtual void OnNextBufferChange()
		{
		}

		public void Print(string value)
		{
			if (String.IsNullOrEmpty(value)) return;
			buffer.Append(value);
			_prevItem?.OnNextBufferChange();
			_nextItem?.OnPrevBufferChange();
		}

		public void Print(object value)
		{
			if (IOUtil.IsNumericType(value.GetType()))
			{
				buffer.Append(_numberFormatter.FormatNumber((double)value));
				return;
			}
			buffer.Append(value);
			_prevItem?.OnNextBufferChange();
			_nextItem?.OnPrevBufferChange();
		}

		public void Clear()
		{
#if UNITY
			_buffer.Length = 0;
#else
			buffer.Clear();
#endif
		}

		public override string ToString() => buffer.ToString();
	}
}