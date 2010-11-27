using System;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;

namespace Phoenix.Communication
{
    /// <summary>
    /// Designed for writing variable-sized packets.
    /// </summary>
    public sealed class PacketWriter
    {
        private byte[] data;
        private int size;
        private int blockSizeOffset;

        public PacketWriter()
        {
            data = new byte[64];
            size = 0;
            blockSizeOffset = -1;
        }

        public PacketWriter(byte id)
        {
            data = new byte[64];
            size = 0;
            blockSizeOffset = -1;

            Write(id);
        }

        /// <summary>
        /// Packet size in bytes.
        /// </summary>
        public int Lenght
        {
            get { return size; }
        }

        /// <summary>
        /// Gets packet data.
        /// </summary>
        /// <returns>Array of exact packet lenght.</returns>
        public byte[] GetBytes()
        {
            Array.Resize<byte>(ref data, size);

            if (blockSizeOffset >= 0)
            {
                ByteConverter.BigEndian.ToBytes((ushort)size, data, blockSizeOffset);
            }

            return data;
        }

        private void EnsureDataSize(int s)
        {
            while (data.Length < size + s)
            {
                Array.Resize<byte>(ref data, data.Length * 2);
            }
        }

        /// <summary>
        /// Creates marker where will be later inserted block size. Takes 2 bytes.
        /// </summary>
        /// <returns>Written bytes count.</returns>
        public int WriteBlockSize()
        {
            if (blockSizeOffset >= 0)
                throw new InvalidOperationException("Cannot insert block size in packet twice.");

            blockSizeOffset = size;
            EnsureDataSize(2);
            size += 2;
            return 2;
        }

        public int Write(sbyte value)
        {
            const int ValueSize = 1;
            EnsureDataSize(ValueSize);
            size += ByteConverter.BigEndian.ToBytes(value, data, size);
            return ValueSize;
        }

        public int Write(byte value)
        {
            const int ValueSize = 1;
            EnsureDataSize(ValueSize);
            size += ByteConverter.BigEndian.ToBytes(value, data, size);
            return ValueSize;
        }

        public int Write(short value)
        {
            const int ValueSize = 2;
            EnsureDataSize(ValueSize);
            size += ByteConverter.BigEndian.ToBytes(value, data, size);
            return ValueSize;
        }

        public int Write(ushort value)
        {
            const int ValueSize = 2;
            EnsureDataSize(ValueSize);
            size += ByteConverter.BigEndian.ToBytes(value, data, size);
            return ValueSize;
        }

        public int Write(int value)
        {
            const int ValueSize = 4;
            EnsureDataSize(ValueSize);
            size += ByteConverter.BigEndian.ToBytes(value, data, size);
            return ValueSize;
        }

        public int Write(uint value)
        {
            const int ValueSize = 4;
            EnsureDataSize(ValueSize);
            size += ByteConverter.BigEndian.ToBytes(value, data, size);
            return ValueSize;
        }

        public int Write(long value)
        {
            const int ValueSize = 8;
            EnsureDataSize(ValueSize);
            size += ByteConverter.BigEndian.ToBytes(value, data, size);
            return ValueSize;
        }

        public int Write(ulong value)
        {
            const int ValueSize = 8;
            EnsureDataSize(ValueSize);
            size += ByteConverter.BigEndian.ToBytes(value, data, size);
            return ValueSize;
        }

        /// <summary>
        /// Writes text in ASCII encoding.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Written bytes count.</returns>
        public int WriteAsciiString(string value)
        {
            EnsureDataSize(value.Length + 1);
            int valueSize = ByteConverter.BigEndian.ToBytesAscii(value, data, size);
            size += valueSize + 1;
            return valueSize + 1;
        }

        /// <summary>
        /// Writes text in ASCII encoding.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="exactSize"></param>
        /// <returns>Written bytes count.</returns>
        public int WriteAsciiString(string value, int exactSize)
        {
            EnsureDataSize(exactSize);
            ByteConverter.BigEndian.ToBytesAscii(value, data, size, exactSize);
            size += exactSize;
            data[size - 1] = 0; // Security check
            return exactSize;
        }

        /// <summary>
        /// Writes text in Unicode encoding.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Written bytes count.</returns>
        public int WriteUnicodeString(string value)
        {
            EnsureDataSize(value.Length * 2 + 2);
            int valueSize = ByteConverter.BigEndian.ToBytesUnicode(value, data, size);
            size += valueSize + 2;
            return valueSize + 2;
        }

        /// <summary>
        /// Writes text in Unicode encoding.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Written bytes count.</returns>
        public int WriteUnicodeString(string value, int exactSize)
        {
            EnsureDataSize(exactSize);
            ByteConverter.BigEndian.ToBytesUnicode(value, data, size, exactSize);
            size += exactSize;
            data[size - 2] = 0; // Security check
            data[size - 1] = 0;
            return exactSize;
        }

        public int Write(byte[] data)
        {
            EnsureDataSize(data.Length);

            Array.Copy(data, 0, this.data, size, data.Length);
            size += data.Length;
            return data.Length;
        }

        public int WriteBytes(byte value, int count)
        {
            EnsureDataSize(count);

            for (int i = 0; i < count; i++)
                data[size + i] = value;

            size += count;
            return count;
        }
    }
}
