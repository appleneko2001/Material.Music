using System.Collections.Generic;

namespace Material.Music.Online.Results
{
    public class DownloadSubtitleResult : ResultBase
    {
        public SubtitleType SubtitleType;
        public Dictionary<string, string> ContributedBy;
        public Dictionary<string, string> Subtitles;
    }
}