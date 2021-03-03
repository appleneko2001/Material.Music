using Material.Music.Core.Interfaces;
using Material.Music.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using Material.Music.Core.Engine;
using Material.Music.Core.LocalMedia;
using Material.Music.ViewModels;

namespace Material.Music.Core
{
    public class PlayerCommands : ReactiveObject
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

            var canExecutePlayPauseCommand = this.WhenAnyValue<PlayerCommands, bool, BassChannel>(c => c.CurrentContext.MediaChannel, c => c != null);
            canExecutePlayPauseCommand.Subscribe();
            PlayPauseCommand = ReactiveCommand.Create<BassChannel>(OnPlayPauseCommandExecuted, canExecutePlayPauseCommand);
            PlayPauseCommand.Subscribe();

            MuteCommand = ReactiveCommand.Create(() => OnMuteCommandExecuted(CurrentContext));
            MuteCommand.Subscribe();

            PlayMediaCommand = ReactiveCommand.Create<PlayableBase>(OnPlayMediaCommandExecuted); 
            PlayMediaCommand.Subscribe();
        }
        #endregion

        #region Properties
        public ReactiveCommand<BassChannel, Unit> PlayPauseCommand { get; }

        public ReactiveCommand<Unit, Unit> MuteCommand { get; }

        public ReactiveCommand<PlayableBase, Unit> PlayMediaCommand { get; }
        #endregion

        #region Methods for commands use
        private static void OnPlayMediaCommandExecuted(PlayableBase media)
        {
            var engine = BassEngine.GetInstance();
            var channel = engine.CreateChannel(media as PlayableBase);
            channel?.Resume();
        }

        private static void OnPlayPauseCommandExecuted(BassChannel channel)
        {
            if (channel is null)
                return;
            
            if (channel.PlaybackState is ManagedBass.PlaybackState.Playing)
                channel.Pause();
            else if (channel.PlaybackState is ManagedBass.PlaybackState.Paused || channel.PlaybackState is ManagedBass.PlaybackState.Stopped)
                channel.Resume();
        }

        private static void OnMuteCommandExecuted(PlayerContext context) => context.Muted = !context.Muted;

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
