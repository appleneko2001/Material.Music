using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Text;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using ManagedBass;
using Material.Icons;
using Material.Music.Core;
using Material.Music.Core.Engine;
using Material.Music.Core.Enums;
using Material.Music.Core.EventArgs;
using Material.Music.Core.Interfaces;
using Material.Music.Models;
using Material.Music.Views;
using Material.Styles;

namespace Material.Music.ViewModels
{
    public class PlayerContext : ViewModelBase, IStatusSaveble
    {
        private const string DiscreteTextMediaNotLoaded = "Please load an media for seek.";
        private const string DiscreteTextTimespanFormat = @"hh\:mm\:ss";
        
        #region Instance manager
        private static PlayerContext _instance ;
        public static PlayerContext GetInstance() => _instance ?? null;
        public static void InstantiateContext() {
            _instance = new PlayerContext();
        }

        public PlayerContext()
        {
            _currentMediaChannel = null;
            _playlists = new ObservableCollection<IPlaylist>();
            
            PlayerStatus = new PlayerStatus();
            RequestLoadStatus();
        }

        #endregion

        #region Constants
        public const int Repeat_NoRepeat = 0;
        public const int Repeat_RepeatPlaylist = 1;
        public const int Repeat_RepeatOnePlayable = 2;
        #endregion

        #region Event binder / unbinder
        public void BindToEngine(BassEngine engine)
        {
            engine.OnEngineStopped += OnEngineStoppedCallbacks;
            engine.OnChannelStatusChanged += Engine_OnChannelStatusChanged;
            engine.OnPlayStatusChanged += Engine_OnPlayStatusChanged;
            engine.OnUpdateTick += Engine_OnUpdateTick;
        }

        private void Engine_OnPlayStatusChanged(object sender, PlayStatusEventArgs e)
        {
            if (e.Status != PlayStatusEnums.Muted || e.Status != PlayStatusEnums.Unmuted)
                OnPropertyChanged(nameof(PlayPauseIcon));
            if(e.Status is PlayStatusEnums.ReachedEnd)
                RequestSkipNext(false);
        }

        private void Engine_OnUpdateTick(object sender, double e)
        {
            CurrentPosition = e;
        }

        private void Engine_OnChannelStatusChanged(object sender, ChannelStatusEventArgs e)
        {
            if (e.Status is ChannelStatusEnums.Loaded)
            {
                CurrentMedia = e.Playable;
                MediaChannel = e.Channel;
                Duration = e.Channel.GetDuration();
            }
            else if (e.Status is ChannelStatusEnums.Unloaded)
            {
                if(MediaChannel != null && MediaChannel.PlaybackState == PlaybackState.Playing)
                    MediaChannel.Pause();
                
                CurrentMedia = null;
                MediaChannel = e.Channel;
                CurrentPositionDiscreteText = DiscreteTextMediaNotLoaded;
                Duration = 0.0;
            }
        }

        private void OnEngineStoppedCallbacks (object sender, EventArgs e)
        {
            var engine = sender as BassEngine;
            engine.OnEngineStopped -= OnEngineStoppedCallbacks;
            RequestSaveStatus();
        }
        #endregion

        #region Load-n-Save status manager
        private PlayerStatus PlayerStatus;

        public bool RequestSaveStatus()
        {
            PlayerStatus.SaveStatusAsJson();
            return true;
        }

        public bool RequestLoadStatus()
        {
            _ = PlayerStatus.LoadStatusFromJsonAsync();
            return true;
        }
        #endregion
         
        #region Members for properties
        private IPlaylist _currentPlaylist;
        private PlayableBase _currentMedia;
        private BassChannel _currentMediaChannel; 
        private ObservableCollection<IPlaylist> _playlists;

        private double _duration = 0.0;
        private double _currentPosition = 0.0;

        private string _currentPositionDiscreteText = DiscreteTextMediaNotLoaded;
        private int _repeatMode = Repeat_NoRepeat;
        private bool _isSubtitleWindowOpen;
        private SubtitleWindow _subtitleWindow;
        #endregion


        #region Properties for binding UI Elements and commands

        public ObservableCollection<IPlaylist> Playlists => _playlists;
        
        public IPlaylist CurrentPlaylist { get => _currentPlaylist; set { _currentPlaylist = value; OnPropertyChanged(); } }

        public PlayableBase CurrentMedia
        {
            get => _currentMedia;
            private set
            {
                _currentMedia = value; 
                OnPropertyChanged();
            }
        }

        public double CurrentPosition
        {
            get => _currentPosition;
            set
            {
                _currentPosition = value; 
                OnPropertyChanged();
                if (MediaChannel != null)
                {
                    CurrentPositionDiscreteText = $"{TimeSpan.FromSeconds(CurrentPosition).ToString(DiscreteTextTimespanFormat)} / {TimeSpan.FromSeconds(Duration).ToString(DiscreteTextTimespanFormat)}";
                }
            }
        }
        
        public string CurrentPositionDiscreteText
        {
            get => _currentPositionDiscreteText;
            private set
            {
                _currentPositionDiscreteText = value;
                OnPropertyChanged();
            }
        }

        public double Duration { get => _duration; private set { _duration = value; OnPropertyChanged(); } }
        
        public double WindowWidth
        {
            get => PlayerStatus.WindowWidth;
            set
            {
                PlayerStatus.WindowWidth = value;
                OnPropertyChanged();
            }
        }
        
        public double WindowHeight
        {
            get => PlayerStatus.WindowHeight;
            set
            {
                PlayerStatus.WindowHeight = value;
                OnPropertyChanged();
            }
        }

        public int Volume
        {
            get => PlayerStatus.Volume;
            set
            {
                PlayerStatus.Volume = Math.Clamp(value, 0, 100); 
                OnPropertyChanged();
                if(_currentMediaChannel != null)
                    _currentMediaChannel.Volume = NormalizedVolume;  
                OnPropertyChanged(nameof(VolumeButtonIcon));
            }
        }

        public bool Muted
        {
            get => PlayerStatus.Muted;
            set
            {
                PlayerStatus.Muted = value; 
                OnPropertyChanged(); 
                if (_currentMediaChannel != null) 
                    _currentMediaChannel.Muted = Muted; 
                OnPropertyChanged(nameof(VolumeButtonIcon));
            }
        }

        public bool IsSubtitleWindowOpen
        {
            get => _isSubtitleWindowOpen;
            set
            {
                _isSubtitleWindowOpen = value;
                OnPropertyChanged();
                if (_isSubtitleWindowOpen)
                {
                    if (_subtitleWindow is null)
                        _subtitleWindow = new SubtitleWindow();
                    _subtitleWindow.Show();
                }
                else
                {
                    if(_subtitleWindow != null)
                        _subtitleWindow.Close();
                    _subtitleWindow = null;
                }
            }
        }

        public BassChannel MediaChannel
        {
            get => _currentMediaChannel;
            set
            {
                _currentMediaChannel = value;
                Dispatcher.UIThread.InvokeAsync(() => OnPropertyChanged("MediaChannel"));
                Dispatcher.UIThread.InvokeAsync(PlayerCommands.PlayPauseCommand.RaiseCanExecute);
            }
        }

        public int RepeatMode
        {
            get => PlayerStatus.RepeatMode;
            set 
            { 
                PlayerStatus.RepeatMode = value; 
                OnPropertyChanged(); 
                OnPropertyChanged(nameof(RepeatButtonIcon)); 
            }
        }

        public void RequestSkipNext(bool fromUser)
        {
            
            if(RepeatMode is Repeat_RepeatOnePlayable)
                MediaChannel.Restart();
        }

        public PlayerCommands PlayerCommands => PlayerCommands.GetInstance();
        #endregion

        #region Properties for convert values
        public float NormalizedVolume => Volume / 100f;
        #endregion

        #region Properties for change icon dynamically
        public MaterialIconKind PlayPauseIcon =>
            (MediaChannel?.PlaybackState is ManagedBass.PlaybackState.Playing) 
                ? MaterialIconKind.Pause
                : MaterialIconKind.Play;

        public MaterialIconKind VolumeButtonIcon =>
            Muted ? MaterialIconKind.VolumeMute
                : GetVolumeIconKind(Volume);

        public MaterialIconKind RepeatButtonIcon => 
            RepeatMode == Repeat_RepeatOnePlayable ? MaterialIconKind.RepeatOnce :
            RepeatMode == Repeat_RepeatPlaylist ? MaterialIconKind.Repeat : MaterialIconKind.RepeatOff;
            
        #endregion

        #region Private methods
        private MaterialIconKind GetVolumeIconKind(int volume)
        {
            return ((volume >= 50) ? MaterialIconKind.VolumeHigh
                : (volume > 10) ? MaterialIconKind.VolumeMedium
                : MaterialIconKind.VolumeLow);
        }

        public void SwitchRepeatMode()
        {
            if (RepeatMode < Repeat_RepeatOnePlayable)
                RepeatMode++;
            else
                RepeatMode = Repeat_NoRepeat;
        }
        #endregion
    }
}
