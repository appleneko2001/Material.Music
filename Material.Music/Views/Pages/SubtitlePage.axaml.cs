using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Material.Dialog;
using Material.Dialog.Enums;
using Material.Icons;
using Material.Icons.Avalonia;
using Material.Music.Core.Containers;
using Material.Music.Core.Engine;
using Material.Music.ViewModels;
using Material.Music.Views.Dialogs;
using Material.Music.Views.Interfaces;
using Material.Music.Models;
using Material.Music.Online;

namespace Material.Music.Views.Pages
{
    public class SubtitlePage : UserControl, IHasActionBarMenus
    {
        private SubtitlePageViewModel _viewModel;
        private Control[] _actionBarMenus;
        private ItemsControl SubtitleTextContainer;
        private ScrollViewer SubtitleScroller;
        private SubtitlePageItemViewModel _previousItem;
        
        public SubtitlePage()
        {
            this.InitializeComponent();

            SubtitleTextContainer = this.Get<ItemsControl>(nameof(SubtitleTextContainer));
            SubtitleScroller = this.Get<ScrollViewer>(nameof(SubtitleScroller));
            
            _actionBarMenus = new[]
            {
                CreateActionMenu(MaterialIconKind.Magnify, SearchSubtitle, "Search subtitle / lyric"),
                CreateActionMenu(MaterialIconKind.FileUploadOutline, null, "Load local subtitle / lyric")
            };

            var engine = BassEngine.GetInstance();
            DataContext = _viewModel = new SubtitlePageViewModel();
            engine.OnUpdateTick += BackendEngineOnUpdateTick;
            
            engine.OnChannelStatusChanged += (sender, args) => { _viewModel.UpdateSubtitlePage(args.Playable); };
            DataContainer.OnSavedSubtitle += (sender, s) =>
            {
                var context = PlayerContext.GetInstance();
                if(s == context.CurrentMedia.GetObjectHash())
                    _viewModel.UpdateSubtitlePage(context.CurrentMedia);
            };
        }

        private async void BackendEngineOnUpdateTick(object? sender, double e)
        {
            if (SubtitleTextContainer.ItemCount == 0)
                return;

            if (GetRequirementsBringViewInto(e, out var index))
            {
                var item = await GetSubtitleItem(index);
                if (item != null)
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        item.BringIntoView(new Rect(SubtitleScroller.Bounds.X, SubtitleScroller.Bounds.Y, SubtitleScroller.Bounds.Width, SubtitleScroller.Bounds.Height / 2));
                    });
                }
            }
        }
        
        private bool GetRequirementsBringViewInto(double t, out int index)
        {
            //var ts = TimeSpan.FromSeconds(t);

            index = 0;
            foreach (SubtitlePageItemViewModel item in SubtitleTextContainer.Items)
            {
                if (t > item.Timestamp.TotalSeconds)
                {
                    index++;
                    continue;
                }
                else
                {
                    if(_previousItem == item)
                        return false;
                    _previousItem = item;
                }
            }
            return true;
        }

        private async Task<IControl> GetSubtitleItem(int index)
        {
            return await Dispatcher.UIThread.InvokeAsync(() =>
            {
                return SubtitleTextContainer.ItemContainerGenerator.ContainerFromIndex(index);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public Control[] GetActionBarMenus() => _actionBarMenus;

        public async void SearchSubtitle()
        {
            var context = PlayerContext.GetInstance();
            var content = new SearchSubtitleDialog();

            if (context.CurrentMedia is null)
                return;
            
            content.SetTarget(context.CurrentMedia);
            var dialog = DialogHelper.CreateCustomDialog(new CustomDialogBuilderParams()
            {
                Borderless = true,
                DialogButtons = DialogHelper.CreateSimpleDialogButtons(DialogButtonsEnum.Ok),
                ContentHeader = "Search subtitle of track",
                Content = content,
                Width = 600,
            });
            content.SetHost(dialog);
            var result = await dialog.ShowDialog(MainWindow.Instance);
        }

        public static Control CreateActionMenu(MaterialIconKind iconKind, Action onClick, string tip)
        {
            var b = new Button()
            {
                Classes = Classes.Parse("Flat"),
                Padding = new Thickness(4),
                Margin = new Thickness(8, 0),
                Content = new MaterialIcon()
                {
                    Kind = iconKind
                },
            };
            b.Click += (a, b) => onClick?.Invoke();
            b.SetValue(ToolTip.TipProperty, tip);
            return b;
        }
    }
}
