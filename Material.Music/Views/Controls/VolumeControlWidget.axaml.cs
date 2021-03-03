using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Material.Music.Views.Controls
{
    public class VolumeControlWidget : UserControl
    {
        public VolumeControlWidget()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
