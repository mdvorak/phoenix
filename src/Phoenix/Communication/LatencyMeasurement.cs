using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using Phoenix.Utils;

namespace Phoenix.Communication
{
    public static class LatencyMeasurement
    {
        private static Queue<long> pingQueue = new Queue<long>();
        private static Queue<long> walkQueue = new Queue<long>();

        private const int MaxLatencyListLenght = 20;
        private static List<int> latencyList = new List<int>();

        private static int avLatency = 0;
        private static int curLatency = 0;
        private static DefaultPublicEvent latencyChanged = new DefaultPublicEvent();

        internal static void Init()
        {
            Core.RegisterClientMessageCallback(0x73, new MessageCallback(OnClientPing));
            Core.RegisterServerMessageCallback(0x73, new MessageCallback(OnServerPing));

            Core.RegisterClientMessageCallback(0x02, new MessageCallback(OnWalkRequest));
            Core.RegisterServerMessageCallback(0x21, new MessageCallback(OnWalkRequestFailed));
            Core.RegisterServerMessageCallback(0x22, new MessageCallback(OnWalkRequestSucceed));
            Core.RegisterServerMessageCallback(0x20, new MessageCallback(OnPlayerSync));

            Core.Disconnected += new EventHandler(Core_Disconnected);
        }

        public static int CurrentLatency
        {
            get { return curLatency; }
        }

        public static int AverageLatency
        {
            get { return avLatency; }
        }

        private static void SetLatency(int cur, int av)
        {
            if (cur != curLatency || av != avLatency)
            {
                curLatency = cur;
                avLatency = av;

                latencyChanged.Invoke(null, EventArgs.Empty);
            }
        }

        public static event EventHandler LatencyChanged
        {
            add { latencyChanged.AddHandler(value); }
            remove { latencyChanged.RemoveHandler(value); }
        }

        private static void UpdateLatency()
        {
            while (latencyList.Count > MaxLatencyListLenght)
            {
                latencyList.RemoveAt(0);
            }

            if (latencyList.Count > 0)
            {
                int sum = 0;
                for (int i = 0; i < latencyList.Count; i++)
                {
                    sum += latencyList[i];
                }

                SetLatency(latencyList[latencyList.Count - 1], sum / latencyList.Count);
            }
            else
            {
                SetLatency(0, 0);
            }
        }


        internal static void SendPing()
        {
            if (Core.LoggedIn)
            {
                byte[] data = new byte[2];
                data[0] = 0x73;
                data[1] = 0x7F;

                pingQueue.Enqueue(NativeTimer.timeGetTime());
                Core.SendToServer(data);
            }
        }

        static CallbackResult OnClientPing(byte[] data, CallbackResult lastResult)
        {
            pingQueue.Enqueue(NativeTimer.timeGetTime());
            System.Diagnostics.Debug.WriteLine("Ping sent by client", "Information");
            return CallbackResult.Normal;
        }

        static CallbackResult OnServerPing(byte[] data, CallbackResult lastResult)
        {
            if (pingQueue.Count > 0)
            {
                latencyList.Add((int)(NativeTimer.timeGetTime() - pingQueue.Dequeue()));
                UpdateLatency();
            }

            return data[1] != 0x7F ? CallbackResult.Normal : CallbackResult.Eat;
        }

        static CallbackResult OnWalkRequest(byte[] data, CallbackResult prevResult)
        {
            walkQueue.Enqueue(NativeTimer.timeGetTime());
            return CallbackResult.Normal;
        }

        static CallbackResult OnWalkRequestFailed(byte[] data, CallbackResult prevResult)
        {
            if (walkQueue.Count > 0)
            {
                latencyList.Add((int)(NativeTimer.timeGetTime() - walkQueue.Dequeue()));
                walkQueue.Clear();
                UpdateLatency();
            }
            return CallbackResult.Normal;
        }

        static CallbackResult OnWalkRequestSucceed(byte[] data, CallbackResult prevResult)
        {
            if (walkQueue.Count > 0)
            {
                latencyList.Add((int)(NativeTimer.timeGetTime() - walkQueue.Dequeue()));
                UpdateLatency();
            }
            return CallbackResult.Normal;
        }

        static CallbackResult OnPlayerSync(byte[] data, CallbackResult prevResult)
        {
            uint serial = ByteConverter.BigEndian.ToUInt32(data, 1);

            if (Phoenix.WorldData.World.RealPlayer != null && serial == Phoenix.WorldData.World.PlayerSerial)
            {
                walkQueue.Clear();
                UpdateLatency();
            }

            return CallbackResult.Normal;
        }

        static void Core_Disconnected(object sender, EventArgs e)
        {
            latencyList.Clear();
            UpdateLatency();
        }
    }
}
