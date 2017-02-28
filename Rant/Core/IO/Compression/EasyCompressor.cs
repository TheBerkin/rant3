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
using System.IO;

using Rant.Core.IO.Compression.LZMA;

namespace Rant.Core.IO.Compression
{
	internal static class EasyCompressor
	{
		internal static event ProgressUpdateEvent ProgressUpdate;

		public static byte[] Compress(byte[] data)
		{
			var inStream = new MemoryStream(data);
			var stream = new MemoryStream();

			var enc = new Encoder();

			enc.WriteCoderProperties(stream);
			EasyCompressorProgress progress = null;
			if (ProgressUpdate != null)
				progress = new EasyCompressorProgress(ProgressUpdate);
			long dataSize = data.Length;
			for (int i = 0; i < 8; i++)
				stream.WriteByte((byte)(dataSize >> (8 * i)));
			enc.Code(inStream, stream, data.Length, -1, progress);
			inStream.Close();
			stream.Close();
			return stream.ToArray();
		}

		public static byte[] Decompress(byte[] data)
		{
			var inStream = new MemoryStream(data);
			var outStream = new MemoryStream();

			var dec = new Decoder();
			EasyCompressorProgress progress = null;
			if (ProgressUpdate != null)
				progress = new EasyCompressorProgress(ProgressUpdate);
			var props = new byte[5];
			inStream.Read(props, 0, 5);
			dec.SetDecoderProperties(props);
			long outSize = 0;
			for (int i = 0; i < 8; i++)
				outSize |= (long)(byte)inStream.ReadByte() << (8 * i);
			long compressedSize = inStream.Length - inStream.Position;
			dec.Code(inStream, outStream, compressedSize, outSize, progress);
			inStream.Close();
			outStream.Close();
			return outStream.ToArray();
		}

		internal delegate void ProgressUpdateEvent(object sender, CompressionProgressEventArgs e);
	}

	internal class EasyCompressorProgress : ICodeProgress
	{
		private readonly EasyCompressor.ProgressUpdateEvent _handler;

		internal EasyCompressorProgress(EasyCompressor.ProgressUpdateEvent handler)
		{
			_handler = handler;
		}

		public void SetProgress(long inSize, long outSize)
		{
			_handler.Invoke(null, new CompressionProgressEventArgs { Progress = inSize / outSize });
		}
	}

	internal class CompressionProgressEventArgs : EventArgs
	{
		public double Progress;
	}
}