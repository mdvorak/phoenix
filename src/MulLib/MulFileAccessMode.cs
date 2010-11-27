using System;
using System.Collections.Generic;
using System.Text;

namespace MulLib
{
    /// <summary>
    /// Access mode to data file. Used when data file is too large to load it completly to memory.
    /// </summary>
    public enum MulFileAccessMode
    {
        /// <summary>
        /// File will be opened for readonly access only.
        /// </summary>
        ReadOnly,
        /// <summary>
        /// Method tries to open file for write access. If it fails, file will be opened for readonly access.
        /// </summary>
        TryWrite,
        /// <summary>
        /// File will be opened for write access; otherwise exception will be thrown.
        /// </summary>
        RequestWrite
    }
}
