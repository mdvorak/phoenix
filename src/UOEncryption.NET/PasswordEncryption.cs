using System;
using System.Text;

namespace UOEncryption
{
    public class PasswordEncryption
    {
        public static byte[] Encrypt(byte[] input)
        {
            int len = input.Length;
            byte[] output = new byte[len];
            NativeEncryption.PWEncrypt(input, output, len);
            return output;
        }

        public static byte[] Decrypt(byte[] input)
        {
            int len = input.Length;
            byte[] output = new byte[len];
            NativeEncryption.PWDecrypt(input, output, len);
            return output;
        }

        public static string Encrypt(string input)
        {
            return Encoding.ASCII.GetString(Encrypt(Encoding.ASCII.GetBytes(input)));
        }

        public static string Decrypt(string input)
        {
            return Encoding.ASCII.GetString(Decrypt(Encoding.ASCII.GetBytes(input)));
        }
    }
}
