using System;
using System.IO;

namespace Rant.IO.Compression
{
    internal static class EasyCompressor
    {
        internal delegate void ProgressUpdateEvent(object sender, CompressionProgressEventArgs e);
        internal static event ProgressUpdateEvent ProgressUpdate;

        public static byte[] Compress(byte[] data)
        {
            var inStream = new MemoryStream(data);
            var stream = new MemoryStream();

            var enc = new LZMA.Encoder();

            enc.WriteCoderProperties(stream);
            EasyCompressorProgress progress = null;
            if(ProgressUpdate != null)
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
            
            var dec = new LZMA.Decoder();
            EasyCompressorProgress progress = null;
            if (ProgressUpdate != null)
                progress = new EasyCompressorProgress(ProgressUpdate);
            byte[] props = new byte[5];
            inStream.Read(props, 0, 5);
            dec.SetDecoderProperties(props);
            long outSize = 0;
            for (int i = 0; i < 8; i++)
                outSize |= ((long)(byte)inStream.ReadByte()) << (8 * i);
            long compressedSize = inStream.Length - inStream.Position;
            dec.Code(inStream, outStream, compressedSize, outSize, progress);
            inStream.Close();
            outStream.Close();
            return outStream.ToArray();
        }
    }

    internal class EasyCompressorProgress : ICodeProgress
    {
        private EasyCompressor.ProgressUpdateEvent _handler;

        internal EasyCompressorProgress(EasyCompressor.ProgressUpdateEvent handler)
        {
            _handler = handler;
        }

        public void SetProgress(long inSize, long outSize)
        {
            _handler.Invoke(null, new CompressionProgressEventArgs() { Progress = inSize / outSize });
        }
    }

    internal class CompressionProgressEventArgs : EventArgs
    {
        public double Progress;
    }
}
