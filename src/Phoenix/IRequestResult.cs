using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix
{
    public enum RequestState
    {
        /// <summary>
        /// This shouldn't happen. It will problably mean some internal error.
        /// </summary>
        Unknown,
        /// <summary>
        /// Request hasn't been processed yet.
        /// </summary>
        Pending,
        /// <summary>
        /// Request was successfully processed.
        /// </summary>
        Completed,
        /// <summary>
        /// Request was cancelled before it could be processed.
        /// </summary>
        Cancelled,
        /// <summary>
        /// Request failed.
        /// </summary>
        Failed,
        /// <summary>
        /// Request failed due to timeout.
        /// </summary>
        Timeout
    }

    public interface IRequestResult
    {
        RequestState State { get; }
        bool Finished { get; }
        bool Failed { get; }
        bool Succeeded { get; }

        void Wait();
        bool Wait(int timeout);
    }
}
