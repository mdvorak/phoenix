using System;
using System.Text.RegularExpressions;

namespace UOEncryption
{
    public class Encryption
    {
        private IEncryptor encryptor;
        private IDecryptor decryptor;

        public Encryption(IEncryptor enc, IDecryptor dec)
        {
            if (enc == null) throw new ArgumentNullException("enc");
            if (dec == null) throw new ArgumentNullException("dec");

            encryptor = enc;
            decryptor = dec;
        }

        public IEncryptor Encryptor
        {
            get { return encryptor; }
        }

        public IDecryptor Decryptor
        {
            get { return decryptor; }
        }

        /// <summary>
        /// Encrypts outgoing data.
        /// </summary>
        public byte[] Encrypt(byte[] input)
        {
            return encryptor.Encrypt(input);
        }

        /// <summary>
        /// Encrypts outgoing data.
        /// </summary>
        public byte[] Encrypt(byte[] input, int len)
        {
            return encryptor.Encrypt(input, len);
        }

        /// <summary>
        /// Decrypts incoming data.
        /// </summary>
        public byte[] Decrypt(byte[] input)
        {
            return decryptor.Decrypt(input);
        }

        /// <summary>
        /// Decrypts incoming data.
        /// </summary>
        public byte[] Decrypt(byte[] input, int len)
        {
            return decryptor.Decrypt(input, len);
        }

        public override string ToString()
        {
            return String.Format("UOEncryption.Encryption object using {0} for encryption and {1} for decryption.", encryptor.Description, decryptor.Description);
        }

        public static Encryption CreateClientLogin(LoginEncryptionType type, uint seed, uint key1, uint key2)
        {
            IEncryptor encryptor;
            IDecryptor decryptor;

            switch (type)
            {
                case LoginEncryptionType.None:
                    NoEncryption encryption = new NoEncryption();
                    encryptor = encryption;
                    decryptor = encryption;
                    break;

                case LoginEncryptionType.Old:
                case LoginEncryptionType.Rare:
                    throw new Exception("Unsupported login encryption.");

                case LoginEncryptionType.New:
                    encryptor = new LoginEncryption(seed, key1, key2);
                    decryptor = new NoEncryption();
                    break;

                default:
                    throw new Exception("Internal error.");
            }

            return new Encryption(encryptor, decryptor);
        }

        public static Encryption CreateServerLogin(LoginEncryptionType type, uint seed, uint key1, uint key2)
        {
            IEncryptor encryptor;
            IDecryptor decryptor;

            switch (type)
            {
                case LoginEncryptionType.None:
                    NoEncryption encryption = new NoEncryption();
                    encryptor = encryption;
                    decryptor = encryption;
                    break;

                case LoginEncryptionType.Old:
                case LoginEncryptionType.Rare:
                    throw new Exception("Unsupported login encryption.");

                case LoginEncryptionType.New:
                    encryptor = new NoEncryption();
                    decryptor = new LoginEncryption(seed, key1, key2);
                    break;

                default:
                    throw new Exception("Internal error.");
            }

            return new Encryption(encryptor, decryptor);
        }

        public static Encryption CreateClientGame(GameEncryptionType type, uint seed)
        {
            IEncryptor encryptor;
            IDecryptor decryptor;

            switch (type)
            {
                case GameEncryptionType.None:
                    encryptor = new NoEncryption();
                    decryptor = new Huffman();
                    break;

                case GameEncryptionType.Old: // Blowfish
                    encryptor = new BlowfishEncryption();
                    decryptor = new Huffman();
                    break;

                case GameEncryptionType.Rare:  // Blowfish + Twofish
                    encryptor = new BlowfishTwofishEncryption(seed);
                    decryptor = new Huffman();
                    break;

                case GameEncryptionType.New: // Twofish + MD5
                    encryptor = new TwofishEncryption(seed);
                    decryptor = new HuffmanMD5Encryption(seed);
                    break;

                default:
                    throw new Exception("Internal error.");
            }

            return new Encryption(encryptor, decryptor);
        }

        public static Encryption CreateServerGame(GameEncryptionType type, uint seed)
        {
            IEncryptor encryptor;
            IDecryptor decryptor;

            switch (type)
            {
                case GameEncryptionType.None:
                    encryptor = new Huffman();
                    decryptor = new NoEncryption();
                    break;

                case GameEncryptionType.Old: // Blowfish
                    encryptor = new Huffman();
                    decryptor = new BlowfishEncryption();
                    break;

                case GameEncryptionType.Rare:  // Blowfish + Twofish
                    encryptor = new Huffman();
                    decryptor = new BlowfishTwofishEncryption(seed);
                    break;

                case GameEncryptionType.New: // Twofish + MD5
                    encryptor = new HuffmanMD5Encryption(seed);
                    decryptor = new TwofishEncryption(seed);
                    break;

                default:
                    throw new Exception("Internal error.");
            }

            return new Encryption(encryptor, decryptor);
        }
    }
}
