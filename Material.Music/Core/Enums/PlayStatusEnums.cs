using System;
using System.Collections.Generic;
using System.Text;

namespace Material.Music.Core.Enums
{
    public enum PlayStatusEnums
    {
        /// <summary>
        /// Mute status changed as muted
        /// </summary>
        Muted,

        /// <summary>
        /// Mute status changed as unmuted
        /// </summary>
        Unmuted,

        /// <summary>
        /// Audio is playing
        /// </summary>
        Play,

        /// <summary>
        /// Audio is paused
        /// </summary>
        Pause,

        /// <summary>
        /// Audio is stopped and reseted seek.
        /// </summary>
        Stop,

        /// <summary>
        /// Audio playback is completed
        /// </summary>
        ReachedEnd,
    }
}
