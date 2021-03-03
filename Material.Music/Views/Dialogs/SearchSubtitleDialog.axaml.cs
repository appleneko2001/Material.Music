using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Material.Music.Views.Dialogs
{
    public class SearchSubtitleDialog : UserControl
    {
        public SearchSubtitleDialog()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
