using System;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using Material.Music.ViewModels;

namespace Material.Music.Online.Results
{
    public enum SubtitleType
    {
        /// <summary>
        /// Common lyric file. Most used by applications from China. For example, Netease Cloud Music, QQ Music, etc. 
        /// </summary>
        Lrc,
        
        /// <summary>
        /// Xml Lyric File. Created by @appleneko2001. 
        /// </summary>
        Xlrc,
        
        /// <summary>
        /// Unknown subtitle type, not supported in current version.
        /// </summary>
        Unknown,
    }

    public class SubtitleDetailsItem : ViewModelBase
    {
        private string _language;
        public string Language
        {
            get => _language;
            set
            {
                _language = value;
                OnPropertyChanged();
            }
        }
        private string _author;
        public string Author
        {
            get => _author;
            set
            {
                _author = value;
                OnPropertyChanged();
            }
        }
    }
    
    public class SearchSubtitleResultItem : ViewModelBase
    {
        public SubtitleType SubtitleType;

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private SearchTrackResultItem _trackInfo;
        public SearchTrackResultItem TrackInfo
        {
            get => _trackInfo;
            set
            {
                _trackInfo = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<SubtitleDetailsItem> _resultItems = new ObservableCollection<SubtitleDetailsItem>();
        public ObservableCollection<SubtitleDetailsItem> ResultItems => _resultItems;
    }

    public class SearchSubtitleResult : ResultBase
    {
        private ObservableCollection<SearchSubtitleResultItem> _resultItems = new ObservableCollection<SearchSubtitleResultItem>();
        public ObservableCollection<SearchSubtitleResultItem> ResultItems => _resultItems;

        private int _resultCount;
        public int ResultCount
        {
            get => _resultCount;
            set
            {
                _resultCount = value;
                OnPropertyChanged();
            }
        }
    }
}