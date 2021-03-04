using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Material.Dialog;
using Material.Dialog.Interfaces;
using Material.Music.Core;
using Material.Music.Core.Containers;
using Material.Music.Views.Dialogs.ViewModels;

namespace Material.Music.Views.Dialogs
{
    public class SearchSubtitleDialog : UserControl
    {
        private SearchSubtitleViewModel _viewModel;
        private PlayableBase _targetPlayable;
        private IDialogWindow<DialogResult> _dialogHost;
        
        public SearchSubtitleDialog()
        {
            this.InitializeComponent();

            DataContext = _viewModel = new SearchSubtitleViewModel();
        }

        public void SetHost(IDialogWindow<DialogResult> dialog) => _dialogHost = dialog;
        
        public void SetTarget(PlayableBase target)
        {
            _targetPlayable = target;
            _viewModel.Keywords = $"{_targetPlayable.TrackInfo.Artist} {_targetPlayable.Title}";
        }

        public SearchSubtitleResultItemViewModel GetResult() => _viewModel.SelectedItem;
        
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button.DataContext as SearchSubtitleResultItemViewModel;
            _viewModel.SelectedItem = item;
            _viewModel.CarouselIndex = 1;
        }

        private void BackButton_OnClick(object? sender, RoutedEventArgs e)
        {
            _viewModel.SelectedItem = null;
            _viewModel.CarouselIndex = 0;
        }

        private void DownloadButton_OnClick(object? sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                try
                {
                    _viewModel.Downloading = true;
                    var result = _viewModel.selectedProvider.DownloadSubtitle(_viewModel.SelectedItem.Id);
                    DataContainer.SaveSubtitle(result, _targetPlayable);
                    Dispatcher.UIThread.InvokeAsync(() => _dialogHost?.GetWindow().Close());
                }
                catch
                {
                    
                }
                _viewModel.Downloading = false;
            });
        }
    }
}
