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
using System.Linq;

using Rant.Core.Compiler.Syntax;

namespace Rant.Core.Constructs
{
	internal class BlockManager
	{
		private readonly Dictionary<BlockAttribs, List<RstBlock>> _blockList;
		private BlockAttribs _prevAttribs;
		private int _prevCount = 0;

		public BlockManager()
		{
			_blockList = new Dictionary<BlockAttribs, List<RstBlock>>();
		}

		public BlockAttribs SetPrevAttribs(BlockAttribs attribs)
		{
			_prevAttribs = attribs;
			return attribs;
		}

		public BlockAttribs GetPrevious(int repeatMaximum)
		{
			if (repeatMaximum < 0) return _prevAttribs;

			if (_prevCount < repeatMaximum - 1)
			{
				_prevCount += 1;
				return _prevAttribs;
			}
			_prevCount = 0;
			return new BlockAttribs();
		}

		public void Add(BlockAttribs attribs, RstBlock block)
		{
			if (!_blockList.ContainsKey(attribs))
				_blockList.Add(attribs, new List<RstBlock> { block });
			else
				_blockList[attribs].Add(block);
		}

		public void RemoveAttribs(BlockAttribs attribs)
		{
			if (!_blockList.ContainsKey(attribs))
				throw new InvalidOperationException("Attribs don't exist");

			_blockList.Remove(attribs);
		}

		public void RemoveBlock(BlockAttribs attribs, RstBlock block)
		{
			if (!_blockList.ContainsKey(attribs))
				throw new InvalidOperationException("Attribs don't exist");

			if (!_blockList[attribs].Contains(block))
				throw new InvalidOperationException("Block doesn't exist");

			_blockList[attribs].Remove(block);
		}

		public BlockAttribs GetAttribs(RstBlock block) => _blockList.Single(p => p.Value.Contains(block)).Key;
	}
}