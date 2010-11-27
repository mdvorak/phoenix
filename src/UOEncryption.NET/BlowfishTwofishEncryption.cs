using System;

namespace UOEncryption
{
    public class BlowfishTwofishEncryption : IEncryptor, IDecryptor
    {
        private BlowfishEncryption blow;
        private TwofishEncryption two;

        public BlowfishTwofishEncryption(uint seed)
        {
            blow = new BlowfishEncryption();
            two = new TwofishEncryption(seed);
        }

        public byte[] Encrypt(byte[] input)
        {
            byte[] output = blow.Encrypt(input);
            return two.Encrypt(output);
        }

        public byte[] Encrypt(byte[] input, int lent)
        {
            byte[] output = blow.Encrypt(input);
            return two.Encrypt(output);
        }

        public byte[] Decrypt(byte[] input)
        {
            byte[] output = two.Encrypt(input);
            return blow.Decrypt(output);
        }

        public byte[] Decrypt(byte[] input, int len)
        {
            byte[] output = two.Encrypt(input, len);
            return blow.Decrypt(output);
        }

        public string Description
        {
            get { return "Blowfish + Twofish encryption alghoritm"; }
        }
    }
}
