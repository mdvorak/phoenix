using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Phoenix
{
    /// <summary>
    /// Contains informations about current session.
    /// </summary>
    public static class LoginInfo
    {
        private static IPEndPoint address = new IPEndPoint(0, 0);
        private static string shard = "";
        private static string account = "";
        private static string player = "";
        private static uint playerId = 0;

        internal static event EventHandler Changed;

        /// <summary>
        /// Gets IP address that is currently client connected to.
        /// </summary>
        public static IPEndPoint Address
        {
            get { return address; }
        }

        /// <summary>
        /// Gets shard name.
        /// </summary>
        public static string Shard
        {
            get { return shard; }
        }

        /// <summary>
        /// Gets player account.
        /// </summary>
        public static string Account
        {
            get { return account; }
        }

        /// <summary>
        /// Gets player name.
        /// </summary>
        public static string Player
        {
            get { return player; }
        }

        /// <summary>
        /// Gets player serial.
        /// </summary>
        public static uint PlayerId
        {
            get { return playerId; }
        }

        private static void OnChanged(EventArgs e)
        {
            try
            {
                SyncEvent.Invoke(Changed, null, e);
            }
            catch (Exception e0)
            {
                System.Diagnostics.Trace.WriteLine(String.Format("Exception in LoginInfo.Changed event. Exception:\r\n{0}", e0), "Error");
            }
        }

        /// <summary>
        /// Called by Phoenix.Init()
        /// </summary>
        internal static void Init()
        {
            Core.RegisterClientMessageCallback(0x80, new MessageCallback(OnLoginAndRequestShardList));
            Core.RegisterServerMessageCallback(0xA8, new MessageCallback(OnServerList));
            Core.RegisterClientMessageCallback(0xA0, new MessageCallback(OnServerSelect));
            Core.RegisterServerMessageCallback(0x8C, new MessageCallback(OnServerConnectRedirect));
            Core.RegisterClientMessageCallback(0x91, new MessageCallback(OnServerLoginRequest));
            Core.RegisterClientMessageCallback(0x5D, new MessageCallback(OnCharacterListSelect));
            Core.RegisterServerMessageCallback(0x1B, new MessageCallback(OnLoginConfirm));

            Core.Disconnected += new EventHandler(Core_Disconnected);
        }

        static void Core_Disconnected(object sender, EventArgs e)
        {
            address = new IPEndPoint(0, 0);
            shard = "";
            account = "";
            player = "";
            playerId = 0;

            OnChanged(EventArgs.Empty);
        }

        private static CallbackResult OnLoginAndRequestShardList(byte[] data, CallbackResult prevResult)
        {
            if (data[0] != 0x80) throw new ArgumentException("Invalid packet passed to OnLoginAndRequestShardList.");

            account = ByteConverter.BigEndian.ToAsciiString(data, 1, 30);

            OnChanged(EventArgs.Empty);
            return CallbackResult.Normal;
        }

        private static Dictionary<ushort, string> serverList = null;

        private static CallbackResult OnServerList(byte[] data, CallbackResult prevResult)
        {
            if (data[0] != 0xA8) throw new ArgumentException("Invalid packet passed to OnServerList.");

            ushort servers = ByteConverter.BigEndian.ToUInt16(data, 4);

            if (data.Length < 6 + servers * 40)
                throw new ArgumentException("Invalid packet size.", "data");

            serverList = new Dictionary<ushort, string>(servers);

            for (int i = 0; i < servers; i++)
            {
                ushort id = ByteConverter.BigEndian.ToUInt16(data, 6 + i * 40);
                serverList.Add(id, ByteConverter.BigEndian.ToAsciiString(data, 8 + i * 40, 30));

            }

            OnChanged(EventArgs.Empty);
            return CallbackResult.Normal;
        }

        private static CallbackResult OnServerSelect(byte[] data, CallbackResult prevResult)
        {
            if (data[0] != 0xA0) throw new ArgumentException("Invalid packet passed to OnServerSelect.");

            if (serverList == null)
                throw new InvalidOperationException("ServerList not received yet.");

            ushort shardIndex = ByteConverter.BigEndian.ToUInt16(data, 1);
            shard = "";
            serverList.TryGetValue(shardIndex, out shard);
            serverList = null;

            OnChanged(EventArgs.Empty);
            return CallbackResult.Normal;
        }

        private static CallbackResult OnServerConnectRedirect(byte[] data, CallbackResult prevResult)
        {
            if (data[0] != 0x8C) throw new ArgumentException("Invalid packet passed to OnServerConnectRedirect.");

            long ip = ByteConverter.LittleEndian.ToUInt32(data, 1);
            ushort port = ByteConverter.BigEndian.ToUInt16(data, 5);

            address = new IPEndPoint(ip, port);

            OnChanged(EventArgs.Empty);
            return CallbackResult.Normal;
        }

        private static CallbackResult OnServerLoginRequest(byte[] data, CallbackResult prevResult)
        {
            if (data[0] != 0x91) throw new ArgumentException("Invalid packet passed to OnServerLoginRequest.");

            account = ByteConverter.BigEndian.ToAsciiString(data, 5, 30);

            OnChanged(EventArgs.Empty);
            return CallbackResult.Normal;
        }

        private static CallbackResult OnCharacterListSelect(byte[] data, CallbackResult prevResult)
        {
            if (data[0] != 0x5D) throw new ArgumentException("Invalid packet passed to OnCharacterListSelect.");

            player = ByteConverter.BigEndian.ToAsciiString(data, 5, 30);
            if (player.Contains("\0")) player = player.Remove(player.IndexOf('\0'));

            OnChanged(EventArgs.Empty);
            return CallbackResult.Normal;
        }

        private static CallbackResult OnLoginConfirm(byte[] data, CallbackResult prevResult)
        {
            if (data[0] != 0x1B) throw new ArgumentException("Invalid packet passed to OnLoginConfirm.");

            playerId = ByteConverter.BigEndian.ToUInt32(data, 1);

            OnChanged(EventArgs.Empty);
            return CallbackResult.Normal;
        }

        private static CallbackResult OnCharacterStatus(byte[] data, CallbackResult prevResult)
        {
            if (data[0] != 0x11) throw new ArgumentException("Invalid packet passed to OnCharacterStatus.");

            if (ByteConverter.BigEndian.ToUInt32(data, 3) == playerId)
            {
                player = ByteConverter.BigEndian.ToAsciiString(data, 7, 30);
                if (player.Contains("\0")) player = player.Remove(player.IndexOf('\0'));
            }

            OnChanged(EventArgs.Empty);
            return CallbackResult.Normal;
        }
    }
}
