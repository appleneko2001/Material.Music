using Material.Music.ViewModels;

namespace Material.Music.Views.Dialogs.ViewModels
{
    public class SearchSubtitleViewModel : ViewModelBase
    {
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