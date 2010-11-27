using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;
using Phoenix.Utils;

namespace Phoenix.Communication
{
    class ActionSpamLimiter
    {
        private int rate;
        private int burst;
        private int critical;

        private int current;
        private long last;

        public ActionSpamLimiter(int rate, int burst, int critical)
        {
            this.rate = rate;
            this.burst = burst;
            this.critical = critical;
        }

        public bool IsCritical
        {
            get { return current > critical; }
        }

        public bool Hit(int weight)
        {
            if (weight < 0)
                throw new ArgumentException("weight");

            long now = NativeTimer.timeGetTime();

            if (current > 0) {
                current -= (int)((now - last) / rate);
                if (current < 0) current = 0;
            }

            if (current > burst)
                return false;

            current += weight;
            last = now;

            return true;
        }
    }
}
