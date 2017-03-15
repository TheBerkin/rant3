#region License

// https://github.com/TheBerkin/Rant
// 
// Copyright (c) 2017 Nicholas Fleck
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System.Collections.Generic;
using System.Text;

namespace Rant.Core.Output
{
    /// <summary>
    /// Specially designed linked list for storing targets and output buffers, with support for change events for
    /// auto-formatting functionality.
    /// </summary>
    internal class OutputChain
    {
        // Engine
        private readonly Sandbox sandbox;
        // Targets
        private readonly Dictionary<object, OutputChainBuffer> targets = new Dictionary<object, OutputChainBuffer>();

        public OutputChain(Sandbox sb, string name)
        {
            sandbox = sb;
            First = new OutputChainBuffer(sb, null);
            Last = First;
            Name = name;
        }

        // Buffer endpoint references

        // Public
        public OutputChainBuffer First { get; private set; }
        public OutputChainBuffer Last { get; private set; }
        public ChannelVisibility Visibility { get; set; } = ChannelVisibility.Public;
        public string Name { get; }

        public OutputChainBuffer AddBuffer()
        {
            return Last = new OutputChainBuffer(sandbox, Last);
        }

		public void AddBufferBefore(int bufferInvIndex, string value)
		{
			OutputChainBuffer bufReference = Last;
			for(int i = 0; i < bufferInvIndex; i++)
			{
				bufReference = bufReference.Prev;
			}
			var bufInsert = new OutputChainBuffer(sandbox, bufReference.Prev);
			if (bufInsert.Next == First) First = bufInsert;
			bufInsert.Print(value);
		}

        public void InsertTarget(object targetName)
        {
			// Check if the buffer was already created
			if (!targets.TryGetValue(targetName, out OutputChainBuffer buffer))
				buffer = targets[targetName] = AddBuffer();
			else
				Last = new OutputChainBuffer(sandbox, Last, buffer);

			// Then add an empty buffer after it so we don't start printing onto the target.
			AddBuffer();
        }

        public void PrintToTarget(object targetName, object value)
        {
			if (!targets.TryGetValue(targetName, out OutputChainBuffer buffer))
				buffer = targets[targetName] = new OutputChainBuffer(sandbox, null);

			buffer.Print(value);
        }

        public void ClearTarget(object targetName)
        {
            OutputChainBuffer buffer;
            if (targets.TryGetValue(targetName, out buffer))
                buffer.Clear();
        }

        public string GetTargetValue(string targetName)
        {
            OutputChainBuffer buffer;
            return targets.TryGetValue(targetName, out buffer) ? buffer.ToString() : string.Empty;
        }

        public void Print(string value)
        {
            if (Last.GetType() != typeof(OutputChainBuffer)) AddBuffer();
            Last.Print(value);
        }

        public void Print(object obj)
        {
            if (Last.GetType() != typeof(OutputChainBuffer)) AddBuffer();
            Last.Print(obj);
        }

        public OutputChainBuffer AddArticleBuffer()
        {
            // If the last buffer is empty, just replace it.
            var b = Last = new OutputChainArticleBuffer(sandbox, Last);
            return b;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(256);
            var buffer = First;
            while (buffer != null)
            {
                sb.Append(buffer);
                buffer = buffer.Next;
            }
            return sb.ToString();
        }
    }
}