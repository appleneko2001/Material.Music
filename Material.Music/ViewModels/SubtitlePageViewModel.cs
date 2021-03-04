using System;
using System.Collections.ObjectModel;
using System.Linq;
using LyricXml;
using Material.Music.Core;
using Material.Music.Core.Containers;
using Opportunity.LrcParser;

namespace Material.Music.ViewModels
{
    public class SubtitlePageItemViewModel : ViewModelBase
    {
        private TimeSpan _timestamp;

        public TimeSpan Timestamp
        {
            get => _timestamp;
            set
            {
                _timestamp = value;
                OnPropertyChanged();
            }
        }

        private string _firstLine;

        public string FirstLine
        {
            get => _firstLine;
            set
            {
                _firstLine = value;
                OnPropertyChanged();
            }
        }
        
        private string _secondLine;

        public string SecondLine
        {
            get => _secondLine;
            set
            {
                _secondLine = value;
                OnPropertyChanged();
            }
        }
    }
    
    public class SubtitlePageViewModel : ViewModelBase
    {
        private ObservableCollection<SubtitlePageItemViewModel> _subtitleItems;

        public ObservableCollection<SubtitlePageItemViewModel> SubtitleItems => _subtitleItems;

        public SubtitlePageViewModel()
        {
            _subtitleItems = new ObservableCollection<SubtitlePageItemViewModel>();
        }

        public void UpdateSubtitlePage(PlayableBase playable)
        {
            if (playable is null)
            {
                _subtitleItems.Clear();
                return;
            }

            try
            {
                var lyric = DataContainer.LoadSubtitle(playable);
                if (lyric != null)
                {
                    if (lyric.Timelines == null)
                        return;
                
                    string GetLine(LyricTimeline t, int i)
                    {
                        if (lyric.Languages.Length < i)
                            return null;
                        
                        var k = lyric.Languages[i];
                        if(t.Elements.ContainsKey(k)) 
                            return t.Elements[k];
                        return null;
                    }
                
                    foreach (var timestamp in lyric.Timelines)
                    {
                        SubtitleItems.Add(new SubtitlePageItemViewModel()
                        {
                            FirstLine = GetLine(timestamp, 0),
                            SecondLine = GetLine(timestamp, 1),
                            Timestamp = timestamp.Timestamp
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}