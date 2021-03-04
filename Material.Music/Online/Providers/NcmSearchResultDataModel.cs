using System;
using System.Collections.Generic;
using Material.Music.Online.Results;
using Newtonsoft.Json;

namespace NeteaseCloudMusic.Provider
{
    public class NcmObjectBase
    {
        [JsonProperty("id")]
        public int Id;
    }

    public class NcmResultObjectBase : NcmObjectBase
    {
        [JsonProperty("name")]
        public string Name;
    }
    
    public class ResultDataModel
    {
        public object result;
        public int code;
    }
    public sealed class SearchSongResultModel : IDisposable
    {
        private bool _disposed;
        public List<SongItemDataModel> songs;
        public int songCount;

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                if(songs != null)
                songs.Clear();
                songs = null;
                songCount = 0;
                GC.SuppressFinalize(this);
            }
        }

        public T ToResult<T>()
        {
            throw new NotImplementedException();
        }

        public object ToResult()
        {
            var result = new SearchTrackResult();
            result.ResultCount = songCount;
            result.Provider = NcmSubtitleProvider.ProviderName;
            result.ProviderInfoLink = NcmSubtitleProvider.ProviderLink;
            
            if(songs != null)
                foreach (var item in songs)
                {
                    var album = item.album.Name + (item.album.Translated == null ? "" : $" ({item.album.Translated})");
                    
                    result.ResultItems.Add(new SearchTrackResultItem()
                    {
                        Id = item.Id,
                        Album = item.album.ToString(),
                        AlbumArtist = item.album.artist.ToString(),
                        AlbumIllust = item.album.picUrl,
                        Artists = item.GetFullArtistsString(),
                        Title = item.Name
                    });
                }
            return result;
        }

        public SearchSubtitleResult ToSearchSubtitleResult()
        {
            var result = new SearchSubtitleResult();
            result.ResultCount = songCount;
            result.Provider = NcmSubtitleProvider.ProviderName;
            result.ProviderInfoLink = NcmSubtitleProvider.ProviderLink;
            
            if(songs != null)
                foreach (var item in songs)
                {
                    var album = item.album.Name + (item.album.Translated == null ? "" : $" ({item.album.Translated})");
                    var r = new SearchSubtitleResultItem()
                    {
                        Name = $"{item.Name} by {item.GetFullArtistsString()}",
                        TrackInfo = new SearchTrackResultItem()
                        {
                            Album = album,
                            AlbumArtist = item.album.artist.ToString(),
                            AlbumIllust = item.album.picUrl,
                            Artists = item.GetFullArtistsString(),
                            Id = item.Id,
                            Title = item.Name
                        },
                        SubtitleType = SubtitleType.Lrc
                    };
                    
                    result.ResultItems.Add(r);
                }
            return result;
        }
    }
    public class SongItemDataModel : NcmResultObjectBase
    {
        public int copyrightId;
        public int status;
        public List<ArtistInfoDataModel> artists;
        public AlbumInfoDataModel album;
        public string GetFullArtistsString()
        {
            var result = "";
            var seperators = artists.Count - 1;
            for(var i = 0;i < artists.Count; i++)
            {
                result += artists[i].Name + (artists[i].Translated == null ? "" : $"({artists[i].Translated})");
                if (seperators > 0)
                {
                    result+= ", ";
                    seperators--;
                }
            }
            return result;
        }
    }
    public sealed class SongDetailDataModel : NcmResultObjectBase, IDisposable
    {
        private bool disposed;
        public int no; // Track number
        public List<ArtistInfoDataModel> ar; // Artists
        public AlbumInfoDataModel al; // Album
        public long publishTime;
        public DateTime PublishTime => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(publishTime).ToLocalTime(); // From milisecond long time stamp to DateTime

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                Name = null;
                Id = 0;
                no = 0;
                ar.Clear();
                ar = null;
                al = null;
                publishTime = 0;
                GC.SuppressFinalize(this);
            }
        }

        public string GetFullArtistsString()
        {
            var result = "";
            var seperators = ar.Count - 1;
            for (var i = 0; i < ar.Count; i++)
            {
                result += ar[i].Name + (ar[i].Translated == null ? "" : $"({ar[i].Translated})");
                if (seperators > 0)
                {
                    result += ", ";
                    seperators--;
                }
            }
            return result;
        }
    }
    public class ArtistInfoDataModel: NcmResultObjectBase
    {
        [JsonProperty("trans")]
        public string Translated; // Translated
    }
    public class AlbumInfoDataModel: NcmResultObjectBase
    {
        public string picId;
        public string picUrl; // Album picture link
        public long pic;
        [JsonProperty("trans")]
        public string Translated; // Translated
        public ArtistInfoDataModel artist;
    }
    public class LyricDetailDataModel
    {
        public bool sgc;
        public bool sfy;
        public bool qfy;
        [JsonProperty("transUser")]
        public UserInfoDataModel TranslatedBy; // Translation provider user
        [JsonProperty("lyricUser")]
        public UserInfoDataModel LyricBy; // Original lyric provider user
        [JsonProperty("lrc")]
        public LrcDataModel LyricDetails; // Original lyric data
        //public LrcDataModel klyric; // Real-Time lyric data
        [JsonProperty("tlyric")]
        public LrcDataModel TranslatedLyricDetails; // Translated lyric data
    }
    public class UserInfoDataModel: NcmObjectBase
    {
        [JsonProperty("nickname")]
        public string Nickname;
    }
    public class LrcDataModel
    {
        [JsonProperty("version")]
        public int Version;
        
        [JsonProperty("lyric")]
        public string LyricData;
    }
}