using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using JetBrains.Annotations;
using Material.Music.Core;
using Material.Music.Online;
using Material.Music.Online.Interfaces;
using Material.Music.Online.Results;
using Material.Music.ViewModels;

namespace Material.Music.Views.Dialogs.ViewModels
{
    public class SearchSubtitleResultItemViewModel : ViewModelBase
    {
        private SearchSubtitleResultItem _base;
        public SearchSubtitleResultItemViewModel(SearchSubtitleResultItem data)
        {
            _base = data;
        }

        public long Id => _base.TrackInfo.Id;
        
        public string Name => _base.Name;

        public string Title => _base.TrackInfo?.Title;

        public string Album => _base.TrackInfo?.Album;

        public string Artists => _base.TrackInfo?.Artists;
    }
    
    public class SearchSubtitleViewModel : ViewModelBase
    {
        public ISubtitleProvider selectedProvider;

        public SearchSubtitleViewModel()
        {
            selectedProvider = ApiManager.Instance.SubtitleProviders.First();
        }
        
        private bool _searchProcessing;
        public bool SearchProcessing
        {
            get => _searchProcessing;
            set
            {
                _searchProcessing = value;
                OnPropertyChanged();
            }
        }
        
        private bool _downloading;
        public bool Downloading
        {
            get => _downloading;
            set
            {
                _downloading = value;
                OnPropertyChanged();
            }
        }

        private string _downloadFailReason;
        public string DownloadFailReason
        {
            get => _downloadFailReason;
            set
            {
                _downloadFailReason = value;
                OnPropertyChanged();
            }
        }

        public void Search()
        {
            Task.Run(() =>
            {
                try
                {
                    SearchProcessing = true;

                    var result = selectedProvider.SearchSubtitle(_keywords);
                    ResultList.Clear();
                
                    foreach (var item in result.ResultItems)
                    {
                        Dispatcher.UIThread.InvokeAsync(()=> ResultList.Add(new SearchSubtitleResultItemViewModel(item)));
                    }
                }
                catch(Exception e)
                {
                    SelectedItemTip = e.Message;
                }
                SearchProcessing = false;
            });
        }

        private string _keywords;
        public string Keywords
        {
            get => _keywords;
            set
            {
                _keywords = value;
                OnPropertyChanged();
            }
        }

        private int _carouselIndex;
        public int CarouselIndex
        {
            get => _carouselIndex;
            set
            {
                _carouselIndex = value;
                OnPropertyChanged();
            }
        }

        private SearchSubtitleResultItemViewModel _selectedItem;
        public SearchSubtitleResultItemViewModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<SearchSubtitleResultItemViewModel> _resultList = new ObservableCollection<SearchSubtitleResultItemViewModel>();
        public ObservableCollection<SearchSubtitleResultItemViewModel> ResultList => _resultList; 

        private string _selectedItemTip;

        public string SelectedItemTip
        {
            get => _selectedItemTip;
            set
            {
                _selectedItemTip = value;
                OnPropertyChanged();
            }
        }
    }
}