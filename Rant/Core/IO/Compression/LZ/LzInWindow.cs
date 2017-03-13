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
    internal class InWindow
    {
        public uint _blockSize; // Size of Allocated memory block
        public byte[] _bufferBase = null; // pointer to buffer with data
        public uint _bufferOffset;
        private uint _keepSizeAfter; // how many BYTEs must be kept buffer after _pos
        private uint _keepSizeBefore; // how many BYTEs must be kept in buffer before _pos
        private uint _pointerToLastSafePosition;
        public uint _pos; // offset (from _buffer) of curent byte
        private uint _posLimit; // offset (from _buffer) of first byte when new block reading must be done
        private Stream _stream;
        private bool _streamEndWasReached; // if (true) then _streamPos shows real end of stream
        public uint _streamPos; // offset (from _buffer) of first not read byte from Stream

        public void MoveBlock()
        {
            uint offset = _bufferOffset + _pos - _keepSizeBefore;
            // we need one additional byte, since MovePos moves on 1 byte.
            if (offset > 0)
                offset--;

            uint numBytes = _bufferOffset + _streamPos - offset;

            // check negative offset ????
            for (uint i = 0; i < numBytes; i++)
                _bufferBase[i] = _bufferBase[offset + i];
            _bufferOffset -= offset;
        }

        public virtual void ReadBlock()
        {
            if (_streamEndWasReached)
                return;
            while (true)
            {
                int size = (int)(0 - _bufferOffset + _blockSize - _streamPos);
                if (size == 0)
                    return;
                int numReadBytes = _stream.Read(_bufferBase, (int)(_bufferOffset + _streamPos), size);
                if (numReadBytes == 0)
                {
                    _posLimit = _streamPos;
                    uint pointerToPostion = _bufferOffset + _posLimit;
                    if (pointerToPostion > _pointerToLastSafePosition)
                        _posLimit = _pointerToLastSafePosition - _bufferOffset;

                    _streamEndWasReached = true;
                    return;
                }
                _streamPos += (uint)numReadBytes;
                if (_streamPos >= _pos + _keepSizeAfter)
                    _posLimit = _streamPos - _keepSizeAfter;
            }
        }

        private void Free()
        {
            _bufferBase = null;
        }

        public void Create(uint keepSizeBefore, uint keepSizeAfter, uint keepSizeReserv)
        {
            _keepSizeBefore = keepSizeBefore;
            _keepSizeAfter = keepSizeAfter;
            uint blockSize = keepSizeBefore + keepSizeAfter + keepSizeReserv;
            if (_bufferBase == null || _blockSize != blockSize)
            {
                Free();
                _blockSize = blockSize;
                _bufferBase = new byte[_blockSize];
            }
            _pointerToLastSafePosition = _blockSize - keepSizeAfter;
        }

        public void SetStream(Stream stream)
        {
            _stream = stream;
        }

        public void ReleaseStream()
        {
            _stream = null;
        }

        public void Init()
        {
            _bufferOffset = 0;
            _pos = 0;
            _streamPos = 0;
            _streamEndWasReached = false;
            ReadBlock();
        }

        public void MovePos()
        {
            _pos++;
            if (_pos > _posLimit)
            {
                uint pointerToPostion = _bufferOffset + _pos;
                if (pointerToPostion > _pointerToLastSafePosition)
                    MoveBlock();
                ReadBlock();
            }
        }

        public byte GetIndexByte(int index)
        {
            return _bufferBase[_bufferOffset + _pos + index];
        }

        // index + limit have not to exceed _keepSizeAfter;
        public uint GetMatchLen(int index, uint distance, uint limit)
        {
            if (_streamEndWasReached)
            {
                if (_pos + index + limit > _streamPos)
                    limit = _streamPos - (uint)(_pos + index);
            }
            distance++;
            // Byte *pby = _buffer + (size_t)_pos + index;
            uint pby = _bufferOffset + _pos + (uint)index;

            uint i;
            for (i = 0; i < limit && _bufferBase[pby + i] == _bufferBase[pby + i - distance]; i++) ;
            return i;
        }

        public uint GetNumAvailableBytes()
        {
            return _streamPos - _pos;
        }

        public void ReduceOffsets(int subValue)
        {
            _bufferOffset += (uint)subValue;
            _posLimit -= (uint)subValue;
            _pos -= (uint)subValue;
            _streamPos -= (uint)subValue;
        }
    }
}