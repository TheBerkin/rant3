using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Rant.Core.IO
{
    /// <summary>
    /// Provides comprehensive binary writing functionality including support for writing arrays and enumeration members.
    /// </summary>
    internal class EasyWriter : IDisposable
    {
        private const byte TrueByte = 1;
        private const byte FalseByte = 0;

        private readonly Stream _stream;
        private readonly bool _leaveOpen;
        private Endian _endian;

        /// <summary>
        /// Creates a new instance of the Rant.IO.EasyWriter class from the specified stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public EasyWriter(Stream stream)
        {
            _stream = stream;
            _endian = Endian.Little;
            _leaveOpen = false;
        }

        /// <summary>
        /// Creates a new instance of the Rant.IO.EasyWriter class from the specified stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="endianness">The endianness in which to write data.</param>
        /// <param name="leaveOpen">Specifies whether or not to leave the stream open after the writer is disposed.</param>
        public EasyWriter(Stream stream, Endian endianness = Endian.Little, bool leaveOpen = false)
        {
            _stream = stream;
            _endian = endianness;
            _leaveOpen = leaveOpen;
        }

        /// <summary>
        /// Creates a new instance of the Rant.IO.EasyWriter class from the specified file path and mode.
        /// </summary>
        /// <param name="path">The path to the file to write.</param>
        /// <param name="endianness">The endianness in which to write data.</param>
        /// <param name="mode">Specifies how the operating system should open the file.</param>
        public EasyWriter(string path, FileMode mode = FileMode.Create, Endian endianness = Endian.Little)
        {
            _stream = File.Open(path, mode);
            _endian = endianness;
            _leaveOpen = false;            
        }

        /// <summary>
        /// The underlying stream for this instance.
        /// </summary>
        public Stream BaseStream => _stream;

        /// <summary>
        /// Gets or sets the endianness in which data is written.
        /// </summary>
        public Endian Endianness
        {
            get { return _endian; }
            set { _endian = value; }            
        }

        /// <summary>
        /// The current writing position of the stream.
        /// </summary>
        public long Position
        {
            get { return _stream.Position; }
            set { _stream.Position = value; }
        }

        /// <summary>
        /// The current length of the stream.
        /// </summary>
        public long Length
        {
            get { return _stream.Length; }
            set { _stream.SetLength(value); }
        }

        /// <summary>
        /// Writes a byte to the stream.
        /// </summary>
        /// <param name="value">The byte to write.</param>
        public EasyWriter Write(byte value)
        {
            _stream.WriteByte(value);
            return this;
        }

        /// <summary>
        /// Writes an series of bytes to the stream.
        /// </summary>
        /// <param name="value">The byte array to write.</param>
        /// <returns></returns>
        public EasyWriter WriteBytes(byte[] value)
        {
            _stream.Write(value, 0, value.Length);
            return this;
        }

        public EasyWriter Write(bool value)
        {
            _stream.Write(new[] {value ? TrueByte : FalseByte}, 0, 1);
            return this;
        }

        /// <summary>
        /// Writes a signed byte to the stream.
        /// </summary>
        /// <param name="value">The signed byte to write.</param>
        public EasyWriter Write(sbyte value)
        {
            _stream.Write(BitConverter.GetBytes(value), 0, 1);
            return this;
        }

        /// <summary>
        /// Writes a 16-bit unsigned integer to the stream.
        /// </summary>
        /// <param name="value">The 16-bit unsigned integer to write.</param>
        public EasyWriter Write(ushort value)
        {
            byte[] data = BitConverter.GetBytes(value);
            IOUtil.ConvertEndian(data, _endian);
            _stream.Write(data, 0, 2);
            return this;
        }

        /// <summary>
        /// Writes a 16-bit signed integer to the stream.
        /// </summary>
        /// <param name="value">The 16-bit signed integer to write.</param>
        public EasyWriter Write(short value)
        {
            byte[] data = BitConverter.GetBytes(value);
            IOUtil.ConvertEndian(data, _endian);
            _stream.Write(data, 0, 2);
            return this;
        }

        /// <summary>
        /// Writes a 32-bit unsigned integer to the stream.
        /// </summary>
        /// <param name="value">The 32-bit unsigned integer to write.</param>
        public EasyWriter Write(uint value)
        {
            byte[] data = BitConverter.GetBytes(value);
            IOUtil.ConvertEndian(data, _endian);
            _stream.Write(data, 0, 4);
            return this;
        }

        /// <summary>
        /// Writes a 32-bit signed integer to the stream.
        /// </summary>
        /// <param name="value">The 32-bit signed integer to write.</param>
        public EasyWriter Write(int value)
        {
            byte[] data = BitConverter.GetBytes(value);
            IOUtil.ConvertEndian(data, _endian);
            _stream.Write(data, 0, 4);
            return this;
        }

        /// <summary>
        /// Writes a 64-bit unsigned integer to the stream.
        /// </summary>
        /// <param name="value">The 64-bit unsigned integer to write.</param>
        public EasyWriter Write(ulong value)
        {
            byte[] data = BitConverter.GetBytes(value);
            IOUtil.ConvertEndian(data, _endian);
            _stream.Write(data, 0, 8);
            return this;
        }

        /// <summary>
        /// Writes a 64-bit signed integer to the stream.
        /// </summary>
        /// <param name="value">The 64-bit signed integer to write.</param>
        public EasyWriter Write(long value)
        {
            byte[] data = BitConverter.GetBytes(value);
            IOUtil.ConvertEndian(data, _endian);
            _stream.Write(data, 0, 8);
            return this;
        }

        /// <summary>
        /// Writes a single-precision floating-point number to the stream.
        /// </summary>
        /// <param name="value">The single-precision floating-point number to write.</param>
        public EasyWriter Write(float value)
        {
            byte[] data = BitConverter.GetBytes(value);
            IOUtil.ConvertEndian(data, _endian);
            _stream.Write(data, 0, 4);
            return this;
        }

        /// <summary>
        /// Writes a double-precision floating-point number to the stream.
        /// </summary>
        /// <param name="value">The double-precision floating-point number to write.</param>
        public EasyWriter Write(double value)
        {
            byte[] data = BitConverter.GetBytes(value);
            IOUtil.ConvertEndian(data, _endian);
            _stream.Write(data, 0, 8);
            return this;
        }

        /// <summary>
        /// Writes a 128-bit decimal number to the stream.
        /// </summary>
        /// <param name="value">The 128-bit decimal number to write.</param>
        public EasyWriter Write(decimal value)
        {
            Write<decimal>(value);
            return this;
        }

        /// <summary>
        /// Writes a Unicode string to the stream.
        /// </summary>
        /// <param name="value">The Unicode string to write.</param>
        /// <param name="nullTerminated">Whether or not the string should be written as a C-string.</param>
        public EasyWriter Write(string value, bool nullTerminated = false)
        {
            var bytes = Encoding.Unicode.GetBytes(value);
            if(!nullTerminated)
                Write(bytes.Length);
            _stream.Write(bytes, 0, bytes.Length);
            if (nullTerminated)
                _stream.WriteByte(0);
            return this;
        }

        /// <summary>
        /// Writes a string of the specified encoding to the stream.
        /// </summary>
        /// <param name="value">The string to write to the stream.</param>
        /// <param name="encoding">The encoding to write the string in.</param>
        public EasyWriter Write(string value, Encoding encoding, bool nullTerminated = false)
        {
            var bytes = encoding.GetBytes(value);
            if (!nullTerminated)
                Write(bytes.Length);
            _stream.Write(bytes, 0, bytes.Length);
            if (nullTerminated)
                _stream.WriteByte(0);
            return this;
        }

        /// <summary>
        /// Writes a Unicode string array to the stream.
        /// </summary>
        /// <param name="value">The Unicode string array to write.</param>
        public EasyWriter Write(string[] value)
        {
            int count = value.Length;
            Write(count);
            foreach(var str in value)
            {
                Write(str);
            }
            return this;
        }

        /// <summary>
        /// Writes a string array of the specified encoding to the stream.
        /// </summary>
        /// <param name="value">The string array to write.</param>
        /// <param name="encoding">The encoding to write the strings in.</param>
        public EasyWriter Write(string[] value, Encoding encoding)
        {
            int count = value.Length;
            Write(count);
            foreach(string str in value)
            {
                Write(str, encoding);
            }
            return this;
        }

        /// <summary>
        /// Writes an array of values to the stream.
        /// </summary>
        /// <typeparam name="T">The type of value stored in the array.</typeparam>
        /// <param name="array">The array to write.</param>
        /// <param name="prefixLength">Indices to the writer if the array length should be prefixed to the data.</param>
        /// <param name="use64bit">Indicates to the writer that the array length is 64-bit rather than 32-bit.</param>
        public EasyWriter WriteArray<T>(T[] array, bool prefixLength = false, bool use64bit = false) where T : struct
        {
            bool isNumeric = IOUtil.IsNumericType(typeof(T));
            if (prefixLength)
            {
                if (use64bit)
                {
                    Write(array.LongLength);
                }
                else
                {
                    Write(array.Length);
                }
            }

            foreach(T item in array)
            {
                Write<T>(item, isNumeric);
            }
            return this;
        }

        /// <summary>
        /// Writes the specified byte array to the stream.
        /// </summary>
        /// <param name="value">The byte array to write.</param>
        public EasyWriter Write(byte[] value)
        {
            _stream.Write(value, 0, value.Length);
            return this;
        }

        /// <summary>
        /// Writes a dictionary of the specified key and value types to the stream.
        /// </summary>
        /// <typeparam name="TKey">The key type of the dictionary.</typeparam>
        /// <typeparam name="TValue">The value type of the dictionary.</typeparam>
        /// <param name="value">The dictionary to write.</param>
        public EasyWriter Write<TKey, TValue>(Dictionary<TKey, TValue> value)
        {
            var ktype = typeof(TKey);
            bool kIsString = ktype == typeof(String);
            var vtype = typeof(TValue);
            bool vIsString = vtype == typeof(String);

            if (!ktype.IsValueType && !kIsString)
            {
                throw new ArgumentException("TKey must be either a value type or System.String.");
            }
            else if (!vtype.IsValueType && !vIsString)
            {
                throw new ArgumentException("TValue must be either a value type or System.String.");
            }

            Write(value.Count);

            bool isKNumeric = IOUtil.IsNumericType(typeof(TKey));
            bool isVNumeric = IOUtil.IsNumericType(typeof(TValue));
            
            foreach(KeyValuePair<TKey, TValue> pair in value)
            {
                if (kIsString)
                {
                    Write(pair.Key.ToString());
                }
                else
                {                    
                    Write<TKey>(pair.Key, isKNumeric);
                }
                
                if (vIsString)
                {
                    Write(pair.Value.ToString());
                }
                else
                {
                    Write<TValue>(pair.Value, isVNumeric);
                }                
            }
            return this;
        }

        /// <summary>
        /// Writes a struct or enumeration member to the stream.
        /// </summary>
        /// <typeparam name="TStruct">The type of the struct or enum.</typeparam>
        /// <param name="value">The object to write.</param>
        /// <param name="convertEndian">Indicates to the writer if endianness attributes should be regarded.</param>
        public EasyWriter Write<TStruct>(TStruct value, bool convertEndian = true)
        {
            if (!typeof(TStruct).IsValueType)
            {
                throw new ArgumentException("TStruct must be a value type.");
            }

            var type = typeof(TStruct);
            int size = type.IsEnum ? Marshal.SizeOf(Enum.GetUnderlyingType(type)) : Marshal.SizeOf(value);
            byte[] data = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            
            if (type.IsEnum)
            {
                object i = Convert.ChangeType(value, Enum.GetUnderlyingType(type));
                Marshal.StructureToPtr(i, ptr, false); 
            }
            else if (convertEndian)
            {
                TStruct i = value;
                IOUtil.ConvertStructEndians<TStruct>(ref i);
                Marshal.StructureToPtr(i, ptr, false);     
            }
            
            Marshal.Copy(ptr, data, 0, size);   

            if (convertEndian && (IOUtil.IsNumericType(type) || type.IsEnum))
            {
                IOUtil.ConvertEndian(data, _endian);
            }

            Marshal.FreeHGlobal(ptr);
            _stream.Write(data, 0, size);
            return this;
        }

        /// <summary>
        /// Write a bit field to the stream.
        /// </summary>
        /// <param name="value">The bit field to write.</param>
        /// <returns></returns>
        public EasyWriter Write(BitField value)
        {
            _stream.Write(value._field, 0, value._field.Length);
            return this;
        }

        /// <summary>
        /// Writes a nullable value to the stream.
        /// </summary>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        /// <param name="value">The nullable value to write.</param>
        public EasyWriter Write<T>(T? value)
            where T : struct
        {
            bool hasValue = value.HasValue;
            Write(hasValue);
            if (hasValue)
            {
                Write(value.Value);
            }
            return this;
        }

        /// <summary>
        /// Closes the writer and the underlying stream.
        /// </summary>
        public void Close()
        {
            _stream.Close();
        }

        /// <summary>
        /// Releases all resources used by the current instance of the Rant.IO.EasyWriter class.
        /// </summary>
        public void Dispose()
        {
            if (!_leaveOpen)
            {
                _stream.Dispose();
            }
        }
    }
}
