using System.ComponentModel;
using System.Runtime.CompilerServices;
using Material.Icons;
using Material.Styles;
using Material.Music.Core.Interfaces;

namespace Material.Music.ViewModels
{
    public class BaseSetterViewModel : INotifyPropertyChanged
    {
        #region Notify property changed

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public BaseSetterViewModel(string? title, string? caption = null, MaterialIconKind? icon = null)
        {
            _setterTitle = title;
            _setterCaption = caption;
            _setterIcon = icon;
        }
        
        private string? _setterTitle;
        public string? SetterTitle
        {
            get => _setterTitle;
            set { _setterTitle = value; OnPropertyChanged(); }
        }

        private string? _setterCaption;
        public string? SetterCaption
        {
            get => _setterCaption;
            set { _setterCaption = value; OnPropertyChanged(); }
        }

        private MaterialIconKind? _setterIcon;
        public MaterialIconKind? SetterIcon
        {
            get => _setterIcon;
            set { _setterIcon = value; OnPropertyChanged();}
        }
    }
}