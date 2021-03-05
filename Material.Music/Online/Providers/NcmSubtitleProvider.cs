using System;
using System.Collections.Generic;
using Material.Music.Online.Interfaces;
using Material.Music.Online.Results;
using NeteaseCloudMusicApi;
using Newtonsoft.Json.Linq;

namespace NeteaseCloudMusic.Provider
{
    public class NcmSubtitleProvider : ISubtitleProvider
    {
        internal const string ProviderLink = "music.163.com";
        internal const string ProviderName = "Netease Cloud Music";
        public string ApiName => ProviderName;

        private CloudMusicApi _backend;
        
        public NcmSubtitleProvider()
        {
            _backend = new CloudMusicApi();
        }

        public SearchSubtitleResult SearchSubtitle(string keywords = "")
        {
            if (string.IsNullOrEmpty(keywords))
                throw new ArgumentNullException("Keywords argument cannot be empty!");
            var query = new Dictionary<string, object>() { { "keywords", keywords } };
            var root = GetResult(CloudMusicApiProviders.Search, query);
            using (var result = root["result"].ToObject<SearchSongResultModel>())
            {
                if (result != null)
                {
                    if (result.songCount == 0 && result.songs?.Count != 0)
                        if (result.songs != null)
                            result.songCount = result.songs.Count;

                    return result.ToSearchSubtitleResult();
                }
                else
                    return new SearchSubtitleResult() { ResultCount = 0 };
            }
        }

        public DownloadSubtitleResult DownloadSubtitle(long id)
        {
            var root = GetResult(CloudMusicApiProviders.Lyric, 
                new Dictionary<string, object>() { { "id", id } });
            var data = root.ToObject<LyricDetailDataModel>();
            var subs = new Dictionary<string, string>();

            if(data.LyricDetails != null)
                subs.Add("origin", data.LyricDetails.LyricData);
            if(data.TranslatedLyricDetails != null)
                subs.Add("zh-hans", data.TranslatedLyricDetails.LyricData);

            var contributors = new Dictionary<string, string>();
            if(data.LyricBy!= null)
                contributors.Add("origin", data.LyricBy.Nickname);
            if(data.TranslatedBy != null)
                contributors.Add("zh-hans", data.TranslatedBy.Nickname);
            
            var result = new DownloadSubtitleResult()
            {
                SubtitleType = SubtitleType.Lrc,
                Subtitles = subs,
                ContributedBy = contributors,
                Provider = ProviderName,
                ProviderInfoLink = ProviderLink
            };
            return result;
        }

        public JObject GetResult(CloudMusicApiProvider func, Dictionary<string, object> param)
        {
            JObject data = null;
            data = _backend.RequestAsync(func, param).Result.Item2;
            if (data == null)
                throw new ArgumentException("Request result cannot be NULL!");
            else
            {
                var root = data;
                var requestResult = root["code"].ToObject<int>();
                if (requestResult == 200)
                {
                    return data;
                }
                throw new NotImplementedException($"Request returned exception ({requestResult})");
            }
        }
    }
}