using System;
using System.Text;

namespace UOEncryption
{
    public class LoginEncryption : IEncryptor, IDecryptor
    {
        private LoginCryptObj obj;

        /// <summary>
        /// Initializes the new object.
        /// </summary>
        /// <param name="seed">Encryption seed in Little Endian!!</param>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        public LoginEncryption(uint seed, uint key1, uint key2)
        {
            obj = new LoginCryptObj();
            obj.pseed = seed;
            obj.k1 = key1;
            obj.k2 = key2;
            NativeEncryption.LoginCryptInit(ref obj);
        }

        internal LoginCryptObj Object
        {
            get { return obj; }
        }

        public byte[] Encrypt(byte[] data)
        {
            int len = data.Length;
            byte[] encryptedData = new byte[len];
            NativeEncryption.LoginCryptEncrypt(ref obj, data, encryptedData, len);
            return encryptedData;
        }

        public byte[] Encrypt(byte[] data, int len)
        {
            if (data.Length < len) throw new ArgumentOutOfRangeException("len", "Requested data lenght is larger than specified buffer.");
            byte[] encryptedData = new byte[len];
            NativeEncryption.LoginCryptEncrypt(ref obj, data, encryptedData, len);
            return encryptedData;
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
			get { return "Login encryption alghoritm"; }
		}

        public static bool CalculateKeys(byte[] Plaintext, byte[] Ciphertext, uint Seed, out uint Key1, out uint Key2)
        {
            if (Plaintext.Length < 61) throw new ArgumentException("Plaintext array must be at least 61 bytes long.", "Plaintext");
            if (Ciphertext.Length < 61) throw new ArgumentException("Ciphertext array must be at least 61 bytes long.", "Ciphertext");
            return NativeEncryption.CalculateKeys(Plaintext, Ciphertext, ref Seed, out Key1, out Key2) > 0;
        }
    }
}
