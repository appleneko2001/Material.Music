using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Material.Music.Core.IPCPipes;

namespace Material.Music
{
    class Program
    {
        private const string Identicator = "NekoPlayer:3c5969ce-f047-41be-9b9e-89f38cb5fa64";
        private static string IdenticatorMutex = $"Global\\{Identicator}:Mutex";
        private static string IdenticatorIPCPipe = $"{Identicator}:IPC";
        public static readonly string SocketPath = Path.Combine(Path.GetTempPath(), "NekoPlayerInstance.sock");
        public static string[] StartArguments;

        public static event EventHandler<string> OnIpcClientRequestReceived;

        private static IPCPipeInterface _ipcPipe;

        [STAThread]
        public static void Main(string[] args)
        { 
            var mutex = new Mutex(true, IdenticatorMutex, out var notRunning); 

            StartArguments = args;
              
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                _ipcPipe = new WindowsIPCPipe(IdenticatorIPCPipe, notRunning);

            if (notRunning)
            {
                if (_ipcPipe != null)
                    _ipcPipe.OnReceivedData += (s, e) =>
                        OnIpcClientRequestReceived?.Invoke(s, Encoding.UTF8.GetString(e));
                var app = BuildAvaloniaApp().StartWithClassicDesktopLifetime(args); 
                OnApplicationClosing?.Invoke(null, null); 
            }
            else
            {
                if(args.Length == 0)
                {
                    _ipcPipe?.SendData(Encoding.UTF8.GetBytes("activateWindow"));
                    return;
                }

                var mediaPath = args[0]; 
                _ipcPipe?.SendData(Encoding.UTF8.GetBytes($"open://{mediaPath}")); 
            }
            (_ipcPipe as IDisposable)?.Dispose();
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();


        public static event EventHandler OnApplicationClosing;
    }
}