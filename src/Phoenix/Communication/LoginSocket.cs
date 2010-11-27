using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Phoenix.Configuration;
using UOEncryption;

namespace Phoenix.Communication
{
    sealed class LoginSocket : UltimaSocket
    {
        private enum Phase
        {
            Seed,
            Keys,
            Normal
        }

        private Phase phase;

        public LoginSocket(long s, uint addr, int port)
            : base(s, addr, port)
        {
            phase = Phase.Seed;
            PredefinedPacketSizes = new int[] { 4, 62 };

            Trace.WriteLine("Login socket created.", "Communication");
        }

        protected override bool OnServerMessage(byte[] data)
        {
            if (data[0] == 0x8C) // Redirection packet
            {
                if (data.Length != 11) throw new SocketException("Invalid redirection packet lenght.", this, data);

                CommunicationManager.Redirecting = true;
                CommunicationManager.GameSeed = ByteConverter.LittleEndian.ToUInt32(data, 7);

                Trace.WriteLine(String.Format("Game seed sent by server: {0}", CommunicationManager.GameSeed), "Communication");
            }

            return base.OnServerMessage(data);
        }

        protected override bool OnClientMessage(byte[] data)
        {
            switch (phase) {
                case Phase.Seed:
                    if (data.Length != 4) throw new SocketException("Error while initializing LoginSocket. Invalid seed lenght.", this, data);

                    // Seed comes in little endian and encryption alghortm expects it in little endian, so i keep it as it is.
                    Seed = ByteConverter.LittleEndian.ToUInt32(data, 0);
                    CommunicationManager.LoginSeed = Seed;

                    SendToServer(data);
                    phase = Phase.Keys;
                    return true;

                case Phase.Keys:
                    if (data.Length != 62) throw new SocketException("Error while initializing LoginSocket. Invalid login packet lenght.", this, data);


                    // Detect and create client encryption
                    uint key1;
                    uint key2;
                    LoginEncryptionType clientEnc = GetClientEncryption(data, out key1, out key2);
                    ClientEncryption = Encryption.CreateServerLogin(clientEnc, Seed, key1, key2);


                    // Read and create server encryption
                    uint serverKey1;
                    uint serverKey2;
                    LoginEncryptionType serverEnc;


                    int v = Int32.Parse(Core.LaunchData.ServerEncryption);

                    switch (v) {
                        case 0:
                            serverKey1 = 0;
                            serverKey2 = 0;
                            serverEnc = LoginEncryptionType.None;
                            Trace.WriteLine("Using no server encryption.", "Communication");
                            break;

                        case 1:
                            serverKey1 = key1;
                            serverKey2 = key2;
                            serverEnc = clientEnc;
                            Trace.WriteLine(String.Format("Server key1: {1} key2: {2}", Seed.ToString("X"), serverKey1.ToString("X"), serverKey2.ToString("X")), "Communication");
                            break;

                        default:
                            try {
                                serverKey1 = UInt32.Parse(Core.LaunchData.ServerKey1, System.Globalization.NumberStyles.HexNumber);
                                serverKey2 = UInt32.Parse(Core.LaunchData.ServerKey2, System.Globalization.NumberStyles.HexNumber);
                                serverEnc = LoginEncryptionType.New;
                            }
                            catch (Exception e) {
                                throw new Exception("Error parsing server login keys.", e);
                            }
                            Trace.WriteLine(String.Format("Server key1: {1} key2: {2}", Seed.ToString("X"), serverKey1.ToString("X"), serverKey2.ToString("X")), "Communication");
                            break;
                    }

                    ServerEncryption = Encryption.CreateClientLogin(serverEnc, Seed, serverKey1, serverKey2);

                    byte[] decrypted = ClientEncryption.Decrypt(data);

                    // Save used account and password. They will be used to detect game encryprion.
                    CommunicationManager.Username = ByteConverter.BigEndian.ToAsciiString(decrypted, 1, 30);
                    CommunicationManager.Password = ByteConverter.BigEndian.ToAsciiString(decrypted, 31, 30);

                    SendToServer(decrypted);

                    // Zero password for security reasons
                    for (int i = 31; i < 61; i++)
                        decrypted[i] = 0;

                    Core.OnClientMessage(decrypted, CallbackResult.Sent);

                    phase = Phase.Normal;
                    return true;
            }

            return base.OnClientMessage(data);
        }

        private LoginEncryptionType GetClientEncryption(byte[] encryptedLoginPacket, out uint key1, out uint key2)
        {
            byte[] plain = PacketBuilder.LoginRequestShardList(Core.LaunchData.Username, Core.LaunchData.Password);

            if (memcmp(plain, encryptedLoginPacket, 61) != 0) // Encrypted client
            {
                if (!Core.ClientKeys.Calculated) {
                    if (!LoginEncryption.CalculateKeys(plain, encryptedLoginPacket, Seed, out key1, out key2))
                        throw new SocketException("Error while calculating login keys. Make sure you used same account and password as in launcher.", this, encryptedLoginPacket);

                    Core.ClientKeys.ClientInfo.Key1 = key1.ToString("X");
                    Core.ClientKeys.ClientInfo.Key2 = key2.ToString("X");
                    Core.ClientKeys.ClientInfo.Hash = Core.LaunchData.ClientHash;

                    Core.ClientKeys.Save();
                }
                else {
                    try {
                        key1 = UInt32.Parse(Core.ClientKeys.ClientInfo.Key1, System.Globalization.NumberStyles.HexNumber);
                        key2 = UInt32.Parse(Core.ClientKeys.ClientInfo.Key2, System.Globalization.NumberStyles.HexNumber);
                    }
                    catch (Exception e) {
                        throw new Exception("Error parsing client login keys.", e);
                    }
                }

                Trace.WriteLine(String.Format("Client key1: {1} key2: {2}", Seed.ToString("X"), key1.ToString("X"), key2.ToString("X")), "Communication");
                return LoginEncryptionType.New;
            }
            else {
                key1 = 0;
                key2 = 0;

                if (!Core.ClientKeys.Calculated) {
                    Core.ClientKeys.ClientInfo.Key1 = key1.ToString("X");
                    Core.ClientKeys.ClientInfo.Key2 = key2.ToString("X");
                    Core.ClientKeys.ClientInfo.Hash = Core.LaunchData.ClientHash;

                    Core.ClientKeys.Save();
                }

                Trace.WriteLine("Non-encrypted client detected.", "Communication");
                return LoginEncryptionType.None;
            }
        }
    }
}
