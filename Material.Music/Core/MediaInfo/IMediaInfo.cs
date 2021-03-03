using System;
using System.Collections.Generic;
using System.Text;

namespace Material.Music.Core.MediaInfo
{
    public interface IMediaInfo
    {
        string TrackId { get; }
        string Artist { get; }
        string AlbumArtist { get; }
        string Album { get; }
        string Year { get; }
        string Genre { get; }
        string Title { get; }  
    }
}
