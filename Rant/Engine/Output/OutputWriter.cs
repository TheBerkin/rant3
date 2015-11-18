using System;
using System.Collections.Generic;

namespace Rant.Engine.Output
{
	internal class OutputWriter
	{
		private readonly Sandbox sandbox;
		private readonly OutputChain mainChain;
		private readonly Dictionary<string, OutputChain> chains = new Dictionary<string, OutputChain>(); 
		private readonly Stack<OutputChain> chainStack = new Stack<OutputChain>();
		private readonly HashSet<OutputChain> activeChains = new HashSet<OutputChain>();

		private const string MainChannelName = "main";

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
			{
				chain = chains[name] = new OutputChain(sandbox, name);
			}
			else if (activeChains.Contains(chain))
			{
				return;
			}
			chain.Visibility = visibility;
			chainStack.Push(chain);
			activeChains.Add(chain);
		}

		public void Do(Action<OutputChain> chainAction)
		{
			bool fInternal = false;
			foreach (var chain in chainStack)
			{
				chainAction(chain);
				switch (chain.Visibility)
				{
					case ChannelVisibility.Public:
						if (fInternal) return;
						if (chain != mainChain)
						{
							chainAction(mainChain);
						}
						return;
					case ChannelVisibility.Private:
						return;
					case ChannelVisibility.Internal:
						fInternal = true;
						break;
				}
			}
		}

		public void Print(string value) => Do(chain => chain.Print(value));

		public void Print(object obj) => Do(chain => chain.Print(obj));

		public RantOutput ToRantOutput() => new RantOutput(sandbox.RNG.Seed, sandbox.StartingGen, activeChains);
	}
}