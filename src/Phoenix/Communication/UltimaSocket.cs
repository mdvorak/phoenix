using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
using UOEncryption;

namespace Phoenix.Communication
{
    abstract class UltimaSocket
    {
        [DllImport("msvcrt.dll")]
        protected static extern int memcmp(byte[] a1, byte[] a2, int len);

        private readonly object sendSync = new object();
        private long socket;
        private uint address;
        private int port;

        private int clientMsgIndex;
        private int[] predefinedClientMsgs;

        private uint seed;

        private Encryption clientEncryption;
        private Encryption serverEncryption;

        /// <summary>
        /// Encrypted Phoenix->Client buffer.
        /// </summary>
        private List<byte[]> toClientBuffer;

        /// <summary>
        /// Encrypted Phoenix->Server buffer.
        /// </summary>
        private List<byte[]> toServerBuffer;

        /// <summary>
        /// Decrypted Client->Phoenix buffer for part messages.
        /// </summary>
        private byte[] fromClientBuffer;

        /// <summary>
        /// Decrypted Server->Phoenix buffer for part messages.
        /// </summary>
        private byte[] fromServerBuffer;

        /// <summary>
        /// Can be created only from main thread.
        /// </summary>
        public UltimaSocket(long s, uint addr, int port)
        {
            CommunicationManager.CheckThread();

            socket = s;
            address = addr;
            this.port = port;

            clientMsgIndex = 0;
            predefinedClientMsgs = null;

            seed = 0;

            // No encryption
            clientEncryption = new Encryption(new NoEncryption(), new NoEncryption());
            serverEncryption = new Encryption(new NoEncryption(), new NoEncryption());

            toClientBuffer = new List<byte[]>();
            toServerBuffer = new List<byte[]>();
            fromClientBuffer = null;
            fromServerBuffer = null;
        }

        public long Socket
        {
            get { return socket; }
        }

        public uint Address
        {
            get { return address; }
        }

        public int Port
        {
            get { return port; }
        }

        /// <summary>
        /// Predefined packets in Client->Phoenix stream.
        /// </summary>
        protected int[] PredefinedPacketSizes
        {
            get { return predefinedClientMsgs; }
            set { predefinedClientMsgs = value; }
        }

        public bool DataAvailable
        {
            get { return toClientBuffer.Count > 0; }
        }

        public uint Seed
        {
            get { return seed; }
            protected set { seed = value; }
        }

        /// <summary>
        /// Phoenix->Client encryption and Client->Phoenix decryption
        /// </summary>
        protected Encryption ClientEncryption
        {
            get { return clientEncryption; }
            set { clientEncryption = value; }
        }

        /// <summary>
        /// Server->Phoenix decryption and Phoenix->Server encryption
        /// </summary>
        protected Encryption ServerEncryption
        {
            get { return serverEncryption; }
            set { serverEncryption = value; }
        }

        /// <summary>
        /// Reads data from server.
        /// </summary>
        public void ReceiveData()
        {
            CommunicationManager.CheckThread();

            lock (CommunicationManager.SyncRoot)
            {
                const int BuffLen = 65536;

                byte[] encryptedBuffer = new byte[BuffLen];
                int readBytes = WinSock.recv(socket, encryptedBuffer, BuffLen, 0);

                if (readBytes > 0)
                {
                    CommunicationManager.BandwidthManager.Download(readBytes);

                    byte[] decryptedBuffer = serverEncryption.Decrypt(encryptedBuffer, readBytes);
                    ProcessMessages(decryptedBuffer, true);
                }
            }
        }

        /// <summary>
        /// Checks packet integrity.
        /// </summary>
        /// <param name="data">Packet data.</param>
        /// <returns>Returns true if packet is ok; false if not or data is null.</returns>
        private bool CheckPacketIntegrity(byte[] data)
        {
            if (data == null || data.Length < 1)
                return false;

            if (predefinedClientMsgs != null)
                return true;

            int len = PacketLenghts.GetLenght(data[0]);

            if (len == PacketLenghts.Dynamic)
            {
                if (data.Length < 3) return false;
                len = ByteConverter.BigEndian.ToUInt16(data, 1);
            }

            return data.Length == len;
        }

        public void SendToClient(byte[] data)
        {
            lock (sendSync)
            {
                PacketLogging.Write(PacketDirection.PhoenixToClient, data);

                if (!CheckPacketIntegrity(data))
                    throw new SocketException("Packet integrity error.", this, data);

                if (PacketFilter.OnServerMessage(data))
                    return;

                byte[] encryptedBuffer = clientEncryption.Encrypt(data);
                toClientBuffer.Add(encryptedBuffer);
            }
        }

        public void SendToServer(byte[] data)
        {
            lock (sendSync)
            {
                PacketLogging.Write(PacketDirection.PhoenixToServer, data);

                if (!CheckPacketIntegrity(data))
                    throw new SocketException("Packet integrity error.", this, data);

                if (PacketFilter.OnClientMessage(data))
                    return;

                byte[] encryptedBuffer = serverEncryption.Encrypt(data);

                toServerBuffer.Add(encryptedBuffer);
            }
        }

        public void SendData()
        {
            CommunicationManager.CheckThread();

            lock (CommunicationManager.SyncRoot)
            {
                lock (sendSync)
                {
                    while (toServerBuffer.Count > 0)
                    {
                        InternalSendToServer(toServerBuffer[0]);
                        toServerBuffer.RemoveAt(0);
                    }
                }
            }
        }

        private void InternalSendToServer(byte[] data)
        {
            CommunicationManager.BandwidthManager.Upload(data.Length);

            int sent = WinSock.send(socket, data, data.Length, 0);

            if (sent != data.Length)
            {
                UO.Print(Env.DefaultErrorColor, "Fatal: Error sending message to server.");
                System.Diagnostics.Trace.WriteLine("Error sending message to server.", "Communication");
            }
        }

        /// <summary>
        /// Called when client is reading data.
        /// </summary>
        public int Recv(long socket, byte[] buff, int len, int flags)
        {
            CommunicationManager.CheckThread();

            lock (CommunicationManager.SyncRoot)
            {
                if (socket != this.socket) throw new ArgumentException("socket");

                // This shouldn't happen
                if (len > buff.Length) len = buff.Length;

                ReceiveData();

                if (!DataAvailable || len == 0)
                    return 0;

                lock (sendSync)
                {
                    int offset = 0;

                    while (toClientBuffer.Count > 0 && offset < len)
                    {
                        byte[] data = toClientBuffer[0];
                        toClientBuffer.RemoveAt(0);

                        int spaceLeft = len - offset;
                        int dataPart = Math.Min(data.Length, spaceLeft);

                        Array.Copy(data, 0, buff, offset, dataPart);
                        offset += dataPart;

                        if (dataPart < data.Length)
                        {
                            System.Diagnostics.Debug.Print("!!Sending part message to client!!");

                            int left = data.Length - dataPart;
                            byte[] dataLeft = new byte[left];

                            Array.Copy(data, dataPart, dataLeft, 0, left);

                            toClientBuffer.Insert(0, dataLeft);
                        }
                    }

                    return offset;
                }
            }
        }

        /// <summary>
        /// Called when client is sending data to server
        /// </summary>
        public int Send(long socket, byte[] buff, int len, int flags)
        {
            CommunicationManager.CheckThread();

            lock (CommunicationManager.SyncRoot)
            {
                if (socket != this.socket) throw new ArgumentException("socket");

                int buffLen = len;

                // Process predefined messages
                while (len > 0 && predefinedClientMsgs != null && clientMsgIndex < predefinedClientMsgs.Length)
                {
                    if (fromClientBuffer != null) throw new SocketException("Trying to read predefined message but there are unprocessed data in buffer.", this, buff);

                    int msgLen = predefinedClientMsgs[clientMsgIndex];
                    int newLen = len - msgLen;

                    if (newLen < 0)
                    {
                        throw new SocketException("Part predefined messages are not supported.", this, buff);
                    }

                    byte[] msg = new byte[msgLen];
                    Array.Copy(buff, msg, msgLen);

                    byte[] newBuf = new byte[newLen];
                    Array.Copy(buff, msgLen, newBuf, 0, newLen);
                    len = newLen;

                    byte[] decryptedBuffer = clientEncryption.Decrypt(msg, msgLen);
                    ProcessMessages(decryptedBuffer, false);
                }

                if (len > 0)
                {
                    byte[] decryptedBuffer = clientEncryption.Decrypt(buff, len);
                    ProcessMessages(decryptedBuffer, false);
                }

                SendData();

                return buffLen;
            }
        }

        /// <summary>
        /// Called when server sent some data to client.
        /// </summary>
        /// <param name="data">Decrypted data buffer.</param>
        /// <returns>Return true to eat packet.</returns>
        protected virtual bool OnServerMessage(byte[] data)
        {
            return Core.OnServerMessage(data, CallbackResult.Normal) != CallbackResult.Normal;
        }

        /// <summary>
        /// Called when client sent some data to server.
        /// </summary>
        /// <param name="data">Decrypted data buffer.</param>
        /// <returns>Return true to eat packet.</returns>
        protected virtual bool OnClientMessage(byte[] data)
        {
            return Core.OnClientMessage(data, CallbackResult.Normal) != CallbackResult.Normal;
        }

        private byte[] ReadPacket(byte[] buffer, int msgLen, ref int offset, bool fromServer)
        {
            if (offset + msgLen <= buffer.Length)
            {
                byte[] msg = new byte[msgLen];
                Array.Copy(buffer, offset, msg, 0, msgLen);
                offset += msgLen;

                if (fromServer)
                {
                    PacketLogging.Write(PacketDirection.ServerToPhoenix, msg);
                    bool eat = OnServerMessage(msg);
                    if (!eat) SendToClient(msg);
                }
                else
                {
                    PacketLogging.Write(PacketDirection.ClientToPhoenix, msg);
                    bool eat = OnClientMessage(msg);
                    if (!eat) SendToServer(msg);
                }

                return null;
            }
            else
            {
                // Part message
                int partLen = buffer.Length - offset;
                byte[] partBuffer = new byte[partLen];
                Array.Copy(buffer, offset, partBuffer, 0, partLen);
                offset += partLen;
                return partBuffer;
            }
        }

        private void ProcessMessages(byte[] data, bool fromServer)
        {
            byte[] buffer = data;

            byte[] partBuffer;
            if (fromServer)
                partBuffer = fromServerBuffer;
            else
                partBuffer = fromClientBuffer;

            // Part message
            if (partBuffer != null)
            {
                buffer = new byte[partBuffer.Length + data.Length];

                Array.Copy(partBuffer, buffer, partBuffer.Length);
                Array.Copy(data, 0, buffer, partBuffer.Length, data.Length);

                partBuffer = null;
            }

            int offset = 0;

            // Separate messages
            while (offset < buffer.Length)
            {
                int msgLen = PacketLenghts.GetLenght(buffer[offset]);

                if (!fromServer && predefinedClientMsgs != null)
                {
                    if (clientMsgIndex < predefinedClientMsgs.Length)
                    {
                        msgLen = predefinedClientMsgs[clientMsgIndex];
                    }
                    else
                    {
                        predefinedClientMsgs = null;
                    }
                }

                if (msgLen == 0)
                {
                    byte[] msg = new byte[data.Length - offset];
                    Array.Copy(data, offset, msg, 0, data.Length - offset);
                    throw new SocketException("Error in communication. Wrong encryption?", this, msg);
                }

                if (msgLen != PacketLenghts.Dynamic)
                {
                    // Fixed sized packet
                    partBuffer = ReadPacket(buffer, msgLen, ref offset, fromServer);
                }
                else
                {
                    // Variable sized packet
                    if (offset + 3 <= buffer.Length)
                    {
                        msgLen = ByteConverter.BigEndian.ToInt16(buffer, offset + 1);

                        // Now we know size
                        partBuffer = ReadPacket(buffer, msgLen, ref offset, fromServer);
                    }
                    else
                    {
                        // Part message does not contain size
                        int partLen = buffer.Length - offset;
                        partBuffer = new byte[partLen];
                        Array.Copy(buffer, offset, partBuffer, 0, partLen);
                        offset += partLen;
                    }
                }

                clientMsgIndex++;
            }

            if (fromServer)
                fromServerBuffer = partBuffer;
            else
                fromClientBuffer = partBuffer;
        }

        public string Dump()
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine(GetType().ToString() + " dump:");

            IPAddress ipAddress = new IPAddress(address);
            str.AppendFormat("Socket: {0} Address: {1},{2}\n", socket, ipAddress, port);
            str.AppendFormat("Seed: {0}\nPredefined messages pending: {1}\n", seed, predefinedClientMsgs != null);
            str.AppendFormat("Client encryption: {0}\nServer encryption: {1}\n", clientEncryption, serverEncryption);
            str.AppendLine();

            if (fromClientBuffer != null)
                str.Append("Client->Phoenix part buffer: " + PacketLogging.BuildString(fromClientBuffer));
            if (fromServerBuffer != null)
                str.Append("Server->Phoenix part buffer: " + PacketLogging.BuildString(fromServerBuffer));
            str.AppendLine();

            if (toClientBuffer.Count > 0)
            {
                str.AppendLine("Phoenix->Client pending messages:");
                foreach (byte[] data in toClientBuffer)
                    str.Append(PacketLogging.BuildString(data));
            }
            else
            {
                str.AppendLine("Phoenix->Client: No messages pending.");
            }
            if (toServerBuffer.Count > 0)
            {
                str.AppendLine("Phoenix->Server pending messages:");
                foreach (byte[] data in toServerBuffer)
                    str.Append(PacketLogging.BuildString(data));
            }
            else
            {
                str.AppendLine("Phoenix->Server: No messages pending.");
            }
            str.AppendLine();

            return str.ToString();
        }
    }
}
