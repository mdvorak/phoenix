using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using UOEncryption;

namespace Phoenix.Communication
{
    sealed class GameSocket : UltimaSocket
    {
        private enum Phase
        {
            Seed,
            EncryptionDetection,
            CharacterList,
            CharListSelect,
            Normal
        }

        private Phase phase;
        private bool fixCharList = true;
        private Queue<byte[]> pendingData;
        private uint playerSerial;

        public GameSocket(long s, uint addr, int port)
            : base(s, addr, port)
        {
            phase = Phase.Seed;
            pendingData = new Queue<byte[]>();
            playerSerial = UInt32.MaxValue;

            PredefinedPacketSizes = new int[] { 4, 65 };

            Trace.WriteLine("Game socket created.", "Communication");
        }

        public GameSocket(long s, uint addr, int port, uint seed)
            : base(s, addr, port)
        {
            Seed = seed;

            phase = Phase.EncryptionDetection;
            pendingData = new Queue<byte[]>();
            playerSerial = UInt32.MaxValue;

            PredefinedPacketSizes = new int[] { 65 };

            Debug.WriteLine("Game socket redirecting.", "Communication");
        }

        private void SendPendingData()
        {
            if (pendingData == null)
                throw new InternalErrorException("Invalid call of SendPendingData.");

            if (pendingData.Count > 0)
            {
                Debug.WriteLine("Processing pending data..", "Communication");
            }

            while (pendingData.Count > 0)
            {
                byte[] data = pendingData.Dequeue();
                if (!base.OnServerMessage(data))
                {
                    SendToClient(data);
                }
            }
            pendingData.Clear();
        }

        protected override bool OnServerMessage(byte[] data)
        {
            if (fixCharList)
            {
                if (phase == Phase.CharacterList)
                {
                    if (data[0] == 0xA9)
                    {
                        Trace.WriteLine("Character list received from server.", "Communication");
                        SendPendingData();
                        pendingData = null;
                        phase = Phase.Normal;
                    }
                    else
                    {
                        switch (data[0])
                        {
                            case 0x1B:
                                playerSerial = ByteConverter.BigEndian.ToUInt32(data, 1);
                                SendToServer(PacketBuilder.CharacterSkillsStatsRequest(playerSerial, 4));
                                break;

                            case 0x11:
                                uint serial = ByteConverter.BigEndian.ToUInt32(data, 3);
                                if (serial == playerSerial)
                                {
                                    string name = ByteConverter.BigEndian.ToAsciiString(data, 7, 30);

                                    Trace.WriteLine("Sending fake character list.", "Communication");
                                    SendToClient(PacketBuilder.CharacterList(name));
                                    Core.CharListSent = true;
                                    phase = Phase.CharListSelect;
                                }
                                break;
                        }

                        pendingData.Enqueue(data);
                        return true;
                    }
                }
                else if (phase == Phase.CharListSelect)
                {
                    if (data[0] == 0xA9)
                    {
                        Trace.WriteLine("Warning: Unexcpected character list received. Packet dropped.");
                        return true;
                    }
                    else
                    {
                        pendingData.Enqueue(data);
                        return true;
                    }
                }
            }

            return base.OnServerMessage(data);
        }

        protected override bool OnClientMessage(byte[] data)
        {
            switch (phase)
            {
                case Phase.Seed:
                    if (data.Length != 4) throw new SocketException("Invalid seed lenght.", this, data);

                    Seed = ByteConverter.LittleEndian.ToUInt32(data, 0);
                    Trace.WriteLine(String.Format("Game seed sent by client: {0}", Seed.ToString("X")), "Communication");

                    SendToServer(data);
                    phase = Phase.EncryptionDetection;
                    return true;

                case Phase.EncryptionDetection:
                    CommunicationManager.ClientEncryption = DetectEncryption(data);

                    Trace.WriteLine(String.Format("Client is using {0} encryption.", CommunicationManager.ClientEncryption), "Communication");
                    ClientEncryption = UOEncryption.Encryption.CreateServerGame(CommunicationManager.ClientEncryption, Seed);

                    CommunicationManager.ServerEncryption = GetServerEncryption(CommunicationManager.ClientEncryption);

                    Trace.WriteLine(String.Format("Server is using {0} encryption.", CommunicationManager.ServerEncryption), "Communication");
                    ServerEncryption = UOEncryption.Encryption.CreateClientGame(CommunicationManager.ServerEncryption, Seed);

                    byte[] decrypted = ClientEncryption.Decrypt(data);

                    SendToServer(decrypted);
                    Core.OnClientMessage(decrypted, CallbackResult.Sent);

                    phase = fixCharList ? Phase.CharacterList : Phase.Normal;
                    return true;

                case Phase.CharListSelect:
                    if (data[0] != 0x5D)
                        throw new Exception("Invalid packet received from client. Did you selected properly character from list?");

                    Trace.WriteLine("Character from fake list selected.", "Communication");

                    SendPendingData();
                    pendingData = null;
                    phase = Phase.Normal;
                    return true;
            }

            return base.OnClientMessage(data);
        }

        private GameEncryptionType DetectEncryption(byte[] data)
        {
            if (data.Length != 65) throw new SocketException("Invalid login (0x91) packet lenght.", this, data);

            byte[] plain = PacketBuilder.ServerLoginRequest(Seed, CommunicationManager.Username, CommunicationManager.Password);
            CommunicationManager.Password = null;

            if (CompareLoginRequestPackets(plain, data))
                return GameEncryptionType.None;

            Encryption oldEnc = Encryption.CreateClientGame(GameEncryptionType.Old, Seed);
            Encryption rareEnc = Encryption.CreateClientGame(GameEncryptionType.Rare, Seed);
            Encryption newEnc = Encryption.CreateClientGame(GameEncryptionType.New, Seed);

            byte[] encrypted;

            encrypted = oldEnc.Encrypt(plain);
            if (CompareLoginRequestPackets(encrypted, data))
                return GameEncryptionType.Old;

            encrypted = rareEnc.Encrypt(plain);
            if (CompareLoginRequestPackets(encrypted, data))
                return GameEncryptionType.Rare;

            encrypted = newEnc.Encrypt(plain);
            if (CompareLoginRequestPackets(encrypted, data))
                return GameEncryptionType.New;

            throw new SocketException("Failed to detect game encryption.", this, data);
        }

        private bool CompareLoginRequestPackets(byte[] data1, byte[] data2)
        {
            int usernameLen = CommunicationManager.Username.Length + 1;
            return memcmp(data1, data2, 5 + usernameLen) == 0;
        }

        private GameEncryptionType GetServerEncryption(GameEncryptionType clientEncryption)
        {
            int v = Int32.Parse(Core.LaunchData.ServerEncryption);
            switch (v)
            {
                case 0:
                    return GameEncryptionType.None;
                case 1:
                    return clientEncryption;
                case 2:
                    return GameEncryptionType.Old;
                case 3:
                    return GameEncryptionType.Rare;
                case 4:
                    return GameEncryptionType.New;
                default:
                    Trace.WriteLine("server game encryption. Using same as client.", "Communication");
                    return CommunicationManager.ClientEncryption;
            }
        }
    }
}
