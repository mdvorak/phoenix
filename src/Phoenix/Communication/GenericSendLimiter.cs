using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Phoenix.Communication
{
    class GenericSendLimiter
    {
        private Timer timer;
        private AutoResetEvent ready;
        private int count;
        private int maxCount;
        private string message;

        public GenericSendLimiter(int maxCount)
        {
            timer = new Timer(new TimerCallback(TimerCallback), null, 1000, 1000 / maxCount);

            ready = new AutoResetEvent(true);
            count = 0;
            this.maxCount = maxCount;
        }

        public void SetRefreshRate(int interval)
        {
            timer.Change(0, interval);
        }

        private void TimerCallback(object state)
        {
            if (count > 0)
            {
                count--;
                ready.Set();
            }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public void Send()
        {
            ready.WaitOne();
            count++;

            if (count < maxCount)
                ready.Set();
            else
            {
                if (message != null)
                {
                    UO.PrintWarning(message);
                }
            }
        }
    }
}
