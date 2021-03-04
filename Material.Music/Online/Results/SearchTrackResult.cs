using System;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using Material.Music.ViewModels;

namespace Material.Music.Online.Results
{
    public class SearchTrackResultItem : ViewModelBase
    {
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
        
        private string _artists;
        public string Artists
        {
            get => _artists;
            set
            {
                _artists = value;
                OnPropertyChanged();
            }
        }
        
        private string _album;
        public string Album
        {
            get => _album;
            set
            {
                _album = value;
                OnPropertyChanged();
            }
        }
        
        private string _albumArtist;
        public string AlbumArtist
        {
            get => _albumArtist;
            set
            {
                _albumArtist = value;
                OnPropertyChanged();
            }
        }
        
        private string _albumIllust;
        public string AlbumIllust
        {
            get => _albumIllust;
            set
            {
                _albumIllust = value;
                OnPropertyChanged();
            }
        }
        
        private long _id;
        public long Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }
    }

    public class SearchTrackResult : ResultBase
    {
        private ObservableCollection<SearchTrackResultItem> _resultItems = new ObservableCollection<SearchTrackResultItem>();
        public ObservableCollection<SearchTrackResultItem> ResultItems => _resultItems;

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