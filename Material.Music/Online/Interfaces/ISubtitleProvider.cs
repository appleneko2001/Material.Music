using Material.Music.Online.Results;

namespace Material.Music.Online.Interfaces
{
    public interface ISubtitleProvider
    {
        string ApiName { get; }
        SearchSubtitleResult SearchSubtitle(string keywords = "");
        DownloadSubtitleResult DownloadSubtitle(long id);
    }
}