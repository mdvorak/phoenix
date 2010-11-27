using System;

namespace UOEncryption
{
    public class HuffmanMD5Encryption : IEncryptor, IDecryptor
    {
        private MD5 md5;
        private Huffman huff;

        public HuffmanMD5Encryption(uint seed)
        {
            // Twofish table is required for MD5 table creation.
            TwofishEncryption two;
            two = new TwofishEncryption(seed);

            md5 = new MD5(two.Object.subData3);
            huff = new Huffman();
        }

        public byte[] Encrypt(byte[] input)
        {
            byte[] output = huff.Compress(input);
            return md5.Encrypt(output);
        }

        public byte[] Encrypt(byte[] input, int len)
        {
            byte[] output = huff.Compress(input, len);
            return md5.Encrypt(output);
        }

        public byte[] Decrypt(byte[] input)
        {
            byte[] output = md5.Encrypt(input);
            return huff.Decompress(output);
        }

        public byte[] Decrypt(byte[] input, int len)
        {
            byte[] output = md5.Encrypt(input, len);
            return huff.Decompress(output);
        }

        public string Description
        {
            get { return "Huffman compression + MD5 encryption alghoritm"; }
        }
    }
}
