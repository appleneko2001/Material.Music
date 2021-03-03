using System;
using System.Collections.Generic;
using System.Text;

namespace Material.Music.Core.Enums
{
    public enum ChannelStatusEnums
    {
        /// <summary>
        /// Channel still preparing.
        /// </summary>
        Loading,
        
        /// <summary>
        /// Channel is ready for use.
        /// </summary>
        Loaded,

        /// <summary>
        /// Channel is waiting for buffers.
        /// </summary>
        Buffering,

        /// <summary>
        /// Channel is released.
        /// </summary>
        Unloaded,
    }
}
