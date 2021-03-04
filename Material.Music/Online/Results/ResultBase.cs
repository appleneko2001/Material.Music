using Material.Music.ViewModels;

namespace Material.Music.Online.Results
{
    public class ResultBase : ViewModelBase
    {
        private string _provider;
        public string Provider
        {
            get => _provider;
            set
            {
                _provider = value;
                OnPropertyChanged();
            }
        }
        
        private string _providerInfoLink;
        public string ProviderInfoLink
        {
            get => _providerInfoLink;
            set
            {
                _providerInfoLink = value;
                OnPropertyChanged();
            }
        }
    }
}