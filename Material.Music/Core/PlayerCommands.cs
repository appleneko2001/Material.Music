using Material.Music.Core.Interfaces;
using Material.Music.Models;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using ManagedBass;
using Material.Music.Commands;
using Material.Music.Core.Engine;
using Material.Music.Core.LocalMedia;
using Material.Music.ViewModels;

namespace Material.Music.Core
{
    public class PlayerCommands
    {
        #region Instance manager, constructor and player context reference.
        public static PlayerCommands GetInstance() => _instance;

        public static PlayerCommands CreateInstance() => _instance = new PlayerCommands();

        private static PlayerCommands _instance;

        public PlayerContext CurrentContext { get; private set; }

        /// <summary>
        /// Initialize commands
        /// </summary>
        private PlayerCommands()
        {
            CurrentContext = PlayerContext.GetInstance();

            PlayPauseCommand = new RelayCommand(OnPlayPauseCommandExecuted, CanExecutePlayPauseCommand);
            MuteCommand = new RelayCommand((p) => OnMuteCommandExecuted(CurrentContext));
            PlayMediaCommand = new RelayCommand(OnPlayMediaCommandExecuted);
        }
        #endregion

        #region Properties
        public RelayCommand PlayPauseCommand { get; }

        public RelayCommand MuteCommand { get; }

        public RelayCommand PlayMediaCommand { get; }
        #endregion

        #region Methods for commands use

        private static bool CanExecutePlayPauseCommand(object channel)
        {
            return channel != null;
        }
        
        private static void OnPlayPauseCommandExecuted(object param)
        {
            var channel = param as BassChannel;
            if (channel is null)
                return;
            
            if (channel.PlaybackState is ManagedBass.PlaybackState.Playing)
                channel.Pause();
            else if (channel.PlaybackState is ManagedBass.PlaybackState.Paused || channel.PlaybackState is ManagedBass.PlaybackState.Stopped)
                channel.Resume();
        }

        private static void OnMuteCommandExecuted(PlayerContext context) => context.Muted = !context.Muted;
        
        private static void OnPlayMediaCommandExecuted(object param) => OnPlayMediaCommandExecuted(param as PlayableBase);
        
        private static void OnPlayMediaCommandExecuted(PlayableBase media)
        {
            var engine = BassEngine.GetInstance();
            var channel = engine.CreateChannel(media as PlayableBase);
            channel?.Resume();
        }
        #endregion

        #region Public methods
        public static void PlayLocalFile(string path)
        {
            var engine = BassEngine.GetInstance();
            var channel = engine.CreateChannel(new LocalPlayable(path));
            channel?.Resume();
        }
        #endregion
    }
}
