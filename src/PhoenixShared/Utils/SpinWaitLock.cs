using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Phoenix.Utils
{
    /// <summary>
    /// Spin lock implementation.
    /// </summary>
    /// <remarks>
    /// This class was taken from http://msdn.microsoft.com/en-us/magazine/cc163726.aspx.
    /// </remarks>
    public struct SpinWaitLock
    {
        private const Int32 c_lsFree = 0;
        private const Int32 c_lsOwned = 1;
        private Int32 m_LockState; // Defaults to 0=c_lsFree

        public void Enter()
        {
            Thread.BeginCriticalRegion();
            while (true) {
                // If resource available, set it to in-use and return
                if (Interlocked.Exchange(
                    ref m_LockState, c_lsOwned) == c_lsFree) {
                    return;
                }

                // Efficiently spin, until the resource looks like it might 
                // be free. NOTE: Just reading here (as compared to repeatedly 
                // calling Exchange) improves performance because writing 
                // forces all CPUs to update this value
                while (Thread.VolatileRead(ref m_LockState) == c_lsOwned) {
                    StallThread();
                }
            }
        }

        public void Exit()
        {
            // Mark the resource as available
            Interlocked.Exchange(ref m_LockState, c_lsFree);
            Thread.EndCriticalRegion();
        }

        private static readonly bool IsSingleCpuMachine = (Environment.ProcessorCount == 1);

        private static void StallThread()
        {
            // On a single-CPU system, spinning does no good
            if (IsSingleCpuMachine) SwitchToThread();

            // Multi-CPU system might be hyper-threaded, let other thread run
            else Thread.SpinWait(1);
        }

        [DllImport("kernel32", ExactSpelling = true, SetLastError = false)]
        private static extern void SwitchToThread();
    }

}
