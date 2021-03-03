using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Diagnostics;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Material.Styles;
using Material.Music.Core;
using Material.Music.Core.Engine;
using Material.Music.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Material.Dialog;
using Material.Icons;
using Material.Music.ViewModels;
using Material.Music.Core.Enums;
using Material.Music.Core.EventArgs;
using Material.Music.Core.LocalMedia;

namespace Material.Music.Views
{
    public class SubtitleWindow : Window
    {
        private SubtitleWindowViewModel _viewModel;
        public SubtitleWindowViewModel ViewModel => _viewModel;

        public SubtitleWindow()
        {
            _viewModel = new SubtitleWindowViewModel(this);
            DataContext = ViewModel;
            
            void InitializeComponent() => AvaloniaXamlLoader.Load(this);
            InitializeComponent();

#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void TopLevel_OnClosed(object? sender, EventArgs e)
        {
            PlayerContext.GetInstance().IsSubtitleWindowOpen = false;
        }

        private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e) => this.BeginMoveDrag(e);

        private void CloseButton_OnClick(object? sender, RoutedEventArgs e) => this.Close();
    }
}