using ManagedBass;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Collections.Specialized;
using System.Threading;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using System.Threading.Tasks;
using Material.Dialog;
using Material.Music.Core.Enums;
using Material.Music.Core.EventArgs;
using Material.Music.Core.LocalMedia;
using Material.Music.Core.Logging;
using Material.Music.ViewModels;
using Material.Music.Models;
using Material.Music.Core.MediaInfo;
using Material.Music.Core.Interfaces;
using static Material.Music.Core.Logging.Logger;

namespace Material.Music.Core.Engine
{
    public class BassEngine
    {
        public const int BASS_STREAM_NULL = 0;

        #region Instance manager, service initializer
        public static BassEngine GetInstance() => _instance;

        public static BassEngine CreateInstance(BassDevice device = null) => _instance = new BassEngine(device);

        private static BassEngine _instance;

        /// <summary>
        /// Constructor of engine to prepare interacts with Bass library
        /// </summary>
        /// <param name="device"></param>
        private BassEngine(BassDevice device = null)
        {
            Errors? error = null;
            bool retried = false;

            while (true)
            {
                if (device is null || device.DeviceId == null)
                    device = BassDevice.GetDefaultDevice();
                if (device is null)
                    throw new ArgumentNullException(null, "No output devices.");

                if (!Bass.Init(device.Identicator, Flags: DeviceInitFlags.Default))
                {
                    error = Bass.LastError;
                    if (!retried)
                    {
                        if (error == Errors.Device || error == Errors.Busy)
                        {
                            device = BassDevice.GetDefaultDevice();
                            retried = true;
                            continue;
                        }
                        else
                            throw new ArgumentException(error.ToString());
                    }
                    else
                        throw new ArgumentException(error.ToString());
                }
                break;
            }
            InitializeServices();
            if (error != null)
            {
                //ExceptMessage.PopupExcept(error, additionalMessage: "We found a problem when instantiate engine. You have to check the settings of output devices, or other things.");
            }
        }

        private void InitializeServices()
        {
            LoadAllPlugins(_pluginsPath);
            InitTimer();
            BindEvents();
        }
        #endregion

        #region Engine event handler, binder and unbinder
        public PlayerContext CurrentContext { get; private set; }

        public event EventHandler<double> OnUpdateTick;

        public event EventHandler<ChannelStatusEventArgs> OnChannelStatusChanged;

        public event EventHandler<PlayStatusEventArgs> OnPlayStatusChanged;

        public event EventHandler OnEngineStopped;

        /// <summary>
        /// Stop host, and frees all dependencies objects. Can be used only when completely closed player.
        /// This is the one part of free resources, unbinds events can be found in <see cref="BindEvents(PlayerContext)"/>
        /// </summary>
        private void Free()
        {
            DisposeTimer();
            CloseAllChannels();
            UnbindChannelManager();
        }

        public void BindEvents()
        {
            EventHandler StopEngineMethod = null;
            StopEngineMethod = (sender, args) =>
            {
                OnEngineStopped -= StopEngineMethod;
                Free();
            };

            OnEngineStopped += StopEngineMethod;
            OnPlayStatusChanged += PlayStatusChanged;
            BindChannelManager();

            // Bind OnApplicationClosing
            Program.OnApplicationClosing += (a, b) => OnEngineStopped(this, null);
        }

        private void PlayStatusChanged(object sender, PlayStatusEventArgs e)
        {
            if (e.Status is PlayStatusEnums.Play)
                StartTimer();
            else if (
                e.Status is PlayStatusEnums.Pause ||
                e.Status is PlayStatusEnums.Stop || 
                e.Status is PlayStatusEnums.ReachedEnd)
                StopTimer();
        }

        public void BindPlayerContext(PlayerContext context)
        {
            CurrentContext = context;
            CurrentContext.BindToEngine(this);
        }
        #endregion

        #region System.Threading Timer Controller
        private const int TimerCallPeriod = 50; // 50 / 1000 = 50 times call per sec.
        private Timer _updateTickTimer;

        private void InitTimer() => _updateTickTimer = new Timer((s) => TimerCallbackFunction());

        private void StartTimer() => _updateTickTimer.Change(0, TimerCallPeriod);

        private void StopTimer() => _updateTickTimer.Change(Timeout.Infinite, Timeout.Infinite);

        private void DisposeTimer() => _updateTickTimer.Dispose();

        private void TimerCallbackFunction()
        {
            if (BassCurrentChannel is null)
                return;
            var position = BassCurrentChannel.GetPosition();
            OnUpdateTick?.Invoke(this, position);
        }
        #endregion

        #region Plugins manager

        public string GetPluginsPath() => _pluginsPath;
        private readonly string _pluginsPath = Path.Combine(Utils.GetProgramParentDir(), "BassModules");
        private Dictionary<string, int> LoadedPlugins = new Dictionary<string, int>();
        private readonly string[] _pluginsName = new string[] { "bass_aac", "bass_alac", "bass_flac" };
        /// <summary>
        /// Load all required BASS plugins.
        /// </summary>
        /// <param name="path">The BASS plugins directory</param>
        private void LoadAllPlugins(string path)
        {
            var exceptions = new Dictionary<string, Exception>();
            var error = false;
            
            foreach (var plugin in _pluginsName)
            {
                var libPath = Path.Combine(path, Utils.PlatformSpecificLibName(plugin));
                try
                {
                    if (!File.Exists(libPath))
                        throw new FileNotFoundException("Required library file not found.", libPath);

                    var id = Bass.PluginLoad(libPath);
                    if (id != 0)
                    {
                        var info = Bass.PluginGetInfo(id);
                        LoadedPlugins.Add(plugin, id);
                    }
                    else
                    {
                        var errorInfo = Bass.LastError;
                    }
                }
                catch (Exception e)
                { 
                    error = true;
                    exceptions.Add(plugin, e);
                }
            }

            if (error)
            {
                var builder = new StringBuilder();
                foreach (var e in exceptions)
                {
                    builder.AppendLine($"{e.Key} -- {e.Value.Message}");
                }
                
                Log(LogKind.Severe, true, "Some plugins cannot be loaded.", builder.ToString());
            }
        }
        /// <summary>
        /// Free all loaded BASS plugins.
        /// </summary>
        /// <returns>Return a list if we can't unload some plugins.</returns>
        private List<string> UnloadAllLoadedPlugins()
        {
            var fails = new List<string>();
            var removes = new List<string>();
            foreach (var plugin in LoadedPlugins)
            {
                if (!Bass.PluginFree(plugin.Value))
                {
                    fails.Add(plugin.Key);
                }
                else
                    removes.Add(plugin.Key);
            }
            foreach (var p in removes)
                LoadedPlugins.Remove(p);
            return fails;
        }
        #endregion

        #region Channel manager, event binder and unbinder
        public BassChannel BassCurrentChannel { get; private set; } = null;

        private ObservableCollection<BassChannel> m_BassChannels = new ObservableCollection<BassChannel>();

        public BassChannel CreateChannel(PlayableBase media)
        {
            if (media is null)
                throw new ArgumentNullException(nameof(media));
            try
            {
                OnChannelStatusChanged?.Invoke(this,
                    new ChannelStatusEventArgs
                        {Channel = BassChannel.Null, Playable = null, Status = ChannelStatusEnums.Unloaded});
                CloseAllChannels();

                var stream = media.GetBassStream().GetBassFileController();
                var channelId = Bass.CreateStream(StreamSystem.Buffer, BassFlags.Prescan, stream);
                if (channelId != BASS_STREAM_NULL)
                {
                    var channel = new BassChannel(channelId);
                    channel.Volume = CurrentContext.NormalizedVolume;
                    channel.Muted = CurrentContext.Muted;
                    m_BassChannels.Add(channel);
                    OnChannelStatusChanged?.Invoke(this,
                        new ChannelStatusEventArgs
                            {Channel = channel, Playable = media, Status = ChannelStatusEnums.Loaded});
                    return channel;
                }
                else
                    throw new Exception("Stream initialized unsuccessful");
            }
            catch (UnauthorizedAccessException)
            {
                LogWarn($"The file or resource \"{media?.GetMediaPath()}\" has denied access.", true);
            }
            catch (Exception e)
            {
                LogError(e, true);
            }
            return null;
        }

        private void OnBassChannelQueuesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (m_BassChannels.Count is 0)
            { 
                if(e.OldItems != null)
                    if (BassCurrentChannel != null && e.OldItems.Contains(BassCurrentChannel))
                        BassCurrentChannel = null;
            }
            else
                BassCurrentChannel = m_BassChannels.Last();

            if(e.OldItems != null)
                foreach(BassChannel channel in e.OldItems)
                    channel.Close(); 
        }

        internal void CallbackFromChannel(object sender, PlayStatusEventArgs e) => OnPlayStatusChanged?.Invoke(sender, e);

        internal void CallbackFromChannel(object sender, ChannelStatusEventArgs e) => OnChannelStatusChanged?.Invoke(sender, e);

        private void BindChannelManager() => m_BassChannels.CollectionChanged += OnBassChannelQueuesChanged;

        private void UnbindChannelManager() => m_BassChannels.CollectionChanged -= OnBassChannelQueuesChanged;

        public void CloseAllChannels() => m_BassChannels.DynamicForEach(c => m_BassChannels.Remove(c));
        #endregion

        #region Utilities
        public static async Task<TimeSpan> TestMediaDurationAsync(LocalPlayable playable)
        {
            var result = await Task.Run(() =>
            {
                var channelId = Bass.CreateStream(StreamSystem.NoBuffer, BassFlags.Default, playable.GetBassStream().GetBassFileController());
                if (channelId != BASS_STREAM_NULL)
                {
                    var realDuration = Bass.ChannelBytes2Seconds(channelId, Bass.ChannelGetLength(channelId));
                    Bass.StreamFree(channelId);
                    return TimeSpan.FromSeconds(realDuration);
                }
                else
                {
                    var error = Bass.LastError;
                    return TimeSpan.Zero;
                }
            });
            return result;
        }
        #endregion
    }
}
