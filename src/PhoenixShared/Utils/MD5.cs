using System;
using System.IO;

namespace Phoenix.Utils
{
    public static class MD5
    {
        public static string ComputeHash(byte[] data)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            return BitConverter.ToString(md5.ComputeHash(data)).Replace("-", "").ToLowerInvariant();
        }

        public static string ComputeHash(System.IO.Stream stream)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
        }

        public static string ComputeHash(string path)
        {
            using (Stream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
                return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
