using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;

namespace UOEncryption
{
    [StructLayout(LayoutKind.Sequential)]
    struct HuffmanObj
    {
        public int has_incomplete;
        public int incomplete_node;
        public sbyte incomplete_byte;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct LoginCryptObj
    {
        public uint pseed;
        public uint k1, k2;
        /* private, dynamic vars used by the crypt code, each encrypted byte changes them */
        uint m_key1, m_key2; /* 0 - m_CryptMaskLo 1 - m_CryptMaskHi */
        uint m_k1, m_k2; /* 1 - m_MasterHi  2 - m_MasterLo */
    }

    [StructLayout(LayoutKind.Sequential)]
    struct BlowfishObj
    {
        /// <summary>
        /// Must be 8 bytes long.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeEncryption.CRYPT_GAMESEED_LENGTH)]
        public byte[] seed;
        public int table_index;
        public int stream_pos;
        public int block_pos;

        public static BlowfishObj Create
        {
            get
            {
                BlowfishObj obj = new BlowfishObj();
                obj.seed = new byte[NativeEncryption.CRYPT_GAMESEED_LENGTH];
                return obj;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct keyInstance
    {
        public byte direction;
        public int keyLen;
        public int numRounds;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 68)]
        public sbyte[] keyMaterial;
        public uint keySig;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public uint[] key32;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] sboxKeys;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public uint[] subKeys;

        public static keyInstance Create
        {
            get
            {
                keyInstance inst = new keyInstance();
                inst.keyMaterial = new sbyte[68];
                inst.key32 = new uint[8];
                inst.sboxKeys = new uint[4];
                inst.subKeys = new uint[40];
                return inst;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct cipherInstance
    {
        public byte mode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] IV;
        public uint cipherSig;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] iv32;

        public static cipherInstance Create
        {
            get
            {
                cipherInstance inst = new cipherInstance();
                inst.IV = new byte[16];
                inst.iv32 = new uint[4];
                return inst;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct TwofishObj
    {
        public keyInstance ki;
        public cipherInstance ci;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] tabUsed;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] subData3;
        /// <summary>
        /// Init by user before calling TwofishInit() 
        /// </summary>
        public uint IP;
        public uint dwIndex; /* init: 0 */
        public int tabEnable; /* init: 0 */
        public int pos; /* init: 0 */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public int[] numRounds; /* init {0x00, 0x10, 0x10, 0x10} */

        public static TwofishObj Create
        {
            get
            {
                TwofishObj obj = new TwofishObj();
                obj.ki = keyInstance.Create;
                obj.ci = cipherInstance.Create;
                obj.tabUsed = new byte[256];
                obj.subData3 = new byte[256];
                obj.numRounds = new int[4];
                return obj;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct MD5Obj
    {
        public uint TableIdx;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Digest;

        public static MD5Obj Create
        {
            get
            {
                MD5Obj obj = new MD5Obj();
                obj.TableIdx = 0;
                obj.Digest = new byte[16];
                return obj;
            }
        }
    }

    class NativeEncryption
    {
        /// <summary>
        /// Declares the minimal size of a buffer to decompress n bytes.
        /// </summary>
        /// <param name="i">Bytes count.</param>
        /// <returns>Required bytes.</returns>
        public static int MIN_DECBUF_SIZE(int i) { return ((i * 4) + 4); }

        [DllImport("UOEncryption.dll")]
        public static extern void Compress([In, Out] byte[] dest, byte[] src, out int dest_size, ref int src_size);

        [DllImport("UOEncryption.dll")]
        public static extern void Decompress([In, Out] byte[] dest, byte[] src, out int dest_size, ref int src_size, ref HuffmanObj obj);

        [DllImport("UOEncryption.dll")]
        public static extern void DecompressClean(ref HuffmanObj obj);

        public const int GETKEYS_MIN_SIZE = 61;

        [DllImport("UOEncryption.dll")]
        public static extern void LoginCryptInit(ref LoginCryptObj obj);

        [DllImport("UOEncryption.dll")]
        public static extern void LoginCryptEncrypt(ref LoginCryptObj obj, byte[] input, byte[] output, int len);

        [DllImport("UOEncryption.dll")]
        public static extern int CalculateKeys(byte[] Plaintext, byte[] Ciphertext, ref uint Seed, out uint Key1, out uint Key2);

        public const int CRYPT_GAMEKEY_COUNT = 25;
        public const int CRYPT_GAMESEED_LENGTH = 8;

        [DllImport("UOEncryption.dll")]
        public static extern void BlowfishInit(ref BlowfishObj Obj);

        [DllImport("UOEncryption.dll")]
        public static extern void BlowfishEncrypt(ref BlowfishObj Obj, byte[] input, [In, Out] byte[] output, int len);

        [DllImport("UOEncryption.dll")]
        public static extern void BlowfishDecrypt(ref BlowfishObj Obj, byte[] input, [In, Out] byte[] output, int len);

        [DllImport("UOEncryption.dll")]
        public static extern void TwofishInit(ref TwofishObj Obj);

        [DllImport("UOEncryption.dll")]
        public static extern void TwofishEncrypt(ref TwofishObj Obj, byte[] input, [In, Out] byte[] output, int len);

        [DllImport("UOEncryption.dll")]
        public static extern void MD5Init(ref MD5Obj Obj, byte[] Data, uint Size);

        [DllImport("UOEncryption.dll")]
        public static extern void MD5Encrypt(ref MD5Obj Obj, byte[] input, [In, Out] byte[] output, int len);

        [DllImport("UOEncryption.dll")]
        public static extern void PWEncrypt(byte[] input, [In, Out] byte[] output, int len);

        [DllImport("UOEncryption.dll")]
        public static extern void PWDecrypt(byte[] input, [In, Out] byte[] output, int len);
    }
}
