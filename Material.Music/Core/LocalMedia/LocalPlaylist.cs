using System.Collections.ObjectModel;
using Material.Music.Core.Interfaces;
using Material.Music.ViewModels;

namespace Material.Music.Core.LocalMedia
{
    public class LocalPlaylist : ViewModelBase, IPlaylist
    {

        public LocalPlaylist()
        {
            _playables = new ObservableCollection<PlayableBase>();
        }

        private string _name;
        
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                this.OnPropertyChanged();
            }
        }

        private ObservableCollection<PlayableBase> _playables;

        public ObservableCollection<PlayableBase> Playables => _playables;
    }
}