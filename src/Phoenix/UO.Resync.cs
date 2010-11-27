using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Phoenix.Communication;
using Phoenix.Configuration;
using Phoenix.WorldData;

namespace Phoenix
{
    partial class UO
    {
        private static DateTime lastResyncTime = new DateTime();

        /// <summary>
        /// Gets time until resync will be allowed. If it is less than zero Resync can be called immediatly.
        /// </summary>
        public static int ResyncTimeRemaining
        {
            get
            {
                TimeSpan elapsed = DateTime.Now - lastResyncTime;
                return Config.ResyncInterval - elapsed.Seconds;
            }
        }

        /// <summary>
        /// Request resync with server.
        /// </summary>
        /// <returns>Time in seconds until next resync will be allowed.</returns>
        [Command("resync")]
        public static int Resync()
        {
            if (Config.ResyncInterval < 1) Config.ResyncInterval.Value = 1;

            TimeSpan elapsed = DateTime.Now - lastResyncTime;

            if (elapsed.Seconds > Config.ResyncInterval)
            {
                byte[] data = PacketBuilder.WalkRequestSucceeded(0);
                Core.SendToServer(data);
                lastResyncTime = DateTime.Now;
                UO.Print("Resync with server requested.");
                return Config.ResyncInterval;
            }
            else
            {
                return Config.ResyncInterval - elapsed.Seconds;
            }
        }
    }
}
