using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rant.IO.Bson
{
    /// <summary>
    /// Represents a document encoded in BSON.
    /// </summary>
    public class BsonDocument
    {
        /// <summary>
        /// The top item of this BSON document.
        /// </summary>
        public BsonItem Top;

        /// <summary>
        /// Retreives the value of the given key if it exists.
        /// </summary>
        /// <param name="key">The key to retrieve.</param>
        /// <returns>The value of the given key, or null if it does not exist.</returns>
        public BsonItem this[string key]
        {
            get
            {
                if(Top.HasKey(key))
                    return Top[key];
                return null;
            }
            set
            {
                Top[key] = value;
            }
        }

        private Dictionary<string, int> _stringTable;
        private int _stringTableIndex = 0;
        private BsonStringTableMode _stringTableMode = BsonStringTableMode.None;

        /// <summary>
        /// Creates an empty BSON document.
        /// <param name="useStringTable">Whether or not to generate and use a string table.</param>
        /// </summary>
        public BsonDocument(BsonStringTableMode mode = BsonStringTableMode.None)
        {
            _stringTableMode = mode;
            _stringTable = new Dictionary<string, int>();
            Top = new BsonItem();
        }
        
        /// <summary>
        /// Returns this document encoded in BSON as a byte array.
        /// </summary>
        /// <returns>This document converted to BSON.</returns>
        public byte[] ToByteArray()
        {
            var stream = new MemoryStream();
            Write(stream);
            stream.Close();
            return stream.ToArray();
        }

        public byte[] GenerateStringTable()
        {
            var stream = new MemoryStream();
            var writer = new EasyWriter(stream);
            writer.Write(_stringTable.Count);
            foreach(string key in _stringTable.Keys)
            {
                writer.Write(_stringTable[key]);
                writer.Write(key, Encoding.UTF8);
            }
            writer.Close();
            stream.Close();
            return stream.ToArray();
        }

        /// <summary>
        /// Writes this BSON document to the specified path.
        /// </summary>
        /// <param name="path">The path to write to.</param>
        public void Write(string path)
        {
            var writer = new EasyWriter(path);
            Write(writer);
        }

        /// <summary>
        /// Writes this document as BSON to the specified stream.
        /// </summary>
        /// <param name="stream">The stream that will be written to.</param>
        public void Write(Stream stream)
        {
            var writer = new EasyWriter(stream);
            Write(writer);
        }

        /// <summary>
        /// Writes this document using the specified EasyWriter.
        /// </summary>
        /// <param name="writer">The writer that will be used to write this document.</param>
        internal void Write(EasyWriter writer)
        {
            _stringTableIndex = 0;
            _stringTable.Clear();
            WriteItem(writer, Top, null, true);
            writer.Close();
        }

        private void WriteItem(EasyWriter writer, BsonItem item, string name, bool isTop = false)
        {
            if(!isTop)
            {
                writer.Write(item.Type);
                writer.Write(GetKeyName(name), Encoding.UTF8, true);
            }

            if (item.HasValues) // object or array - we need to write a document
            {
                var stream = new MemoryStream();
                var vWriter = new EasyWriter(stream);
                foreach(string key in item.Keys)
                    WriteItem(vWriter, item[key], key);
                vWriter.Close();
                var data = stream.ToArray();
                // length of data + length of length + null terminator
                writer.Write(data.Length + 4 + 1);
                writer.Write(data);
                writer.Write((byte)0x00);
                return;
            }

            switch(item.Type)
            {
                case 0x01: // double
                    writer.Write((double)item.Value);
                    break;
                case 0x02: // string
                    var bytes = Encoding.UTF8.GetBytes(GetKeyName((string)item.Value, true));
                    writer.Write(bytes.Length + 1); // includes null terminator
                    writer.WriteBytes(bytes);
                    writer.Write((byte)0x00);
                    break;
                case 0x05: // binary
                    var byteArray = (byte[])item.Value;
                    writer.Write(byteArray.Length);
                    writer.Write((byte)0x00);
                    writer.Write(byteArray);
                    break;
                case 0x07: // objectid
                    writer.Write((byte[])item.Value);
                    break;
                case 0x08: // bool
                    writer.Write((bool)item.Value);
                    break;
                case 0x09: // UTC datetime
                case 0x11: // timestamp
                case 0x12: // 64 bit int
                    writer.Write((long)item.Value);
                    break;
                case 0x10: // 32 bit int
                    writer.Write((int)item.Value);
                    break;
                default:
                    throw new NotSupportedException($"Item type {item.Type} not supported.");
            }
        }

        /// <summary>
        /// Determines the correct name for the provided key.
        /// This will return the string table key instead if it's enabled.
        /// </summary>
        /// <param name="name">The name of the key.</param>
        /// <returns>The correct name for the provided key.</returns>
        private string GetKeyName(string name, bool value = false)
        {
            if (_stringTableMode == BsonStringTableMode.None ||
                value && _stringTableMode == BsonStringTableMode.Keys)
                return name;
            if (_stringTable.ContainsKey(name))
                return _stringTable[name].ToString();
            _stringTable[name] = _stringTableIndex++;
            return _stringTable[name].ToString();
        }

        /// <summary>
        /// Reads a BSON document from the specified byte array.
        /// </summary>
        /// <param name="data">The byte array that this document will be read from.</param>
        /// <returns>The document that was read.</returns>
        public static BsonDocument Read(byte[] data)
        {
            return Read(new EasyReader(data));
        }

        /// <summary>
        /// Reads a BSON document from the specified stream.
        /// </summary>
        /// <param name="data">The stream that this document will be read from.</param>
        /// <returns>The document that was read.</returns>
        public static BsonDocument Read(Stream data)
        {
            return Read(new EasyReader(data));
        }

        /// <summary>
        /// Reads a BSON document from the specified EasyReader.
        /// </summary>
        /// <param name="reader">The reader that will be used to read this document.</param>
        /// <returns>The document that was read.</returns>
        internal static BsonDocument Read(EasyReader reader)
        {
            var document = new BsonDocument();

            var length = reader.ReadInt32();
            while(!reader.EndOfStream)
            {
                var code = reader.ReadByte();
                if (code == 0x00) // end of document
                    break;
                var name = reader.ReadCString();
                var data = ReadItem(code, document, reader);
                document.Top[name] = data;
            }
            return document;
        }

        private static BsonItem ReadItem(byte code, BsonDocument document, EasyReader reader)
        {
            object val = null;
            switch (code)
            {
                case 0x01: // double
                    val = reader.ReadDouble();
                    break;
                case 0x02: // string
                    val = reader.ReadString(Encoding.UTF8).TrimEnd('\x00');
                    break;
                case 0x03: // document
                case 0x04: // array
                    val = Read(reader).Top;
                    break;
                case 0x05: // binary
                    var length = reader.ReadInt32();
                    var subtype = reader.ReadByte();
                    if (subtype != 0x00)
                        throw new NotSupportedException("BSON subtypes other than 'generic binary data' are not supported.");
                    val = reader.ReadBytes(length);
                    break;
                case 0x06: // undefined
                    break;
                case 0x07: // ObjectId
                    // why does this parser support ObjectIds and not other binary data?
                    // shhhhh
                    val = Encoding.ASCII.GetString(reader.ReadBytes(12));
                    break;
                case 0x08: // boolean
                    val = reader.ReadBoolean();
                    break;
                case 0x09: // UTC datetime
                    val = reader.ReadInt64();
                    break;
                case 0x0A: // null
                    break;
                case 0x0B: // regex
                    // why are you using regex in a Rant package?
                    throw new NotSupportedException("Regular expressions are not supported.");
                case 0x0C: // db pointer
                    throw new NotSupportedException("DB pointers are not supported.");
                case 0x0D: // Javascript code
                case 0x0F: // JS code with scope
                    throw new NotSupportedException("Javascript in BSON is not supported.");
                case 0x0E: // depreceated
                    val = reader.ReadString(Encoding.UTF8);
                    break;
                case 0x10: // 32 bit integer
                    val = reader.ReadInt32();
                    break;
                case 0x11: // timestamp
                case 0x12: // 64 bit integer
                    val = reader.ReadInt64();
                    break;
                case 0xFF: // min key
                case 0x7F: // max key
                    // we don't care about these so let's just skip em
                    break;
            }
            if (!(val is BsonItem))
                return new BsonItem(val) { Type = code };
            var i = (BsonItem)val;
            i.Type = code;
            return i;
        }
    }
}
