using System;

namespace UOEncryption
{
    public class Huffman : IEncryptor, IDecryptor
    {
        private HuffmanObj obj;

        public Huffman()
        {
            obj = new HuffmanObj();
            NativeEncryption.DecompressClean(ref obj);
        }

        public byte[] Compress(byte[] src, out int dest_size)
        {
            int len = src.Length;
            byte[] dest = new byte[len * 2];
            NativeEncryption.Compress(dest, src, out dest_size, ref len);
            return dest;
        }

        public byte[] Compress(byte[] src, int len, out int dest_size)
        {
            if (src.Length < len) throw new ArgumentOutOfRangeException("len", "Requested data lenght is larger than specified buffer.");
            byte[] dest = new byte[len * 2];
            NativeEncryption.Compress(dest, src, out dest_size, ref len);
            return dest;
        }

        public byte[] Compress(byte[] src)
        {
            int destLen;
            byte[] data = Compress(src, out destLen);
            Array.Resize<byte>(ref data, destLen);
            return data;
        }

        public byte[] Compress(byte[] src, int len)
        {
            int destLen;
            byte[] data = Compress(src, len, out destLen);
            Array.Resize<byte>(ref data, destLen);
            return data;
        }

        public byte[] Decompress(byte[] src, out int dest_size)
        {
            int len = src.Length;
            byte[] dest = new byte[NativeEncryption.MIN_DECBUF_SIZE(len)];
            NativeEncryption.Decompress(dest, src, out dest_size, ref len, ref obj);
            return dest;
        }

        public byte[] Decompress(byte[] src, int len, out int dest_size)
        {
            if (src.Length < len) throw new ArgumentOutOfRangeException("len", "Requested data lenght is larger than specified buffer.");
            byte[] dest = new byte[NativeEncryption.MIN_DECBUF_SIZE(len)];
            NativeEncryption.Decompress(dest, src, out dest_size, ref len, ref obj);
            return dest;
        }

        public byte[] Decompress(byte[] src)
        {
            int destLen;
            byte[] data = Decompress(src, out destLen);
            Array.Resize<byte>(ref data, destLen);
            return data;
        }

        public byte[] Decompress(byte[] src, int len)
        {
            int destLen;
            byte[] data = Decompress(src, len, out destLen);
            Array.Resize<byte>(ref data, destLen);
            return data;
        }

        byte[] IEncryptor.Encrypt(byte[] input)
        {
            return Compress(input);
        }

        byte[] IEncryptor.Encrypt(byte[] input, int len)
        {
            return Compress(input, len);
        }

        byte[] IDecryptor.Decrypt(byte[] input)
        {
            return Decompress(input);
        }

        byte[] IDecryptor.Decrypt(byte[] input, int len)
        {
            return Decompress(input, len);
        }

        public string Description
        {
            get { return "Huffman compression alghoritm"; }
        }
    }
}
