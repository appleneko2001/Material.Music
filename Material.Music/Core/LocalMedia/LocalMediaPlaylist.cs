using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Material.Music.Core.Interfaces;
using Material.Music.Core.Logging;
using Material.Music.ViewModels;

namespace Material.Music.Core.LocalMedia
{
    public class LocalMediaPlaylist : ViewModelBase, IPlaylist
    {
        public readonly string[] SearchFilterExt = new string[] { "mp3", "flac", "m4a", "wma", "wav" };

        public readonly long MinimumFileSizeFilter = 1024 * 1024 * 1; // 1 MB minimum

        public LocalMediaPlaylist()
        {
            m_Playables = new ObservableCollection<PlayableBase>();
        }

        public void ScanDirectory(string dir)
        {
            try
            {
                if (Directory.Exists(dir))
                {
                    string[] files = Utils.EnumerateFiles(dir, Utils.ConvertFilterToPatterns(SearchFilterExt));
                    foreach(var filePath in files)
                    {
                        FileInfo file = new FileInfo(filePath);
                        if(file.Length > MinimumFileSizeFilter)
                        {
                            Playables.Add(new LocalPlayable(file));
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Logger.LogWarn(e.Message);
            }
        }

        public string Name => "LocalMediaPlaylist";

        public ObservableCollection<PlayableBase> Playables => m_Playables;

        private ObservableCollection<PlayableBase> m_Playables;
    }
}
