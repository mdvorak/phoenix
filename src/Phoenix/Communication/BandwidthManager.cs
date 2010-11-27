using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Phoenix.Communication
{
    public sealed class BandwidthManager
    {
        private const int Refresh = 500;

        private object syncRoot;
        private Timer timer;
        private DefaultPublicEvent bandwidthChanged;

        private long totalUpload;
        private long totalDownload;
        private int currentUpload;
        private int currentDownload;
        private int uploadBandwidth;
        private int downloadBandwidth;

        public BandwidthManager()
        {
            syncRoot = new object();
            bandwidthChanged = new DefaultPublicEvent();
            timer = new Timer(new TimerCallback(Callback), null, Refresh, Refresh);

            Reset();
        }

        private void OnBandwidthChanged(EventArgs e)
        {
            bandwidthChanged.Invoke(this, e);
        }

        private void Callback(object state)
        {
            lock (syncRoot) {
                uploadBandwidth = (int)(currentUpload * (1000.0f / Refresh));
                downloadBandwidth = (int)(currentDownload * (1000.0f / Refresh));

                currentUpload = 0;
                currentDownload = 0;
            }

            OnBandwidthChanged(EventArgs.Empty);
        }

        public event EventHandler BandwidthChanged
        {
            add { bandwidthChanged.AddHandler(value); }
            remove { bandwidthChanged.RemoveHandler(value); }
        }

        public long TotalUpload
        {
            get { return totalUpload; }
        }

        public long TotalDownload
        {
            get { return totalDownload; }
        }

        public int UploadBandwidth
        {
            get { return uploadBandwidth; }
        }

        public int DownloadBandwidth
        {
            get { return downloadBandwidth; }
        }

        internal void Reset()
        {
            lock (syncRoot) {
                totalUpload = 0;
                totalDownload = 0;
                currentUpload = 0;
                currentDownload = 0;
                uploadBandwidth = 0;
                downloadBandwidth = 0;
            }

            OnBandwidthChanged(EventArgs.Empty);
        }

        internal void Upload(int bytes)
        {
            lock (syncRoot) {
                bytes += 20; // TCP header
                totalUpload += bytes;
                currentUpload += bytes;
            }
        }

        internal void Download(int bytes)
        {
            lock (syncRoot) {
                bytes += 20; // TCP header
                totalDownload += bytes;
                currentDownload += bytes;
            }
        }
    }
}
