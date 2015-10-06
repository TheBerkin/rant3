using System;
using System.IO;

namespace Rant.IO.Compression
{
    internal static class EasyCompressor
    {
        public static byte[] Compress(byte[] data)
        {
            var inStream = new MemoryStream(data);
            var stream = new MemoryStream();

            var enc = new LZMA.Encoder();

            enc.WriteCoderProperties(stream);
            enc.Code(inStream, stream, data.Length, -1, null);
            stream.Close();
            return stream.ToArray();
        }
    }
}
