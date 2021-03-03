using System;
using System.Collections.Generic;
using System.Text;
using Material.Music.Appleneko2001;
using Material.Music.Core.Engine;
using Material.Music.Core.MediaInfo;

namespace Material.Music.Core.Interfaces
{
    public interface IPlayable
    {
        string Identicator { get; }
        string GetObjectHash();
        BassFileStream GetBassStream();
        /// <summary>
        /// No more needed this instance! Close it now!
        /// </summary>
        /// <param name="completelyDispose">FULLY DESTROY INSTANCE?</param>
        void Close(bool completelyDispose = false);
        /// <summary>
        /// Check track status manually
        /// </summary>
        void CheckStatus();
        /// <summary>
        /// The track file is from local storage?
        /// </summary>
        bool IsLocalMedia { get; }
        /// <summary>
        /// Get track status. It should be return true if file is available and readable.
        /// </summary>
        bool Ready { get; }
        /// <summary>
        /// Track container type. It can be known by analyze file magic header to get more accuracy determine type.
        /// </summary>
        AudioHeaderData MediaFormat { get; }
        /// <summary>
        /// Track duration.
        /// </summary>
        TimeSpan Duration { get; }
        /// <summary>
        /// Track information (or tags? whatever)
        /// </summary>
        IMediaInfo TrackInfo { get; }
        /// <summary>
        /// Track title. It should be read directly from TrackInfo properties or filename.
        /// </summary>
        string Title { get; }
    }
}
