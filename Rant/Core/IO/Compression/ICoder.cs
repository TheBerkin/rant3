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

namespace Rant.Core.IO.Compression
{
    /// <summary>
    /// The exception that is thrown when an error in input stream occurs during decoding.
    /// </summary>
    internal class DataErrorException : ApplicationException
    {
        public DataErrorException() : base("Data Error")
        {
        }
    }

    /// <summary>
    /// The exception that is thrown when the value of an argument is outside the allowable range.
    /// </summary>
    internal class InvalidParamException : ApplicationException
    {
        public InvalidParamException() : base("Invalid Parameter")
        {
        }
    }

    internal interface ICodeProgress
    {
        /// <summary>
        /// Callback progress.
        /// </summary>
        /// <param name="inSize">
        /// input size. -1 if unknown.
        /// </param>
        /// <param name="outSize">
        /// output size. -1 if unknown.
        /// </param>
        void SetProgress(long inSize, long outSize);
    }

    internal interface ICoder
    {
        /// <summary>
        /// Codes streams.
        /// </summary>
        /// <param name="inStream">
        /// input Stream.
        /// </param>
        /// <param name="outStream">
        /// output Stream.
        /// </param>
        /// <param name="inSize">
        /// input Size. -1 if unknown.
        /// </param>
        /// <param name="outSize">
        /// output Size. -1 if unknown.
        /// </param>
        /// <param name="progress">
        /// callback progress reference.
        /// </param>
        /// <exception cref="DataErrorException">
        /// if input stream is not valid
        /// </exception>
        void Code(Stream inStream, Stream outStream,
            long inSize, long outSize, ICodeProgress progress);
    }

    /*
    public interface ICoder2
    {
         void Code(ISequentialInStream []inStreams,
                const UInt64 []inSizes, 
                ISequentialOutStream []outStreams, 
                UInt64 []outSizes,
                ICodeProgress progress);
    };
  */

    /// <summary>
    /// Provides the fields that represent properties idenitifiers for compressing.
    /// </summary>
    internal enum CoderPropID
    {
        /// <summary>
        /// Specifies default property.
        /// </summary>
        DefaultProp = 0,

        /// <summary>
        /// Specifies size of dictionary.
        /// </summary>
        DictionarySize,

        /// <summary>
        /// Specifies size of memory for PPM*.
        /// </summary>
        UsedMemorySize,

        /// <summary>
        /// Specifies order for PPM methods.
        /// </summary>
        Order,

        /// <summary>
        /// Specifies Block Size.
        /// </summary>
        BlockSize,

        /// <summary>
        /// Specifies number of postion state bits for LZMA (0 &lt;= x &lt;= 4).
        /// </summary>
        PosStateBits,

        /// <summary>
        /// Specifies number of literal context bits for LZMA (0 &lt;= x &lt;= 8).
        /// </summary>
        LitContextBits,

        /// <summary>
        /// Specifies number of literal position bits for LZMA (0 &lt;= x &lt;= 4).
        /// </summary>
        LitPosBits,

        /// <summary>
        /// Specifies number of fast bytes for LZ*.
        /// </summary>
        NumFastBytes,

        /// <summary>
        /// Specifies match finder. LZMA: "BT2", "BT4" or "BT4B".
        /// </summary>
        MatchFinder,

        /// <summary>
        /// Specifies the number of match finder cyckes.
        /// </summary>
        MatchFinderCycles,

        /// <summary>
        /// Specifies number of passes.
        /// </summary>
        NumPasses,

        /// <summary>
        /// Specifies number of algorithm.
        /// </summary>
        Algorithm,

        /// <summary>
        /// Specifies the number of threads.
        /// </summary>
        NumThreads,

        /// <summary>
        /// Specifies mode with end marker.
        /// </summary>
        EndMarker
    }


    internal interface ISetCoderProperties
    {
        void SetCoderProperties(CoderPropID[] propIDs, object[] properties);
    }

    internal interface IWriteCoderProperties
    {
        void WriteCoderProperties(Stream outStream);
    }

    internal interface ISetDecoderProperties
    {
        void SetDecoderProperties(byte[] properties);
    }
}