using System;
using System.Text;

namespace UOEncryption
{
    public class TwofishEncryption : IEncryptor, IDecryptor
    {
        private TwofishObj obj;

        public TwofishEncryption(uint seed)
        {
            obj = TwofishObj.Create;
            obj.IP = seed;
            NativeEncryption.TwofishInit(ref obj);
        }

        internal TwofishObj Object
        {
            get { return obj; }
        }

        public byte[] Encrypt(byte[] input)
        {
            int len = input.Length;
            byte[] output = new byte[len];
            NativeEncryption.TwofishEncrypt(ref obj, input, output, len);
            return output;
        }

        public byte[] Encrypt(byte[] input, int len)
        {
            if (input.Length < len) throw new ArgumentOutOfRangeException("len", "Requested data lenght is larger than specified buffer.");
            byte[] output = new byte[len];
            NativeEncryption.TwofishEncrypt(ref obj, input, output, len);
            return output;
        }

        byte[] IDecryptor.Decrypt(byte[] input)
        {
            return Encrypt(input);
        }

        byte[] IDecryptor.Decrypt(byte[] input, int len)
        {
            return Encrypt(input, len);
        }

        public string Description
        {
            get { return "Twofish encryption alghoritm"; }
        }
    }
}
