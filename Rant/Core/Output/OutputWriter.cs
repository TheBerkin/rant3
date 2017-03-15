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

using System;
using System.Collections.Generic;

using Rant.Core.Formatting;

namespace Rant.Core.Output
{
    internal class OutputWriter
    {
        private const string MainChannelName = "main";
        private readonly HashSet<OutputChain> activeChains = new HashSet<OutputChain>();
        private readonly Dictionary<string, OutputChain> chains = new Dictionary<string, OutputChain>();
        private readonly Stack<OutputChain> chainStack = new Stack<OutputChain>();
        private readonly OutputChain mainChain;
        private readonly Sandbox sandbox;

        public OutputWriter(Sandbox sb)
        {
            sandbox = sb;
            mainChain = chains[MainChannelName] = new OutputChain(sb, MainChannelName);
            chainStack.Push(mainChain);
            activeChains.Add(mainChain);
        }

        public bool CloseChannel()
        {
            if (chainStack.Peek() == mainChain) return false;
            activeChains.Remove(chainStack.Pop());
            return true;
        }

        public void OpenChannel(string name, ChannelVisibility visibility)
        {
            OutputChain chain;
            if (!chains.TryGetValue(name, out chain))
                chain = chains[name] = new OutputChain(sandbox, name);
            else if (activeChains.Contains(chain))
                return;
            chain.Visibility = visibility;
            chainStack.Push(chain);
            activeChains.Add(chain);
        }

        public void Do(Action<OutputChain> chainAction)
        {
            bool fInternal = false;
            foreach (var chain in chainStack)
            {
                if (fInternal && chain == mainChain) return;
                chainAction(chain);
                switch (chain.Visibility)
                {
                    case ChannelVisibility.Public:
                        if (fInternal) return;
                        if (chain != mainChain)
                            chainAction(mainChain);
                        return;
                    case ChannelVisibility.Private:
                        return;
                    case ChannelVisibility.Internal:
                        fInternal = true;
                        break;
                }
            }
        }

        public int GetChannelLength(string channelName)
        {
			if (!chains.TryGetValue(channelName, out OutputChain c)) return 0;
			var buffer = c.First;			
            int length = 0;
            do
            {
                length += buffer.Length;
            } while ((buffer = buffer.Next) != null);
            return length;
        }

        public void Capitalize(Capitalization caps) => Do(chain => chain.Last.Caps = caps);
        public void Print(string value) => Do(chain => chain.Print(value));
        public void Print(object obj) => Do(chain => chain.Print(obj));
        public void InsertTarget(object targetName) => Do(chain => chain.InsertTarget(targetName));
        public void PrintToTarget(object targetName, string value) => Do(chain => chain.PrintToTarget(targetName, value));
		public object InsertAnonymousTarget()
		{
			var obj = new object();
			Do(chain => chain.InsertTarget(obj));
			return obj;
		}
		public void BackPrint(int invIndex, string value)
		{
			Do(chain => chain.AddBufferBefore(invIndex, value));
		}
        public RantOutput ToRantOutput() => new RantOutput(sandbox.RNG.Seed, sandbox.StartingGen, chains.Values);
    }
}