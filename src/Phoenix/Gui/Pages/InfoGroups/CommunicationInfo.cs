using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Phoenix.Communication;
using Phoenix.WorldData;

namespace Phoenix.Gui.Pages.InfoGroups
{
    [InfoGroup("Communication")]
    public partial class CommunicationInfo : UserControl
    {
        private const int MaxPingTestCount = 20;

        private DateTime lastTime = DateTime.Now;
        private TimeSpan timeConnected = new TimeSpan();
        private DateTime lastPingTest = new DateTime();
        private int pingTestCount = 0;

        public CommunicationInfo()
        {
            InitializeComponent();

            Core.LoginComplete += new EventHandler(Core_LoginComplete);
            Core.Disconnected += new EventHandler(Core_Disconnected);
            CommunicationManager.BandwidthManager.BandwidthChanged += new EventHandler(BandwidthManager_BandwidthChanged);
            LatencyMeasurement.LatencyChanged += new EventHandler(LatencyMeasurement_LatencyChanged);

            pingBox.Text = "0 ms";
            pingBox.ForeColor = Color.FromArgb(0, 0, 150);
          //  currentPingBox.Text = "0 ms";
          //  currentPingBox.ForeColor = Color.FromArgb(0, 0, 150);
        }

        void Core_LoginComplete(object sender, EventArgs e)
        {
            // StartLatencyTest();
        }

        void Core_Disconnected(object sender, EventArgs e)
        {
            latencyTestTimer.Enabled = false;

            pingBox.Text = "0 ms";
            pingBox.ForeColor = Color.FromArgb(0, 0, 150);
          //  currentPingBox.Text = "0 ms";
          //  currentPingBox.ForeColor = Color.FromArgb(0, 0, 150);
        }

        void LatencyMeasurement_LatencyChanged(object sender, EventArgs e)
        {
            pingBox.Text = Core.Latency.ToString() + " ms";
            pingBox.ForeColor = Helper.GetStatusColor((float)Core.Latency / 250.0f, 150);

           // currentPingBox.Text = Core.CurrentLatency.ToString() + " ms";
            //currentPingBox.ForeColor = Helper.GetStatusColor((float)Core.CurrentLatency / 250.0f, 150);
        }

        void BandwidthManager_BandwidthChanged(object sender, EventArgs e)
        {
            Phoenix.Communication.BandwidthManager man = Phoenix.Communication.CommunicationManager.BandwidthManager;
            upBandBox.Text = SizeConverter.ToOptimal(man.UploadBandwidth);
            upTotalBox.Text = SizeConverter.ToOptimal(man.TotalUpload);
            downBandBox.Text = SizeConverter.ToOptimal(man.DownloadBandwidth);
            downTotalBox.Text = SizeConverter.ToOptimal(man.TotalDownload);

            if ((long)timeConnected.TotalSeconds > 0)
            {
                averageUpBandBox.Text = SizeConverter.ToOptimal(man.TotalUpload / (long)timeConnected.TotalSeconds);
                averageDownBandBox.Text = SizeConverter.ToOptimal(man.TotalDownload / (long)timeConnected.TotalSeconds);
            }
            else
            {
                averageUpBandBox.Text = SizeConverter.ToOptimal(0);
                averageDownBandBox.Text = SizeConverter.ToOptimal(0);
            }
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            DateTime time = DateTime.Now;
            if (CommunicationManager.Connected)
            {
                timeConnected += time - lastTime;
            }
            lastTime = time;

            if (IsHandleCreated)
            {
                timeConnectedBox.Text = String.Format("{0}:{1:00}:{2:00}", timeConnected.Hours, timeConnected.Minutes, timeConnected.Seconds);
            }
        }

        private void latencyTestTimer_Tick(object sender, EventArgs e)
        {
            LatencyMeasurement.SendPing();
            pingTestCount++;

            if (pingTestCount > MaxPingTestCount)
            {
                latencyTestTimer.Enabled = false;
            }
        }

        private void pingBox_DoubleClick(object sender, EventArgs e)
        {
            StartLatencyTest();
        }

        public void StartLatencyTest()
        {
            if (Core.LoggedIn)
            {
                TimeSpan interval = new TimeSpan(0, 1, 0);
                TimeSpan elapsed = DateTime.Now - lastPingTest;
                if (elapsed > interval)
                {
                    pingTestCount = 0;
                    latencyTestTimer.Enabled = true;
                    lastPingTest = DateTime.Now;
                    UO.PrintInformation("Ping test started.");
                }
                else
                {
                    TimeSpan remaining = interval - elapsed;
                    UO.PrintWarning("You cannot test ping yet. Time remaining: {0}:{1:00}", remaining.Minutes, remaining.Seconds);
                }
            }
        }
    }
}
