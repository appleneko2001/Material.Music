using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Material.Music.Core.Interfaces
{
    public interface IPlaylist
    {
        
        string Name { get; }
        ObservableCollection<PlayableBase> Playables { get; }
    }
}
