using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Material.Music.ViewModels;
using Material.Music.Models;

namespace Material.Music.Views.Pages
{
    public class NowPlayingPage : UserControl
    {
        public NowPlayingPage()
        {
            this.InitializeComponent();

            DataContext = PlayerContext.GetInstance();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
