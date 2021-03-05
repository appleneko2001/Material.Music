using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LyricXml;
using Material.Music.Online.Results;
using Opportunity.LrcParser;

namespace Material.Music.Core.Containers
{
    public class DataContainer
    {
        public static string AppdataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), "NekoPlayer");

        public static string SubtitlesPath = Path.Combine(AppdataPath, "Subtitles");

        public static event EventHandler<string> OnSavedSubtitle; 
        public static event EventHandler<string> OnDeletedSubtitle; 
        
        public static string ConvertLrcToXLrc(Dictionary<string, string> lrcs, IReadOnlyDictionary<string, string> additionalParams = null)
        {
            var lyrics = new Dictionary<string, IParseResult<Line>>();
            foreach (var lrc in lrcs)
            {
                lyrics.Add(lrc.Key, Lyrics.Parse(lrc.Value));
            }

            var xmlLyric = new Lyric();
            foreach (var lyric in lyrics)
            {
                foreach (var line in lyric.Value.Lyrics.Lines)
                {
                    LyricTimeline item;

                    var existsItem = xmlLyric.Timelines.Where(e => e.Timestamp == line.Timestamp);
                    if (existsItem.Any())
                    {
                        item = existsItem.First();
                    }
                    else
                    {
                        item = new LyricTimeline()
                        {
                            Timestamp = line.Timestamp
                        };
                    }
                    
                    if (item.Elements.ContainsKey(lyric.Key))
                    {
                        if(!string.IsNullOrWhiteSpace(line.Content))
                            item.Elements.Add(lyric.Key + "1", line.Content);
                    }
                    else
                    {
                        item.Elements.Add(lyric.Key, line.Content);
                    }
                    
                    if (!xmlLyric.Timelines.Exists(e => e == item))
                        xmlLyric.Timelines.Add(item);
                }
                xmlLyric.AddLanguage(lyric.Key);
            }

            if (additionalParams != null)
            {
                foreach (var param in additionalParams)
                {
                    xmlLyric.Attributes.Add(param.Key, param.Value);
                }
            }
            
            return xmlLyric.ToXml();
        }

        public static void DeleteSubtitle(PlayableBase target)
        {
            var hash = target.GetObjectHash();
            var path = Path.Combine(SubtitlesPath, $"{hash}.xlrc");
            if (File.Exists(path))
            {
                File.Delete(path);
                OnDeletedSubtitle?.Invoke(null, hash);
            }
        }
        
        public static void SaveSubtitle(DownloadSubtitleResult result, PlayableBase target)
        {
            Utils.CreateIfNotExistDir(SubtitlesPath);
            
            if (result.SubtitleType is SubtitleType.Lrc)
            {
                var attributes = new Dictionary<string, string>();
                attributes.Add(nameof(result.Provider), result.Provider);
                if (!string.IsNullOrWhiteSpace(result.ProviderInfoLink))
                {
                    attributes.Add(nameof(result.ProviderInfoLink), result.ProviderInfoLink);
                }
                foreach (var contributor in result.ContributedBy)
                {
                    attributes.Add($"{contributor.Key}-Contributor", contributor.Value);
                }
                
                var xml = ConvertLrcToXLrc(result.Subtitles, attributes);
                var hash = target.GetObjectHash();
                var path = Path.Combine(SubtitlesPath, $"{hash}.xlrc");

                DeleteSubtitle(target);
                
                File.WriteAllText(path, xml);
                OnSavedSubtitle(null, hash);
            }
        }

        public static LyricXml.Lyric LoadSubtitle(PlayableBase playable)
        {
            var path = Path.Combine(SubtitlesPath, $"{playable.GetObjectHash()}.xlrc");
            if (File.Exists(path))
            {
                return new Lyric(File.ReadAllText(path));
            }
            return null;
        }
    }
}