using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;

namespace Phoenix
{
    public static class SyncEvent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="sender"></param>
        /// <param name="e">Remember that this instance is not affected by event handlers in ISynchronizeInvoke.</param>
        public static void Invoke(Delegate handler, object sender, EventArgs e)
        {
            DelegateInvoker.Invoke(handler, new object[] { sender, e });
        }

        /// <summary>
        /// Invokes all nonsynchronized handlers on new thread (all on one).
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void BeginInvoke(Delegate handler, object sender, EventArgs e)
        {
            DelegateInvoker.BeginInvoke(handler, new object[] { sender, e });
        }

        /// <summary>
        /// Invokes every handler method in new thread (except synchronized handlers).
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void InvokeAsync(Delegate handler, object sender, EventArgs e)
        {
            DelegateInvoker.InvokeAsync(handler, new object[] { sender, e });
        }
    }
}
