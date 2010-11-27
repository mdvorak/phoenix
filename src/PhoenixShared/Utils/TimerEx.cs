using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Phoenix.Utils
{
    public class TimerEx : IDisposable
    {
        private Timer internalTimer;
        private int interval;
        public event EventHandler Callback;
        public event EventHandler IntervalChanged;

        public TimerEx(int interval)
        {
            this.interval = interval;
        }

        public TimerEx(int interval, EventHandler callback, bool start)
        {
            this.interval = interval;

            if (callback != null)
                Callback += callback;

            if (start)
                Start();
        }

        public int Interval
        {
            get { return interval; }
            set
            {
                if (value != interval)
                {
                    interval = value;
                    OnIntervalChanged(EventArgs.Empty);
                }
            }
        }

        public bool Running
        {
            get { return internalTimer != null; }
        }

        protected void OnIntervalChanged(EventArgs e)
        {
            if (Running)
            {
                internalTimer.Change(interval, interval);
            }

            if (IntervalChanged != null) IntervalChanged(this, e);
        }

        protected void OnCallback(EventArgs e)
        {
            if (Callback != null) Callback(this, e);
        }

        public void Start()
        {
            if (!Running)
            {
                internalTimer = new Timer(new TimerCallback(TimerCallback), null, interval, interval);
            }
        }

        public void Stop()
        {
            if (Running)
            {
                internalTimer.Change(Timeout.Infinite, Timeout.Infinite);
                internalTimer.Dispose();
                internalTimer = null;
            }
        }

        private void TimerCallback(object arg)
        {
            OnCallback(EventArgs.Empty);
        }

        void IDisposable.Dispose()
        {
            Stop();
        }
    }
}
