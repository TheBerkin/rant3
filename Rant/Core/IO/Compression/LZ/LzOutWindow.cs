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

using System.IO;

namespace Rant.Core.IO.Compression.LZ
{
	internal class OutWindow
	{
		private byte[] _buffer = null;
		private uint _pos;
		private Stream _stream;
		private uint _streamPos;
		private uint _windowSize = 0;
		public uint TrainSize = 0;

		public void Create(uint windowSize)
		{
			if (_windowSize != windowSize)
				_buffer = new byte[windowSize];
			_windowSize = windowSize;
			_pos = 0;
			_streamPos = 0;
		}

		public void Init(Stream stream, bool solid)
		{
			ReleaseStream();
			_stream = stream;
			if (!solid)
			{
				_streamPos = 0;
				_pos = 0;
				TrainSize = 0;
			}
		}

		public bool Train(Stream stream)
		{
			long len = stream.Length;
			uint size = len < _windowSize ? (uint)len : _windowSize;
			TrainSize = size;
			stream.Position = len - size;
			_streamPos = _pos = 0;
			while (size > 0)
			{
				uint curSize = _windowSize - _pos;
				if (size < curSize)
					curSize = size;
				int numReadBytes = stream.Read(_buffer, (int)_pos, (int)curSize);
				if (numReadBytes == 0)
					return false;
				size -= (uint)numReadBytes;
				_pos += (uint)numReadBytes;
				_streamPos += (uint)numReadBytes;
				if (_pos == _windowSize)
					_streamPos = _pos = 0;
			}
			return true;
		}

		public void ReleaseStream()
		{
			Flush();
			_stream = null;
		}

		public void Flush()
		{
			uint size = _pos - _streamPos;
			if (size == 0)
				return;
			_stream.Write(_buffer, (int)_streamPos, (int)size);
			if (_pos >= _windowSize)
				_pos = 0;
			_streamPos = _pos;
		}

		public void CopyBlock(uint distance, uint len)
		{
			uint pos = _pos - distance - 1;
			if (pos >= _windowSize)
				pos += _windowSize;
			for (; len > 0; len--)
			{
				if (pos >= _windowSize)
					pos = 0;
				_buffer[_pos++] = _buffer[pos++];
				if (_pos >= _windowSize)
					Flush();
			}
		}

		public void PutByte(byte b)
		{
			_buffer[_pos++] = b;
			if (_pos >= _windowSize)
				Flush();
		}

		public byte GetByte(uint distance)
		{
			uint pos = _pos - distance - 1;
			if (pos >= _windowSize)
				pos += _windowSize;
			return _buffer[pos];
		}
	}
}