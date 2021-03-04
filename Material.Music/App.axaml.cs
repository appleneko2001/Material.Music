using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
/*using DesktopNotifications;
using DesktopNotifications.Apple;
using DesktopNotifications.FreeDesktop;
using DesktopNotifications.Windows;*/
using Material.Music.Core;
using Material.Music.Core.Engine;
using Material.Music.Core.LocalMedia;
using Material.Music.ManagedBassFix.Tags;
using Material.Music.Online;
using Material.Music.ViewModels;
using Material.Music.Views;
using NeteaseCloudMusic.Provider;

namespace Material.Music
{
    public class App : Application
    {
        //public static INotificationManager NotificationManager { get; private set; }
        public static App Instance { get; private set; }
        
        private MainWindow _mainWindow;
        public MainWindow MainWindow => _mainWindow;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            Instance = this;

            var engine = BassEngine.CreateInstance(BassDevice.GetDefaultDevice());
            var tagReader = new BassTagsLibrary(System.IO.Path.Combine(engine.GetPluginsPath(), Utils.PlatformSpecificLibName("tags")));
            PlayerContext.InstantiateContext();
            var commands = PlayerCommands.CreateInstance();
            engine.BindPlayerContext(PlayerContext.GetInstance()); 
            
            Localizer.Localizer.Instance.LoadLanguage("en-US");
            
            // Register a method for play media from second instances request.
            Program.OnIpcClientRequestReceived += (sender, arg) => ParseRequest(arg);
            
            //NotificationManager = CreateManager();
            //NotificationManager.Initialize();
            
            var startUpArgs = Environment.GetCommandLineArgs().Skip(1);
            if (startUpArgs != null && startUpArgs.Any())
            {
                if (File.Exists(startUpArgs.ElementAt(0)))
                    PlayMedia(startUpArgs.ElementAt(0));
            }

            ApiManager.Instance.RegisterSubtitleApi(new NcmSubtitleProvider());
            
            base.Initialize();
        }

        public static void FocusWindow()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                var window = Instance?._mainWindow;
                
                if (window.WindowState == WindowState.Minimized)
                    window.WindowState = WindowState.Normal;

                window.Topmost = true;
                window.Activate();
                window.Topmost = false; 
            });
        }

        private void ParseRequest(string args)
        {
            if (args.StartsWith("activateWindow"))
            {
                FocusWindow();
            }
            else if (args.StartsWith("open://"))
            {
                var s = args.Remove(0, "open://".Length);
                PlayMedia(s);
            } 
        }

        private void PlayMedia(string mediaPath) 
        {
            var context = PlayerContext.GetInstance();

            var playlist = new LocalMediaPlaylist();
            playlist.ScanDirectory(Path.GetDirectoryName(mediaPath));
            context.CurrentPlaylist = playlist;
             
            PlayerCommands.PlayLocalFile(mediaPath);
        }
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = _mainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
        
        /*private static INotificationManager CreateManager()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new FreeDesktopNotificationManager();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsNotificationManager();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new AppleNotificationManager();
            }

            throw new PlatformNotSupportedException();
        }*/
    }
}