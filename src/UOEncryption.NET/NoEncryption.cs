using System;

namespace UOEncryption
{
    public class NoEncryption : IEncryptor, IDecryptor
    {
        public NoEncryption()
        {
        }

        public byte[] Encrypt(byte[] input)
        {
            return input;
        }

        public byte[] Encrypt(byte[] input, int len)
        {
            if (input.Length < len) throw new ArgumentOutOfRangeException("len", "Requested data lenght is larger than specified buffer.");
            byte[] output = new byte[len];
            Array.Copy(input, output, len);
            return output;
        }

        public byte[] Decrypt(byte[] input)
        {
            return input;
        }

        public byte[] Decrypt(byte[] input, int len)
        {
            if (input.Length < len) throw new ArgumentOutOfRangeException("len", "Requested data lenght is larger than specified buffer.");
            byte[] output = new byte[len];
            Array.Copy(input, output, len);
            return output;
        }

        public string Description
        {
            get { return "No encryption"; }
        }
    }
}
