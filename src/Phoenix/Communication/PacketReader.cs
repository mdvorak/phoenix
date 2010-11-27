using System;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;

namespace Phoenix.Communication
{
    /// <summary>
    /// Designed for reading variable-sized packets. Direct reading fixed-sized packets is a bit faster.
    /// </summary>
    public sealed class PacketReader
    {
        private byte[] data;
        private int offset;

        public PacketReader(byte[] data)
        {
            this.data = data;
            offset = 0;
        }

        public byte PacketId
        {
            get { return data[0]; }
        }

        /// <summary>
        /// Packet size in bytes.
        /// </summary>
        public int Length
        {
            get { return data.Length; }
        }

        public void Reset()
        {
            offset = 0;
        }

        private void TestSize(int size)
        {
            if (offset + size > data.Length)
                throw new IndexOutOfRangeException("Packet data offset is out of range.");
        }

        public int Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        public sbyte ReadSByte()
        {
            TestSize(1);
            return (sbyte)data[offset++];
        }

        public byte ReadByte()
        {
            TestSize(1);
            return data[offset++];
        }

        public short ReadInt16()
        {
            TestSize(2);
            short value = ByteConverter.BigEndian.ToInt16(data, offset);
            offset += 2;
            return value;
        }

        public ushort ReadUInt16()
        {
            TestSize(2);
            ushort value = ByteConverter.BigEndian.ToUInt16(data, offset);
            offset += 2;
            return value;
        }

        public int ReadInt32()
        {
            TestSize(4);
            int value = ByteConverter.BigEndian.ToInt32(data, offset);
            offset += 4;
            return value;
        }

        public uint ReadUInt32()
        {
            TestSize(4);
            uint value = ByteConverter.BigEndian.ToUInt32(data, offset);
            offset += 4;
            return value;
        }

        public long ReadInt64()
        {
            TestSize(8);
            long value = ByteConverter.BigEndian.ToInt64(data, offset);
            offset += 8;
            return value;
        }

        public ulong ReadUInt64()
        {
            TestSize(8);
            ulong value = ByteConverter.BigEndian.ToUInt64(data, offset);
            offset += 8;
            return value;
        }

        public string ReadAnsiString(int lenght)
        {
            TestSize(lenght);
            string value = ByteConverter.BigEndian.ToAsciiString(data, offset, lenght);
            offset += lenght;
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lenght">Byte count.</param>
        /// <returns></returns>
        public string ReadUnicodeString(int lenght)
        {
            TestSize(lenght);
            string value = ByteConverter.BigEndian.ToUnicodeString(data, offset, lenght);
            offset += lenght;
            return value;
        }

        public void Skip(int bytesCount)
        {
            TestSize(bytesCount);
            offset += bytesCount;
        }
    }
}
