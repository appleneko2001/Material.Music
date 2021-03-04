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
        
        public static string ConvertLrcToXLrc(Dictionary<string, string> lrcs)
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

                    item.Elements.Add(lyric.Key, line.Content);

                    if (!xmlLyric.Timelines.Exists(e => e == item))
                        xmlLyric.Timelines.Add(item);
                }
                xmlLyric.AddLanguage(lyric.Key);
            }
            return xmlLyric.ToXml();
        }
        
        public static void SaveSubtitle(DownloadSubtitleResult result, PlayableBase target)
        {
            Utils.CreateIfNotExistDir(SubtitlesPath);
            
            if (result.SubtitleType is SubtitleType.Lrc)
            {
                var xml = ConvertLrcToXLrc(result.Subtitles);
                var path = Path.Combine(SubtitlesPath, $"{target.GetObjectHash()}.xlrc");
                File.WriteAllText(path, xml);
                OnSavedSubtitle(null, target.GetObjectHash());
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