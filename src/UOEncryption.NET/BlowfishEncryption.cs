using System;
using System.Text;

namespace UOEncryption
{
    public class BlowfishEncryption : IEncryptor, IDecryptor
    {
        private BlowfishObj obj;

        public BlowfishEncryption()
        {
            obj = BlowfishObj.Create;
            NativeEncryption.BlowfishInit(ref obj);
        }

        internal BlowfishObj Object
        {
            get { return obj; }
        }

        public byte[] Encrypt(byte[] input)
        {
            int len = input.Length;
            byte[] output = new byte[len];
            NativeEncryption.BlowfishEncrypt(ref obj, input, output, len);
            return output;
        }

        public byte[] Encrypt(byte[] input, int len)
        {
            if (input.Length < len) throw new ArgumentOutOfRangeException("len", "Requested data lenght is larger than specified buffer.");
            byte[] output = new byte[len];
            NativeEncryption.BlowfishEncrypt(ref obj, input, output, len);
            return output;
        }

        public byte[] Decrypt(byte[] input)
        {
            int len = input.Length;
            byte[] output = new byte[len];
            NativeEncryption.BlowfishDecrypt(ref obj, input, output, len);
            return output;
        }

        public byte[] Decrypt(byte[] input, int len)
        {
            if (input.Length < len) throw new ArgumentOutOfRangeException("len", "Requested data lenght is larger than specified buffer.");
            byte[] output = new byte[len];
            NativeEncryption.BlowfishDecrypt(ref obj, input, output, len);
            return output;
        }

        public string Description
        {
            get { return "Blowfish encryption alghoritm"; }
        }
    }
}
