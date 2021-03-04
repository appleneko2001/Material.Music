using System.IO;
using Material.Music.Online.Results;

namespace Material.Music.Online.Interfaces
{
    public interface ITrackInfoProvider
    {
        string ApiName { get; }
        SearchTrackResult SearchTrack(string keywords = "");
    }
}