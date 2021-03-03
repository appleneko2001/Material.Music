using System.Collections.ObjectModel;

namespace Material.Music.ViewModels
{
    public class SubtitlePageItemViewModel : ViewModelBase
    {
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
    }
}