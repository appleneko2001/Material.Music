using System;
using System.Collections.Specialized;
using System.Text;
using Avalonia;
using Avalonia.Threading;
using Material.Dialog;
using Material.Dialog.Icons;
using Material.Music.Core.StackoverflowSolutions;

namespace Material.Music.Core.Logging
{
    public class Logger
    {
        private const string ErrorPrefix = "An exception occurred: ";
        private const string SeverePrefix = "An severe exception occurred: ";

        static Logger()
        {
            _stack = new ObservableStack<LogInfo>();
            _stack.CollectionChanged += StackOnCollectionChanged;
        }

        private static void StackOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action is NotifyCollectionChangedAction.Add)
            {
                ProcessLog(_stack.Pop());
            }
        }

        private static ObservableStack<LogInfo> _stack;

        public static void LogInfo(bool showDialog = false, params object[] infos) => 
            Log(LogKind.Info, showDialog, infos);
        
        public static void LogWarn(string info, bool showDialog = false) => 
            Log(LogKind.Error, showDialog, info);

        public static void LogError(Exception exception, bool showDialog = false) => 
            Log(LogKind.Error, showDialog, ErrorPrefix, exception);

        public static void LogSevere(Exception exception, bool showDialog = true) => 
            Log(LogKind.Severe, showDialog, SeverePrefix, exception);
        
        public static void Log(LogKind kind = LogKind.Info, bool showDialog = false, params object[] infos)
        {
            _stack?.Push(new LogInfo(){Kind = kind, ShowDialog = showDialog, Infos = infos});
        }

        private static void ProcessLog(LogInfo log)
        {
            StringBuilder builder = new StringBuilder();
            
            foreach (var info in log.Infos)
            {
                switch (info)
                {
                    case string s:
                        builder.AppendLine(s);
                        break;
                    case Exception e:
                        builder.AppendLine(e.Message);
                        builder.AppendLine($"Stacktrace: {e.StackTrace}");
                        break;
                    default:
                        builder.AppendLine(info.ToString());
                        break;
                }
            }

            var defaultForeColor = Console.ForegroundColor;
            switch (log.Kind)
            {
                case LogKind.Info:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogKind.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogKind.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogKind.Severe:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
            }
            Console.WriteLine(builder.ToString());
            Console.ForegroundColor = defaultForeColor;

            if (log.ShowDialog)
            {
                DialogIconKind? iconKind = null;

                switch (log.Kind)
                {
                    case LogKind.Info:
                        iconKind = DialogIconKind.Info;
                        break;
                    case LogKind.Warning:
                        iconKind = DialogIconKind.Warning;
                        break;
                    case LogKind.Error:
                        iconKind = DialogIconKind.Error;
                        break;
                    case LogKind.Severe:
                        iconKind = DialogIconKind.Stop;
                        break;
                }

                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var dialog = DialogHelper.CreateAlertDialog(new AlertDialogBuilderParams()
                    {
                        Width = 800,
                        ContentHeader = "Message from Logging",
                        SupportingText = builder.ToString(),
                        DialogHeaderIcon = iconKind
                    });

                    if (App.Instance.MainWindow.IsVisible)
                        dialog.ShowDialog(App.Instance.MainWindow);
                    else
                        dialog.Show();
                });
            }
        }
    }
}