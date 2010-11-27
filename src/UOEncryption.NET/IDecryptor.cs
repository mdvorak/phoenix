using System;

namespace UOEncryption
{
    public interface IDecryptor
    {
        byte[] Decrypt(byte[] data);
        byte[] Decrypt(byte[] data, int len);
        string Description { get; }
    }
}
