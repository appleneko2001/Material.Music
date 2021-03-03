using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.VisualTree;
using Material.Music.Core;
using Material.Music.Views;

namespace Material.Music.ViewModels
{
    public class SubtitleWindowViewModel : ViewModelBase
    {
        private SubtitleWindow parent;
        public SubtitleWindowViewModel(SubtitleWindow window)
        {
            parent = window;
        }
        
        private bool _lockWidget;

        public bool LockWidget
        {
            get => _lockWidget;
            set
            {
                _lockWidget = value;
                ApplyHitTestVisible(_lockWidget);
            }
        }
        
        private void ApplyHitTestVisible(bool v)
        {
            var windowImpl = ((Window) parent).PlatformImpl;
            var handle = windowImpl.Handle.Handle;
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Utils.WindowsServices.SetWindowHitTestVisible(handle, v);
        }
    }
}