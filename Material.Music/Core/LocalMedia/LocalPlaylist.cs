using System.Collections.ObjectModel;
using Material.Music.Core.Interfaces;
using ReactiveUI;

namespace Material.Music.Core.LocalMedia
{
    public class LocalPlaylist : ReactiveObject, IPlaylist
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
                this.RaisePropertyChanged();
            }
        }

        private ObservableCollection<PlayableBase> _playables;

        public ObservableCollection<PlayableBase> Playables => _playables;
    }
}