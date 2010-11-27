using System;

namespace UOEncryption
{
    public class MD5 : IEncryptor, IDecryptor
    {
        MD5Obj obj;

        public MD5(byte[] Data)
        {
            obj = MD5Obj.Create;
            NativeEncryption.MD5Init(ref obj, Data, (uint)Data.Length);
        }

        internal MD5Obj Object
        {
            get { return obj; }
        }

        public byte[] Encrypt(byte[] input)
        {
            int len = input.Length;
            byte[] output = new byte[len];
            NativeEncryption.MD5Encrypt(ref obj, input, output, len);
            return output;
        }

        public byte[] Encrypt(byte[] input, int len)
        {
            if (input.Length < len) throw new ArgumentOutOfRangeException("len", "Requested data lenght is larger than specified buffer.");
            byte[] output = new byte[len];
            NativeEncryption.MD5Encrypt(ref obj, input, output, len);
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
            get { return "MD5 encryption alghoritm"; }
        }
    }
}
